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


        string ClearTags(string Source)
        {
            while (Utils.GetValue(Source, "<(.*)>", true) != "")
                Source = Utils.RemoveValue(Source, "<(.*)>", true);
            return Source;
        }

        void Search()
        {
            HttpWebRequest wr = (HttpWebRequest)HttpWebRequest.CreateDefault(new System.Uri("http://www.google.com/search?q=" + HttpUtility.UrlEncode(m_text) + "&ie=UTF-8&oe=UTF-8"));
            wr.Method = "GET";
            wr.ContentType = "application/x-www-form-urlencoded";
            WebResponse wp = wr.GetResponse();
            StreamReader sr = new StreamReader(wp.GetResponseStream());
            string data = m_r.f("results");
            string res = sr.ReadToEnd();
            string rx = "<li(.*)</cite>";
            int count = 1;
            while (Utils.GetValue(res, rx) != "" && count <= m_count)
            {
                string single = HttpUtility.HtmlDecode(Utils.GetValue(res, rx, true).Replace("<br>", "").Replace("<br/>", ""));
                string name = ClearTags(Utils.GetValue(single, "<a(.*)</a>", true));
                string site = ClearTags(Utils.GetValue(single, "<cite>(.*)</cite>"));
                single = ClearTags(Utils.RemoveValue(Utils.RemoveValue(single, "<a(.*)</a>", true), "<cite>(.*)</cite>", true)).Replace("- [ Translate this page ]", "");
                single = Utils.RemoveValue(single, "<cite>(.*)</cite>", true);
                data += (m_count != 1 ? "\n- " + count.ToString() + " -\n" : "\n") + name + "\n" + single + "\n" + site + "\n";
                res = Utils.RemoveValue(res, rx, true);
                count++;
            }

            if (count == 1)
            { m_r.Reply(m_r.f("google_nothing_found")); return; }
            else
            {
                try
                {
                    string all = ClearTags(Utils.GetValue(res, "<p>&nbsp;Results <b>(.*)</b> for <b>"));
                    all = "-- " + all.Substring(all.IndexOf("of about") + 8) + " --";
                    data += all;
                }
                catch { }
            }
            m_r.Reply(data);

            }
    


         
       
    
}
}
