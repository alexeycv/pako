using System;
using System.Collections.Generic;
using System.Text;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;
using System.IO;
using Core.Client;
using Core.Conference;
using Core.Manager;

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
                r.Reply(r.FormatPattern("volume_info", n, d + n.ToLower() + " list"));
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
                        rs = m_r.FormatPattern("test_passed");
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

                        access = Sh.S.GetAccess(msg, user);
                        rs = access.ToString();

                        break;
                    }

            



                case "myup":
                    {
                        if (ws.Length == 2)
                        {
                            long tt = DateTime.Now.Ticks - Sh.Ticks;
                            TimeSpan ts = TimeSpan.FromTicks(tt);
                            string patern = m_r.FormatPattern("my_uptime");
                            string data =
                                (ts.Days > 31 ? Convert.ToString(System.Math.Truncate((double)ts.Days / 31)) + m_r.FormatPattern("month") + " " : "") +
                                (ts.Days > 0 ? ts.Days.ToString() + " " + m_r.FormatPattern("day") + " " : "") +
                                (ts.Hours > 0 ? ts.Hours.ToString() + " " + m_r.FormatPattern("hour") + " " : "") +
                                (ts.Minutes > 0 ? ts.Minutes.ToString() + " " + m_r.FormatPattern("minute") + " " : "") +
                                (ts.Seconds > 0 ? ts.Seconds.ToString() + " " + m_r.FormatPattern("second") + " " : "");

                            rs = patern + " " + data;
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



                case "greet":
                    {
                      
                        if (m_r.MUC != null)
                        {

                            m_b = m_b.Replace("{nick}", m_r.MUser.Nick)
                                     .Replace("{server}", Sh.S.Config.Jid.Server)
                                     .Replace("{room}", m_r.MUC.Jid.ToString())
                                     .Replace("{jid}", m_r.MUser.Jid.ToString());

                            ws = Utils.SplitEx(m_b, 3);
                            if (ws.Length > 3)
                            {

                                Jid Jid = new Jid(ws[2]);
                                Jid Room = m_r.MUC.Jid;

                                if ((Jid.Bare == null) ||
                                    (Room.Bare == null) ||
                                    (Jid.Server == null) ||
                                    (Room.Server == null))
                                {
                                    syntax_error = true;
                                    break;
                                }

                                if (Sh.S.Tempdb.AddGreet(Jid, Room, ws[3]))
                                    rs = m_r.Agree();
                                else
                                    rs = m_r.FormatPattern("greet_already");
                                break;
                            }
                            else
                                syntax_error = true;
                        }
                        else
                        {
                            m_b = m_b.Replace("{myserver}", Sh.S.Config.Jid.Server);
                            ws = Utils.SplitEx(m_b, 4);
                            if (ws.Length > 4)
                            {

                                Jid Jid = new Jid(ws[3]);
                                Jid Room = new Jid(ws[2]);

                                if ((Jid.Bare == null) ||
                                    (Room.Bare == null) ||
                                    (Jid.Server == null) ||
                                    (Room.Server == null))
                                {
                                    syntax_error = true;
                                    break;
                                }

                                if (Sh.S.Tempdb.AddGreet(Jid, Room, ws[4]))
                                    rs = m_r.Agree();
                                else
                                    rs = m_r.FormatPattern("greet_already");
                                break;
                            }
                            else
                                syntax_error = true;
                        }
                        break;
                    }

                case "degreet":
                    {
                        if (m_r.MUC == null)
                        {
                            ws = Utils.SplitEx(m_b, 3);
                            if (ws.Length > 3)
                            {

                                Jid Jid = new Jid(ws[3]);
                                Jid Room = new Jid(ws[2]);

                                if ((Jid.Bare == null) ||
                                    (Room.Bare == null) ||
                                    (Jid.Server == null) ||
                                    (Room.Server == null))
                                {
                                    syntax_error = true;
                                    break;
                                }

                                if (Sh.S.Tempdb.DelGreet(Jid, Room))
                                    rs = m_r.Agree();
                                else
                                    rs = m_r.FormatPattern("greet_not_existing");
                                break;
                            }
                            else
                                syntax_error = true;
                        }
                        else
                        {
                            ws = Utils.SplitEx(m_b, 2);
                            if (ws.Length > 2)
                            {

                                Jid Jid = new Jid(ws[2]);
                                Jid Room = m_r.MUC.Jid;

                                if ((Jid.Bare == null) ||
                                    (Room.Bare == null) ||
                                    (Jid.Server == null) ||
                                    (Room.Server == null))
                                {
                                    syntax_error = true;
                                    break;
                                }

                                if (Sh.S.Tempdb.DelGreet(Jid, Room))
                                    rs = m_r.Agree();
                                else
                                    rs = m_r.FormatPattern("greet_not_existing");
                                break;
                            }
                            else
                                syntax_error = true;
                        }
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


                case "cs":
                    {
                        if (ws.Length > 2)
                        {
                            CSharpCompiler cs = new CSharpCompiler();
                            cs.Compiled += new CompilerHandler(cs_Compiled);
                            cs.Compile(ws[2], 5000);
                            Console.WriteLine("OK");
                            return;

                        }
                        else
                            syntax_error = true;

                        break;
                    }


                  
                case "list":
                    {
                        rs = m_r.FormatPattern("volume_list", n) + "\nsay, list, calc, test, access";
                        break;
                    }

                default:
                    {
                        rs = m_r.FormatPattern("volume_cmd_not_found", n, ws[1], d + n.ToLower() + " list");
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

            Console.WriteLine("CS");
            switch (e.State)
            {
                case CompilerEventArgs.CompilerState.Done:
                    m_r.Reply(">>\n" + e.Output + "_");
                    break;
                case CompilerEventArgs.CompilerState.Error:
                    string data = "";
                    foreach (System.CodeDom.Compiler.CompilerError ce in e.Errors)
                    {
                        data += m_r.FormatPattern("cs_error",(ce.Line - 13).ToString(), ce.ErrorText)+"\n";
                    }
                    data = data.TrimEnd('\n');
                    m_r.Reply(data);
                    break;

                case CompilerEventArgs.CompilerState.TimeOut:
                    m_r.Reply(m_r.FormatPattern("cs_time_out"));
                    break;

            }
        }

    }
}
