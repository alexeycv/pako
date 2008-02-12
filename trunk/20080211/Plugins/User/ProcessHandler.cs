using System;
using System.Collections.Generic;
using System.Text;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;
using System.Threading;
using System.IO;
using Core.Client;
using Core.Special;
using Core.Conference;
using Core.Manager;
using agsXMPP.protocol.x.muc;
using System.Net;
using System.Web;


namespace Plugin
{
    public class UserHandler
    {
        string[] ws;
        Message m_msg;
        Response m_r;
        string self;
        bool syntax_error = false;
        Jid s_jid;
        string s_nick;
        string m_b;
        string n;
        string d;
        SessionHandler Sh;

        public UserHandler(Response r, string Name)
        {

            Sh = r.Sh;
            m_b = r.Msg.Body;
            ws = Utils.SplitEx(m_b, 2);
            m_msg = r.Msg;
            m_r = r;
            s_jid = r.Msg.From;
            s_nick = r.Msg.From.Resource;
            d = r.Delimiter;
            n = Name;
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








                case "version":
                    {
                        Jid Jid = m_r.Msg.From;

                        if (ws.Length > 2)
                        {
                            string arg = ws[2].Trim();

                            if (Sh.S.C.MyJID.ToString() == arg)
                                {
                                    m_r.Reply(m_r.FormatPattern("version_me") + " " + "Pako bot " + Sh.S.MyVersion + " (" + Sh.S.OSVersion + ")");
                                    return;
                                }

                            if (m_r.MUC != null)
                            {
                                if (m_r.MUC.MyNick == arg)
                                {
                                    m_r.Reply(m_r.FormatPattern("version_me") + " " + "Pako bot " + Sh.S.MyVersion + " (" + Sh.S.OSVersion + ")");
                                    return;
                                }
                                else
                                {
                                    if (m_r.MUC.UserExists(arg))
                                        Jid = new Jid(m_r.MUC.Jid.Bare + "/" + arg);
                                    else
                                        Jid = new Jid(ws[2]);
                                }
                            }
                            else
                                 Jid = new Jid(arg);
                               
                        }

                        VersionCB vcb = new VersionCB(m_r, Jid);
                        return;
                    }



                case "ping":
                    {
                        Jid Jid = m_r.Msg.From;

                        if (ws.Length > 2)
                        {
                            string arg = ws[2].Trim();

                            if (Sh.S.C.MyJID.ToString() == arg)
                            {
                                m_r.Reply(m_r.FormatPattern("ping_me"));
                                return;
                            }

                            if (m_r.MUC != null)
                            {
                                if (m_r.MUC.MyNick == arg)
                                {
                                    m_r.Reply(m_r.FormatPattern("ping_me"));
                                    return;
                                }
                                else
                                {
                                    if (m_r.MUC.UserExists(arg))
                                        Jid = new Jid(m_r.MUC.Jid.Bare + "/" + arg);
                                    else
                                        Jid = new Jid(arg);
                                }
                            }
                            else
                                Jid = new Jid(arg);
                        }

                        PingCB vcb = new PingCB(m_r, Jid);
                        return;
                    }



                case "disco":
                    {
                        ws = Utils.SplitEx(m_b, 3);
                        int num = 0;
                        if (ws.Length > 2)
                        {

                            if (ws.Length == 4)
                            {
                                try
                                {
                                    num = Convert.ToInt32(ws[3]);
                                }
                                catch
                                {
                                    syntax_error = true;
                                    break;
                                }
                            }

                        }
                        else
                        {
                            syntax_error = true;
                            break;
                        }
                         DiscoCB vcb = new DiscoCB(m_r, new Jid(ws[2]), num);
                        return;
                    }
          



                case "time":
                    {
                        Jid Jid = m_r.Msg.From;
                        
                        if (ws.Length > 2)
                        {
                            string arg = ws[2].Trim();

                            if (Sh.S.C.MyJID.ToString() == arg)
                            {
                                m_r.Reply(m_r.FormatPattern("time_me") + " " + DateTime.Now.ToString());
                                return;
                            }

                            if (m_r.MUC != null)
                            {
                                if (m_r.MUC.MyNick == arg)
                                {
                                    m_r.Reply(m_r.FormatPattern("time_me") + " " + DateTime.Now.ToString());
                                    return;
                                }
                                else
                                {
                                    if (m_r.MUC.UserExists(arg))
                                        Jid = new Jid(m_r.MUC.Jid.Bare + "/" + arg);
                                    else
                                        Jid = new Jid(arg);
                                }
                            }
                            else
                                Jid = new Jid(arg);
                        }

                        TimeCB vcb = new TimeCB(m_r, Jid);
                        return;
                    }
          

                case "list":
                    {
                        if (ws.Length == 2)
                        {
                            rs = m_r.FormatPattern("volume_list", n) + "\nlist, time, version, ping, disco";
                        }
                        break;
                    }
          
                default:
                    {
                        m_r.Reply(m_r.FormatPattern("volume_cmd_not_found", n, ws[1], d + n.ToLower() + " list"));
                        break;
                    }

            }


            if (syntax_error)
                m_r.se(self);
            else
            if (rs != null)
                m_r.Reply( rs);
               
        }

      

    }
}