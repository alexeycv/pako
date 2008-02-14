using System;
using System.Collections.Generic;
using System.Text;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;
using Core.Client;
using Core.Special;
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


        public string GetXep(string tip)
       {

           HttpWebRequest wr = (HttpWebRequest)HttpWebRequest.CreateDefault(new System.Uri("http://www.xmpp.org/extensions/xeps.xml"));
           wr.Method = "GET";
           wr.ContentType = "application/x-www-form-urlencoded";
           WebResponse _wp = wr.GetResponse();
           StreamReader sr = new StreamReader(_wp.GetResponseStream());

           doc.LoadXml(sr.ReadToEnd());
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
                           return "XEP "+number+":\n"+ name+" Type: "+ el.GetTag("type")+" Status: "+ el.GetTag("status")+"\n"+"http://www.xmpp.org/extensions/xep-" + number+ ".html";
               }
               catch
               {
                   string[] xparts = Utils.SplitEx(name, name.Length);
                   string[] wparts = Utils.SplitEx(tip, tip.Length);
                   foreach (string xp in xparts)
                   {
                       foreach (string wp in wparts)
                       {
                           if (xp.ToLower().IndexOf(wp.ToLower()) > -1)
                               return "XEP " + number + ":\n" + name + " Type: " + el.GetTag("type") + " Status: " + el.GetTag("status") + "\n" + "http://www.xmpp.org/extensions/xep-" + number + ".html";
                       }
                   }

                      
               }

           }
                   return null;
       }

    }
}
