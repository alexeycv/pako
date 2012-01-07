/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Pako Jabber-bot.                                                              *
 * Copyright. All rights reserved © 2007-2008 by Klichuk Bogdan (Bbodio's Lab)   *
 * Copyright. All rights reserved © 2009-2012 by Alexey Bryohov                  *
 * Contact information is here: http://code.google.com/p/pako                    *
 *                                                                               *
 * Pako is under GNU GPL v3 license:                                             *
 * YOU CAN SHARE THIS SOFTWARE WITH YOUR FRIEND, MAKE CHANGES, REDISTRIBUTE,     *
 * CHANGE THE SOFTWARE TO SUIT YOUR NEEDS, THE GNU GENERAL PUBLIC LICENSE IS     *
 * FREE, COPYLEFT LICENSE FOR SOFTWARE AND OTHER KINDS OF WORKS.                 *
 *                                                                               *
 * Visit http://www.gnu.org/licenses/gpl.html for more information about         *
 * GNU General Public License v3 license                                         *
 *                                                                               *
 * Download source code: http://pako.googlecode.com/svn/trunk                    *
 * See the general information here:                                             *
 * http://code.google.com/p/pako.                                                *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;
using System.Collections.Generic;
using System.Text;
using agsXMPP;
using Core;
using Core.Plugins;
using Core.Other;
using my = agsXMPP.protocol.client;

namespace Core.Kernel
{
	public class Cmd
	{
		private string m_body = null;
		private Response m_r = null;
		private int? naccess = null;
		private int? m_first_access = null;
		private AccessType acc_type;
		private CmdAccessibilityType avail;
		private bool has_alias;
		private string volume;
		private string[] args;

		public int? CompleteAccess {
			get { return naccess == null ? 0 : naccess; }
		}

		public int? FistStageAccess {
			get { return m_first_access == null ? 0 : m_first_access; }
		}

		public Response Response {
			get { return m_r; }
		}

		public AccessType AccessType {
			get { return acc_type; }
		}


		public CmdAccessibilityType Accessibility {
			get { return avail; }
		}

		public string Body {
			get { return m_body; }
		}

		public string Volume {
			get { return volume; }
		}

		public string[] vArgs {
			get { return args; }
			set { args = value; }
		}

		public bool IsAlias {
			get { return has_alias; }
		}

		public string[] Args ()
		{
			return args;
		}

		public string[] Args (int Fixed)
		{
			return Utils.SplitEx (Body, Fixed);
		}


		public AccessType CompleteAccessType {
			get { return acc_type; }
		}


		/// <summary>
		/// Take the cmd instance as its body
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		public static implicit operator string (Cmd c)
		{
			return c.Body;
		}

		public static Cmd CreateInstance (string body, Response r, bool? roster)
		{
			Cmd cmd = new Cmd (body, r, roster);
			return cmd;
		}

		/// <summary>
		/// Private constructor
		/// </summary>
		/// <param name="Body"></param>
		/// <param name="r"></param>
		Cmd (string cmd, Response r, bool? roster)
		{
			m_r = r;
			bool alias_exists = false;
			
			this.m_body = cmd;
			string pure_cmd = null;
			//command test
			if (r.MUC != null && r.MUser != null && (roster == null || (roster != null && roster == false))) {
				// If source is muc
				r.Access = r.Access ?? 0;
				//access level. 0 if not defined
				// Checking access levels
				@out.exe ("cmd_muc_access_setting_started");
				naccess = r.MUC.OptionsHandler.GetOption ("cmdaccess") == "+" ? r.MUC.AccessManager.GetAccess (m_body, ref pure_cmd) : null;
				this.acc_type = AccessType.SetByMuc;
				
				// Checking for aliases
				alias_exists = r.MUC.HasAlias (m_body);
				@out.exe ("pure_cmd: '" + pure_cmd + "'");
				this.has_alias = alias_exists;
				@out.exe ("cmd_muc_access_setting_finished");
				
				// Checking for command recursion
				if (r.MUC.chkal_rec (m_body, 0, r.Sh.S.Config.RecursionLevel, r)) {
					@out.exe ("recursion_excided");
					//r.Reply(r.f("commands_recursion", r.Sh.S.Config.RecursionLevel.ToString()));
					this.avail = CmdAccessibilityType.AliasRecursion;
					return;
					//exit, if resursion
				}
				
				int steps = 0;
				string al_name = null, al_body = null, al_formatted = null, al_temp_f = null;
				
				//aliases
				/*Getting alias with a recursion. We need to check all levels. For example:
				 * *alias add echo=muc echo #~
				 * *alias add echo2=muc echo #~
				 * *alias add echo3=echo2*
				 * It's launc like this: echo3 => echo2 => echo => muc echo
				 * 
				 * */
				while (r.MUC.HasAlias (m_body)) {
					@out.exe ("alias_launching");
					m_body = r.MUC.GetAlias (m_body, ref al_name, ref al_body, ref al_temp_f, r);
					if (steps == 0)
						al_formatted = al_temp_f;
					@out.exe ("alias_launched");
					steps++;
				}
				r.Msg.Body = m_body;
				// Now m_body has a full command without aliases
				
				// Checking access rithts for alias body and command body
				if (alias_exists) {
					int? _nalias = 0;
					int? _naccess = 0;
					r.Format = false;
					string a_purec_cmd = null;
					string _purec_cmd = null;
					
					// Getting command access levels
					
					// Get a local access
					_nalias = r.MUC.OptionsHandler.GetOption ("cmdaccess") == "+" ? r.MUC.AccessManager.GetAccess (al_body, ref a_purec_cmd) : null;
					_naccess = r.MUC.OptionsHandler.GetOption ("cmdaccess") == "+" ? r.MUC.AccessManager.GetAccess (m_body, ref _purec_cmd) : null;
					
					// Get a global access.
					// Global access will owerride local.
					_nalias = r.Sh.S.AccessManager.GetAccess (al_body, a_purec_cmd, _nalias);
					_naccess = r.Sh.S.AccessManager.GetAccess (m_body, _purec_cmd, _naccess);
					
					this.m_first_access = _nalias;
					_nalias = _nalias ?? 0;
					_naccess = _naccess ?? 0;
					
					@out.exe ("test_cmd: " + al_formatted);
					@out.exe ("test_access: " + _naccess.ToString ());
					@out.exe ("test_access_finish: " + _nalias.ToString ());
					
					// Checking a user access
					// if user access less then foll command access and alias access not equal command access ==> exit
					if (_nalias != _naccess && _naccess > r.Access) {
						@out.exe ("access_not_enough");
						this.naccess = _naccess;
						//r.Reply(r.f("access_not_enough", naccess.ToString()));
						this.avail = CmdAccessibilityType.NotAccessible;
						return;
						//exit, if user haven't access
					}
				} else
					//no aloases
					@out.exe ("alias_not_found: stage2");
				
				
				
			}
			//end muc-specific part
			
			args = Utils.SplitEx (m_body, 2);
			// getting a command aqd arguments
			volume = args[0];
			
			// Get command global access
			@out.exe ("cmd_global_access_setting_started: " + m_body);
			naccess = r.Sh.S.AccessManager.GetAccess (m_body, pure_cmd, naccess, ref this.acc_type);
			
			if (naccess == null) {
				// !!! NOTICE: access will be 0, but if command need a high security level it's must be checked MANUALY !!!
				this.acc_type = AccessType.None;
				@out.exe ("cmd_no_access_notifies_found_access=0");
			}
			
			//if (volume == "admin" )
			// Security access. TODO: link it with config file
			if (volume == "admin") {
				naccess = naccess ?? 100;
			}
			
			naccess = naccess ?? 0;
			
			if ((r.Sh.S.PluginHandler.Handles (volume)) || (alias_exists)) {
				if (r.Access < naccess) {
					@out.exe ("access_not_enough");
					this.avail = CmdAccessibilityType.NotAccessible;
					return;
					// exiut if no access
				}
			}
			
		}

		/// <summary>
		/// Destructor
		/// </summary>
		~Cmd ()
		{
			@out.exe ("cmd_destroyed");
		}


		/// <summary>
		/// Execute a command
		/// </summary>

		public bool Execute ()
		{
			@out.exe ("cmd_body: " + m_body);
			if (m_r.Sh.S.PluginHandler.Handles (volume)) {
				
				object obj = m_r.Sh.S.PluginHandler.Execute (volume);
				if (obj != null) {
					@out.exe ("cmd_body: handles");
					IPlugin plugin = (IPlugin)obj;
					PluginTransfer pt = new PluginTransfer (m_r);
					@out.exe ("EXE");
					plugin.PerformAction (pt);
					return true;
				}
				return false;
			}
			return false;
		}


		/// <summary>
		/// Overriding the ToString method of the object
		/// </summary>
		/// <returns></returns>
		public override string ToString ()
		{
			return Body;
		}
		
	}
}
