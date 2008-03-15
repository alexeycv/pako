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

        public ConfigHandler(Response r)
        {
            Sh = r.Sh;
            m_b = r.Msg.Body;
            ws = Utils.SplitEx(m_b, 2);
            m_msg = r.Msg;
            m_r = r;
            s_jid = r.Msg.From;
            d = r.Delimiter;
          

          
            if (ws.Length < 2)
            {
                r.Reply(r.f("volume_info", n, d + n.ToLower() + " list"));
                return;
            }

            self = ws[0] + " " + ws[1];
            Handle();



        }


        static public string Find(string root, string file, string found_pattern, string not_found_pattern)
        {
            string _local_res = "";
            int files_found = 0;
            int dirs_found = 0;
            foreach (string _file in Directory.GetFiles(root, file, SearchOption.AllDirectories)) { files_found++; _local_res += "   <" + Path.GetFileName(_file) + ">    " + _file + "\n"; }
            foreach (string _dir in Directory.GetDirectories(root, file, SearchOption.AllDirectories)) { dirs_found++; _local_res += "   [" + Path.GetFileName(_dir) + "]    " + _dir + "\n"; }
            return _local_res != "" ? _local_res.Substring(1) + found_pattern.Replace("{1}", files_found.ToString()).Replace("{2}", dirs_found.ToString()) :
                not_found_pattern;
        }




        public void Handle()
        {




            string cmd = ws[1];
            string rs = null;
            switch (cmd)
            {
                case "list":
                    {
                        rs = m_r.f("volume_list", n) + "\nlist, jid, nick, password, port, rectime, reconnects, cmdaccess, msglimit, lang, status, prefix, usessl, compression, starttls, pl_load, pl_unload, pl_info, errors, set_help, set_pattern, cmd, restart, quit, muc_lang, muc_status, muc_nick, myroot, find, proc_new, proc_kill, proc_show, vip, gmsg, eval, vip_del, censor, uncensor, allcensor";
                        break;
                    }

                case "jid":
                    {
                        if (ws.Length > 2)
                        {
                            Sh.S.Config.Jid = new Jid(ws[2]);
                            rs = m_r.Agree();
                        }
                        else
                            rs = Sh.S.Config.Jid.ToString();
                        break;
                    }

                case "muc_lang":
                    {
                            ws = Utils.SplitEx(m_b, 3);
                            if (ws.Length > 3)
                            {
                                string lang = ws[3].Trim();
                                if (Sh.S.Rg.GetResponse(lang) != null)
                                {
                                    if (Sh.S.AutoMucManager.SetLanguage(new Jid(ws[2]), lang))
                                    {
                                        if (Sh.S.GetMUC(new Jid(ws[2])) != null)
                                        {
                                            Sh.S.GetMUC(new Jid(ws[2])).Language = lang;
                                            if (m_r.MUC != null)
                                            {
                                                if (m_r.MUC.Jid.ToString() == ws[2])
                                                {
                                                    m_r.Document = Sh.S.Rg.GetResponse(lang).Document;
                                                    m_r.Language = lang;
                                                }
                                            }

                                            @out.exe(Sh.S.GetMUC(new Jid(ws[2])).Language);
                                        }

                                        rs = m_r.Agree();
                                    }
                                    else
                                        syntax_error = true;
                                }
                                else
                                    rs = m_r.f("lang_pack_not_found", lang);

                            }
                            else
                                syntax_error = true;
                       
                        break;
                    }

                case "muc_status":
                    {
                            ws = Utils.SplitEx(m_b, 3);
                            if (ws.Length > 3)
                            {
                                string status = ws[3].Trim();
                                if (Sh.S.AutoMucManager.SetStatus(new Jid(ws[2]), status))
                                {
                                    if (Sh.S.GetMUC(new Jid(ws[2])) != null)
                                    {
                                        Presence pres = new Presence();
                                        pres.To = new Jid(ws[2]);
                                        pres.Status = status;
                                        pres.Show = Sh.S.GetMUC(new Jid(ws[2])).MyShow;
                                        m_r.Connection.Send(pres);

                                    }

                                    rs = m_r.Agree();
                                }
                                else
                                    syntax_error = true;

                            }
                            else
                                syntax_error = true;
                       
                        break;
                    }


                case "muc_nick":
                    {
                            ws = Utils.SplitEx(m_b, 3);
                            if (ws.Length > 3)
                            {
                                string nick = ws[3].Trim();
                                if (Sh.S.AutoMucManager.SetStatus(new Jid(ws[2]), nick))
                                {
                                    if (Sh.S.GetMUC(new Jid(ws[2])) != null)
                                    {
                                        Sh.S.GetMUC(new Jid(ws[2])).ChangeNick(ws[3]);
                                    }

                                    rs = m_r.Agree();
                                }
                                else
                                    syntax_error = true;

                            }
                            else
                                syntax_error = true;
                       
                        break;
                    }

                case "nick":
                    {
                        if (ws.Length > 2)
                        {
                            Sh.S.Config.Nick = ws[2];
                            rs = m_r.Agree();
                        }
                        else
                            rs = Sh.S.Config.Nick;
                        break;
                    }

                case "password":
                    {
                        if (ws.Length > 2)
                        {
                            Sh.S.Config.Password = ws[2];
                            rs = m_r.Agree();
                        }
                        else
                            rs = Sh.S.Config.Password;
                        break;
                    }

                case "cmdaccess":
                    {
                        string _cmd = Utils.GetValue(m_b, "[(.*)]");
                        m_b = Utils.RemoveValue(m_b, "[(.*)]", true);
                        ws = Utils.SplitEx(m_b, 3);
                        if ((ws.Length > 2) && (_cmd != ""))
                        {
                            try
                            {
                                Sh.S.AccessManager.SetAccess(_cmd, Convert.ToInt32(ws[2]));
                                rs = m_r.Agree();
                            }
                            catch
                            {
                                syntax_error = true;
                            }
                        }
                        else
                        {
                            int access;

                            if (m_r.MUC != null)
                            {
                                access = Sh.S.AccessManager.GetAccess(_cmd);

                                if (access == -1)
                                {
                                    string a = "", b = "";
                                    _cmd = m_r.MUC.GetAlias(_cmd, ref a, ref b);
                                    access = Sh.S.AccessManager.GetAccess(_cmd);
                                }
                                
                            }
                            else
                                access = Sh.S.AccessManager.GetAccess(_cmd);
                                rs = access.ToString();
                        }
                        break;
                    }

                case "censor":
                    {
                       ws = Utils.SplitEx(m_b, 3);
                        if ((ws.Length > 2))
                        {

                            string reason = m_r.f("kick_censored_reason");
                            if (ws.Length == 4)
                                reason = ws[3];
                            Sh.S.Censor.AddCensor(ws[2], reason);
                            rs = m_r.Agree();
                        }
                        else
                            syntax_error = true;
                        break;
                    }

                case "uncensor":
                    {
                        if ((ws.Length > 2))
                        {
                            if (Sh.S.Censor.DelRoomCensor(ws[2]))
                                rs = m_r.Agree();
                            else
                                rs = m_r.Deny();
                        }
                        else
                            syntax_error = true;
                        break;
                    }

                case "allcensor":
                    {
                        if ((ws.Length == 2))
                        {
                            string data = Sh.S.Censor.GetRoomCensorList("{1}) {2} => \"{3}\"");
                            rs = data != null ? data : m_r.f("censor_list_empty");
                        }
                        else
                            syntax_error = true;
                        break;
                    }

                case "status":
                    {
                        if (ws.Length > 2)
                        {
                            Sh.S.Config.Status = ws[2];
                            rs = m_r.Agree();
                        }
                        else
                            rs = Sh.S.Config.Status;
                        break;
                    }

                case "prefix":
                    {
                        if (ws.Length > 2)
                        {
                            Sh.S.Config.Delimiter = ws[2];
                            rs = m_r.Agree();
                        }
                        else
                            rs = Sh.S.Config.Delimiter;
                        break;
                    }

                case "usessl":
                    {
                        if (ws.Length > 2)
                        {
                            try
                            {
                                Sh.S.Config.UseSSL = Convert.ToBoolean(ws[2]);
                                rs = m_r.Agree();
                            }
                            catch
                            {
                                syntax_error = true;
                            }

                        }
                        else
                            rs = Sh.S.Config.UseSSL.ToString();
                        break;
                    }

                case "compression":
                    {
                        if (ws.Length > 2)
                        {
                            try
                            {
                                Sh.S.Config.UseCompression = Convert.ToBoolean(ws[2]);
                                rs = m_r.Agree();
                            }
                            catch
                            {
                                syntax_error = true;
                            }

                        }
                        else
                            rs = Sh.S.Config.UseCompression.ToString();
                        break;
                    }

                case "debug":
                    {
                        if (ws.Length > 2)
                        {
                            try
                            {
                                Sh.S.Config.Debug = Convert.ToBoolean(ws[2]);
                                @out.Debug = true;
                                rs = m_r.Agree();
                            }
                            catch
                            {
                                syntax_error = true;
                            }

                        }
                        else
                            rs = Sh.S.Config.Debug.ToString();
                        break;
                    }

                case "autosub":
                    {
                        if (ws.Length > 2)
                        {
                            try
                            {
                                Sh.S.Config.AutoSubscribe = Convert.ToBoolean(ws[2]);
                                rs = m_r.Agree();
                            }
                            catch
                            {
                                syntax_error = true;
                            }

                        }
                        else
                            rs = Sh.S.Config.AutoSubscribe.ToString();
                        break;
                    }

                case "starttls":
                    {
                        if (ws.Length > 2)
                        {
                            try
                            {
                                Sh.S.Config.UseStartTls = Convert.ToBoolean(ws[2]);
                                rs = m_r.Agree();
                            }
                            catch
                            {
                                syntax_error = true;
                            }

                        }
                        else
                            rs = Sh.S.Config.UseStartTls.ToString();
                        break;
                    }

                case "quit":
                    {

                        foreach (Jid admin in Sh.S.Config.Administartion())
                        {
                            Message msg = new Message();
                            msg.To = admin;
                            msg.Body = m_r.f("admin_leave");
                            msg.Type = MessageType.chat;
                            Sh.S.C.Send(msg);
                        }

                        if (ws.Length > 2)
                            Sh.S.Exit(m_r, ws[2]);
                        else
                            Sh.S.Exit(m_r);
                        Environment.Exit(0);
                        return;
                    }

                case "restart":
                    {
                        if (ws.Length > 2)
                            Sh.S.Exit(m_r, ws[2]);
                        else
                            Sh.S.Exit(m_r);
                        return;
                    }

                case "myroot":
                    {
                        rs = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                        break;
                    }

                case "cmd":
                    {
                        if (ws.Length > 2)
                        {
                            Stdior std = new Stdior();
                            rs = std.Execute(ws[2]);
                            break;
                        }
                        else
                            syntax_error = true;

                        break;
                    }

                case "dict_clear":
                    {
                        if (ws.Length == 2)
                        {
                            Sh.Defs.Clear();
                            rs = m_r.Agree();
                            break;
                        }
                        else
                            syntax_error = true;

                        break;
                    }

                case "set_help":
                    {
                        string _cmd = Utils.GetValue(m_b, "[(.*)]");
                        m_b = Utils.RemoveValue(m_b, "[(.*)]", true);
                        ws = Utils.SplitEx(m_b, 3);


                        if ((_cmd != "") && (ws.Length > 3))
                        {
                            if (Sh.S.Rg.GetResponse(ws[2]) != null)
                            {
                                Sh.S.Rg.GetResponse(ws[2]).SetHelp(_cmd, ws[3]);
                                rs = m_r.Agree();
                            }
                            else
                                rs = m_r.f("lang_pack_not_found", ws[2]);

                        }
                        else
                            syntax_error = true;

                        break;


                    }

                case "set_pattern":
                    {

                        ws = Utils.SplitEx(m_b, 4);

                        if (ws.Length > 4)
                        {
                            if (Sh.S.Rg.GetResponse(ws[2]) != null)
                            {
                                Sh.S.Rg.GetResponse(ws[2]).SetPattern(ws[3], ws[4]);
                                rs = m_r.Agree();
                            }
                            else
                                rs = m_r.f("lang_pack_not_found", ws[2]);

                        }
                        else
                            syntax_error = true;
                        break;
                    }

                case "get_pattern":
                    {

                        ws = Utils.SplitEx(m_b, 4);

                        if (ws.Length > 3)
                        {
                            if (Sh.S.Rg.GetResponse(ws[2]) != null)
                            {
                               rs = Sh.S.Rg.GetResponse(ws[2]).f(ws[3]);
                            }
                            else
                                rs = m_r.f("lang_pack_not_found", ws[2]);

                        }
                        else
                            syntax_error = true;
                        break;
                    }

                case "pl_load":
                    {
                        if (ws.Length > 2)
                        {
                            string data = Sh.S.PluginHandler.LoadPlugin(ws[2]);
                            if (data != null)
                            {
                                rs = data == "plugin_loaded" ? m_r.f("plugin_loaded") : data;
                            }
                            else
                                rs = m_r.f("plugin_load_failed");
                        }
                        else
                            syntax_error = true;
                        break;
                    }

                case "pl_info":
                    {
                        if (ws.Length > 2)
                        {

                            if (Sh.S.PluginHandler.Handles(ws[2]))
                            {
                                IPlugin pl = ((IPlugin)Sh.S.PluginHandler.Plugins[ws[2]]);
                                rs = "<" + pl.File + "> " + pl.Name + " [" + pl.Comment + "]";
                            }
                            else
                                rs = m_r.f("plugin_not_existing");


                        }
                        else
                            syntax_error = true;
                        break;
                    }

                case "pl_unload":
                    {
                        if (ws.Length > 2)
                        {
                            rs = m_r.f(Sh.S.PluginHandler.UnloadPlugin(ws[2]) ? "plugin_unloaded" : "plugin_not_existing");
                        }
                        else
                            syntax_error = true;
                        break;
                    }
                case "port":
                    {
                        if (ws.Length > 2)
                        {
                            try
                            {
                                Sh.S.Config.Port = Convert.ToInt32(ws[2]);
                                rs = m_r.Agree();
                            }
                            catch
                            {
                                rs = m_r.Deny();
                            }

                        }
                        else
                            rs = Sh.S.Config.Port.ToString();
                        break;
                    }

                case "rectime":
                    {
                        if (ws.Length > 2)
                        {
                            try
                            {
                                Sh.S.Config.ReconnectTime = Convert.ToInt32(ws[2]);
                                rs = m_r.Agree();
                            }
                            catch
                            {
                                rs = m_r.Deny();
                            }

                        }
                        else
                            rs = Sh.S.Config.Port.ToString();
                        break;
                    }


                case "reconnects":
                    {
                        if (ws.Length > 2)
                        {
                            try
                            {
                                Sh.S.Config.MaxReconnects = Convert.ToInt32(ws[2]);
                                rs = m_r.Agree();
                            }
                            catch
                            {
                                rs = m_r.Deny();
                            }
                        }
                        else
                            rs = Sh.S.Config.MaxReconnects.ToString();
                        break;
                    }




                case "msglimit":
                    {
                        if (ws.Length > 2)
                        {
                            try
                            {
                                Sh.S.Config.MSGLimit = Convert.ToInt32(ws[2]);
                                rs = m_r.Agree();
                            }
                            catch
                            {
                                rs = m_r.Deny();
                            }

                        }
                        else
                            rs = Sh.S.Config.MSGLimit.ToString();
                        break;
                    }



                case "lang":
                    {

                        if (ws.Length > 2)
                        {
                            if (Sh.S.Rg.GetResponse(ws[2]) != null)
                            {
                                Sh.S.Config.Language = ws[2];
                                rs = m_r.Agree();
                            }
                            else
                            {
                                rs = m_r.f("lang_pack_not_found", ws[2]);
                            }
                        }
                        else

                            rs = Sh.S.Config.Language;
                        break;
                    }

                case "errors":
                    {

                        if (ws.Length == 2)
                        {

                            rs = Sh.S.ErrorLoger.Read();
                            if (rs.Trim('\n', ' ') == "")
                                rs = m_r.f("error_list_empty");
                        }
                        else
                            syntax_error = true;
                        break;
                    }

                case "langs":
                    {
                       if (ws.Length == 2)
                        {
                            rs = Sh.S.Rg.GetPacketsList(true);
                        }
                        else
                            syntax_error = true;
                        break;
                    }

                case "vipaccess":
                    {
                        ws = Utils.SplitEx(m_b, 3);
                        if (ws.Length > 2)
                        {
                            if (ws[2] == "del")
                            {
  
                                if (ws.Length > 3)
                                {
                                    try
                                    {
                                        if (Sh.S.VipAccess.DelVip(Convert.ToInt32(ws[3])))
                                            rs = m_r.f("vip_deleted");
                                        else
                                            rs = m_r.f("vip_not_found", ws[3]);
                                    }
                                    catch
                                    {
                                        if (Sh.S.VipAccess.DelVip(new Jid(ws[3])))
                                            rs = m_r.f("vip_deleted");
                                        else
                                            rs = m_r.f("vip_not_found", ws[3]);
                                    }
                                }
                                else
                                    syntax_error = true;     
                            }
                                else
                                if (ws[2] == "clear")
                                {
                                    if (ws.Length == 3)
                                    {
                                        Sh.S.VipAccess.Clear();
                                        rs = m_r.f("vip_list_cleared");
                                    }
                                    else
                                        syntax_error = true;

                                }
                                else
                                    if (ws[2] == "show")
                                    {
                                        if (ws.Length == 3)
                                        {
                                            rs = Sh.S.VipAccess.GetAllVips("{1}) {2} : {3}");
                                            if (rs == null)
                                                rs = m_r.f("vip_list_empty");
                                        }
                                        else
                                        {
                                            if (!Sh.S.JidValid(ws[3]))
                                            {
                                                rs = m_r.f("jid_not_valid", ws[3]);
                                                break;
                                            }
                                            else
                                            {
                                                rs = Sh.S.VipAccess.GetAccess(new Jid(ws[3])).ToString();
                                                if (rs == "-1")
                                                    rs = m_r.f("vip_not_found", ws[3]);
                                            }
                                        }

                                    }
                                    else
                                        if (ws[2] == "count")
                                        {
                                            if (ws.Length == 3)
                                            {
                                                rs = Sh.S.VipAccess.Count().ToString();
                                            }
                                            else
                                                syntax_error = true;

                                        }
                                else
                                {
                                    if (ws.Length > 3)
                                    {
                                        Jid Jid = new Jid(ws[2]);
                                        if (!Sh.S.JidValid(Jid))
                                        {
                                            rs = m_r.f("jid_not_valid", Jid.ToString());
                                            break;
                                        }
                                        else
                                        {
                                            try
                                            {
                                                int ac = Convert.ToInt32(ws[3]);
                                                if ((ac > 100) || (ac < 0))
                                                {
                                                    syntax_error = true;
                                                    break;
                                                }
                                                Sh.S.VipAccess.AddVip(Jid, ac);
                                                rs = m_r.Agree();
                                            }
                                            catch
                                            {
                                                syntax_error = true;
                                            }
                                        }

                                    }
                                    else
                                    {
                                        syntax_error = true;
                                    }




                                }
                        }
                        else
                            syntax_error = true;
                        break;
                        }



                case "viplang":
                    {
                        ws = Utils.SplitEx(m_b, 3);
                        if (ws.Length > 2)
                        {

                            if (ws[2] == "del")
                            {

                                if (ws.Length > 3)
                                {
                                    try
                                    {
                                        if (Sh.S.VipLang.DelVip(Convert.ToInt32(ws[3])))
                                            rs = m_r.f("vip_deleted");
                                        else
                                            rs = m_r.f("vip_not_found", ws[3]);
                                    }
                                    catch
                                    {
                                        if (Sh.S.VipLang.DelVip(new Jid(ws[3])))
                                            rs = m_r.f("vip_deleted");
                                        else
                                            rs = m_r.f("vip_not_found", ws[3]);
                                    }
                                }
                                else
                                    syntax_error = true;     
                            }
                                else
                                if (ws[2] == "clear")
                                {
                                    if (ws.Length == 3)
                                    {
                                        Sh.S.VipLang.Clear();
                                        rs = m_r.f("vip_list_cleared");
                                    }
                                    else
                                        syntax_error = true;

                                }
                                else
                                    if (ws[2] == "show")
                                    {
                                        if (ws.Length == 3)
                                        {
                                            rs = Sh.S.VipLang.GetAllVips("{1}) {2} : {3}");
                                            if (rs == null)
                                                rs = m_r.f("vip_list_empty");
                                        }
                                        else
                                        {
                                            if (!Sh.S.JidValid(ws[3]))
                                            {
                                                rs = m_r.f("jid_not_valid", ws[3]);
                                                break;
                                            }
                                            else
                                            {
                                                rs = Sh.S.VipLang.GetLang(new Jid(ws[3]));
                                                if (rs == null)
                                                    rs = m_r.f("vip_not_found", ws[3]);
                                            }
                                        }

                                    }
                                    else
                                        if (ws[2] == "count")
                                        {
                                            if (ws.Length == 3)
                                            {
                                                rs = Sh.S.VipLang.Count().ToString();
                                            }
                                            else
                                                syntax_error = true;

                                        }
                                else
                                {

                                    if (ws.Length > 3)
                                    {
                                        Jid Jid = new Jid(ws[2]);
                                        if (!Sh.S.JidValid(Jid))
                                        {
                                            rs = m_r.f("jid_not_valid", Jid).ToString();
                                            break;
                                        }
                                        else
                                        {

                                            if (Sh.S.Rg.GetResponse(ws[3]) != null)
                                            {
                                                Sh.S.VipLang.AddVip(Jid, ws[3]);
                                                rs = m_r.Agree();
                                            }
                                            else
                                                rs = m_r.f("lang_pack_not_found", ws[3]);
                                       
                                        }

                                    }
                                    else
                                    {
                                        syntax_error = true;
                                    }


                                }
                        }
                        else
                            syntax_error = true;
                        break;
                    }



              
                case "find":
                    {
                        string word = Utils.GetValue(m_b, "[(.*)]");
                        m_b = Utils.RemoveValue(m_b, "[(.*)]", true);
                        ws = Utils.SplitEx(m_b, 2);

                        if ((ws.Length > 2) && (word != ""))
                        {
                            rs = Find(ws[2].Trim(), word, m_r.f("files_found"), m_r.f("files_not_found"));
                        }
                        else
                            syntax_error = true;
                        break;
                    }



                case "pako":
                    {
                        if (ws.Length == 2)
                        {
                            Process p = Process.GetCurrentProcess();
                            ProcessModule pm = p.MainModule;
                            rs = p.MainModule.FileVersionInfo.ToString();
                        }
                        else
                            syntax_error = true;
                        break;
                    }

                case "gmsg":
                    {
                        if (ws.Length > 2)
                        {
                            Hashtable mucs = Sh.S.MUCs;
                            foreach (MUC m in mucs.Values)
                            {
                                Message msg = new Message();
                                msg.To = m.Jid;
                                msg.Type = MessageType.groupchat;
                                msg.Body = ws[2];
                                m_r.Connection.Send(msg);
                                rs = m_r.Agree();
                            }
                        }
                        else
                            syntax_error = true;
                        break;

                    }
                case "heap":
                    {

                        if (ws.Length == 2)
                            rs = new decimal(Environment.WorkingSet / 1000000).ToString() + " mb";
                        else
                            syntax_error = true;
                        break;
                    }

   
       

                case "eval":
                    {

                        if (ws.Length > 2)
                        {
                            Evaluator ev = new Evaluator();
                            string data = ev.Compile(ws[2], Sh);
                            if (data == null)
                                rs = m_r.f("eval_error");
                            else
                                rs = data;
                        }
                        else
                            syntax_error = true;
                        break;
                    }
                case "proc_show":
                    {
                        if (ws.Length == 2)
                        {
                            Process[] p = Process.GetProcesses();
                            string str = "";
                            int i = 1;
                            foreach (Process pp in p)
                            {
                                str += i.ToString() + ") " + pp.ProcessName + "  " + Convert.ToString(pp.WorkingSet / 1000) + "  " + pp.BasePriority + "\n";
                                i++;
                            }
                            str += m_r.f("all_processes") + (i - 1).ToString();
                            rs = str;
                        }
                        else
                            syntax_error = true;
                        break;
                    }

                case "proc_kill":
                    {

                        if (ws.Length > 2)
                        {
                            Process[] p = Process.GetProcesses();
                            bool killed = false;
                            int number;
                            try
                            {

                                number = Convert.ToInt32(ws[2]);
                                if ((number > 0) && (number <= p.Length))
                                {
                                    p[number - 1].Kill();
                                    killed = true;
                                }
                                else
                                    killed = false;
                            }
                            catch
                            {
                                killed = false;
                            }

                            if (killed)
                                rs = m_r.f("process_killed");
                            else
                                rs = m_r.f("process_not_found", ws[2]);


                        }
                        else
                            syntax_error = true;


                        break;
                    }

                case "proc_new":
                    {
                        if (ws.Length > 2)
                        {
                            try
                            {
                                string str = Environment.ExpandEnvironmentVariables(ws[2]);
                                ProcessStartInfo si = new ProcessStartInfo(str);

                                Process p = Process.Start(si);
                                rs = m_r.f("process_started");
                            }
                            catch
                            {
                                rs = m_r.f("process_not_found", ws[2]);
                            }

                        }
                        else
                            syntax_error = true;

                        break;
                    }

                default:
                    {
                        rs = m_r.f("volume_cmd_not_found", n, ws[1], d + n.ToLower() + " list");
                        break;
                    }

            }

            if (syntax_error)
                m_r.se(self);
            else
                if (rs != null)
                    m_r.Reply(rs);

        }

    }
}
