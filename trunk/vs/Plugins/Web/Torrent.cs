using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using Core.Kernel;
using Core.Other;

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

            HttpWebRequest wr = (HttpWebRequest)HttpWebRequest.CreateDefault(new System.Uri("http://www.mininova.org/search/?search=" + HttpUtility.UrlEncode("freebsd")));
            wr.Method = "GET";
            wr.ContentType = "application/x-www-form-urlencoded";
            WebResponse _wp = wr.GetResponse();
            StreamReader sr = new StreamReader(_wp.GetResponseStream());
            string data = HttpUtility.HtmlDecode(sr.ReadToEnd());
            string single = Utils.GetValue(data, "<a href=\"/tor(.*)<br />", true);
            string result = "";
            int i = 1;
            while (single.IndexOf("<a href=\"/tor") > -1)
            {
                if (m_num > 0)
                    if (i > m_num)
                        break;
                i++;
                string link = "http://www.mininova.org" + Utils.GetValue(single, "\"(.*)\"").Replace("tor", "get");
                string name = Utils.GetValue(single, ">(.*)<");
                single = Utils.RemoveValue(single, "<a(.*)<a", true);
                string volume = Utils.GetValue(single, ">(.*)<");
                single = Utils.RemoveValue(single, "(.*)</td>", true);
                string size = Utils.GetValue(single, ">(.*)</");
                string seeds = Utils.GetValue(single, "<span class=\"g\">(.*)</span>");
                string leechers = Utils.GetValue(single, "<span class=\"b\">(.*)</span>");
                if (single.IndexOf("<a href=\"/tor") > -1)
                    single = single.Remove(0, single.IndexOf("<a href=\"/tor"));
                else
                    break;
                result += " • [" + volume + "]    " + name + "    " + size + "  " + seeds + "/" + leechers + " \n                          " + link + "\n";
            }
            r.Reply(r.FormatPattern("torrent_results")+"\n"+result.Trim('\n'));
        }
    }
}
