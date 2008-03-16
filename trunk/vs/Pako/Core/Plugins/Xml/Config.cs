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
using Core.Kernel;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;

namespace Core.Xml
{
    public class Config : XMLContainer
    {

        public Config(string File)
        {
            Open(File, 16);
        }
 

         public string Nick
        {
            get
            {
                lock (Document)
                {
                    return Document.RootElement.SelectSingleElement("bot").SelectSingleElement("nick").GetAttribute("value");
                }
            }

            set
            {
                lock (Document)
                {
                     Document.RootElement.SelectSingleElement("bot").SelectSingleElement("nick").SetAttribute("value", value);
                     Save();
                }
            }

        }
        public bool AutoSubscribe
        {
            get
            {
                lock (Document)
                {
                    return Document.RootElement.SelectSingleElement("bot").SelectSingleElement("autosubscribe").GetAttributeBool("value");
                }
            }

            set
            {
                lock (Document)
                {
                    Document.RootElement.SelectSingleElement("bot").SelectSingleElement("autosubscribe").SetAttribute("value", value);
                    Save();
                }
            }

        }
        public bool Debug
        {
            get
            {
                lock (Document)
                {
                    return Document.RootElement.SelectSingleElement("bot").SelectSingleElement("debug").GetAttributeBool("value");
                }
            }

            set
            {
                lock (Document)
                {
                    Document.RootElement.SelectSingleElement("bot").SelectSingleElement("debug").SetAttribute("value", value);
                    Save();
                }
            }

        }
         public string ConnectServer
         {
             get
             {
                 lock (Document)
                 {
                     return  Document.RootElement.SelectSingleElement("bot").SelectSingleElement("connect_server").GetAttribute("value");
                 }
             }

             set
             {
                 lock (Document)
                 {
                     Document.RootElement.SelectSingleElement("bot").SelectSingleElement("connect_server").SetAttribute("value", value);
                     Save();
                 }
             }

         }


        public Jid Jid
        {
            get
            {
                lock (Document)
                {
                    return  new Jid(Document.RootElement.SelectSingleElement("bot").SelectSingleElement("jid").GetAttribute("value"));
                }
            }

            set
            {
                lock (Document)
                {
                    Document.RootElement.SelectSingleElement("bot").SelectSingleElement("jid").SetAttribute("value", value.ToString());
                    Save();
                }
            }

        }




        public string Password
        {
            get
            {
                lock (Document)
                {
                    return Document.RootElement.SelectSingleElement("bot").SelectSingleElement("password").GetAttribute("value");
                }
            }

            set
            {
                lock (Document)
                {
                    Document.RootElement.SelectSingleElement("bot").SelectSingleElement("password").SetAttribute("value", value);
                    Save();
                }
            }

        }



        public string Delimiter
        {
            get
            {
                lock (Document)
                {
                    return Document.RootElement.SelectSingleElement("bot").SelectSingleElement("delimiter").GetAttribute("value");
                }
            }

            set
            {
                lock (Document)
                {
                    Document.RootElement.SelectSingleElement("bot").SelectSingleElement("delimiter").SetAttribute("value", value);
                    Save();
                }
            }

        }


        public string Status
        {
            get
            {
                lock (Document)
                {
                    return Document.RootElement.SelectSingleElement("bot").SelectSingleElement("status").GetAttribute("value");
                }
            }

            set
            {
                lock (Document)
                {
                    Document.RootElement.SelectSingleElement("bot").SelectSingleElement("status").SetAttribute("value", value);
                    Save();
                }
            }

        }

        public Jid[] Administartion()
        {
            lock (Document)
            {
                Jid[] jids_set = new Jid[Document.RootElement.SelectSingleElement("bot").SelectSingleElement("admins").GetAttribute("value").Split('|').Length];
                int i = 0;
                foreach (string jid in Document.RootElement.SelectSingleElement("bot").SelectSingleElement("admins").GetAttribute("value").Split('|'))
                {
                    jids_set[i] = new Jid(jid);
                    i++;
                }
                return jids_set;
            }
        }

