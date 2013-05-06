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
using System.Collections;
using System.Threading;
using System.Text;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.protocol.x.muc;
using Core.Kernel;
using Core.Conference;
using Core.Other;
using Core.Xml;

using Core.Plugins;

namespace Core.Kernel
{
	public class PresenceHandler
	{
		Presence pres;
		SessionHandler Sh;

		public PresenceHandler (Presence m_pres, SessionHandler sh)
		{
			pres = m_pres;
			Sh = sh;
			Handle ();
		}
		public void Handle ()
		{
			try {
				_Handle ();
			} 
			catch (Exception ex) {
				
				string clrf = "\n";
				string msg_source = pres.ToString ();
				
				if (Utils.OS == Platform.Windows) {
					clrf = "\r\n";
					msg_source = pres.ToString ().Replace ("\n", "\r\n");
					
				}
				string data = "====== [" + DateTime.Now.ToString () + "] ===================================================>" + clrf + "   Stanza:" + clrf + msg_source + clrf + clrf + ex.ToString () + clrf + clrf + clrf;
				
				Sh.S.ErrorLoger.Write (data);
				
				if (Sh.S.Config.SendErrorMessagesToAdmin) {
					Message _msg = new Message ();
					_msg.To = Sh.S.Config.Administartion ()[0];
					_msg.Type = MessageType.chat;
					_msg.Body = "ERROR:   " + ex.ToString ();
					Sh.S.C.Send (_msg);
				}
			}
		}

