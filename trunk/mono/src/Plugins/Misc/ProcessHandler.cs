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
        string n ;
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





            string cmd = ws[1];
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
                        int access;
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
                        rs = access.ToString();

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


                case "join":
                    {

                        ws = Utils.SplitEx(m_b, 3);

                        if (ws.Length == 3)
                        {
                            if (!Sh.S.AutoMucManager.Exists(new Jid(ws[2].Trim())))
                            {
                                MUC m = new MUC(Sh.S.C, new Jid(ws[2].Trim()), Sh.S.Config.Nick, Sh.S.Config.Status, Sh.S.Config.Language, ShowType.NONE, Sh.S.Censor.SQLiteConnection);
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
                                    MUC m = new MUC(Sh.S.C, new Jid(ws[2].Trim()), ws[3].Trim(), Sh.S.Config.Status, Sh.S.Config.Language, ShowType.NONE, Sh.S.Censor.SQLiteConnection);
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
                        if (ws.Length > 2)
                        {
                            Jid room = new Jid(ws[2].Trim());

                            if (Sh.S.AutoMucManager.Exists(room))
                            {

                                m_r.Reply(m_r.Agree());
                                if (Sh.S.GetMUC(room) != null)
                                {
                                    Message msg = new Message();
                                    msg.To = room;
                                    msg.Body = m_r.f("muc_leave");
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
                            foreach (string _cmd in cmds)
                            {
                                Message msg = new Message();
                                object obj = m_r.Msg;
                                msg = (Message)obj;
                                msg.Body = _cmd.Trim(' ','\n');
                                CommandHandler cmd_handler = new CommandHandler(msg, Sh);
                                Thread.Sleep(1500);
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
                        rs = m_r.f("volume_list", n) + "\nlist, access, calc, cs, make, say, test";
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
                        data += m_r.f("cs_error",(ce.Line - 1).ToString(), ce.ErrorText)+"\n";
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