        public bool BotAdmin(Jid Jid)
        {
            lock (Document)
            {
                if (Jid == null)
                    return false;
                else
                {
                    foreach (string s in Document.RootElement.SelectSingleElement("bot").SelectSingleElement("admins").GetAttribute("value").Split('|'))
                    {
                        if (s == Jid.Bare)
                            return true;
                    }
                    return false;
                }
            }
        }


        public int MSGLimit
        {
            get
            {
                lock (Document)
                {
                    return  Document.RootElement.SelectSingleElement("bot").SelectSingleElement("msglimit").GetAttributeInt("value");
                }
            }

            set
            {
                lock (Document)
                {
                    Document.RootElement.SelectSingleElement("bot").SelectSingleElement("msglimit").SetAttribute("value", value);
                    Save();
                }
            }

        }


        public bool UseSSL
        {
            get
            {
                lock (Document)
                {
                    return  Document.RootElement.SelectSingleElement("bot").SelectSingleElement("ssl").GetAttributeBool("value");
                }
            }

            set
            {
                lock (Document)
                {
                    Document.RootElement.SelectSingleElement("bot").SelectSingleElement("ssl").SetAttribute("value", value.ToString());
                    Save();
                }
            }

        }

        public bool UseCompression
        {
            get
            {
                lock (Document)
                {
                    return Document.RootElement.SelectSingleElement("bot").SelectSingleElement("compression").GetAttributeBool("value");
                }
            }

            set
            {
                lock (Document)
                {
                    Document.RootElement.SelectSingleElement("bot").SelectSingleElement("compression").SetAttribute("value", value.ToString());
                    Save();
                }
            }

        }


        public bool UseStartTls
        {
            get
            {
                lock (Document)
                {
                    return Document.RootElement.SelectSingleElement("bot").SelectSingleElement("starttls").GetAttributeBool("value");
                }
            }

            set
            {
                lock (Document)
                {
                    Document.RootElement.SelectSingleElement("bot").SelectSingleElement("starttls").SetAttribute("value", value.ToString());
                    Save();
                }
            }

        }


        public int ReconnectTime
        {
            get
            {
                lock (Document)
                {
                    return Document.RootElement.SelectSingleElement("bot").SelectSingleElement("reconnect_time").GetAttributeInt("value");
                }
            }

            set
            {
                lock (Document)
                {
                    Document.RootElement.SelectSingleElement("bot").SelectSingleElement("reconnect_time").SetAttribute("value", value);
                    Save();
                }
            }

        }

        public int MaxReconnects
        {
            get
            {
                lock (Document)
                {
                    return Document.RootElement.SelectSingleElement("bot").SelectSingleElement("max_reconnects").GetAttributeInt("value");
                }
            }

            set
            {
                lock (Document)
                {
                    Document.RootElement.SelectSingleElement("bot").SelectSingleElement("max_reconnects").SetAttribute("value", value);
                    Save();
                }
            }

        }

        public int Port
        {
            get
            {
                lock (Document)
                {
                    return  Document.RootElement.SelectSingleElement("bot").SelectSingleElement("port").GetAttributeInt("value");
                }
            }

            set
            {
                lock (Document)
                {
                    Document.RootElement.SelectSingleElement("bot").SelectSingleElement("port").SetAttribute("value", value);
                    Save();
                }
            }

        }


        public string Language
        {
            get
            {
                lock (Document)
                {
                    return Document.RootElement.SelectSingleElement("bot").SelectSingleElement("lang").GetAttribute("value");
                }
            }

            set
            {
                lock (Document)
                {
                    Document.RootElement.SelectSingleElement("bot").SelectSingleElement("lang").SetAttribute("value", value);
                    Save();
                }
            }

        }

    }
}
