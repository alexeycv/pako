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
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
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



            string cmd = ws[1].ToLower();
            string rs = null;
            switch (cmd)
            {
                case "dns":
                    {
                        if (ws.Length > 2)
                        {
                            try
                            {

                                IPHostEntry ip_host_entry = Dns.GetHostEntry(ws[2]);
                                string ip_data = "";
                                bool _host = true;
                                if (ip_host_entry.AddressList[0].ToString() == ws[2] && ip_host_entry.HostName == ws[2])
                                {
                                    rs = m_r.f("resolve_fail");
                                    break;
                                }
                               
                                if (ip_host_entry.HostName != ws[2] && ip_host_entry.AddressList[0].ToString() == ws[2])
                                    _host = false;
                              
                                @out.exe("host: "+_host.ToString());
                                @out.exe(ip_host_entry.HostName);
                                if (!_host)
                                    ip_data = ip_host_entry.HostName;
                                else
                                {
                                    @out.exe(ip_host_entry.AddressList.Length.ToString());

                                    foreach (IPAddress s in ip_host_entry.AddressList)
                                    {
                                        ip_data += s.ToString() + ", ";  
                                    }
                                    ip_data = ip_data.Trim(' ', ',');
                                }


                                if (ip_data.Trim() == ws[2].Trim())
                                    rs = m_r.f("resolve_fail");
                                else
                                    rs = ip_data;
                            }
                            catch
                            {
                                rs = m_r.f("resolve_fail");
                            }
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
                            int startn = 1;
                            if (ws.Length > 3)
                            {
                                try
                                {
                                    if (ws[2].StartsWith("#"))
                                    {
                                        startn = Convert.ToInt32(ws[2].Substring(1));
                                        number = startn;
                                        text = ws[3];
                                    }
                                    else
                                    {
                                        number = Convert.ToInt32(ws[2]);
                                        text = ws[3];
                                    }
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

                            if (number <= 10 && number > 0 && startn <= 10 && startn > 0)
                            {
                                try
                                {
                                    GoogleSearchService g = new GoogleSearchService();
                                    @out.exe(startn.ToString() + " " + number + " " + "'" + (text == null ? "null" : text) + "'");
                                    GoogleSearchResult res = g.doGoogleSearch(Sh.S.Config.GetTag("googlekey"), HttpUtility.HtmlEncode(text), 0, 10, false, "restrict", true, "lr", "ie", "oe");
                                    string data = m_r.f("results");
                                    if (res.resultElements.Length == 0)
                                    {
                                        rs = m_r.f("google_nothing_found");
                                        break;
                                    }
                                    int index = 1;
                                    foreach (ResultElement r in res.resultElements)
                                    {
                                        if (index >= startn && index <= number)
                                        data += "\n" + index.ToString() + ")  " + HttpUtility.HtmlDecode(Utils.ClearTags(r.title + (r.snippet.Trim() != "" ? "\n" + r.snippet : "") + "\n" + r.URL + (r.cachedSize.Trim() != "" ? "  (" + r.cachedSize + ")" : "")));
                                        index++;
                                    }
                                    rs = data+"\n-- "+res.estimatedTotalResultsCount.ToString()+" --";
                                }
                                catch
                                {
                                    rs = m_r.f("google_failed");
                                }

                            }
                            else syntax_error = true;
                        }
                        else
                            syntax_error = true;
                        break;
                    }


                case "tld":
                    {
                        ws = Utils.SplitEx(m_b, 3);
                        if (ws.Length > 2)
                        {
                            FileStream fs = File.Open(Utils.dir(Utils.CD, "Static", "tlds.txt"), FileMode.Open, FileAccess.Read, FileShare.Read);
                            StreamReader sr = new StreamReader(fs);
                            string[] lines = sr.ReadToEnd().Split('\n');
                            sr.Close();
                            fs.Close();
                            bool find = false;
                            if (ws.Length > 3)
                                if (ws[2] == "find")
                                    find = true;
                                if (find)
                                {
                                    string src = "";
                                    int count = 0;
                                    foreach (string line in lines)
                                    {
                                        string[] _ws = line.Split('=');
                                        if (_ws.Length < 2 || line.Trim() == "")
                                            continue;
                                        if (_ws[1].Trim().ToLower().IndexOf(ws[3].ToLower()) >-1)
                                        { count++;  src += " � " + _ws[0].Trim() + " - " + _ws[1].Trim() + "\n"; }

                                    }
                                    rs = src != "" ? m_r.f("results")+"\n"+ src+"-- "+count.ToString()+" --" : m_r.f("tld_not_found", ws[3]);
                                    m_r.Reply(rs);
                                    return;

                                }
                                else
                                {
                                    ws = Utils.SplitEx(m_b, 2);
                                    foreach (string line in lines)
                                    {
                                        string[] _ws = line.Split('=');
                                        if (_ws.Length < 2 || line.Trim() == "")
                                            continue;
                                        if (_ws[0].Trim().ToLower() == ws[2].ToLower())
                                        { rs = _ws[1].Trim(); m_r.Reply(rs); return; }
                                        if (_ws[1].Trim().ToLower() == ws[2].ToLower())
                                        { rs = _ws[0].Trim(); m_r.Reply(rs); return; }

                                    }
                                    rs = m_r.f("tld_not_found", ws[2]);
                                }
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


                case "gt":
                    {
                        ws = Utils.SplitEx(m_msg.Body, 3);

                        if (ws.Length > 2)
                        {
                            if (ws[2] == "show")
                            {

                                TranslateUtil tr = new TranslateUtil();
                                if (ws.Length > 3)
                                {
                                    rs = tr.GetPair(ws[3]) ?? m_r.f("lang_not_existing");
                                    break;
                                }
                                else
                                {
                                    string data = "";
                                    Dictionary<string, string> dict = tr.GetAllPairs();
                                    foreach (string key in dict.Keys)
                                    {
                                        data += "\n" + key + " = " + dict[key];
                                    }
                                    rs = data;
                                    break;
                                }
                            }
                        }
                        if (ws.Length > 3)
                        {
                            string a, b;
                            string[] lns = ws[2].Split('|');
                            if (lns.Length != 2)
                            {
                                syntax_error = true;
                                break;
                            }
                            a = lns[0].Trim();
                            b = lns[1].Trim();
                            TranslateUtil tr = new TranslateUtil();
                            if (a == "" || b == "")
                            {
                                syntax_error = true;
                                break;
                            }
                            tr.Execute(ws[3],a,b,  m_r);
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
                            string data = xeps.GetXep(m_r.f("results"), ws[2]);
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
                                    if (number <= 0)
                                        throw new Exception();
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
                                @out.exe("rss_retreived");
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
                                        data += rf.Description.Trim(' ', '\n') + "\n-------------\n";

                                }
                                int i = 1;
                                foreach (RssItem ri in rf.Items)
                                {
                                    @out.exe("rss_item");
                                    if (number != 0 && i > number) break; 
                                    if (!String.IsNullOrEmpty(ri.Title))
                                        data += ri.Title.Trim(' ', '\n') + "\n";
                                    if (!String.IsNullOrEmpty(ri.Description))
                                    {
                                        string body = ri.Description;
                                        Regex rx = new Regex(@"<[^<>]+?>");
                                        body = rx.Replace(body, new MatchEvaluator(delegate(Match m)
                                        {
                                            @out.exe("RSS_REGEX: \"" + m.ToString()+"\"");
                                            string src = m.ToString().Replace("<", "").Replace(">", "").Trim();
                                            return src == "br" ? "\n" : "";
                                        }));
                                        data += HttpUtility.HtmlDecode(body) + "\n";
                                    }
                                    if (!String.IsNullOrEmpty(ri.Author))
                                        data += "[" + ri.Author + "]\n";

                                    data += "----------------\n\n";
                                    i++;
                                }
                                @out.exe("rss_data:");
                             //   @out.exe(data);

                                rs = data.Trim('\n','\v','\r');
                            }
                            else syntax_error = true;
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
                                int timeout = 30000;
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
                            rs = m_r.f("volume_list", n) + "\nlist, dns, tld, whois, google, gt, headers, rss, xep, rfc, curr, torrent, wiki, ping, svn, browse";
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
