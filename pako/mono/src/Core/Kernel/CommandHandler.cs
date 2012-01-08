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

using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System;
using agsXMPP;
using System.Web;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;
using System.IO;
using System.Net;
using Core.Kernel;
using Core.Conference;
using Core.Plugins;
using System.Threading;
using Core.Xml;
using Core.Other;

namespace Core.Kernel
{

	public class CommandHandler
	{

		#region Variables

		Message m_msg;
		Jid s_jid;
		string s_nick;
		MUser m_user;
		string original;
		Jid m_jid;
		MUC m_muc;
		SessionHandler Sh;
		Message emulate;
		CmdhState _signed;
		int _level;

		bool _allowMyJid = false;
		int? userAccessLevel;

		#endregion ;


		public CommandHandler (agsXMPP.protocol.client.Message msg, SessionHandler s, Message emulation, CmdhState signed, int level)
		{			
			msg.From = new Jid (msg.From.Bare.ToLower () + (msg.From.Resource != "" ? "/" + msg.From.Resource : ""));
			m_msg = msg;
			s_jid = msg.From;
			Sh = s;
			if (msg.Body == null || msg.Body == "")
				return;
			if (s_jid.Bare == Sh.S.C.MyJID.Bare)
				return;
			emulate = emulation;
			_signed = signed;
			_level = level;
			Thread thr = new Thread (new ThreadStart (Handle));
			thr.Start ();			
			
		}

		public CommandHandler (agsXMPP.protocol.client.Message msg, SessionHandler s, Message emulation, CmdhState signed, int level, bool allowMyJid, int? userAccessLevel)
		{
			msg.From = new Jid (msg.From.Bare.ToLower () + (msg.From.Resource != "" ? "/" + msg.From.Resource : ""));
			m_msg = msg;
			s_jid = msg.From;
			Sh = s;
			if (msg.Body == null || msg.Body == "")
				return;
			
			if (!allowMyJid) {
				if (s_jid.Bare == Sh.S.C.MyJID.Bare) {
					//@out.write ("Handler EXIT");
					return;
				}
			}
			
			emulate = emulation;
			_signed = signed;
			_level = level;
			
			this._allowMyJid = allowMyJid;
			
			Thread thr = new Thread (new ThreadStart (Handle));
			thr.Start ();
			
			//@out.write ("Handler OK");
		}

		public void Handle ()
		{
			// _Handle();
			try {
				_Handle ();
			} catch (Exception ex) {
				
				string clrf = "\n";
				string msg_source = m_msg.ToString ();
				
				if (Utils.OS == Platform.Windows) {
					clrf = "\r\n";
					msg_source = m_msg.ToString ().Replace ("\n", "\r\n");
					
				}
				string data = "====== [" + DateTime.Now.ToString () + "] ===================================================>" + clrf + "   Stanza:" + clrf + msg_source + clrf + clrf + ex.ToString () + clrf + clrf + clrf;
				
				Sh.S.ErrorLoger.Write (data);
				
				if (Sh.S.Config.SendErrorMessagesToAdmin) {
					foreach (Jid j in Sh.S.Config.Administartion ()) {
						Message _msg = new Message ();
						_msg.To = j;
						_msg.Type = MessageType.chat;
						_msg.Body = "ERROR:   " + ex.ToString () + "\n\nStack trace: \n" + ex.StackTrace + "\nSource:\n" + msg_source;
						Sh.S.C.Send (_msg);
					}
				}
			}
		}


