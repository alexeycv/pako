using System;
using System.Collections.Generic;
using System.Text;
using agsXMPP;
using agsXMPP.Xml.Dom;

namespace Core.Client
{
    public class Actor : XMLContainer
    {

        public Actor(string file)
        {
            Open(file, 20);

        }

        public string Act(string phrase)
        {
            string response = null;
            foreach (Element el in Document.RootElement.SelectElements("phrase"))
            {
                string code = el.GetAttribute("name");



            }
            return response;
        }

    }
}
