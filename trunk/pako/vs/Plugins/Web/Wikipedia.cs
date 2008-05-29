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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Net;
using System.Text.RegularExpressions;
using Core.Kernel;
using Core.Other;

namespace www
{
	/// <summary>
	/// Description of Wikipedia.
	/// </summary>
	public class Wikipedia
	{
		
		
		Response m_r;
		string m_text;
		
		public Wikipedia(string text, Response r)
		{
			m_r = r;
			m_text = text;
			Handle();
		}
		
		
		void Handle()
		{
			
			
			try
			{
				string lang = m_r.Language;
				lang = lang == "ua" ? "uk" : lang;
			HttpWebRequest wr = (HttpWebRequest)HttpWebRequest.CreateDefault(new System.Uri("http://"+lang+".wikipedia.org/wiki/"+m_text));
            wr.Method = "GET";
            wr.KeepAlive  = false;
            wr.ContentType = "application/x-www-form-urlencoded";
            wr.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
            WebResponse wp = wr.GetResponse();
            StreamReader sr = new StreamReader(wp.GetResponseStream());
            string res = sr.ReadToEnd();
         if (res.IndexOf("<p>") == -1)
         {
         	m_r.Reply(m_r.f("wiki_not_found", m_text));
         	return;
         }
          while (res.IndexOf("<table") > -1)
          	res = Utils.RemoveValue(res, "<table(.*)</table>",true);
           Regex r = new Regex("<p>(.*)</p>");
           string data = "";
           foreach (Match m in r.Matches(res))
           {
           	string d = m.ToString();
           	while ((d.IndexOf("<") >-1) && (d.IndexOf(">") >-1))
           		d = Utils.RemoveValue(d, "<(.*)>",true);
           	if (d.Trim() != "")
           	data += d.ToString()+"\n";
           }
           
           m_r.Reply(data == "" ? m_r.f("wiki_not_found", m_text) : "Wikipedia:  \""+m_text+"\"\nhttp://"+lang+".wikipedia.org/wiki/"+HttpUtility.UrlEncode(m_text)+"\n"+HttpUtility.HtmlDecode(data).Trim('\n','\r',' '));
			} catch { m_r.Reply(m_r.f("wiki_not_found")); }
           
		}
		
	}
}
