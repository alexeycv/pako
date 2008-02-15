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





    public class TranslateUtil
    {

        public Hashtable Modes = new Hashtable();





        string m_text = "";
        string m_pair = "";
        Message m_msg;
        Response m_r;


        public  TranslateUtil()
        {



            Modes.Add("ef", new string[] { "en%7Cfr", "English to French" });
            Modes.Add("fe", new string[] { "fr%7Cen", "French to English" });
            Modes.Add("ed", new string[] { "en%7Cde", "English to German" });
            Modes.Add("de", new string[] { "de%7Cen", "German to English" });
            Modes.Add("es", new string[] { "en%7Ces", "English to Spanish" });
            Modes.Add("se", new string[] { "es%7Cen", "Spanish to English" });
            Modes.Add("ei", new string[] { "en%7Cit", "English to Italian" });
            Modes.Add("ie", new string[] { "it%7Cen", "Italian to English" });
            Modes.Add("ep", new string[] { "en%7Cpt", "English to Portugese" });
            Modes.Add("pe", new string[] { "pt%7Cen", "Portugese to English" });
            Modes.Add("ae", new string[] { "ar%7Cen", "Arabic to English" });
            Modes.Add("ze", new string[] { "zh%7Cen", "Chinese to English" });
            Modes.Add("zCT", new string[] { "zh-CN|%7C-TW", "ChineseSimplified to Traditional" });
            Modes.Add("zTC", new string[] { "zh-TW%7Czh-CN", "ChineseTraditional to Simplified" });
            Modes.Add("ne", new string[] { "nl%7Cen", "Dutch to English" });
            Modes.Add("ea", new string[] { "en%7Car", "English to Arabic" });
            Modes.Add("ezC", new string[] { "en%7Czh-CN", "English to Chinese (Simplified)" });
            Modes.Add("ezT", new string[] { "en%7Czh-TW", "English to Chinese (Traditional)" });
            Modes.Add("en", new string[] { "en%7Cnl", "English to Dutch" });
            Modes.Add("eg", new string[] { "en%7Cel", "English to Greek" });
            Modes.Add("ej", new string[] { "en%7Cja", "English to Japanese" });
            Modes.Add("ek", new string[] { "en%7Cko", "English to Korean" });
            Modes.Add("er", new string[] { "en%7Cru", "English to Russian" });
            Modes.Add("fd", new string[] { "fr%7Cde", "French to German" });
            Modes.Add("df", new string[] { "de%7Cfr", "German to French" });
            Modes.Add("ge", new string[] { "el%7Cen", "Greek to English" });
            Modes.Add("je", new string[] { "ja%7Cen", "Japanese to English" });
            Modes.Add("ke", new string[] { "ko%7Cen", "Korean to English" });
            Modes.Add("re", new string[] { "ru%7Cen", "Russian to English" });


        }




        public bool PairExists(string pair)
        {
            return Modes[pair] != null ? true : false;
        }


        public void Execute(string Text, string langpair, Response r)
        {
            m_msg = r.Msg;
            m_r = r;
            m_pair = langpair;
            m_text = Text;
            @out.exe(m_pair);
            @out.exe(m_text);
            if (PairExists(m_pair))
            {
                Translate();
            }
            else
                m_r.Reply(m_r.FormatPattern("Pair_not_existing"));
        }

        void Translate()
        {

            m_pair = ((string[])Modes[m_pair])[0];
            //HTMLPage ww = new HTMLPage();
            HttpWebRequest wr = (HttpWebRequest)HttpWebRequest.CreateDefault(new System.Uri("http://translate.google.com/translate_t?text=" +HttpUtility.UrlEncode(m_text)+"&langpair=" +m_pair));
            wr.Method = "GET";
            wr.ContentType = "application/x-www-form-urlencoded";
            wr.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
            WebResponse wp = wr.GetResponse();
            StreamReader sr = new StreamReader(wp.GetResponseStream());

          //  string data = "";

            string res = sr.ReadToEnd();


           res = res.Substring(res.IndexOf("</textarea>")+10).GetValue("<textarea(.*)/textarea>").GetValue(">(.*)<");
            
            m_r.Reply(HttpUtility.HtmlDecode(res));
        }
            
            /*
            if (stuff.Length == 0)
                MessageBox.Show("no content");
            else
            {
                Regex findData = new Regex(@"<(?<tag>.*).*>(?<text>.*)</\k<tag>>");
                Match foundData = findData.Match(stuff[0]);
                MessageBox.Show(foundData.Groups["text"].Value);
            }*/


         
       
    
}
}