/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Pako Jabber-bot. Bbodio's Lab.                                                *
 * Copyright. All rights reserved © 2007-2008 by Klichuk Bogdan (Bbodio's Lab)   *
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
using System.Collections.Specialized;
using System.Collections;
using System.Text;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;
using System.Threading;
using System.IO;
using Core.Kernel;
using Core.Conference;
using Core.Other;
using Core.Plugins;
using agsXMPP.protocol.x.muc;
using agsXMPP.protocol.x.muc.iq.admin;
using agsXMPP.protocol.x.muc.iq.owner;
using System.Diagnostics;
using Mono.Data.SqliteClient;

namespace Plugin
{

	/// <summary>
	/// An Admin plugin handler
	/// </summary>
	public class SchedulerHandler
	{
		string[] ws;
		static Message m_msg;
		static Response m_r;
		string self;
		string d;
		bool syntax_error = false;
		Jid s_jid;
		string m_b;
		string n = "scheduler";
		SessionHandler Sh;
		bool _isInAdminList = false;


		public SchedulerHandler (Response r)
		{
			Sh = r.Sh;
			m_b = r.Msg.Body;
			ws = Utils.SplitEx (m_b, 2);
			m_msg = r.Msg;
			m_r = r;
			s_jid = r.Msg.From;
			d = r.Delimiter;
			
			if (ws.Length < 2) {
				r.Reply (r.f ("volume_info", n, d + n.ToLower () + " list"));
				return;
			}
			
			self = ws[0] + " " + ws[1];
			Handle ();
			
			
			
		}

		/// <summary>
		/// Handles a plug-in
		/// </summary>
		public void Handle ()
		{
			
			string cmd = ws[1].ToLower ();
			string rs = null;
			switch (cmd) {
			case "list":
				
				{
					rs = m_r.f ("volume_list", n) + "\nlist, tsaks, add, del, show, set_type, set_command, set_time, set_period";
					break;
				}

			
			case "test1":
				
				{
					rs = "Passed";
					
					break;
				}

			case "test":
				
				{
					if (ws.Length > 2) {
						string cmds_source = ws[2].Trim ();
						string[] cmds = cmds_source.Split (new string[] { "&&" }, StringSplitOptions.RemoveEmptyEntries);
					
					#region Response
					/*
					Jid m_jid = _mucUser != null ? _mucUser.Jid : _fromJid;
			
					string vl = null;
					if (_muc != null)
						vl = _muc.VipLang.GetLang (m_jid);
				
					if (vl == null)
						vl = Session.S.VipLang.GetLang (m_jid);
			
					Response r = new Response (Session.S.Rg[vl != null ? vl : _mucUser != null ? _mucUser.Language : Session.S.Config.Language]);
			
					// Get user access
					int? access = 0;
					access = Session.S.GetAccess (msg, _mucUser, _muc);
			
					if (access != null)
						r.Access = access;
					else
						r.Access = 0;
			
					r.Msg = msg;
					r.MSGLimit = Session.S.Config.MucMSGLimit;
					r.MUC = _muc;
					r.Level = level;
					r.MUser = _mucUser;
					r.Delimiter = Session.S.Config.Delimiter;
			;
					r.Sh = Session;
			
					//@out.write ("Response OK");					
					*/
					#endregion
					
						
						//m_r.Connection.Send (msg);
					
						if (cmds.Length > Sh.S.Config.RecursionLevel) {
							m_r.Reply (m_r.f ("commands_recursion", Sh.S.Config.RecursionLevel.ToString ()));
							break;
						}
						foreach (string _cmd in cmds) {
							//Message msg = new Message ();
							//object obj = m_r.Msg;
							//msg = (Message)obj;
							Message msg = new Message ();
							msg.To = new Jid("pako-dev@conference.jabber.ru");
							msg.From = new Jid(m_r.MUC.Jid.Bare + "/" + m_r.MUC.MyNick);//m_r.Msg.From;//Sh.S.C.MyJID;
							msg.Type = MessageType.groupchat;
							msg.Body = _cmd.Trim (' ', '\n');
						
							//if (m_r.MUC.UserExists(arg))
							//	Jid = new Jid(m_r.MUC.Jid.Bare + "/" + arg);
							//else
							//	Jid = new Jid(arg);
						
							//CommandHandler cmd_handler = new CommandHandler (msg, Sh, m_r.Emulation, CmdhState.PREFIX_NOT_POSSIBLE, m_r.Level, true, 10);
							Sh.S.Sleep ();
						}
					} else
						syntax_error = true;
					
					break;
				}

			default:
				
				
				{
					rs = m_r.f ("volume_cmd_not_found", n, ws[1], d + n.ToLower () + " list");
					break;
				}

				
			}
			
			if (syntax_error)
				m_r.se (self); else if (rs != null)
				m_r.Reply (rs);
			
		}
		
	}
}
