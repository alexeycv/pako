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
using System.Text.RegularExpressions;
using System.Web;
using System.Net;
using System.IO;
using Core.Kernel;
using Core.Other;
using agsXMPP.Xml.Dom;


namespace www
{
    class Currency
    {
        public Currency()
        {

        }



        public string GetList()
        {
            HttpWebRequest wr = (HttpWebRequest)HttpWebRequest.CreateDefault(new System.Uri("http://www.xe.com/ucc/full.php"));
            wr.Method = "GET";
            wr.ContentType = "application/x-www-form-urlencoded";
            WebResponse _wp = wr.GetResponse();
            StreamReader sr = new StreamReader(_wp.GetResponseStream());
            Document doc = new Document();
            doc.LoadXml("<Currency>" + Utils.GetValue(sr.ReadToEnd(), "<select(.*)/select>", true) + "</Currency>");
            string data = "";
            foreach (Element el in doc.RootElement.SelectSingleElement("select").SelectElements("option"))
            {
                data += el.Value + "\n";
            }
            return data;
            


        }

        public string Find(string tip)
        {
            HttpWebRequest wr = (HttpWebRequest)HttpWebRequest.CreateDefault(new System.Uri("http://www.xe.com/ucc/full.php"));
            wr.Method = "GET";
            wr.ContentType = "application/x-www-form-urlencoded";
            WebResponse _wp = wr.GetResponse();
            StreamReader sr = new StreamReader(_wp.GetResponseStream());
            Document doc = new Document();
            doc.LoadXml("<Currency>" + Utils.GetValue(sr.ReadToEnd(), "<select(.*)/select>", true) + "</Currency>");

          //  string data = "";

            foreach (Element el in doc.RootElement.SelectSingleElement("select").SelectElements("option"))
            {
                if (el.Value.ToLower().IndexOf(tip.ToLower()) > -1)
                    return el.Value;
            }


            return null;



        }
        public string Handle(string from, string to, double amount)
        {
            HttpWebRequest wr = (HttpWebRequest)HttpWebRequest.CreateDefault(new System.Uri("http://www.xe.com/ucc/convert.cgi?Amount="+(amount*1.0).ToString().Replace(",",".")+"&From="+from.ToUpper()+"&To="+to.ToUpper()));
            wr.Method = "GET";
            wr.ContentType = "application/x-www-form-urlencoded";
            WebResponse _wp = wr.GetResponse();
            StreamReader sr = new StreamReader(_wp.GetResponseStream());

            string temp = sr.ReadToEnd();
            string data = "";
            Regex reg = new Regex("<h2 class=\"XE\" style=\"color:#333\">(.*)<!--");
            MatchCollection mc = reg.Matches(temp);
            data = Utils.GetValue(mc[0].ToString(), "<h2 class=\"XE\" style=\"color:#333\">(.*)<!--");
            data += " = " + Utils.GetValue(mc[1].ToString(), "<h2 class=\"XE\" style=\"color:#333\">(.*)<!--");

            //if (data == "")
            //{
            //    data=temp;
            //}

            //return data;

            //string _sbegin = temp.Substring(temp.IndexOf("<h2 class=\"XE\""), temp.Length - temp.IndexOf("<h2 class=\"XE\"") - 1);
            //string _send = _sbegin.Substring(1, 10);
            //data = _send;

            return data;
        }



    }


}
