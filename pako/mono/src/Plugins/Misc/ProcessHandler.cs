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
using System.IO;
using Core.Kernel;
using Core.Conference;
using Core.Other;
using System.Threading;

namespace Plugin
{
    public class MiscHandler
    {
        string[] ws;
        bool syntax_error = false;
        Response m_r;
        string self;
        Jid s_jid;
        string d;
        Message m_msg;
        string m_b;
        string n;
        SessionHandler Sh;



        public MiscHandler(Response r, string Name)
        {
            Sh = r.Sh;
            m_b = r.Msg.Body;
            ws = Utils.SplitEx(m_b, 2);
            m_msg = r.Msg;
            m_r = r;
            s_jid = r.Msg.From;
            n = Name;
            d = r.Delimiter;

            if (ws.Length < 2)
            {
                r.Reply(r.f("volume_info", n, d + n.ToLower() + " list"));
                return;
            }
            self = ws[0] + " " + ws[1];

            Handle();
        }



        public void Handle()
        {





            string cmd = ws[1].ToLower();
            string rs = null;

            switch (cmd)
            {
                case "test":
                    {
                        rs = m_r.f("test_passed");
                        break;
                    }


                case "access":
                    {
                        int? access;
                        MUser user = m_r.MUser;
                        Message msg = new Message();
                        msg.From = m_msg.From;
                        msg.To = m_msg.To;
                        msg.Body = m_msg.Body;
                        msg.Type = m_msg.Type;

                        if (m_r.MUser != null)
                        {
                            if (ws.Length > 2)
                            {
                                if (m_r.MUC.UserExists(ws[2]))
                                {
                                    user = m_r.MUC.GetUser(ws[2]);

                                }
                                else
                                {
                                    user = null;
                                    msg.From = new Jid(ws[2]);
                                }
                            }

                        }
                        else
                        {
                            user = null;
                        }

                        access = Sh.S.GetAccess(msg, user, m_r.MUC);
                        if ((m_r.Emulation != null) && (ws.Length == 2))
                            access = 100;
                        rs = (access ?? 0).ToString();

                        break;
                    }

                case "langs":
                    {
                        if (ws.Length == 2)
                        {
                            rs = Sh.S.Rg.GetPacketsList(false);
                        }
                        else
                            syntax_error = true;
                        break;
                    }


                case "vars":
                    {
                        if (ws.Length == 2)
                        {
                            string data = m_r.f("variable_list");
                            foreach (string v in Utils.Variables)
                            {
                                data += "\n" + v + " = " + m_r.f("var_" + v.ToLower());
                            }
                            rs = data;
                        }
                        else
                        {
                            string v = ws[2].ToUpper();
                            if (Utils.Variables.Contains(v))
                            {
                                rs = v + " = " + m_r.f("var_" + v.ToLower());
                            }
                            else
                                rs = m_r.f("var_not_found", v);
                        }
                        break;
                    }

                case "cmds":
                    {
                        if (ws.Length == 2)
                        {
                            string acc = m_r.f("access");
                            rs = "\n[========" + acc + "========]";
                            ListDictionary list;
                            if (m_r.MUser != null)
                            {
                                list = m_r.MUC.AccessManager.GetCommands();
                                ListDictionary list_g = Sh.S.AccessManager.GetCommands();
                                foreach (string key in list_g.Keys)
                                {
                                    if (!list.Contains(key))
                                        list.Add(key, list_g[key]);
                                }
                            }else
                                list = Sh.S.AccessManager.GetCommands();
                            foreach (string cm in list.Keys)
                            {
                                string _access = ((int)list[cm]).ToString();
                                rs += "\n" + "[" + cm + "]";
                                for (int i = 1; i <= 28 - (2 + cm.Length) - _access.Length; i++)
                                    rs += i == 1 || i == 28 - (2 + cm.Length) - _access.Length ? " " : i % 2 == 1 ? " " : ".";
                                rs += _access;
                            }
                            rs = list.Count == 0 ? m_r.f("access_list_empty") : rs + "\n[==========================]";
                           
                        }
                        break;

                    }
                case "join":
                    {

                        string _cmd = Utils.GetValue(m_b, "[(.*)]").Trim();
                        m_b = Utils.RemoveValue(m_b, "[(.*)]", true);
                        ws = Utils.SplitEx(m_b, 3);
                        //*misc join roso@c.j.dom Radiohead [password]
                        if (ws.Length == 3)
                        {
                            if (!Sh.S.AutoMucManager.Exists(new Jid(ws[2].Trim())))
                            {
                                MUC m = new MUC(Sh.S.C, new Jid(ws[2].Trim()), Sh.S.Config.Nick, Sh.S.Config.Status, Sh.S.Config.Language, ShowType.NONE, Sh, _cmd.Trim() != "" ? _cmd : null);
                                Sh.S.MUCs.Add(new Jid(ws[2].Trim()), m);
                                MucActivityController mac = new MucActivityController(m_r, m);
                                return;
                            }
                            else
                                rs = m_r.f("muc_already_in", ws[2]);

                        }
                        else
                            if (ws.Length == 4)
                            {
                                if (!Sh.S.AutoMucManager.Exists(new Jid(ws[2].Trim())))
                                {
                                    MUC m = new MUC(Sh.S.C, new Jid(ws[2].Trim()), ws[3].Trim(), Sh.S.Config.Status, Sh.S.Config.Language, ShowType.NONE, Sh,_cmd.Trim() != "" ? _cmd : null);
                                    Sh.S.MUCs.Add(new Jid(ws[2].Trim()), m);
                                    MucActivityController mac = new MucActivityController(m_r, m);
                                    return;

                                }
                                else
                                    rs = m_r.f("muc_already_in", ws[2]);

                            }
                            else
                                syntax_error = true;
                        break;
                    }

                case "leave":
                    {
                        ws = Utils.SplitEx(m_b, 3);
                        if (ws.Length > 2)
                        {
                            Jid room = Sh.S.GetMUCJid(ws[2]) ?? new Jid(ws[2]);

                            if (Sh.S.AutoMucManager.Exists(room))
                            {

                                m_r.Reply(m_r.Agree());
                                if (Sh.S.GetMUC(room) != null)
                                {
                                    Message msg = new Message();
                                    msg.To = room;
                                    msg.Body = ws.Length > 3 ? ws[3] : m_r.f("muc_leave");
                                    msg.Type = MessageType.groupchat;
                                    m_r.Connection.Send(msg);
                                    Presence pr = new Presence();
                                    pr.To = room;
                                    pr.Type = PresenceType.unavailable;
                                    m_r.Connection.Send(pr);
                                }

                                Sh.S.AutoMucManager.DelMuc(room);

                            }
                            else
                                rs = m_r.f("muc_not_in");
                        }
                        else
                            syntax_error = true;
                        break;
                    }





                case "calc":
                    {
                        if (ws.Length > 2)
                        {
                            Jid c_jid = m_r.MUser != null ? s_jid : new Jid(s_jid.Bare);
                            if (!Sh.S.CalcHandler.Exists(c_jid))
                                Sh.S.CalcHandler.AddHandle(c_jid);
                            Sh.S.CalcHandler.GetHandle(c_jid).Execute(ws[2], m_r);
                            return;
                        }
                        else
                            syntax_error = true;
                        break;
                    }

                case "say":
                    {
                        if (ws.Length > 2)
                            rs = ws[2];
                        else
                            syntax_error = true;

                        break;
                    }


                case "make":
                    {
                        if (ws.Length > 2)
                        {
                            string cmds_source = ws[2].Trim();
                            string[] cmds = cmds_source.Split(new string[] { "&&" }, StringSplitOptions.RemoveEmptyEntries);
                            if (cmds.Length > Sh.S.Config.RecursionLevel)
			    {
				m_r.Reply(m_r.f("commands_recursion",Sh.S.Config.RecursionLevel.ToString()));
				break;
			    }
                            foreach (string _cmd in cmds)
                            {
                                Message msg = new Message();
                                object obj = m_r.Msg;
                                msg = (Message)obj;
                                msg.Body = _cmd.Trim(' ', '\n');
                                CommandHandler cmd_handler = new CommandHandler(msg, Sh, m_r.Emulation, CmdhState.PREFIX_NOT_POSSIBLE, m_r.Level);
                                Sh.S.Sleep();
                            }
                        }
                        else
                            syntax_error = true;

                        break;
                    }

                case "cs":
                    {
                        if (ws.Length > 2)
                        {
                            CSharpCompiler cs = new CSharpCompiler();
                            cs.Compiled += new CompilerHandler(cs_Compiled);
                            cs.Compile(ws[2], 5000);
                            @out.exe("OK");
                            return;

                        }
                        else
                            syntax_error = true;

                        break;
                    }



                case "list":
                    {
                        rs = m_r.f("volume_list", n) + "\nlist, access, calc, cs, make, say, test, langs, join, leave, cmds, vars";
                        break;
                    }

                default:
                    {
                        rs = m_r.f("volume_cmd_not_found", n, ws[1], d + n.ToLower() + " list");
                        break;
                    }


            }
            if (syntax_error)
            {
                m_r.se(self);
            }
            else
                if (rs != null)
                    m_r.Reply(rs);


        }

        void cs_Compiled(object sender, CompilerEventArgs e)
        {

            @out.exe("CS");
            switch (e.State)
            {
                case CompilerEventArgs.CompilerState.Done:
                    m_r.Reply(">>\n" + e.Output + "_");
                    break;
                case CompilerEventArgs.CompilerState.Error:
                    string data = "";
                    foreach (System.CodeDom.Compiler.CompilerError ce in e.Errors)
                    {
                        data += m_r.f("cs_error", ((ce.Line - 12) < 0 ? 0 : (ce.Line - 12)).ToString(), ce.ErrorText) + "\n";
                    }
                    data = data.TrimEnd('\n');
                    m_r.Reply(data);
                    break;

                case CompilerEventArgs.CompilerState.TimeOut:
                    m_r.Reply(m_r.f("cs_time_out"));
                    break;

            }
        }

    }
}
