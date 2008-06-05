/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Pako Jabber-bot. Bbodio's Lab.                                                *
 * Copyright. All rights reserved � 2007-2008 by Klichuk Bogdan (Bbodio's Lab)   *
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
using System.Net.NetworkInformation;
using System.Web;


namespace www
{
    public class WwwHandler
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

        public WwwHandler(Response r, string Name)
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
            // Handle();
            Handle();
        }

        private bool check_content(string content_type)
        {
            string[] r = new string[] { "text/plain", "text/html", "text/xml", "text/vnd.sun.j2me.app-descriptor", "text/vnd.wap.wml" };
            foreach (string s in r) if (content_type.IndexOf(s.ToLower()) > -1) return true;
            return false;
        }


        public void Handle()
        {



            string cmd = ws[1];
            string rs = null;

            //   @out.exe("ppppppppppppppppp");
            switch (cmd)
            {








                case "dns":
                    {
                        if (ws.Length > 2)
                        {
                            /* try
                             {
                                 IPHostEntry ip_host_entry = Dns.GetHostByAddress(ws[2]);
                                 rs = ip_host_entry.HostName;
                             }
                             catch
                             {
 */
                            //    int i = Convert.ToInt32("rtr");
                            try
                            {
                                IPHostEntry ip_host_entry = Dns.GetHostEntry(ws[2]);
                                string ip_data = "";
                                bool _host = false;
                                foreach (IPAddress s in ip_host_entry.AddressList)
                                {
                                    ip_data += s.ToString() + " ";
                                    if (s.ToString().Contains(ws[2]))
                                    {
                                        _host = true;
                                        break;
                                    }

                                }

                                if (_host)
                                    ip_data = ip_host_entry.HostName;
                                else
                                    ip_data = ip_data.Trim().Replace(" ", ", ");


                                rs = ip_data;
                            }
                            catch
                            {
                                rs = m_r.f("resolve_fail");
                            }
                            // }
                        }
                        else
                            syntax_error = true;
                        break;
                    }
                case "wiki":
                    {
                        if (ws.Length > 2)
                        {
                            Wikipedia wiki = new Wikipedia(ws[2], m_r);
                        }
                        else
                            syntax_error = true;
                        break;
                    }

                case "whois":
                    {
                        if (ws.Length > 2)
                        {
                            try
                            {
                                string data = WhoisResolver.Whois(ws[2].Trim().ToLower());
                                if (data != null)
                                    rs = data;
                                else
                                    rs = m_r.f("whois_fail");
                            }
                            catch
                            {
                                rs = m_r.f("whois_fail");
                            }
                        }
                        else
                            syntax_error = true;
                        break;
                    }

                case "google":
                    {
                        ws = Utils.SplitEx(m_b, 3);
                        if (ws.Length > 2)
                        {
                            string text;
                            int number = 1;
                            if (ws.Length > 3)
                            {
                                try
                                {
                                    number = Convert.ToInt32(ws[2]);
                                    text = ws[3];
                                }
                                catch
                                {
                                    ws = Utils.SplitEx(m_b, 2);
                                    text = ws[2];
                                }
                            }
                            else
                            {
                                ws = Utils.SplitEx(m_b, 2);
                                text = ws[2];
                            }

                            if ((number <= 10) && (number > 0))
                            {
                                Google g = new Google(text, number, m_r);
                                return;
                            }
                            else syntax_error = true;
                        }
                        else
                            syntax_error = true;
                        break;
                    }

                case "torrent":
                    {
                        ws = Utils.SplitEx(m_b, 3);
                        if (ws.Length > 2)
                        {
                            string text = ws[2];
                            int number = 10;
                            if (ws.Length > 3)
                            {
                                try
                                {
                                    number = Convert.ToInt32(ws[2]);
                                    text = ws[3];
                                }
                                catch { }
                            }

                            if ((number <= 10) && (number > 0))
                            {
                                Torrent g = new Torrent(text, m_r, number);
                                g.Handle();
                                return;
                            }
                            else syntax_error = true;
                        }
                        else
                            syntax_error = true;
                        break;
                    }

                case "gtlangs":
                    {
                        if (ws.Length == 2)
                        {
                            TranslateUtil tr = new TranslateUtil();
                            string data = m_r.f("lang_pairs");
                            foreach (string key in tr.Modes.Keys)
                            {
                                data += "\n" + key + " : " + ((string[])tr.Modes[key])[1];
                            }
                            rs = data;
                        }
                        else
                            syntax_error = true;
                        break;
                    }

                case "gt":
                    {
                        ws = Utils.SplitEx(m_msg.Body.Trim(), 3);

                        if (ws.Length > 3)
                        {
                            TranslateUtil tr = new TranslateUtil();
                            tr.Execute(ws[3], ws[2], m_r);
                            return;
                        }
                        else
                            syntax_error = true;
                        break;
                    }

                case "curr":
                    {
                        ws = Utils.SplitEx(m_msg.Body, 5);

                        if (ws.Length > 3)
                        {
                            if (ws[2] == "find")
                            {
                                ws = Utils.SplitEx(m_msg.Body, 3);
                                Currency c = new Currency();
                                string data = c.Find(ws[3]);
                                if (data != null)
                                    rs = data;
                                else
                                {
                                    rs = m_r.f("currency_search_not_found");
                                }
                                break;
                            }
                        }

                        if (ws.Length == 5)
                        {
                            Currency c = new Currency();
                            try
                            {
                                double d = Convert.ToDouble(ws[2]);
                                @out.exe("currency_: " + (Convert.ToDouble(ws[2]) * 1.0).ToString());

                            }
                            catch { rs = m_r.f("curr_wrong_amount"); break; }
                            try
                            {

                                rs = c.Handle(ws[3], ws[4], Convert.ToDouble(ws[2]));
                            }
                            catch
                            {
                                @out.exe("currency_error");
                                rs = m_r.f("curr_name_not_found");
                            }
                        }
                        else
                            if (ws.Length == 3)
                            {
                                if (ws[2] == "list")
                                {
                                    Currency c = new Currency();
                                    rs = c.GetList();
                                }
                                else
                                    syntax_error = true;
                            }
                            else
                                syntax_error = true;
                        break;
                    }
                case "xep":
                    {

                        if (ws.Length > 2)
                        {

                            XEPS xeps = new XEPS();
                            string data = xeps.GetXep(m_r.f("xeps_result"), ws[2]);
                            rs = data != null ? data : m_r.f("xep_not_found", ws[2]);
                        }
                        else
                            syntax_error = true;
                        break;
                    }

                case "svn":
                    {
                        ws = Utils.SplitEx(m_b, 3);
                        if (ws.Length > 2)
                        {
                            string repos;
                            int number = 1;
                            if (ws.Length > 3)
                            {
                                try
                                {
                                    number = Convert.ToInt32(ws[2]);
                                    repos = ws[3];
                                }
                                catch
                                {
                                    ws = Utils.SplitEx(m_b, 2);
                                    repos = ws[2];
                                }
                            }
                            else
                            {
                                ws = Utils.SplitEx(m_b, 2);
                                repos = ws[2];
                            }

                            if ((number <= 10) && (number > 0))
                            {
                                @out.exe("web_svn_number_satisfied" + number.ToString());
                                @out.exe("web_svn_repos: " + repos);
                                string _r = repos;
                                if (repos.StartsWith("-v "))
                                    repos = repos.Substring(2);
                                if (repos.EndsWith(" -v"))
                                    repos = repos.Substring(0, repos.Length - 2);
                                if (repos.StartsWith("--verbose "))
                                    repos = repos.Substring(9);
                                if (repos.EndsWith(" --verbose"))
                                    repos = repos.Substring(0, repos.Length - 9);
                                bool verbose = repos != _r;
                                repos = Utils.ConsoleEscape(repos.Trim());
                                @out.exe("web_svn_repos_formatted: " + repos);
                                Stdior std = new Stdior();
                                rs = std.Execute("svn log " + repos + (verbose ? " -v" : "") + " --limit " + number.ToString(), Sh.S).TrimStart('-');

                            }
                            else syntax_error = true;
                        }
                        else
                            syntax_error = true;
                        break;
                    }

                case "browse":
                    {
                        ws = Utils.SplitEx(m_b, 3);
                        if (ws.Length > 2)
                        {
                            string link = ws[2];
                            try
                            {
                                link = !link.StartsWith("http://") ? "http://" + link : link;
                                HttpWebRequest wr = (HttpWebRequest)HttpWebRequest.CreateDefault(new System.Uri(link));
                                @out.exe(link);    
                                 wr.Method = "HEAD";
                                wr.ContentType = "application/x-www-form-urlencoded";
                                WebResponse wp = wr.GetResponse();
                                string cont_t = wp.ContentType;
                                if (!check_content(cont_t))
                                {
                                    @out.exe(cont_t);
                                    rs = m_r.f("browse_restricted_content");
                                    break;
                                }
                            }
                            catch
                            {
                                rs = m_r.f("browse_error");
                                break;

                            }
                            link = Utils.ConsoleEscape(link.Trim());
                            Stdior std = new Stdior();
                            rs = std.Execute("elinks -no-references -no-numbering -dump-charset " + Sh.S.Config.GetTag("elinks_charset") + " " + link, Sh.S);
                        }
                        else
                            syntax_error = true;
                        break;
                    }

                case "headers":
                    {
                        if (ws.Length > 2)
                        {
                            try
                            {
                                string link = ws[2];
                                link = !link.StartsWith("http://") ? "http://" + link : link;
                                HttpWebRequest wr = (HttpWebRequest)HttpWebRequest.CreateDefault(new System.Uri(link));
                                wr.Method = "HEAD";
                                wr.ContentType = "application/x-www-form-urlencoded";
                                WebResponse wp = wr.GetResponse();
                                string cont_t = wp.ContentType;
                                string data = "";
                                @out.exe(cont_t);
                                foreach (string head in wp.Headers.Keys)
                                {
                                    if (head.ToLower().IndexOf("cookie") > -1) continue;
                                    data += "\n" + head;
                                    for (int i = 0; i < 30 - head.Length; i++) data += " ";
                                    data += wp.Headers[head];
                                }
                                rs = data;
                            }
                            catch
                            {
                                rs = m_r.f("headers_error");
                            }
                               
                            
                             
                            
                        }
                        else
                            syntax_error = true;
                        break;
                    }
                case "rfc":
                    {
                        if (ws.Length > 2)
                        {
                            string data = Sh.RFCHandler.GetState(ws[2]);
                            rs = data != null ? data.Trim().Trim('\n') : m_r.f("rfc_not_found");
                        }
                        else
                            syntax_error = true;
                        break;
                    }

                case "rss":
                    {
                        ws = Utils.SplitEx(m_b, 3);
                        if (ws.Length > 2)
                        {
                            string text;
                            int number = 0;
                            if (ws.Length > 3)
                            {
                                try
                                {
                                    number = Convert.ToInt32(ws[2]);
                                    if (number == 0)
                                        throw new Exception();
                                    number = number < 0 ? 0 : number;
                                    text = ws[3];
                                }
                                catch
                                {
                                    ws = Utils.SplitEx(m_b, 2);
                                    text = ws[2];
                                }
                            }
                            else
                            {
                                ws = Utils.SplitEx(m_b, 2);
                                text = ws[2];
                            }

                            if (number >= 0)
                            {
                                RssFeed rf;
                                try
                                {
                                    RssReader rss = new RssReader();
                                    rf = rss.Retrieve(text);
                                }
                                catch
                                {
                                    rs = m_r.f("rss_error");
                                    break;
                                }
                                string data = "";
                                if (!String.IsNullOrEmpty(rf.ErrorMessage))
                                    data = m_r.f("rss_error");
                                else
                                {
                                    data = "\n";
                                    if (!String.IsNullOrEmpty(rf.Title))
                                        data += "" + rf.Title.Trim(' ', '\n');
                                    if (!String.IsNullOrEmpty(rf.Language))
                                        data += " (" + rf.Language.Trim(' ', '\n') + ")\n";
                                    else
                                        data += "\n";
                                    if (!String.IsNullOrEmpty(rf.LastBuildDate))
                                        data += rf.LastBuildDate.Trim(' ', '\n') + "\n";
                                    if (!String.IsNullOrEmpty(rf.Category))
                                        data += "(" + rf.Category.Trim(' ', '\n') + ")\n";
                                    if (!String.IsNullOrEmpty(rf.Copyright))
                                        data += rf.Copyright.Trim(' ', '\n') + "\n";
                                    if (!String.IsNullOrEmpty(rf.Description))
                                        data += "=======\n" + rf.Description.Trim(' ', '\n') + "\n=======\n";
                                    data += "\n";

                                }
                                int i = 1;
                                foreach (RssItem ri in rf.Items)
                                {
                                    if (number != 0 && i > number) break; 
                                    if (!String.IsNullOrEmpty(ri.Title))
                                        data += ri.Title.Trim(' ', '\n') + "\n";
                                    if (!String.IsNullOrEmpty(ri.Description))
                                    {
                                        string body = ri.Description;
                                        while ((body.IndexOf("<") != -1) && (body.IndexOf(">") != -1))
                                        {
                                            body = HttpUtility.HtmlDecode(Utils.RemoveValue(body, "<(.*)>", true)).Trim(' ', '\n', '\t', '\v', '\r');
                                        }
                                        data += body + "\n";
                                    }
                                    if (!String.IsNullOrEmpty(ri.Author))
                                        data += "[" + ri.Author + "]\n";

                                    data += "----------------\n\n";
                                    i++;
                                }

                                rs = data;
                            }
                            else syntax_error = true;
                        }
                        else
                            syntax_error = true;
                        break;


                        if (ws.Length > 2)
                        {
                           
                        }
                        else
                            syntax_error = true;
                        break;
                    }

                case "ping":
                    {
                        if (ws.Length > 2)
                        {
                            try
                            {

                                Ping pingSender = new Ping();
                                PingOptions options = new PingOptions();
                                options.DontFragment = true;
                                string data = "abc";
                                byte[] buffer = Encoding.ASCII.GetBytes(data);
                                int timeout = 30;
                                PingReply reply = pingSender.Send(ws[2], timeout, buffer, options);
                                string pattern = m_r.f("ping_packet");
                                if (reply.Status == IPStatus.Success)
                                {
                                    pattern = pattern
                                        .Replace("{1}", reply.Address.ToString())
                                        .Replace("{2}", reply.RoundtripTime.ToString())
                                        .Replace("{3}", reply.Options.Ttl.ToString())
                                        .Replace("{4}", reply.Buffer.Length.ToString());
                                    m_r.Reply(pattern);
                                }
                                else
                                {
                                    m_r.Reply(m_r.f("version_error", ws[2]));
                                }
                            }
                            catch
                            {
                                m_r.Reply(m_r.f("version_error", ws[2]));
                            }

                        }
                        else
                            syntax_error = true;
                        break;
                    }


                case "list":
                    {
                        if (ws.Length == 2)
                        {
                            rs = m_r.f("volume_list", n) + "\nlist, dns, whois, google, gt, gtlangs, xep, rfc, curr, torrent, wiki, ping, svn, browse";
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
                    m_r.Reply(rs);

        }



    }
}
