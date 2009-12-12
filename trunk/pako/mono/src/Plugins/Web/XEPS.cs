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
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;
using Core.Kernel;
using Core.Other;
using System.Web;
using System.Net;
using System.IO;

namespace www
{
    public class XEPS
    {
        Document doc;

        public XEPS()
        {
            doc = new Document();
        }


        public string GetXep(string found_pattern, string tip)
       {

           HttpWebRequest wr = (HttpWebRequest)HttpWebRequest.CreateDefault(new System.Uri("http://www.xmpp.org/extensions/xeps.xml"));
           wr.Method = "GET";
           wr.ContentType = "application/x-www-form-urlencoded";
           WebResponse _wp = wr.GetResponse();
           StreamReader sr = new StreamReader(_wp.GetResponseStream());
           doc.LoadXml(sr.ReadToEnd());

           string result = "";
           int res_count = 0;
           foreach (Element el in doc.RootElement.SelectElements("xep"))
           {
               string name = el.GetTag("name");
               string number = el.GetTag("number");
               int _number = el.GetTagInt("number");
               try
               {
                   int num = Convert.ToInt32(tip);
                   if (_number != 0)
                       if (_number == num)
                       {
                           result += " * XEP " + number + ": " + name + ".   Type: " + el.GetTag("type") + " (" + el.GetTag("status") + ") => http://www.xmpp.org/extensions/xep-" + number + ".html\n";
                           res_count++;
                       }
               }
               catch
               {
                   string[] xparts = name.Split(new char[] { ' ', '=', '-', ':', ':', '\'', '\"', '/', '?', '.', ',', '[', ']', '+', '_', '*', '&','(',')' });
                   string[] wparts = tip.Split(new char[] { ' ', '=', '-', ':', ':', '\'', '\"', '/', '?', '.', ',', '[', ']', '+', '_', '*', '&','(',')' }); 
                  
                   foreach (string xp in xparts)
                   {
                       foreach (string wp in wparts)
                       {
                           if ((xp.Trim() != "") && (wp.Trim() != ""))
                           if (xp.ToLower() == wp.ToLower())
                           {
                               result += " • XEP " + number + ": " + name + ".   Type: " + el.GetTag("type") + " (" + el.GetTag("status") + ") => http://www.xmpp.org/extensions/xep-" + number + ".html\n";
                               res_count++;
                           }
                       }
                   }

                      
               }

           }
                return result != "" ? found_pattern+"\n" + result + "-- " + res_count.ToString() + " --" : null;
       }

    }
}
