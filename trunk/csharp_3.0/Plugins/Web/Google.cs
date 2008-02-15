using System;
using System.Text.RegularExpressions;
using System.Data;
using System.Collections;
using System.IO;
using System.Threading;
using System.Web;
using System.Net;
using Core.Plugins;
using Core.Client;
using Core.Special;
using agsXMPP;
using agsXMPP.protocol.client;



namespace www
{
    /// <summary>
    /// Google Translation Utility Class (c)Peter A. Bromberg 2005
    /// </summary>





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
            HttpWebRequest wr = (HttpWebRequest)HttpWebRequest.CreateDefault(new System.Uri("http://www.google.com/search?q=" + m_text + "&ie=UTF-8&oe=UTF-8"));
            wr.Method = "GET";
            wr.ContentType = "application/x-www-form-urlencoded";
            WebResponse wp = wr.GetResponse();
            StreamReader sr = new StreamReader(wp.GetResponseStream());
            string data = "";
            string res = sr.ReadToEnd();
            if (res.IndexOf("class=g><h2 class=r>") < 0)
            {
                m_r.Reply(m_r.FormatPattern("google_nothing_found"));
                return;
            }
            res = res.Remove(0, res.IndexOf("class=g><h2 class=r>"));
            Regex regex = new Regex("class=g><h2 class=r>(.*)</td></tr></table></div>");
            bool success = false;
            int count = 0;

            while (regex.IsMatch(res) && (m_count > count))
            {
                success = true;
                string _res = "";
                foreach (Match m in regex.Matches(res))
                {
                    _res = m.ToString();
                    break;
                }

                string subject = _res.GetValue("class=l>(.*)</a>");
                string url = _res.Substring(_res.IndexOf("class=g")).GetValue("<a href=\"(.*)\" class=l>", false);
                string body = _res.GetValue("<tr><td class=\"j\"><font size=-1>(.*)<br><span class=a>");
                res = res.GetValue("class=l>(.*)</a>", true);
                _res = subject + "\n" + body + "\n" + url;
                
              while ((_res.IndexOf("<") != -1) && (_res.IndexOf(">") != -1))
                {
                    _res = _res.GetValue("<(.*)>", true);
                }

                if (m_count > 1)
                {
                    if (count == 0)
                        data += "\n" + (count + 1) + "•••\n" + HttpUtility.HtmlDecode(_res);
                    else
                        data += "\n\n" + (count + 1) + "•••\n" + HttpUtility.HtmlDecode(_res);
                }
                else
                {
                    data += HttpUtility.HtmlDecode(_res);
                }
                res = res.GetValue("class=g><h2 class=r>(.*)</td></tr></table></div>", true);
                count++;
            }
            if (!success)
                m_r.Reply(m_r.FormatPattern("google_nothing_found"));
            else
                m_r.Reply(data);
            }
    


         
       
    
}
}