		public void _Handle ()
		{
			// go away if presence is null
			if (pres == null)
				return;
			
			// Handle Muc-Join
			
			//pres.From = new Jid (pres.From.Bare.ToLower () + (!string.IsNullOrEmpty(pres.From.Resource) ? "/" + pres.From.Resource : ""));
			
			//@out.write ("before=" + pres.MucUser.Item.Jid.ToString() + "  ");
			
			//if (pres.MucUser != null && pres.MucUser.Item != null && pres.MucUser.Item.Jid != null)
			//	pres.MucUser.Item.Jid = new Jid (pres.MucUser.Item.Jid.Bare.ToLower () + (!string.IsNullOrEmpty(pres.MucUser.Item.Jid.Resource) ? "/" + pres.MucUser.Item.Jid.Resource : ""));
			
			//@out.write ("after=" + pres.MucUser.Item.Jid.ToString() + "  ");
			
			Jid p_jid = pres.From;
			MUC m_muc = Sh.S.GetMUC (p_jid);
			MUser m_user = null;
			Jid Jid = pres.From;
			if (pres.MucUser != null && pres.MucUser.Item.Jid != null)
				Jid = pres.MucUser.Item.Jid;
			
			switch (pres.Type) {
			case PresenceType.subscribe:
				if (Sh.S.Config.AutoSubscribe) {
					Presence pr = new Presence ();
					pr.To = pres.From;
					pr.Type = PresenceType.subscribed;
					pr.Nickname = new agsXMPP.protocol.extensions.nickname.Nickname (Sh.S.Config.Nick);
					Sh.S.C.Send (pr);
					pr.Type = PresenceType.subscribe;
					Sh.S.C.Send (pr);
				} else {
					Presence pr = new Presence ();
					pr.To = pres.From;
					pr.Type = PresenceType.unsubscribed;
					pr.Nickname = new agsXMPP.protocol.extensions.nickname.Nickname (Sh.S.Config.Nick);
					Sh.S.C.Send (pr);
				}
				break;
			
			case PresenceType.unsubscribe:
				Presence _pr = new Presence ();
				_pr.To = pres.From;
				_pr.Type = PresenceType.unsubscribed;
				_pr.Nickname = new agsXMPP.protocol.extensions.nickname.Nickname (Sh.S.Config.Nick);
				Sh.S.C.Send (_pr);
				_pr.Type = PresenceType.unsubscribe;
				Sh.S.C.Send (_pr);
				break;
			
			case PresenceType.error:
				if (m_muc != null) {
					
					if (p_jid.Resource == m_muc.MyNick) {
						Sh.S.DelMUC (p_jid);
						foreach (Jid j in Sh.S.Config.Administartion ()) {
							Message _msg = new Message ();
							_msg.To = j;
							_msg.Type = MessageType.chat;
							_msg.Body = p_jid.Bare + " => error: " + pres.Error.GetAttribute ("code") + " - " + pres.Error.Condition.ToString ();
							Sh.S.C.Send (_msg);
						}
					}
				}
				break;
			
			case PresenceType.available:
				string vl = null;
				if (m_muc != null)
					vl = m_muc.VipLang.GetLang (Jid);
				if (vl == null)
					vl = Sh.S.VipLang.GetLang (Jid);
				string lng = vl != null ? vl : m_muc != null ? m_muc.Language : Sh.S.Config.Language;
				
				
				
				if (pres.MucUser != null) {
					//if user is just joined...
					bool _justJoined = false;
					bool _affiliationChanged = false;
					bool _statusChanged = false;
					MUser _tempUser = null;
					string _oldStatus = null;
					Affiliation _oldAffiliation = Affiliation.none;
					if (m_muc.GetUser (pres.From.Resource) == null) {
						_justJoined = true;
					} else {
						//Checking for MucUser properties Get old status and access
						_tempUser = m_muc.GetUser (pres.From.Resource);
						_oldStatus = _tempUser.Status;
						_oldAffiliation = _tempUser.Affiliation;
					}
					///
					m_user = m_muc != null ? m_muc.GetUser (pres.From.Resource) : null;
					Jid calcjid = m_muc != null ? pres.From : new Jid (pres.From.Bare);
					
					
					Sh.S.CalcHandler.AddHandle (calcjid);
					int? access = Sh.S.GetAccess (pres, m_muc);
					
					long time;
					if (m_muc != null)
						@out.exe (m_user != null ? "[" + p_jid.User + "]*** " + p_jid.Resource + " is now " + pres.Show.ToString ().Replace ("NONE", "Online") : "[" + p_jid.User + "]*** " + p_jid.Resource + " enters the room as " + pres.MucUser.Item.Affiliation + "/" + pres.MucUser.Item.Role);
					time = m_user != null ? m_user.EnterTime : DateTime.Now.Ticks;
					MUser user = new MUser (pres.From.Resource, Jid, pres.MucUser.Item.Role, pres.MucUser.Item.Affiliation, pres.Status, pres.Show, m_muc != null ? m_muc.Language : Sh.S.Config.Language, time, access, m_user != null ? m_user.Idle : DateTime.Now.Ticks,
					"");
					
					// Checking version and vcard
					
					if (_justJoined == true && Sh.S.GetMUC (p_jid) != null) {
						VersionCB _version = new VersionCB (user, Sh.S.GetMUC (p_jid), Sh, lng, Sh.S.C);
						VCardCB _vcard = new VCardCB (user, Sh.S.GetMUC (p_jid), Sh, lng, Sh.S.C);
					}
					
					if (Sh.S.GetMUC (p_jid) != null)
						Sh.S.GetMUC (p_jid).SetUser (m_user, user);
					@out.exe ("LANGUAGE=" + lng);
					Response r = new Response (Sh.S.Rg[lng]);
					r.MUC = m_muc;
					r.MUser = user;
					r.Sh = Sh;
					
					
					if (m_muc != null && Sh.S.GetMUC (p_jid) != null) {
						string ak;
						m_muc = Sh.S.GetMUC (p_jid);
						
						#region Join logging and whowas
						// muc whowas adder
						if (m_muc.WhoWas != null && _justJoined == true) {
							//m_muc.WhoWas.Append(pres.From + "\n");
							if (!m_muc.WhoWas.ToString ().Contains (user.Nick + "\n")) {
								m_muc.WhoWas.Append (user.Nick + "\n");
							}
							
							//Add seen log entries
							Sh.S.SeenLogger.AddSeenEntry (user.Jid.ToString (), m_muc.Jid.ToString (), user.Nick);
							
							//Add html log entry - join
							if (Sh.S.Config.EnableLogging && m_muc.OptionsHandler.GetOption ("enable_logging") == "+") {
								Sh.S.HtmlLogger.AddHtmlLog ("groupchat", "subscribe", m_muc.Jid.ToString (), user.Nick, " has joined to chat as " + user.Affiliation.ToString ());
							}
						}
						
						if (_oldAffiliation != Affiliation.none && _oldAffiliation != user.Affiliation && m_muc.OptionsHandler.GetOption ("enable_logging") == "+") {
							if (Sh.S.Config.EnableLogging) {
								Sh.S.HtmlLogger.AddHtmlLog ("groupchat", "affiliation", m_muc.Jid.ToString (), user.Nick, " was " + _oldAffiliation + " and become " + user.Affiliation.ToString ());
							}
						}
						
						if (_oldStatus != null && _oldStatus != user.Status && m_muc.OptionsHandler.GetOption ("enable_logging") == "+") {
							if (Sh.S.Config.EnableLogging) {
								Sh.S.HtmlLogger.AddHtmlLog ("groupchat", "status", m_muc.Jid.ToString (), user.Nick, " old status was '" + _oldStatus + "' and become " + user.Status + "'");
							}
						}
						
						//if (Sh.S.Config.EnableLogging && user != null && m_muc.Subject != null)
						//{
						//    Sh.S.HtmlLogger.AddHtmlLog("groupchat", "topic", m_muc.Jid.ToString(), user.Nick, m_muc.Subject);
						//}
						
						#endregion
						
						#region Automation
						
						if (m_muc.OptionsHandler.GetOption ("akick") == "+") {
							ak = Sh.S.Tempdb.IsAutoKick (Jid, p_jid.Resource, p_jid, Sh);
							@out.exe ("censored: " + (ak ?? "NULL"));
							if (ak != null) {
								
								if (m_muc.KickableForCensored (user)) {
									@out.exe ("censored: yes");
									m_muc.Kick (null, user, Utils.FormatEnvironmentVariables (ak, r));
									
									// TODO: Logging autokick
									return;
								}
							}
						}
						
						if (m_muc.OptionsHandler.GetOption ("aban") == "+") {
							ak = Sh.S.Tempdb.IsAutoBan (Jid, p_jid.Resource, p_jid, Sh);
							@out.exe ("censored: " + (ak ?? "NULL"));
							if (ak != null) {
								
								if (m_muc.KickableForCensored (user)) {
									@out.exe ("censored: yes");
									m_muc.Ban (null, user, Utils.FormatEnvironmentVariables (ak, r));
									
									// TODO: Logging autokick
									return;
								}
							}
						}
						
						if (m_muc.OptionsHandler.GetOption ("avisitor") == "+") {
							
							ak = Sh.S.Tempdb.IsAutoVisitor (Jid, p_jid.Resource, p_jid, Sh);
							if (ak != null) {
								if (m_muc.KickableForCensored (user)) {
									m_muc.Devoice (null, user, Utils.FormatEnvironmentVariables (ak, r));
									return;
								}
								
							}
						}
						
						if (m_muc.OptionsHandler.GetOption ("amoderator") == "+") {
							if (Sh.S.Tempdb.AutoModerator (Jid, m_muc.Jid))
								m_muc.Moderator (null, user, null);
						}
						
						#endregion
						
						#region Censor : Status
						
						if ((pres.Status != null) && (user != m_muc.MyNick)) {
							// TODO: Add censore by nick and resource
							string censored = Sh.S.GetMUC (p_jid).IsCensored (pres.Status, m_muc.OptionsHandler.GetOption ("global_censor") == "+");
							if (censored != null) {
								//TODO: Censored logging
								@out.exe (m_muc.KickableForCensored (user).ToString ());
								switch (m_muc.OptionsHandler.GetOption ("censor_result")) {
								case "kick":
									
									{
										if (m_muc.KickableForCensored (user)) {
											m_muc.Kick (null, user, censored);
											return;
										} else {
											Message msg = new Message ();
											r.Msg = new Message ();
											r.Msg.Body = pres.Status;
											r.Msg.From = pres.From;
											r.Msg.Type = MessageType.groupchat;
											r.Reply (censored);
											Sh.S.Sleep ();
										}
									}

									break;
								case "devoice":
									
									{
										if (m_muc.KickableForCensored (user)) {
											m_muc.Devoice (null, user, censored);
											return;
										} else {
											Message msg = new Message ();
											r.Msg = new Message ();
											r.Msg.Body = pres.Status;
											r.Msg.From = pres.From;
											r.Msg.Type = MessageType.groupchat;
											r.Reply (censored);
											Sh.S.Sleep ();
										}
										
									}

									break;
								case "ban":
									
									{
										if (m_muc.KickableForCensored (user)) {
											m_muc.Ban (null, user, censored);
											return;
										} else {
											Message msg = new Message ();
											r.Msg = new Message ();
											r.Msg.Body = pres.Status;
											r.Msg.From = pres.From;
											r.Msg.Type = MessageType.groupchat;
											r.Reply (censored);
											Sh.S.Sleep ();
										}
										
										
									}

									break;
								case "warn":
									
									{
										Message msg = new Message ();
										r.Msg = new Message ();
										r.Msg.Body = pres.Status;
										r.Msg.From = pres.From;
										r.Msg.Type = MessageType.groupchat;
										r.Reply (censored);
										Sh.S.Sleep ();
									}

									break;
								default:
									break;
									
								}
							}
						}
						
						#endregion
						
						Affiliation _aff = Affiliation.none;
						if (m_muc.OptionsHandler.GetOption ("vcensor_affiliation") == "none")
							_aff = Affiliation.none;
						if (m_muc.OptionsHandler.GetOption ("vcensor_affiliation") == "member")
							_aff = Affiliation.member;
						
						bool _executeRCensor = false;
						if (user.Affiliation == _aff)
							_executeRCensor = true;
						if (m_muc.OptionsHandler.GetOption ("vcensor_affiliation") == "visitor")
							if (user.Affiliation == Affiliation.none && user.Role == Role.visitor)
								_executeRCensor = true;
						
						#region Censor : Resourse
						
						ResourceCensorHandler _rc = new ResourceCensorHandler(user, Sh, r, m_muc, pres, p_jid, _executeRCensor);
												/*
                            if ((user.Jid.Resource != null) && (user != m_muc.MyNick) && _executeRCensor)
                            {
                                // TODO: Add censore by nick and resource
                                string censored = Sh.S.GetMUC(p_jid).IsVRCensored(user.Jid.Resource, m_muc.OptionsHandler.GetOption("global_censor") == "+", "res");
                                if (censored != null)
                                {
                                    //TODO: Censored logging
                                    @out.exe(m_muc.KickableForCensored(user).ToString());
                                    switch (m_muc.OptionsHandler.GetOption("rcensor_result"))
                                    {
                                        case "kick":
                                            {
                                                if (m_muc.KickableForCensored(user))
                                                { m_muc.Kick(null, user, censored); return; }
                                                else
                                                {
                                                    Message msg = new Message();
                                                    r.Msg = new Message();
                                                    r.Msg.Body = pres.Status;
                                                    r.Msg.From = pres.From;
                                                    r.Msg.Type = MessageType.groupchat;
                                                    r.Reply(censored);
                                                    Sh.S.Sleep();
                                                }
                                            } break;
                                        case "devoice":
                                            {
                                                if (m_muc.KickableForCensored(user))
                                                { m_muc.Devoice(null, user, censored); return; }
                                                else
                                                {
                                                    Message msg = new Message();
                                                    r.Msg = new Message();
                                                    r.Msg.Body = pres.Status;
                                                    r.Msg.From = pres.From;
                                                    r.Msg.Type = MessageType.groupchat;
                                                    r.Reply(censored);
                                                    Sh.S.Sleep();
                                                }
                                                       
                                            }
                                            break;
                                        case "ban":
                                            {
                                                if (m_muc.KickableForCensored(user))
                                                { m_muc.Ban(null, user, censored); return; }
                                                else
                                                {
                                                    Message msg = new Message();
                                                    r.Msg = new Message();
                                                    r.Msg.Body = pres.Status;
                                                    r.Msg.From = pres.From;
                                                    r.Msg.Type = MessageType.groupchat;
                                                    r.Reply(censored);
                                                    Sh.S.Sleep();
                                                }
                                                 
                                                  
                                            }
                                            break;
                                        case "warn":
                                            {
                                                Message msg = new Message();
                                                r.Msg = new Message();
                                                r.Msg.Body = pres.Status;
                                                r.Msg.From = pres.From;
                                                r.Msg.Type = MessageType.groupchat;
                                                r.Reply(censored);
                                                Sh.S.Sleep();
                                            }
                                            break;
                                        default:
                                            break;

                                    }
                                }
                            }// end of resource censor
						*/

						#endregion

						#region Censor: NickLimit

						//NickLimit handlers
						int _nickLimit = 10;
						try {
							_nickLimit = Convert.ToInt16 (m_muc.OptionsHandler.GetOption ("nick_limit"));
						} catch (Exception err) {
						}
						
						if (user.Nick.Length > _nickLimit) {
							string censored = "Your nick is too big. It large than " + _nickLimit.ToString ();
							switch (m_muc.OptionsHandler.GetOption ("nick_limit_result")) {
							case "kick":
								
								{
									if (m_muc.KickableForCensored (user)) {
										m_muc.Kick (null, user, m_muc.MyNick + ">> " + censored);
										return;
									} else {
										Message msg = new Message ();
										r.Msg = new Message ();
										r.Msg.Body = pres.Status;
										r.Msg.From = pres.From;
										r.Msg.Type = MessageType.groupchat;
										r.Reply (censored);
										Sh.S.Sleep ();
									}
								}

								break;
							case "devoice":
								
								{
									if (m_muc.KickableForCensored (user)) {
										m_muc.Devoice (null, user, censored);
										return;
									} else {
										Message msg = new Message ();
										r.Msg = new Message ();
										r.Msg.Body = pres.Status;
										r.Msg.From = pres.From;
										r.Msg.Type = MessageType.groupchat;
										r.Reply (censored);
										Sh.S.Sleep ();
									}
									
								}

								break;
							case "ban":
								
								{
									if (m_muc.KickableForCensored (user)) {
										m_muc.Ban (null, user, m_muc.MyNick + ">> " + censored);
										return;
									} else {
										Message msg = new Message ();
										r.Msg = new Message ();
										r.Msg.Body = pres.Status;
										r.Msg.From = pres.From;
										r.Msg.Type = MessageType.groupchat;
										r.Reply (censored);
										Sh.S.Sleep ();
									}
									
									
								}

								break;
							case "warn":
								
								{
									Message msg = new Message ();
									r.Msg = new Message ();
									r.Msg.Body = pres.Status;
									r.Msg.From = pres.From;
									r.Msg.Type = MessageType.groupchat;
									r.Reply (censored);
									Sh.S.Sleep ();
								}

								break;
							default:
								break;
								
							}
							//switch
						}
						//if (user.Nick.Length > _nickLimit)
						#endregion
						
						#region Greet
						
						if (m_user == null) {
							string data = Sh.S.Tempdb.Greet (Jid, m_muc.Jid);
							if (data != null) {
								@out.exe (">> " + data);
								
								r.Msg = new Message ();
								r.Msg.From = pres.From;
								r.Msg.Type = MessageType.groupchat;
								r.Reply (Utils.FormatEnvironmentVariables (data, r));
							}
						}
						
						#endregion
						
						#region Tell
						
						ArrayList ar = Sh.S.Tempdb.CheckAndAnswer (p_jid);
						if (ar.Count > 0) {
							foreach (string[] phrase in ar) {
								Jid _sender = new Jid (phrase[2]);
								string s_sender = phrase[2];
								s_sender = _sender.Resource;
								r.Msg = new Message ();
								r.Msg.From = pres.From;
								r.Msg.Type = MessageType.chat;
								r.Reply (r.f ("said_to_tell", s_sender, phrase[1], phrase[3]));
								r.Msg.Type = MessageType.groupchat;
								r.Reply (r.f ("private_notify"));
								Sh.S.Sleep ();
								
							}
						}
						
						#endregion
						
					}
				} else {
					Sh.S.RosterJids.Add (pres.From);
				}
				break;
			
			case PresenceType.unavailable:
				m_user = m_muc != null ? m_muc.GetUser (pres.From.Resource) : null;
				Jid _calcjid = m_muc != null ? pres.From : new Jid (pres.From.Bare);
				Sh.S.CalcHandler.DelHandle (_calcjid);
				
				if (pres.MucUser != null) {
					m_user = m_muc != null ? m_muc.GetUser (pres.From.Resource) : null;
					
					if (m_user != null) {
						//Add html log entry - join
						if (Sh.S.Config.EnableLogging && m_muc.OptionsHandler.GetOption ("enable_logging") == "+") {
							Sh.S.HtmlLogger.AddHtmlLog ("groupchat", "subscribe", m_muc.Jid.ToString (), m_user.Nick, " as " + m_user.Affiliation.ToString () + " has leave chatroom.");
						}
						Sh.S.GetMUC (p_jid).DelUser (m_user);
						// Leaving room loging
						@out.exe ("[" + p_jid.User + "]*** " + p_jid.Resource + " leaves the room");
					} else
						return;
					if (p_jid.Resource == m_muc.MyNick) {
						if (pres.MucUser != null) {
							if (pres.MucUser.Item.Nickname != null) {
								if (pres.MucUser.Item.Nickname != p_jid.Resource) {
									Sh.S.GetMUC (p_jid).MyNick = pres.MucUser.Item.Nickname;
								}
							} else {
								
								Sh.S.DelMUC (p_jid);
								foreach (Jid j in Sh.S.Config.Administartion ()) {
									Message _msg = new Message ();
									_msg.To = j;
									_msg.Type = MessageType.chat;
									string data = p_jid.Bare + " => " + pres.Type.ToString ();
									if (pres.HasTag ("x"))
										if (pres.SelectSingleElement ("x").HasTag ("status"))
											if (pres.SelectSingleElement ("x").SelectSingleElement ("status").HasAttribute ("code"))
												data += " (" + pres.SelectSingleElement ("x").SelectSingleElement ("status").GetAttribute ("code") + ")";
									_msg.Body = data;
									Sh.S.C.Send (_msg);
								}
							}
						}
					}
				}
				break;
			}
		}
	}

