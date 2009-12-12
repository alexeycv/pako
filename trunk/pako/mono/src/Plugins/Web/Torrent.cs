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
using System.Web;
using System.Net;
using System.IO;
using Core.Kernel;
using Core.Other;
using agsXMPP.Xml.Dom;
using System.Text.RegularExpressions;

namespace www
{
    public class Torrent
    {
        string tip;
        Response r;
        int m_num;

        public Torrent(string text, Response resp, int num)
        {
            tip = text;
            r = resp;
            m_num = num;
        }


        public void Handle()
        {
                HttpWebRequest wr = (HttpWebRequest)HttpWebRequest.CreateDefault(new System.Uri("http://www.mininova.org/search/?search=" + HttpUtility.UrlEncode(tip)));
                wr.Method = "GET";
                wr.ContentType = "application/x-www-form-urlencoded";
                WebResponse _wp = wr.GetResponse();
                StreamReader sr = new StreamReader(_wp.GetResponseStream());
                string data = HttpUtility.HtmlDecode(sr.ReadToEnd()).Replace("&","&amp;");
                Document doc = new Document();
                if (Utils.GetValue(data, "<table(.*)</table>").Trim() == "")
                {
                    r.Reply(r.f("torrent_nothing_found"));
                    return;
                }
                doc.LoadXml("<torrent>" + Utils.GetValue(data, "<table(.*)</table>", true) + "</torrent>");
                ElementList els = doc.RootElement.SelectSingleElement("table").SelectElements("tr");
                string all = ""; 
                    try
                {
                     all = Utils.GetValue(data, "</em>(.*)<a href").Trim(' ', '\n');
                }
                    catch (Exception)
                    {
                    }
                int i = 0;
                string result = "";
                foreach (Element el in els)
                {
                    try
                    {
                        if (i != 0)
                        {
                            if (i <= m_num)
                            {
                                Element _el = el.SelectElements("td").Item(2);
                                result += " • [" + _el
                                    .SelectSingleElement("small")
                                    .SelectSingleElement("strong")
                                    .SelectSingleElement("a").Value + "]    " +
                                    _el.SelectElements("a").Item(1).Value + "     |" +
                                    el.SelectElements("td").Item(3).Value + "|  (" +
                                    el.SelectElements("td").Item(4).SelectSingleElement("span").Value + "/" +
                                    el.SelectElements("td").Item(4).SelectSingleElement("span").Value + ")\n                  http://mininova.org" +
                                    _el.SelectElements("a").Item(0).GetAttribute("href") + "\n";


                            }
                            else
                                break;
                        }

                        i++;
                    }
                    catch (Exception)
                    {
                    }



                }
                r.Reply(r.f("torrent_results") + "\n" + result + "-- " + all.Trim('(', ')') + " --");
            
          
        }
    }
}
