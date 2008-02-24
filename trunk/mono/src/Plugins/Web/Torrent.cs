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
            Console.WriteLine("torrent");
                HttpWebRequest wr = (HttpWebRequest)HttpWebRequest.CreateDefault(new System.Uri("http://www.mininova.org/search/?search=" + HttpUtility.UrlEncode(tip)));
                wr.Method = "GET";
                wr.ContentType = "application/x-www-form-urlencoded";
                WebResponse _wp = wr.GetResponse();
                StreamReader sr = new StreamReader(_wp.GetResponseStream());
                string data = HttpUtility.HtmlDecode(sr.ReadToEnd()).Replace("&","&amp;");
                Document doc = new Document();
                Console.WriteLine("data");
                if (Utils.GetValue(data, "<table(.*)</table>").Trim() == "")
                {
                    r.Reply(r.FormatPattern("torrent_nothing_found"));
                    return;
                }
                Console.WriteLine("before load");
                doc.LoadXml("<torrent>" + Utils.GetValue(data, "<table(.*)</table>", true) + "</torrent>");
                Console.WriteLine("after load"); 
            if (doc.RootElement == null)
                Console.WriteLine("root null");
            Console.WriteLine(doc.ToString());
                ElementList els = doc.RootElement.SelectSingleElement("table").SelectElements("tr");
                Console.WriteLine(Utils.GetValue(data, "<table(.*)</table>", true));
                string all = ""; 
            try
                {
                     all = Utils.GetValue(data, "</em>(.*)<a href").Trim(' ', '\n');
                }
                    catch (Exception ex)
                    {
                        r.Reply(ex.ToString());
                    }
                int i = 0;
                if (all == null)
                    Console.WriteLine("all");
                if (els == null)
                    Console.WriteLine("els");
                string result = "";
                Console.WriteLine("gathering...");
                foreach (Element el in els)
                {
                    Console.WriteLine("one...");
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
                    catch (Exception ex)
                    {
                        r.Reply(ex.ToString());
                    }



                }
                r.Reply(r.FormatPattern("torrent_results") + "\n" + result + "-- " + all.Trim('(', ')') + " --");
            
          
        }
    }
}
