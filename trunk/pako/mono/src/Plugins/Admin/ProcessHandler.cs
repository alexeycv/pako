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
	public class ConfigHandler
	{
		string[] ws;
		static Message m_msg;
		static Response m_r;
		string self;
		string d;
		bool syntax_error = false;
		Jid s_jid;
		string m_b;
		string n = "Admin";
		SessionHandler Sh;
		bool _isInAdminList = false;


		public ConfigHandler (Response r)
		{
			Sh = r.Sh;
			m_b = r.Msg.Body;
			ws = Utils.SplitEx (m_b, 2);
			m_msg = r.Msg;
			m_r = r;
			s_jid = r.Msg.From;
			d = r.Delimiter;
			
			if (r.MUser != null) {
				if (Sh.S.Config.AdminInMuc == false) {
					r.Reply (r.f ("rosteronly"));
					return;
				}
			}
			
			// If extended security is enabled
			if (Sh.S.Config.EnhancedSecurity == true) {
				foreach (Jid admin in Sh.S.Config.Administartion ()) {
					Message msg = new Message ();
					msg.To = admin;
					msg.Body = "Admin command was launched by: " + m_r.Msg.From.ToString () + "\nCommand:\n" + m_b;
					msg.Type = MessageType.chat;
					Sh.S.C.Send (msg);
					if (m_r.Msg.From.ToString ().IndexOf (admin.ToString ()) == 0) {
						_isInAdminList = true;
					} else {
						return;
					}
				}
			}
			
			if (ws.Length < 2) {
				r.Reply (r.f ("volume_info", n, d + n.ToLower () + " list"));
				return;
			}
			
			self = ws[0] + " " + ws[1];
			Handle ();
			
			
			
		}

		/// <summary>
		/// Finds a file in a specified directory with a specified name
		/// <example>
		/// Find(@"C:\","pako*","Files found: {1}, Dirs found: {2}","Nothing found");
		/// </example>
		/// </summary>
		/// <param name="root"></param>
		/// <param name="file"></param>
		/// <param name="found_pattern"></param>
		/// <param name="not_found_pattern"></param>
		/// <returns></returns>
		public static string Find (string root, string file, string found_pattern, string not_found_pattern)
		{
			string _local_res = "";
			int files_found = 0;
			int dirs_found = 0;
			foreach (string _file in Directory.GetFiles (root, file, SearchOption.AllDirectories)) {
				files_found++;
				_local_res += "   <" + Path.GetFileName (_file) + ">    " + _file + "\n";
			}
			foreach (string _dir in Directory.GetDirectories (root, file, SearchOption.AllDirectories)) {
				dirs_found++;
				_local_res += "   [" + Path.GetFileName (_dir) + "]    " + _dir + "\n";
			}
			return _local_res != "" ? _local_res.Substring (1) + found_pattern.Replace ("{1}", files_found.ToString ()).Replace ("{2}", dirs_found.ToString ()) : not_found_pattern;
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
					rs = m_r.f ("volume_list", n) + "\nlist, emul, censor, config, set_help, set_pattern, get_pattern, pl_load, pl_info, pl_unload, proc_show, proc_kill, proc_new, muc_lang, muc_status, muc_nick, cmdaccess, dict_clear, vipaccess, viplang, gmsg, cmd, quit, restart, find, langs, eval, myroot, errors, pako, heap, sql";
					break;
				}

			
			case "emul":
				
				{
					//*admin emul roso@conference.jabber.dom muc echo gg!!!
					ws = Utils.SplitEx (m_b, 3);
					if (ws.Length > 3) {
						if (ws[2].Trim () != "all") {
							Jid muc = Sh.S.GetMUCJid(ws[2]) ?? new Jid (ws[2]);
							
							if (Sh.S.GetMUC (muc) != null) {
								Message msg = new Message ();
								msg.From = new Jid (muc.Bare + "/" + Sh.S.GetMUC (muc).MyNick);
								msg.Type = MessageType.chat;
								msg.Body = ws[3].Trim ();
								CommandHandler cmdh = new CommandHandler (msg, Sh, m_r.Msg, CmdhState.PREFIX_NOT_POSSIBLE, m_r.Level);
							} else
								rs = m_r.f ("muc_not_in");
						}
					else{
						Dictionary<Jid, MUC> mucs = Sh.S.MUCs;
						foreach (MUC m in mucs.Values) {
							Jid muc = m.Jid;
							Message msg = new Message ();
							msg.From = new Jid (muc.Bare + "/" + Sh.S.GetMUC (muc).MyNick);
							msg.Type = MessageType.chat;
							msg.Body = ws[3].Trim ();
							CommandHandler cmdh = new CommandHandler (msg, Sh, m_r.Msg, CmdhState.PREFIX_NOT_POSSIBLE, m_r.Level);
							Sh.S.Sleep ();
							Sh.S.Sleep ();
						}
					}
					} else
						syntax_error = true;
					break;
				}

				
			case "sql":
				{
					rs = "Not implemented yet";
				break;
				}
			
			case "muc_lang":
				
				{
					ws = Utils.SplitEx (m_b, 3);
					if (ws.Length > 3) {
						string lang = ws[3].Trim ();
						if (Sh.S.Rg.GetResponse (lang) != null) {
							if (Sh.S.AutoMucManager.SetLanguage (new Jid (ws[2]), lang)) {
								if (Sh.S.GetMUC (new Jid (ws[2])) != null) {
									Sh.S.GetMUC (new Jid (ws[2])).Language = lang;
									if (m_r.MUC != null) {
										if (m_r.MUC.Jid.ToString () == ws[2]) {
											m_r.Document = Sh.S.Rg.GetResponse (lang).Document;
											m_r.Language = lang;
										}
									}
									
									@out.exe (Sh.S.GetMUC (new Jid (ws[2])).Language);
								}
								
								rs = m_r.Agree ();
							} else
								syntax_error = true;
						} else
							rs = m_r.f ("lang_pack_not_found", lang);
						
					} else
						syntax_error = true;
					
					break;
				}

			
			
			case "censor":
				
				{
					ws = Utils.SplitEx (m_b, 3);
					if (ws.Length > 2) {
						if (ws[2] == "del") {
							if (ws.Length > 3) {
								if (Sh.S.Censor.DelRoomCensor (ws[3]))
									rs = m_r.f ("censor_deleted");
								else
									rs = m_r.f ("censor_not_existing");
							} else
								syntax_error = true;
						} else if (ws[2] == "show") {
							if ((ws.Length == 3)) {
								string data = Sh.S.Censor.GetRoomCensorList ("{1}) {2}   =>   \"{3}\"");
								rs = data != null ? data : m_r.f ("censor_list_empty");
							} else
								syntax_error = true;
						} else if (ws[2] == "clear") {
							if ((ws.Length == 3)) {
								Sh.S.Censor.ClearCensor ();
								rs = m_r.f ("censor_list_cleared");
							} else
								syntax_error = true;
						} else {
							
							string reason = null;
							if (m_b.IndexOf ("||") > -1) {
								reason = m_b.Substring (m_b.IndexOf ("||") + 2).Trim () == "" ? null : m_b.Substring (m_b.IndexOf ("||") + 2).Trim ();
								
								if (reason != null)
									m_b = m_b.Remove (m_b.IndexOf ("||"));
							}
							if (reason == null)
								reason = m_r.f ("kick_censored_reason");
							ws = Utils.SplitEx (m_b, 2);
							Sh.S.Censor.AddCensor (ws[2].Trim (), reason);
							rs = m_r.Agree ();
						}
					} else
						syntax_error = true;
					break;
				}

			
			case "muc_status":
				
				{
					ws = Utils.SplitEx (m_b, 3);
					if (ws.Length > 3) {
						string status = ws[3].Trim ();
						if (Sh.S.AutoMucManager.SetStatus (new Jid (ws[2]), status)) {
							if (Sh.S.GetMUC (new Jid (ws[2])) != null) {
								Presence pres = new Presence ();
								pres.To = new Jid (ws[2]);
								pres.Status = status;
								pres.Show = Sh.S.GetMUC (new Jid (ws[2])).MyShow;
								m_r.Connection.Send (pres);
								
							}
							
							rs = m_r.Agree ();
						} else
							syntax_error = true;
						
					} else
						syntax_error = true;
					
					break;
				}

			
			
			case "muc_nick":
				
				{
					ws = Utils.SplitEx (m_b, 3);
					if (ws.Length > 3) {
						string nick = ws[3].Trim ();
						if (Sh.S.AutoMucManager.SetStatus (new Jid (ws[2]), nick)) {
							if (Sh.S.GetMUC (new Jid (ws[2])) != null) {
								Sh.S.GetMUC (new Jid (ws[2])).ChangeNick (ws[3]);
							}
							
							rs = m_r.Agree ();
						} else
							syntax_error = true;
						
					} else
						syntax_error = true;
					
					break;
				}

			
			case "config":
				
				{
					ws = Utils.SplitEx (m_b, 3);
					switch (ws.Length) {
					case 4:
						rs = (Sh.S.Config.SetTag (ws[2], ws[3])) ? m_r.Agree () : m_r.f ("config_parameter_not_found", ws[2]);
						break;
					case 3:
						rs = Sh.S.Config.GetTag (ws[2]) ?? m_r.f ("config_parameter_not_found", ws[2]);
						break;
					default:
						syntax_error = true;
						break;
					}
					break;
				}

			
			
			case "cmdaccess":
				
				{
					
					string _cmd = Utils.GetValue (m_b, "[(.*)]").Trim ();
					m_b = Utils.RemoveValue (m_b, "[(.*)]", true);
					ws = Utils.SplitEx (m_b, 3);
					
					if (ws.Length > 2) {
						if (ws[2] == "show") {
							string acc = m_r.f ("access");
							rs = "\n[========" + acc + "========]";
							ListDictionary list = Sh.S.AccessManager.GetCommands ();
							foreach (string cm in list.Keys) {
								string _access = ((int)list[cm]).ToString ();
								rs += "\n" + "[" + cm + "]";
								for (int i = 1; i <= 28 - (2 + cm.Length) - _access.Length; i++)
									rs += i == 1 || i == 28 - (2 + cm.Length) - _access.Length ? " " : i % 2 == 1 ? " " : ".";
								rs += _access;
							}
							rs = list.Count == 0 ? m_r.f ("access_list_empty") : rs + "\n[==========================]";
							break;
						} else if (ws[2] == "del") {
							
							if (ws.Length == 3 && _cmd != "") {
								if (Sh.S.AccessManager.DelCommand (_cmd))
									rs = m_r.Agree ();
								else
									rs = m_r.f ("commands_not_existing");
							} else
								syntax_error = true;
							break;
						} else {
							
							if (_cmd != "") {
								
								try {
									Sh.S.AccessManager.SetAccess (_cmd, Convert.ToInt32 (ws[2]));
									rs = m_r.Agree ();
								} catch {
									syntax_error = true;
								}
								
							} else
								syntax_error = true;
						}
					} else if (_cmd != "") {
						Cmd cm = Cmd.CreateInstance (_cmd, m_r, true);
						rs = cm.CompleteAccess.ToString ();
					} else
						syntax_error = true;
					
					
					break;
				}

			
			
			case "quit":
				
				{
					
					foreach (Jid admin in Sh.S.Config.Administartion ()) {
						Message msg = new Message ();
						msg.To = admin;
						msg.Body = m_r.f ("admin_leave");
						msg.Type = MessageType.chat;
						Sh.S.C.Send (msg);
					}
					Sh.S.turn_off = true;
					if (ws.Length > 2)
						Sh.S.Exit (m_r, ws[2]);
					else
						Sh.S.Exit (m_r);
					// Environment.Exit(0);
					return;
				}

			
			case "restart":
				
				{
					if (ws.Length > 2)
						Sh.S.Exit (m_r, ws[2]);
					else
						Sh.S.Exit (m_r);
					return;
				}

			
			case "myroot":
				
				{
					if (ws.Length == 2) {
						//access level must be 100
						if (m_r.Access == 100) {
							rs = Utils.CD;
						}
					} else
						syntax_error = true;
					break;
				}

			
			case "cmd":
				
				{
					if (ws.Length > 2) {
						if (Sh.S.Config.AlloweCmd) {
							//access level must be 100
							if (m_r.Access == 100) {
								Stdior std = new Stdior ();
								rs = std.Execute (ws[2], Sh.S);
							}
						} else {
							rs = m_r.f ("admin_restricted");
						}
						break;
					} else
						syntax_error = true;
					
					break;
				}

			
			case "dict_clear":
				
				{
					if (ws.Length == 2) {
						Sh.Defs.Clear ();
						rs = m_r.Agree ();
						break;
					} else
						syntax_error = true;
					
					break;
				}

			
			case "set_help":
				
				{
					string _cmd = Utils.GetValue (m_b, "[(.*)]").Trim ();
					m_b = Utils.RemoveValue (m_b, "[(.*)]", true);
					ws = Utils.SplitEx (m_b, 3);
					
					
					if ((_cmd != "") && (ws.Length > 3)) {
						if (Sh.S.Rg.GetResponse (ws[2]) != null) {
							Sh.S.Rg.GetResponse (ws[2]).SetHelp (_cmd, ws[3]);
							rs = m_r.Agree ();
						} else
							rs = m_r.f ("lang_pack_not_found", ws[2]);
						
					} else
						syntax_error = true;
					
					break;
					
					
				}

			
			case "set_pattern":
				
				{
					
					ws = Utils.SplitEx (m_b, 4);
					
					if (ws.Length > 4) {
						if (Sh.S.Rg.GetResponse (ws[2]) != null) {
							Sh.S.Rg.GetResponse (ws[2]).SetPattern (ws[3], ws[4]);
							rs = m_r.Agree ();
						} else
							rs = m_r.f ("lang_pack_not_found", ws[2]);
						
					} else
						syntax_error = true;
					break;
				}

			
			case "get_pattern":
				
				{
					
					ws = Utils.SplitEx (m_b, 4);
					
					if (ws.Length > 3) {
						if (Sh.S.Rg.GetResponse (ws[2]) != null) {
							rs = Sh.S.Rg.GetResponse (ws[2]).f (ws[3]);
						} else
							rs = m_r.f ("lang_pack_not_found", ws[2]);
						
					} else
						syntax_error = true;
					break;
				}

			
			case "pl_load":
				
				{
					if (ws.Length > 2) {
						string data = Sh.S.PluginHandler.LoadPlugin (ws[2]);
						if (data != null) {
							rs = data == "plugin_loaded" ? m_r.f ("plugin_loaded") : data;
						} else
							rs = m_r.f ("plugin_load_failed");
					} else
						syntax_error = true;
					break;
				}

			
			case "pl_info":
				
				{
					if (ws.Length > 2) {
						
						if (Sh.S.PluginHandler.Handles (ws[2])) {
							string plug = ws[2];
							foreach (string pl_name in Sh.S.PluginHandler.Plugins.Keys) {
								if (pl_name.ToLower () == plug.ToLower ())
									plug = pl_name;
							}
							IPlugin pl = ((IPlugin)Sh.S.PluginHandler.Plugins[plug]);
							rs = "<" + pl.File + "> " + pl.Name + " [" + pl.Comment + "]";
						} else
							rs = m_r.f ("plugin_not_existing");
						
						
					} else
						syntax_error = true;
					break;
				}

			
			case "pl_unload":
				
				{
					
					if (ws.Length > 2) {
						rs = m_r.f (Sh.S.PluginHandler.UnloadPlugin (ws[2].ToLower ()) ? "plugin_unloaded" : "plugin_not_existing");
					} else
						syntax_error = true;
					break;
				}

			
			case "errors":
				
				{
					
					if (ws.Length == 2) {
						
						rs = Sh.S.ErrorLoger.Read ();
						if (rs.Trim ('\n', ' ') == "")
							rs = m_r.f ("error_list_empty");
					} else
						syntax_error = true;
					break;
				}

			
			case "langs":
				
				{
					if (ws.Length == 2) {
						rs = Sh.S.Rg.GetPacketsList (true);
					} else
						syntax_error = true;
					break;
				}

			
			case "vipaccess":
				
				{
					ws = Utils.SplitEx (m_b, 3);
					if (ws.Length > 2) {
						if (ws[2] == "del") {
							
							if (ws.Length > 3) {
								try {
									if (Sh.S.VipAccess.DelVip (Convert.ToInt32 (ws[3])))
										rs = m_r.f ("vip_deleted");
									else
										rs = m_r.f ("vip_not_found", ws[3]);
								} catch {
									if (Sh.S.VipAccess.DelVip (new Jid (ws[3])))
										rs = m_r.f ("vip_deleted");
									else
										rs = m_r.f ("vip_not_found", ws[3]);
								}
							} else
								syntax_error = true;
						} else if (ws[2] == "clear") {
							if (ws.Length == 3) {
								Sh.S.VipAccess.Clear ();
								rs = m_r.f ("vip_list_cleared");
							} else
								syntax_error = true;
							
						} else if (ws[2] == "show") {
							if (ws.Length == 3) {
								rs = Sh.S.VipAccess.GetAllVips ("{1}) {2} : {3}");
								if (rs == null)
									rs = m_r.f ("vip_list_empty");
							} else {
								Jid Jid;
								if (m_r.MUC != null) {
									MUser user = m_r.MUC.GetUser (ws[3]);
									if (user != null)
										if (user.Jid.Bare != m_r.Msg.From.Bare)
											Jid = user.Jid;
										else
											Jid = new Jid (ws[3]);
									else
										Jid = new Jid (ws[3]);
								} else
									Jid = new Jid (ws[3]);
								
								if (!Utils.JidValid (Jid)) {
									rs = m_r.f ("jid_not_valid", Jid.ToString ());
									break;
								} else {
									int? acc = Sh.S.VipAccess.GetAccess (Jid);
									rs = acc == null ? m_r.f ("vip_not_found", Jid.ToString ()) : acc.ToString ();
								}
							}
							
						} else if (ws[2] == "count") {
							if (ws.Length == 3) {
								rs = Sh.S.VipAccess.Count ().ToString ();
							} else
								syntax_error = true;
							
						} else {
							if (ws.Length > 3) {
								Jid Jid;
								if (m_r.MUC != null) {
									MUser user = m_r.MUC.GetUser (ws[2]);
									if (user != null)
										if (user.Jid.Bare != m_r.Msg.From.Bare)
											Jid = user.Jid;
										else
											Jid = new Jid (ws[2]);
									else
										Jid = new Jid (ws[2]);
								} else
									Jid = new Jid (ws[2]);
								
								if (!Utils.JidValid (Jid)) {
									rs = m_r.f ("jid_not_valid", Jid.ToString ());
									break;
								} else {
									try {
										int ac = Convert.ToInt32 (ws[3]);
										if ((ac > 100) || (ac < 0)) {
											syntax_error = true;
											break;
										}
										Sh.S.VipAccess.AddVip (Jid, ac);
										foreach (MUC m in Sh.S.MUCs.Values) {
											foreach (MUser u in m.Users.Values) {
												if (u.Jid.Bare == Jid.Bare)
													Sh.S.GetMUC (m.Jid).GetUser (u).Access = ac;
											}
										}
										
										rs = m_r.Agree ();
									} catch {
										syntax_error = true;
									}
								}
								
							} else {
								syntax_error = true;
							}
							
							
							
							
						}
					} else
						syntax_error = true;
					break;
				}

			
			
			
			case "viplang":
				
				{
					ws = Utils.SplitEx (m_b, 3);
					if (ws.Length > 2) {
						
						if (ws[2] == "del") {
							
							if (ws.Length > 3) {
								try {
									if (Sh.S.VipLang.DelVip (Convert.ToInt32 (ws[3])))
										rs = m_r.f ("vip_deleted");
									else
										rs = m_r.f ("vip_not_found", ws[3]);
								} catch {
									if (Sh.S.VipLang.DelVip (new Jid (ws[3])))
										rs = m_r.f ("vip_deleted");
									else
										rs = m_r.f ("vip_not_found", ws[3]);
								}
							} else
								syntax_error = true;
						} else if (ws[2] == "clear") {
							if (ws.Length == 3) {
								Sh.S.VipLang.Clear ();
								rs = m_r.f ("vip_list_cleared");
							} else
								syntax_error = true;
							
						} else if (ws[2] == "show") {
							if (ws.Length == 3) {
								rs = Sh.S.VipLang.GetAllVips ("{1}) {2} : {3}");
								if (rs == null)
									rs = m_r.f ("vip_list_empty");
							} else {
								if (!Utils.JidValid (ws[3])) {
									rs = m_r.f ("jid_not_valid", ws[3]);
									break;
								} else {
									rs = Sh.S.VipLang.GetLang (new Jid (ws[3]));
									if (rs == null)
										rs = m_r.f ("vip_not_found", ws[3]);
								}
							}
							
						} else if (ws[2] == "count") {
							if (ws.Length == 3) {
								rs = Sh.S.VipLang.Count ().ToString ();
							} else
								syntax_error = true;
							
						} else {
							
							if (ws.Length > 3) {
								Jid Jid = new Jid (ws[2]);
								if (!Utils.JidValid (Jid)) {
									rs = m_r.f ("jid_not_valid", Jid.ToString ()).ToString ();
									break;
								} else {
									
									if (Sh.S.Rg.GetResponse (ws[3]) != null) {
										Sh.S.VipLang.AddVip (Jid, ws[3]);
										rs = m_r.Agree ();
									} else
										rs = m_r.f ("lang_pack_not_found", ws[3]);
									
								}
								
							} else {
								syntax_error = true;
							}
							
							
						}
					} else
						syntax_error = true;
					break;
				}

			
			
			
			
			case "find":
				
				{
					string word = Utils.GetValue (m_b, "[(.*)]").Trim ();
					m_b = Utils.RemoveValue (m_b, "[(.*)]", true);
					ws = Utils.SplitEx (m_b, 2);
					
					if ((ws.Length > 2) && (word != "")) {
						rs = Find (ws[2].Trim (), word, m_r.f ("files_found"), m_r.f ("files_not_found"));
					} else
						syntax_error = true;
					break;
				}

			
			
			
			case "pako":
				
				{
					if (ws.Length == 2) {
						Process p = Process.GetCurrentProcess ();
						ProcessModule pm = p.MainModule;
						rs = p.MainModule.FileVersionInfo.ToString ();
					} else
						syntax_error = true;
					break;
				}

			
			case "gmsg":
				
				{
					//*admin gmsg $ru privet druzi!!
					ws = Utils.SplitEx (m_b, 3);
					if (ws.Length > 2) {
						string lang = null;
						string phrase = null;
						if ((ws[2].StartsWith ("$")) && (ws[2].Length != 1)) {
							if (Sh.S.Rg.GetResponse (ws[2].Substring (1)) != null) {
								lang = ws[2].Substring (1);
								if (ws.Length > 3)
									phrase = ws[3];
							} else {
								ws = Utils.SplitEx (m_b, 2);
								phrase = ws[2];
								
							}
						} else {
							ws = Utils.SplitEx (m_b, 2);
							phrase = ws[2];
						}
						
						if (phrase == null) {
							syntax_error = true;
							break;
						}
						Dictionary<Jid, MUC> mucs = Sh.S.MUCs;
						rs = m_r.Agree ();
						
						foreach (MUC m in mucs.Values) {
							if ((lang != null) && (m.Language != lang))
								continue;
							Message msg = new Message ();
							msg.To = m.Jid;
							msg.Type = MessageType.groupchat;
							msg.Body = phrase;
							m_r.Connection.Send (msg);
							
							
						}
					} else
						syntax_error = true;
					break;
					
				}

			case "heap":
				
				{
					
					if (ws.Length == 2)
						rs = new decimal (Environment.WorkingSet / 1000000).ToString () + " mb";
					else
						syntax_error = true;
					break;
				}

			
			
			
			
			case "eval":
				
				{
					
					if (ws.Length > 2) {
						Evaluator ev = new Evaluator ();
						string data = ev.Compile (ws[2], Sh, m_r);
						if (data == null)
							rs = m_r.f ("eval_error");
						else
							rs = data;
					} else
						syntax_error = true;
					break;
				}

			case "proc_show":
				
				{
					if (ws.Length == 2) {
						Process[] p = Process.GetProcesses ();
						string str = "";
						int i = 1;
						foreach (Process pp in p) {
							str += i.ToString () + ") " + pp.ProcessName + "  " + Convert.ToString (pp.WorkingSet / 1000) + "  " + pp.BasePriority + "\n";
							i++;
						}
						str += m_r.f ("all_processes") + (i - 1).ToString ();
						rs = str;
					} else
						syntax_error = true;
					break;
				}

			
			case "proc_kill":
				
				{
					
					if (ws.Length > 2) {
						Process[] p = Process.GetProcesses ();
						bool killed = false;
						int number;
						try {
							
							number = Convert.ToInt32 (ws[2]);
							if ((number > 0) && (number <= p.Length)) {
								p[number - 1].Kill ();
								killed = true;
							} else
								killed = false;
						} catch {
							killed = false;
						}
						
						if (killed)
							rs = m_r.f ("process_killed");
						else
							rs = m_r.f ("process_not_found", ws[2]);
						
						
					} else
						syntax_error = true;
					
					
					break;
				}

			
			case "proc_new":
				
				{
					if (ws.Length > 2) {
						try {
							string str = Environment.ExpandEnvironmentVariables (ws[2]);
							ProcessStartInfo si = new ProcessStartInfo (str);
							
							Process p = Process.Start (si);
							rs = m_r.f ("process_started");
						} catch {
							rs = m_r.f ("process_not_found", ws[2]);
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
