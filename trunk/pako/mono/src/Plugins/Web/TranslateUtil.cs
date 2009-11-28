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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Threading;
using System.Web;
using System.Net;
using System.Xml;
using Core.Plugins;
using Core.Kernel;
using Core.Other;
using agsXMPP;
using agsXMPP.Xml.Dom;
using agsXMPP.protocol.client;



namespace www
{




    public class TranslateUtil
    {




       

        string m_text = "";
        string a_lang; 
        string b_lang;
        Message m_msg;
        Response m_r;


        public  TranslateUtil()
        {
            //TODO
        }




        public string GetPair(string key)
        {


            HttpWebRequest wr = (HttpWebRequest)HttpWebRequest.CreateDefault(new System.Uri("http://translate.google.com/translate_t"));
            wr.Method = "GET";
            wr.ContentType = "application/x-www-form-urlencoded";
            wr.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
            WebResponse wp = wr.GetResponse();
            StreamReader sr = new StreamReader(wp.GetResponseStream());
            string res = sr.ReadToEnd();
            string opts = Utils.GetValue(res, "<select(.*)</select>", true);
            Document doc = new Document();
            opts = ("<select>" + Utils.RemoveValue(opts, "<(.*)</option>", true)).Replace(" SELECTED ", " ");
            doc.LoadXml("<Document>" + opts + "</Document>");
            @out.exe(opts);
            foreach (Element el in doc.RootElement.SelectSingleElement("select").SelectElements("option"))
            {
                string val = el.GetAttribute("value");
                if (val != "auto" && val == key)
                    return el.InnerXml;
            }
            return null;
        }



        public Dictionary<string,string> GetAllPairs()
        {
            string res = "";
            string opts = "";
            Dictionary<string,string> dict = new Dictionary<string,string>();

            try
            {
                //HttpWebRequest wr = (HttpWebRequest)HttpWebRequest.CreateDefault(new System.Uri("http://translate.google.com/translate_t"));
                HttpWebRequest wr = (HttpWebRequest)HttpWebRequest.CreateDefault(new System.Uri("http://translate.google.com/"));
                wr.Method = "GET";
                wr.ContentType = "application/x-www-form-urlencoded";
                wr.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
                WebResponse wp = wr.GetResponse();
                StreamReader sr = new StreamReader(wp.GetResponseStream());
                res = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                //@out.exe(ex.Message);
            }

            opts = Utils.GetValue(res, "<select(.*)</select>", true);
            Document doc = new Document();
            opts = ("<select>" + Utils.RemoveValue(opts, "<(.*)</option>", true)).Replace(" SELECTED ", " ");
            opts = opts.Replace("&#8212;", "_").Replace("disabled", "");
            doc.LoadXml("<Document>"+opts+"</Document>");
            @out.exe(opts);
            //@out.write(doc.ToString()+"\n\n"+opts) ;
            try
            {
               if (doc != null)
               {
                   foreach (Element el in doc.RootElement.SelectSingleElement("select").SelectElements("option"))
                   {
                       if (el != null)
                       {
                           if (el.GetAttribute("value") != "auto")
                           {
                               try
                               {
                                   if (el.GetAttribute("value") != "separator")
                                       dict.Add(el.GetAttribute("value"), el.InnerXml);
                               }
                               catch (Exception ex)
                               {
                               }
                           }
                       }//if
                   }
               }
            }
            catch (Exception ex)
            {                
            }

            return dict;
        }


        public void Execute(string Text, string alang, string blang, Response r)
        {
            m_msg = r.Msg;
            m_r = r;
            a_lang = alang;
            b_lang = blang;
            m_text = Text;
            @out.exe(alang + " " + blang);
            @out.exe(m_text);
            Translate();
        }

        void Translate()
        {
            string res;
            try
            {
                string m_pair = a_lang + "%7C" + b_lang;
                HttpWebRequest wr = (HttpWebRequest)HttpWebRequest.CreateDefault(new System.Uri("http://translate.google.com/translate_t?text=" + HttpUtility.UrlEncode(m_text) + "&langpair=" + m_pair));
                wr.Method = "GET";
                wr.ContentType = "application/x-www-form-urlencoded";
                wr.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
                WebResponse wp = wr.GetResponse();
                StreamReader sr = new StreamReader(wp.GetResponseStream());
                 res = sr.ReadToEnd();
            }
            catch
            {
                @out.exe("gt: ERROR");
                m_r.Reply(m_r.f("gt_failed"));
                return;
            }

            res = Utils.GetValue(Utils.GetValue(res.Substring(res.IndexOf("</textarea>")+10), "<textarea(.*)/textarea>"), ">(.*)<");
            @out.exe("'" + res + "'");
            string result = HttpUtility.HtmlDecode(HttpUtility.HtmlDecode(res)).Replace("<br>", "\n");
            result = result.Trim() == "" ? m_r.f("lang_not_existing") : result;
            m_r.Reply(result);
        }
            
         


         
       
    
}
}