	class ResourceCensorHandler
	{
		MUser user;
		SessionHandler Sh;
		Response r;
		MUC m_muc;
		Presence pres;
		Jid p_jid;
		bool _executeRCensor;
		
		public ResourceCensorHandler(MUser puser, SessionHandler pSh, Response pr, MUC pm_muc, Presence ppres, Jid pp_jid, bool pexecuteRCensor)
		{
			this.user = puser;
			this.Sh = pSh;
			this.r = pr;
			this.m_muc = pm_muc;
			this.pres = ppres;
			this.p_jid = pp_jid;
			this._executeRCensor = pexecuteRCensor;
			
			Thread _th = new Thread(this._handle);
			_th.Start();
		}
		
		public void _handle()
		{
			try{
				this.Handle();
			}
			catch{}
		}

		public void Handle ()
		{
			if ((user.Jid.Resource != null) && (user != m_muc.MyNick) && _executeRCensor) {
				// TODO: Add censore by nick and resource
				string censored = Sh.S.GetMUC (p_jid).IsVRCensored (user.Jid.Resource, m_muc.OptionsHandler.GetOption ("global_censor") == "+", "res");
				if (censored != null) {
					//TODO: Censored logging
					@out.exe (m_muc.KickableForCensored (user).ToString ());
					switch (m_muc.OptionsHandler.GetOption ("rcensor_result")) {
					case "kick":
						
						{
							if (m_muc.KickableForCensored (user)) {
								m_muc.Kick (null, user, censored);
								return;
							} else {
								Message msg = new Message ();
								r.Msg = new Message ();
								r.Msg.Body = pres.Status;
								r.Msg.From = pres.From;
								r.Msg.Type = MessageType.groupchat;
								r.Reply (censored);
								Sh.S.Sleep ();
							}
						}

						break;
					case "devoice":
						
						{
							if (m_muc.KickableForCensored (user)) {
								m_muc.Devoice (null, user, censored);
								return;
							} else {
								Message msg = new Message ();
								r.Msg = new Message ();
								r.Msg.Body = pres.Status;
								r.Msg.From = pres.From;
								r.Msg.Type = MessageType.groupchat;
								r.Reply (censored);
								Sh.S.Sleep ();
							}
							
						}

						break;
					case "ban":
						
						{
							if (m_muc.KickableForCensored (user)) {
								m_muc.Ban (null, user, censored);
								return;
							} else {
								Message msg = new Message ();
								r.Msg = new Message ();
								r.Msg.Body = pres.Status;
								r.Msg.From = pres.From;
								r.Msg.Type = MessageType.groupchat;
								r.Reply (censored);
								Sh.S.Sleep ();
							}
							
							
						}

						break;
					case "warn":
						
						{
							Message msg = new Message ();
							r.Msg = new Message ();
							r.Msg.Body = pres.Status;
							r.Msg.From = pres.From;
							r.Msg.Type = MessageType.groupchat;
							r.Reply (censored);
							Sh.S.Sleep ();
						}

						break;
					default:
						break;
						
					}
				}
			}
			// end of resource censor
		}
	}
}