		public void _Handle ()
		{
			_signed = _signed == CmdhState.PREFIX_NULL ? m_msg.Type == MessageType.groupchat ? CmdhState.PREFIX_REQUIRED : CmdhState.PREFIX_POSSIBLE : _signed;
			
			//Get a muc subject
			if (m_msg.Subject != null && m_msg.Type == MessageType.groupchat) {
				if (Sh.S.GetMUC (s_jid) != null)
					Sh.S.GetMUC (s_jid).Subject = m_msg.Subject;
			}
			
			// Get MUC object
			if (Sh.S.GetMUC (s_jid) != null) {
				m_muc = Sh.S.GetMUC (s_jid);
			}
			
			// Get MUC User object
			m_user = null;
			if (m_muc != null) {
				if (s_jid.Resource == null) {
					//@out.write ("Handler EXIT (if (s_jid.Resource == null))");
					return;
				}
				// MUC mode woth a null MUC user. Stop process
				if (m_muc.GetUser (s_jid.Resource) != null)
					m_user = m_muc.GetUser (s_jid.Resource);
				else {
					//@out.write ("Handler EXIT m_muc.GetUser (s_jid.Resource) != null");
					return;
				}
				// Because of null MUC user in MUC mode
			}
			
			// Determine a delomoter
			string d = Sh.S.Config.Delimiter;
			
			// User nick in MUC
			s_nick = s_jid.Resource;
			
			// Get message type
			msgType m_type = Utils.GetTypeOfMsg (m_msg, m_user);
			bool is_muser = m_type == msgType.MUC;
			
			// Get message sender jid
			m_jid = is_muser ? m_user.Jid : s_jid;
			
			// Get MUC User (TODO: Refactoring)
			if (m_type == msgType.MUC) {
				Sh.S.GetMUC (s_jid).GetUser (s_jid.Resource).Idle = DateTime.Now.Ticks;
				//m_user = m_muc.GetUser(s_jid.Resource);
			}
			if ((m_type == msgType.MUC) || (m_type == msgType.Roster)) {
				//Add html log entry
				try {
					//if (Sh.S.Config.EnableLogging && m_user != null && m_muc.Subject != null)
					//{
					//    Sh.S.HtmlLogger.AddHtmlLog("groupchat", "topic", m_muc.Jid.ToString(), m_user.Nick, m_muc.Subject);
					//}
					
					if (Sh.S.Config.EnableLogging && m_muc.OptionsHandler.GetOption ("enable_logging") == "+" && m_user != null && m_msg.Body != null && m_msg.Type == MessageType.groupchat) {
						Sh.S.HtmlLogger.AddHtmlLog ("groupchat", "chat", m_muc.Jid.ToString (), m_user.Nick, m_msg.Body);
					}
				} catch (Exception ex) {
				}
				
				//Get message body
				m_msg.Body = m_msg.Body.TrimStart (' ');
				original = m_msg.Body;
				string m_body = m_msg.Body;
				
				string vl = null;
				if (m_muc != null)
					vl = m_muc.VipLang.GetLang (m_jid);
				if (vl == null)
					vl = Sh.S.VipLang.GetLang (m_jid);
				
				//Initializing response object
				Response r = new Response (Sh.S.Rg[vl != null ? vl : is_muser ? m_user.Language : Sh.S.Config.Language]);
				
				// Get user access
				int? access = 0;
				access = Sh.S.GetAccess (m_msg, m_user, m_muc);
				
				if (access != null)
					r.Access = access;
				else
					r.Access = 0;
				
				r.Msg = m_msg;
				r.MSGLimit = Sh.S.Config.MucMSGLimit;
				string aliasb = String.Empty;
				string aliase = String.Empty;
				r.MUC = m_muc;
				r.Level = _level;
				r.MUser = m_user;
				r.Delimiter = d;
				r.Sh = Sh;
				
				if (is_muser) {
					//Determine a message type for a bot in this groupchat
					switch (m_muc.OptionsHandler.GetOption ("mode")) {
					case "private":
						
						
						{
							if (m_msg.Type == MessageType.groupchat)
								return;
							break;
						}

					
					case "groupchat":
						
						
						{
							if (m_msg.Type != MessageType.groupchat)
								return;
							break;
						}

					default:
						
						break;
					}
					
					if (access != null)
						m_user.Access = access;
					else
						m_user.Access = 0;
					
					@out.exe ("[" + s_jid.User + "] " + s_jid.Resource + "> " + m_msg.Body);
					
					//Emulation
					if (emulate == null) {
						@out.exe ("emulate_non_existing");
						if (m_user == m_muc.MyNick)
							if (!this._allowMyJid) {
								return;
							}
					} else {
						@out.exe ("emulate_existing");
						r.Access = 100;
						access = 100;
						m_user.Access = 100;
						r.Emulation = emulate;
					}
					@out.exe ("emulate_extistence_determining_finished");
					
					//userAccessLevel
					if (userAccessLevel != null) {
						r.Access = userAccessLevel;
						access = userAccessLevel;
						m_user.Access = userAccessLevel;
					}
					
					// Checking user input text for a censored phrases
					HandleCensor _hc = new HandleCensor (m_user, Sh, r, m_muc, s_jid, m_msg);
										/*
					string found_censored = Utils.FormatEnvironmentVariables (Sh.S.GetMUC (s_jid).IsCensored (m_msg.Body, m_muc.OptionsHandler.GetOption ("global_censor") == "+"), r);
					@out.exe ("censor_next_stage");
					
					bool _warningExecuted = false;
					
					if (found_censored != null && Convert.ToInt32 (m_muc.OptionsHandler.GetOption ("user_warnings_count")) > 0) {
						if (m_user.WarningsCount < Convert.ToInt32 (m_muc.OptionsHandler.GetOption ("user_warnings_count"))) {
							m_user.WarningsCount++;
							_warningExecuted = true;
							
							@out.exe ("censor_next_warn");
							MessageType original_type = r.Msg.Type;
							r.Msg.Type = MessageType.groupchat;
							r.Reply (r.f ("censor_warning"));
							r.Msg.Type = original_type;
							@out.exe ("censor_next_sleeping");
							Sh.S.Sleep ();
							@out.exe ("censor_next_slept");
						}
					}
					
					if (found_censored != null && !_warningExecuted) {
						switch (m_muc.OptionsHandler.GetOption ("censor_result")) {
						case "kick":
							
							{
								@out.exe ("censor_next_kick");
								if (m_muc.KickableForCensored (m_user)) {
									m_muc.Kick (null, m_user, found_censored);
									return;
								} else {
									MessageType original_type = r.Msg.Type;
									r.Msg.Type = MessageType.groupchat;
									r.Reply (found_censored);
									r.Msg.Type = original_type;
									@out.exe ("censor_next_sleeping");
									Sh.S.Sleep ();
									
									@out.exe ("censor_next_slept");
								}
								break;
							}

						
						case "devoice":
							
							{
								@out.exe ("censor_next_devoice");
								if (m_muc.KickableForCensored (m_user)) {
									m_muc.Devoice (null, m_user, found_censored);
									return;
								} else {
									MessageType original_type = r.Msg.Type;
									r.Msg.Type = MessageType.groupchat;
									r.Reply (found_censored);
									r.Msg.Type = original_type;
									@out.exe ("censor_next_sleeping");
									Sh.S.Sleep ();
									@out.exe ("censor_next_slept");
								}
								break;
							}

						case "ban":
							
							{
								@out.exe ("censor_next_ban");
								if (m_muc.KickableForCensored (m_user)) {
									m_muc.Ban (null, m_user, found_censored);
									return;
								} else {
									MessageType original_type = r.Msg.Type;
									r.Msg.Type = MessageType.groupchat;
									r.Reply (found_censored);
									r.Msg.Type = original_type;
									@out.exe ("censor_next_sleeping");
									Sh.S.Sleep ();
									@out.exe ("censor_next_slept");
								}
								break;
							}

						
						case "warn":
							
							{
								@out.exe ("censor_next_warn");
								MessageType original_type = r.Msg.Type;
								r.Msg.Type = MessageType.groupchat;
								r.Reply (found_censored);
								r.Msg.Type = original_type;
								@out.exe ("censor_next_sleeping");
								Sh.S.Sleep ();
								@out.exe ("censor_next_slept");
								
							}

							break;
						default:
							break;
							
						}
					} else
						@out.exe ("censored_not_found");
					@out.exe ("censored_check_finished");
					
					*/

					// Checking for a message length limit
//NickLimit handlers
int _messageTextLimit = 500;
					try {
						_messageTextLimit = Convert.ToInt16 (m_muc.OptionsHandler.GetOption ("length_limit"));
					} catch (Exception err) {
					}
					
					if (m_msg.Body.Length > _messageTextLimit) {
						string censored = "Your message is too long and large than " + _messageTextLimit.ToString () + " characters.";
						switch (m_muc.OptionsHandler.GetOption ("length_limit_overflow_result")) {
						case "kick":
							
							
							{
								@out.exe ("censor_next_kick");
								if (m_muc.KickableForCensored (m_user)) {
									m_muc.Kick (null, m_user, censored);
									return;
								} else {
									MessageType original_type = r.Msg.Type;
									r.Msg.Type = MessageType.groupchat;
									r.Reply (censored);
									r.Msg.Type = original_type;
									@out.exe ("censor_next_sleeping");
									Sh.S.Sleep ();
									
									@out.exe ("censor_next_slept");
								}
								break;
							}

						
						
						case "devoice":
							
							
							{
								@out.exe ("censor_next_devoice");
								if (m_muc.KickableForCensored (m_user)) {
									m_muc.Devoice (null, m_user, censored);
									return;
								} else {
									MessageType original_type = r.Msg.Type;
									r.Msg.Type = MessageType.groupchat;
									r.Reply (censored);
									r.Msg.Type = original_type;
									@out.exe ("censor_next_sleeping");
									Sh.S.Sleep ();
									@out.exe ("censor_next_slept");
								}
								break;
							}

						
						case "ban":
							
							
							{
								@out.exe ("censor_next_ban");
								if (m_muc.KickableForCensored (m_user)) {
									m_muc.Ban (null, m_user, censored);
									return;
								} else {
									MessageType original_type = r.Msg.Type;
									r.Msg.Type = MessageType.groupchat;
									r.Reply (censored);
									r.Msg.Type = original_type;
									@out.exe ("censor_next_sleeping");
									Sh.S.Sleep ();
									@out.exe ("censor_next_slept");
								}
								break;
							}

						
						
						case "warn":
							
							
							{
								@out.exe ("censor_next_warn");
								MessageType original_type = r.Msg.Type;
								r.Msg.Type = MessageType.groupchat;
								r.Reply (censored);
								r.Msg.Type = original_type;
								@out.exe ("censor_next_sleeping");
								Sh.S.Sleep ();
								@out.exe ("censor_next_slept");
								
							}

							
							break;
						default:
							break;
							
						}
						//switch
					}
					//m_msg.Body.Length > _messageTextLimit
					
					//Initializing aliases
					if (!m_muc.HasAlias (m_body)) {
						#region Replace nick from command
						String _cmdBody = m_msg.Body;
						
						// split bot nick and command, to correct answer on commands like Pako: *muc echo test
						if (is_muser) {
							String[] _cmds = _cmdBody.Split (':');
							if (_cmds.Length > 0) {
								if (_cmds[0].TrimEnd (':').TrimEnd () == m_muc.MyNick) {
									_cmdBody = _cmdBody.Replace (m_muc.MyNick + ":", "").TrimStart ();
								}
							}
							
							//if (_cmds.Length > 0)
							//	@out.write(_cmds[0] + "\n");
							
							_cmds = _cmdBody.Split (',');
							if (_cmds.Length > 0) {
								if (_cmds[0].TrimEnd (',').TrimEnd () == m_muc.MyNick) {
									_cmdBody = _cmdBody.Replace (m_muc.MyNick + ",", "").TrimStart ();
								}
							}
							
							//if (_cmds.Length > 0)
							//	@out.write(_cmds[0] + "\n");
						}
						
						m_msg.Body = _cmdBody;
						
						//@out.write(m_msg.Body + "\n");
						
						#endregion
						
						#region Check prefix and remove it from message
						// if no alias fount
						@out.exe ("alias_not_found: stage1");
						if (!m_msg.Body.StartsWith (d)) {
							// If received phrase was started without command prefix
							if (_signed == CmdhState.PREFIX_REQUIRED)
								return;
						} else {
							// If a received phrase started vith a prefix
							if (_signed == CmdhState.PREFIX_NOT_POSSIBLE)
								return;
							
							m_msg.Body = m_msg.Body.Substring (d.Length);
							m_body = m_msg.Body;
							
							// if a message body have no text
							if (m_msg.Body.Trim () == "")
								return;
						}
						
						#endregion
						
					} else
						@out.exe ("alias_found");
				} else {
					@out.exe ("roster_user");
					
					#region Log
					//Write to log
					try {
						if (Sh.S.Config.EnableLogging && m_msg.Body != null && Sh.S.GetMUC (m_msg.From) == null) {
							Sh.S.HtmlPrivLogger.AddHtmlLog ("groupchat", "chat", m_msg.From.ToString (), m_msg.From.ToString (), m_msg.Body);
						}
					} catch (Exception exx) {
					}
					
					//topic
					try {
						if (Sh.S.Config.EnableLogging && m_msg.Body != null && Sh.S.GetMUC (m_msg.From) != null) {
							Sh.S.HtmlLogger.AddHtmlLog ("groupchat", "topic", m_msg.From.ToString (), "", m_msg.Body);
						}
					} catch (Exception ex) {
					}
					
					#endregion
					
					#region Check prefix and remove it from message
					
					if (!m_msg.Body.StartsWith (d)) {
						if (_signed == CmdhState.PREFIX_REQUIRED)
							return;
					} else {
						if (_signed == CmdhState.PREFIX_NOT_POSSIBLE)
							return;
						m_msg.Body = m_msg.Body.Substring (d.Length);
						m_body = m_msg.Body;
						if (m_msg.Body.Trim () == "")
							return;
					}
					
					#endregion
					
				}
				
				// Executing a command
				string m_retort = null;
				Cmd cmd = null;
				try {
					cmd = Cmd.CreateInstance (m_body, r, null);
					switch (cmd.Accessibility) {
					case CmdAccessibilityType.Accessible:
						cmd.Execute ();
						break;
					case CmdAccessibilityType.NotAccessible:
						r.Reply (r.f ("access_not_enough", cmd.CompleteAccess.ToString ()));
						break;
					case CmdAccessibilityType.AliasRecursion:
						r.Reply (r.f ("commands_recursion", Sh.S.Config.RecursionLevel.ToString ()));
						break;
					}
				} catch (Exception ex) {
				}
				
				// help out
				switch (cmd.Volume) {
				case "help":
					
					
					{
						string[] args = cmd.Args (1);
						
						if (args.Length > 1) {
							string data = args[1].Trim ();
							while (data.IndexOf ("  ") > -1)
								data = data.Replace ("  ", " ");
							m_retort = r.GetHelp (data.ToLower ());
							r.Format = false;
						} else
							m_retort = r.f ("help_info", Sh.S.PluginHandler.GetPluginsList (), d + "<volume> <command>");
						break;
					}

					
				}
				if (m_retort != null)
					r.Reply (m_retort);
				
			}
		}
		
	}

