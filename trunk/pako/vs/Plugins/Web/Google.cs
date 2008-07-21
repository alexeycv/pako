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
using System.Text.RegularExpressions;
using System.Data;
using System.Collections;
using System.IO;
using System.Threading;
using System.Web;
using System.Net;
using Core.Plugins;
using Core.Kernel;
using Core.Other;
using agsXMPP;
using agsXMPP.protocol.client;



namespace www
{




    public class Google
    {

     





        string m_text = "";
        Message m_msg;
        Response m_r;
        int m_count;


        public Google(string Text, int Count, Response r)
        {
            m_text = Text;
            m_msg = r.Msg;
            m_count = Count;
            m_r = r;
            @out.exe("1");
            Search();
        }


 

        void Search()
        {
            HttpWebRequest wr = (HttpWebRequest)HttpWebRequest.CreateDefault(new System.Uri("http://www.google.com/search?q=" + HttpUtility.UrlEncode(m_text) + "&ie=UTF-8&oe=UTF-8"));
            wr.Method = "GET";
            wr.ContentType = "application/x-www-form-urlencoded";
            WebResponse wp = wr.GetResponse();
            StreamReader sr = new StreamReader(wp.GetResponseStream());
            string data = "";
            string res = sr.ReadToEnd();
            string _res_ = res;
            
            if (res.IndexOf("class=g><h2 class=r>") < 0)
            {
                m_r.Reply(m_r.f("google_nothing_found"));
                return;
            }
            res = res.Remove(0, res.IndexOf("class=g><h2 class=r>"));
            Regex regex = new Regex("class=g><h2 class=r>(.*)</td></tr></table></div>");
            bool success = false;
            int count = 0;
            string all = "";

            while (regex.IsMatch(res) && (m_count > count))
            {
 
                success = true;
               all = Utils.GetValue(_res_, "</b> of about <b>(.*)</b> for <b>");
               if ((all == "") || (all.Length > 30))
                all = Utils.GetValue(_res_, "</b> of <b>(.*)</b> for <b>");
               if ((all == "") || (all.Length > 30))
                   all = "unknown";
                string _res = "";
                foreach (Match m in regex.Matches(res))
                {
                    _res = m.ToString();
                    break;
                }

                string subject = Utils.GetValue(_res, "class=l>(.*)</a>");
                string url = Utils.GetValue(_res.Substring(_res.IndexOf("class=g")), "<a href=\"(.*)\" class=l>", false);
                //Console.WriteLine(_res);
                string body = Utils.GetValue(_res, "<tr><td class=\"j\">(.*)<br><span class=a>");
                res = Utils.RemoveValue(res, "class=l>(.*)</a>", true);
                _res = subject + "\n" + body + "\n" + url;
                Regex rx = new Regex(@"<[^<>]+?>");
                _res = rx.Replace(_res, new MatchEvaluator(delegate(Match m)
                {
                    @out.exe("RSS_REGEX: \"" + m.ToString() + "\"");
                    string src = m.ToString().Replace("<", "").Replace(">", "").Trim();
                    return src == "br" ? "\n" : "";
                }));

                body = rx.Replace(body, new MatchEvaluator(delegate(Match m)
                {
                    @out.exe("RSS_REGEX: \"" + m.ToString() + "\"");
                    string src = m.ToString().Replace("<", "").Replace(">", "").Trim();
                    return src == "br" ? "\n" : "";
                }));
   
                if (m_count > 1)
                {
                    if (count == 0)
                        data += "\n***" + (count + 1) + "\n" + HttpUtility.HtmlDecode(_res);
                    else
                        data += "\n\n***" + (count + 1) + "\n" + HttpUtility.HtmlDecode(_res);
                }
                else
                {
                    data += HttpUtility.HtmlDecode(_res);
                }
                res = Utils.RemoveValue(res, "class=g><h2 class=r>(.*)</td></tr></table></div>", true);
                count++;
            }
            if (!success)
                m_r.Reply(m_r.f("google_nothing_found"));
            else
                m_r.Reply(data+"\n-- "+all+" --");
            }
    


         
       
    
}
}
