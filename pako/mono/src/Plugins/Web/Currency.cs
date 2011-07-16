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
using System.Xml;
using System.IO;


namespace www
{
    class Currency
    {
        public Currency()
        {

        }



        public string GetList()
        {
			HttpWebRequest _request = (HttpWebRequest)HttpWebRequest.CreateDefault(new System.Uri("http://www.google.com/finance/converter"));
            _request.Method = "GET";

            HttpWebResponse _response = (HttpWebResponse)_request.GetResponse();
            Encoding _responseEncoding = Encoding.GetEncoding(_response.CharacterSet);
            StreamReader _reader = new StreamReader(_response.GetResponseStream(), _responseEncoding);

            String _data = "";
            _data = _reader.ReadToEnd().Replace("name=from", "name=\"from\"");

            String _resultStr = "";

            Int32 _selectStart = _data.IndexOf("<select");
            Int32 _selectEnd = _data.IndexOf("</select>");

            _resultStr = _data.Substring(_selectStart, _selectEnd - _selectStart + 9);

            XmlDocument _xml = new XmlDocument();
            _xml.LoadXml(_resultStr);

            _resultStr = "";
            XmlNodeList _xmlNodes = _xml.SelectNodes("/select/option");
            foreach (XmlNode _node in _xmlNodes)
            {
                _resultStr += _node.InnerText + "\n";
            }
			
			return _resultStr;
			
			// The old code
			/*
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
            */


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
			HttpWebRequest _request = (HttpWebRequest)HttpWebRequest.CreateDefault(new System.Uri("http://www.google.com/finance/converter?a="+(amount*1.0).ToString().Replace(",",".")+"&from="+from.ToUpper()+"&to="+to.ToUpper()));
            _request.Method = "GET";

            HttpWebResponse _response = (HttpWebResponse)_request.GetResponse();
            Encoding _responseEncoding = Encoding.GetEncoding(_response.CharacterSet);
            StreamReader _reader = new StreamReader(_response.GetResponseStream(), _responseEncoding);

            String _data = "";
            _data = _reader.ReadToEnd();
            String _resultStr = "";

            //_resultStr = _data.Substring(_data.IndexOf("<span id=\"result_box\""));
            Regex _reg = new Regex("<div id=currency_converter_result>(.*)");
            MatchCollection _mc = _reg.Matches(_data);
            _resultStr = _mc[0].ToString().Replace("<div id=currency_converter_result>", "").Replace("<span class=bld>", "").Replace("</span>", "");
			
			return _resultStr;
			
			// This is an old code
			/*
            HttpWebRequest wr = (HttpWebRequest)HttpWebRequest.CreateDefault(new System.Uri("http://www.xe.com/ucc/convert.cgi?Amount="+(amount*1.0).ToString().Replace(",",".")+"&From="+from.ToUpper()+"&To="+to.ToUpper()));
            wr.Method = "GET";
            wr.ContentType = "application/x-www-form-urlencoded";
            WebResponse _wp = wr.GetResponse();
            StreamReader sr = new StreamReader(_wp.GetResponseStream());

            string temp = sr.ReadToEnd();
            string data = "";
			*/
            // 23.05.2010 by Alexey.cv
            // OLD CODEE
            // Commented temporary
            //Regex reg = new Regex("<h2 class=\"XE\" style=\"color:#333\">(.*)<!--");
            //MatchCollection mc = reg.Matches(temp);
            //data = Utils.GetValue(mc[0].ToString(), "<h2 class=\"XE\" style=\"color:#333\">(.*)<!--");
            //data += " = " + Utils.GetValue(mc[1].ToString(), "<h2 class=\"XE\" style=\"color:#333\">(.*)<!--");

            // 23.05.2010 by Alexey.cv
            // xe.com result data parser
			/*
            Regex _reg = new Regex("<td align=\"right\" class=\"rate\" >(.*)<!--");

            MatchCollection _mc = _reg.Matches(temp);

            data = _mc[0].ToString().Replace("<td align=\"right\" class=\"rate\" >", "").Replace("<!--", "");

            _reg = new Regex("<td align=\"left\" class=\"rate\" >(.*)<!--");

            _mc = _reg.Matches(temp);

            data += " = " + _mc[0].ToString().Replace("<td align=\"left\" class=\"rate\" >", "").Replace("<!--", "");
            return data;
            */
        }



    }


}
