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
using System.Threading;
using System.IO;
using Core.Kernel;
using Core.Other;
using Core.Conference;
using Core.Xml;
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
                case "version":
                    {
                        Jid Jid = m_r.Msg.From;

                        if (ws.Length > 2)
                        {
                            string arg = ws[2].Trim();
                            string os = Utils.Bot["os"];
                            string version = Utils.Bot["version"];
                            string name = Utils.Bot["name"];

                            if (Sh.S.C.MyJID.ToString() == arg)
                                {
                                    m_r.Reply(m_r.f("version_me") + " " + name + " " + version + " : " + os);
                                    return;
                                }

                            if (m_r.MUC != null)
                            {
                                if (m_r.MUC.MyNick == arg)
                                {
                                    m_r.Reply(m_r.f("version_me") + " " + name +" "+ version + " : " + os);
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

                case "vcard":
                    {
                        Jid Jid = m_r.Msg.From;

                        if (ws.Length > 2)
                        {
                            string arg = ws[2].Trim();

                            if (m_r.MUC != null)
                            {
                                if (m_r.MUC.MyNick == arg)
                                {
                                    Jid = Sh.S.C.MyJID;
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

                        VCardCB vcb = new VCardCB(m_r, Jid);
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
                                m_r.Reply(m_r.f("ping_me"));
                                return;
                            }

                            if (m_r.MUC != null)
                            {
                                if (m_r.MUC.MyNick == arg)
                                {
                                    m_r.Reply(m_r.f("ping_me"));
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

                case "uptime":
                    {
                        Jid Jid = m_r.Msg.From;
                        if (ws.Length > 2)
                        {
                            string arg = ws[2].Trim();

                            if (Sh.S.C.MyJID.ToString() == arg)
                            {
                                long tt = DateTime.Now.Ticks - Sh.Ticks;
                                string patern = m_r.f("my_uptime");
                                string data = Utils.FormatTimeSpan(tt, m_r);
                                rs = patern + " " + data;
                                m_r.Reply(rs);
                                return;
                            }

                            if (m_r.MUC != null)
                            {
                                if (m_r.MUC.MyNick == arg)
                                {
                                    long tt = DateTime.Now.Ticks - Sh.Ticks;
                                    string patern = m_r.f("my_uptime");
                                    string data = Utils.FormatTimeSpan(tt, m_r);
                                    rs = patern + " " + data;
                                    m_r.Reply(rs);
                                    return;
                                }
                                else
                                {
                                     Jid = new Jid(arg);
                                }
                            }
                            else
                                Jid = new Jid(arg);
                        }
                        else
                        {
                            long tt = DateTime.Now.Ticks - Sh.Ticks;
                            string patern = m_r.f("my_uptime");
                            string data = Utils.FormatTimeSpan(tt, m_r);
                            rs = patern + " " + data;
                            m_r.Reply(rs);
                            return;
 
                        }

                        LastCB vcb = new LastCB(m_r, Jid);
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





                case "info":
                    {
                        Jid Jid = m_r.Msg.From;
                        if (ws.Length > 2)
                        {
                            string arg = ws[2].Trim();

                            if (Sh.S.C.MyJID.ToString() == arg)
                            {
                                m_r.Reply(m_r.f("disco_info_me"));
                                return;
                            }

                            if (m_r.MUC != null)
                            {
                                if (m_r.MUC.MyNick == arg)
                                {
                                    m_r.Reply(m_r.f("disco_info_me"));
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
                        else
                        {
                            syntax_error = true;
                            break;
                        }
                        DiscoInfoCB vcb = new DiscoInfoCB(m_r, Jid);
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
                                m_r.Reply(m_r.f("time_me") + " " + DateTime.Now.ToString());
                                return;
                            }

                            if (m_r.MUC != null)
                            {
                                if (m_r.MUC.MyNick == arg)
                                {
                                    m_r.Reply(m_r.f("time_me") + " " + DateTime.Now.ToString());
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
                            rs = m_r.f("volume_list", n) + "\nlist, time, version, ping, disco, info, uptime, vcard";
                        }
                        break;
                    }
          
                default:
                    {
                        m_r.Reply(m_r.f("volume_cmd_not_found", n, ws[1], d + n.ToLower() + " list"));
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
