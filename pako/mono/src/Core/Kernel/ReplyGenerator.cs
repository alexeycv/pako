/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Pako Jabber-bot.                                                              *
 * Copyright. All rights reserved © 2007-2008 by Klichuk Bogdan (Bbodio's Lab)   *
 * Copyright. All rights reserved © 2009-2012 by Alexey Bryohov                  *
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
using System.Collections;
using System.Text;
using System.IO;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;
using agsXMPP.protocol.x.muc;


namespace Core.Kernel
{
    public class ReplyGenerator
    {
        Hashtable m_resps;
        string m_dir;
        int m_count;
        XmppClientConnection m_con;
        int m_msglimit;
        List<string> broken_packages;

        object[] sobjs = new object[10];

        public ReplyGenerator(XmppClientConnection Con, string Folder, int MSGLimit)
        {

            for (int i = 0; i < 10; i++)
            {
                sobjs[i] = new object();
            }

            m_con = Con;
            m_dir = Folder;          
            m_msglimit = MSGLimit;

            m_count = (Directory.GetDirectories(m_dir)).Length;
            BrokenPackets = new List<string>();
            m_resps = new Hashtable();

          

            foreach (string dir in Directory.GetDirectories(m_dir))
            {
                string name = Path.GetFileName(dir);
                Response _resp = new Response(m_con, dir, name, m_msglimit);
                string validation_string = null;

                try
                {
                     _resp.GetHelp("test");
                }
                 catch
                {
                    validation_string = "help.pack";
                }

                  try
                {
                    if (_resp.Document.RootElement.SelectSingleElement("patterns").Attributes.Count > 0)
                        m_resps.Add(name, _resp); 
                }
                 catch
                {
                    validation_string = "lang.pack";
                }

                if (validation_string != null)
                    BrokenPackets.Add(name+" [broken in "+validation_string+"]");
              
     
            }



        }

        public Response this[string lang]
        {
            get { return (Response)Responses[lang]; }
        }


        public string GetPacketsList(bool ShowBroken)
        {
            string data = "";
            foreach (string pack_name in Responses.Keys)
                data += "\n"+pack_name;
            if (ShowBroken)
            foreach (string pack_name in BrokenPackets)
                data += "\n" + pack_name;
            return data;
        }

    



        public Hashtable Responses
        {
            get { lock (sobjs[0]) { return m_resps; } }
            set { lock (sobjs[0]) { m_resps = value; } }
        }



        public List<string> BrokenPackets
        {
            get { lock (sobjs[1]) { return broken_packages; } }
            set { lock (sobjs[1]) { broken_packages = value; } }
        }

        public Response GetResponse(string Language)
        {
            lock (Responses)
            {
                return (Response)Responses[Language];  
            }
        }



    }
}