	public class HandleCensor
	{
		MUser m_user;
		SessionHandler Sh;
		Response r;
		MUC m_muc;
		Jid s_jid;
		Message m_msg;

		public HandleCensor (MUser pm_user, SessionHandler pSh, Response pr, MUC pm_muc, Jid ps_jid, Message pm_msg)
		{
			this.m_user = pm_user;
			this.Sh = pSh;
			this.r = pr;
			this.m_muc = pm_muc;
			this.s_jid = ps_jid;
			this.m_msg = pm_msg;
			
			Thread _th = new Thread(this._handle);
			_th.Start();
		}

		public void _handle ()
		{
			try{
				this.Handle();
			}
			catch {}
		}

		public void Handle ()
		{
			string found_censored = Utils.FormatEnvironmentVariables (Sh.S.GetMUC (s_jid).IsCensored (m_msg.Body, m_muc.OptionsHandler.GetOption ("global_censor") == "+"), r);
			@out.exe ("censor_next_stage");
			
			bool _warningExecuted = false;
			
			if (found_censored != null && Convert.ToInt32 (m_muc.OptionsHandler.GetOption ("user_warnings_count")) > 0) {
				if (m_user.WarningsCount < Convert.ToInt32 (m_muc.OptionsHandler.GetOption ("user_warnings_count"))) {
					m_user.WarningsCount++;
					_warningExecuted = true;
					
					@out.exe ("censor_next_warn");
					MessageType original_type = r.Msg.Type;
					r.Msg.Type = MessageType.groupchat;
					r.Reply (r.f ("censor_warning"));
					r.Msg.Type = original_type;
					@out.exe ("censor_next_sleeping");
					Sh.S.Sleep ();
					@out.exe ("censor_next_slept");
				}
			}
			
			if (found_censored != null && !_warningExecuted) {
				switch (m_muc.OptionsHandler.GetOption ("censor_result")) {
				case "kick":
					
					
					{
						@out.exe ("censor_next_kick");
						if (m_muc.KickableForCensored (m_user)) {
							m_muc.Kick (null, m_user, found_censored);
							return;
						} else {
							MessageType original_type = r.Msg.Type;
							r.Msg.Type = MessageType.groupchat;
							r.Reply (found_censored);
							r.Msg.Type = original_type;
							@out.exe ("censor_next_sleeping");
							Sh.S.Sleep ();
							
							@out.exe ("censor_next_slept");
						}
						break;
					}

				
				
				case "devoice":
					
					
					{
						@out.exe ("censor_next_devoice");
						if (m_muc.KickableForCensored (m_user)) {
							m_muc.Devoice (null, m_user, found_censored);
							return;
						} else {
							MessageType original_type = r.Msg.Type;
							r.Msg.Type = MessageType.groupchat;
							r.Reply (found_censored);
							r.Msg.Type = original_type;
							@out.exe ("censor_next_sleeping");
							Sh.S.Sleep ();
							@out.exe ("censor_next_slept");
						}
						break;
					}

				
				case "ban":
					
					
					{
						@out.exe ("censor_next_ban");
						if (m_muc.KickableForCensored (m_user)) {
							m_muc.Ban (null, m_user, found_censored);
							return;
						} else {
							MessageType original_type = r.Msg.Type;
							r.Msg.Type = MessageType.groupchat;
							r.Reply (found_censored);
							r.Msg.Type = original_type;
							@out.exe ("censor_next_sleeping");
							Sh.S.Sleep ();
							@out.exe ("censor_next_slept");
						}
						break;
					}

				
				
				case "warn":
					
					
					{
						@out.exe ("censor_next_warn");
						MessageType original_type = r.Msg.Type;
						r.Msg.Type = MessageType.groupchat;
						r.Reply (found_censored);
						r.Msg.Type = original_type;
						@out.exe ("censor_next_sleeping");
						Sh.S.Sleep ();
						@out.exe ("censor_next_slept");
						
					}

					
					break;
				default:
					break;
					
				}
			} else
				@out.exe ("censored_not_found");
			@out.exe ("censored_check_finished");
			
		}
	}
}
