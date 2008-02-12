using System;
using System.Collections.Generic;
using System.Text;
using Core.Client;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;

namespace Core.Manager
{
    public class Config:XMLContainer
    {
         int m_msglimit;
         Jid m_jid;
         string m_pass;
         string m_nick;
         string m_status; 
         string m_delimiter;
         bool starttls;
         bool compression;
         bool ssl;
         int m_reconnect_time;
         int max_reconnects;
         int m_port;
         string m_lang;

        public Config(string File)
        {

            Open(File, 14);
            m_lang = Document.RootElement.SelectSingleElement("bot").SelectSingleElement("lang").GetAttribute("value");
            m_nick = Document.RootElement.SelectSingleElement("bot").SelectSingleElement("nick").GetAttribute("value");
            m_jid = new Jid(Document.RootElement.SelectSingleElement("bot").SelectSingleElement("jid").GetAttribute("value"));
            m_status = Document.RootElement.SelectSingleElement("bot").SelectSingleElement("status").GetAttribute("value");
            m_msglimit = Document.RootElement.SelectSingleElement("bot").SelectSingleElement("msglimit").GetAttributeInt("value");
            m_delimiter = Document.RootElement.SelectSingleElement("bot").SelectSingleElement("delimiter").GetAttribute("value");
            m_pass = Document.RootElement.SelectSingleElement("bot").SelectSingleElement("password").GetAttribute("value");
            m_port = Document.RootElement.SelectSingleElement("bot").SelectSingleElement("port").GetAttributeInt("value");
            m_reconnect_time = Document.RootElement.SelectSingleElement("bot").SelectSingleElement("reconnect_time").GetAttributeInt("value");
            max_reconnects = Document.RootElement.SelectSingleElement("bot").SelectSingleElement("max_reconnects").GetAttributeInt("value");
            compression = Document.RootElement.SelectSingleElement("bot").SelectSingleElement("compression").GetAttributeBool("value");
            starttls = Document.RootElement.SelectSingleElement("bot").SelectSingleElement("starttls").GetAttributeBool("value");
            ssl = Document.RootElement.SelectSingleElement("bot").SelectSingleElement("ssl").GetAttributeBool("value");
            //Console.WriteLine("config");
        }
 

         public string Nick
        {
            get
            {
                lock (aso[0])
                {
                    return m_nick;
                }
            }

            set
            {
                lock (Document)
                {
                     m_nick = value;
                     Document.RootElement.SelectSingleElement("bot").SelectSingleElement("nick").SetAttribute("value", value);
                     Save();
                }
            }

        }



        public Jid Jid
        {
            get
            {
                lock (aso[1])
                {
                    return m_jid;
                }
            }

            set
            {
                lock (Document)
                {
                    m_jid = value;
                    Document.RootElement.SelectSingleElement("bot").SelectSingleElement("jid").SetAttribute("value", value.ToString());
                    Save();
                }
            }

        }




        public string Password
        {
            get
            {
                lock (aso[2])
                {
                    return m_pass;
                }
            }

            set
            {
                lock (Document)
                {
                    m_pass = value;
                    Document.RootElement.SelectSingleElement("bot").SelectSingleElement("password").SetAttribute("value", value);
                    Save();
                }
            }

        }



        public string Delimiter
        {
            get
            {
                lock (aso[3])
                {
                    return m_delimiter;
                }
            }

            set
            {
                lock (Document)
                {
                    m_delimiter = value;
                    Document.RootElement.SelectSingleElement("bot").SelectSingleElement("delimiter").SetAttribute("value", value);
                    Save();
                }
            }

        }


        public string Status
        {
            get
            {
                lock (aso[4])
                {
                    return m_status;
                }
            }

            set
            {
                lock (Document)
                {
                    m_status = value;
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
                lock (aso[5])
                {
                    return m_msglimit;
                }
            }

            set
            {
                lock (Document)
                {
                    m_msglimit = value;
                    Document.RootElement.SelectSingleElement("bot").SelectSingleElement("msglimit").SetAttribute("value", value);
                    Save();
                }
            }

        }


        public bool UseSSL
        {
            get
            {
                lock (aso[6])
                {
                    return ssl;
                }
            }

            set
            {
                lock (Document)
                {
                    ssl = value;
                    Document.RootElement.SelectSingleElement("bot").SelectSingleElement("ssl").SetAttribute("value", value.ToString());
                    Save();
                }
            }

        }

        public bool UseCompression
        {
            get
            {
                lock (aso[7])
                {
                    return compression;
                }
            }

            set
            {
                lock (Document)
                {
                    compression = value;
                    Document.RootElement.SelectSingleElement("bot").SelectSingleElement("compression").SetAttribute("value", value.ToString());
                    Save();
                }
            }

        }


        public bool UseStartTls
        {
            get
            {
                lock (aso[8])
                {
                    return starttls;
                }
            }

            set
            {
                lock (Document)
                {
                    starttls = value;
                    Document.RootElement.SelectSingleElement("bot").SelectSingleElement("starttls").SetAttribute("value", value.ToString());
                    Save();
                }
            }

        }
/*

        public int AdminAccess
        {
            get
            {
                lock (aso[9])
                {
                    return admin_access;
                }
            }

            set
            {
                lock (Document)
                {
                    admin_access = value;
                    Document.RootElement.SelectSingleElement("bot").SelectSingleElement("admin_access").SetAttribute("value", value);
                    Save();
                }
            }

        }

        */

        public int ReconnectTime
        {
            get
            {
                lock (aso[10])
                {
                    return m_reconnect_time;
                }
            }

            set
            {
                lock (Document)
                {
                    m_reconnect_time = value;
                    Document.RootElement.SelectSingleElement("bot").SelectSingleElement("reconnect_time").SetAttribute("value", value);
                    Save();
                }
            }

        }

        public int MaxReconnects
        {
            get
            {
                lock (aso[11])
                {
                    return max_reconnects;
                }
            }

            set
            {
                lock (Document)
                {
                    max_reconnects = value;
                    Document.RootElement.SelectSingleElement("bot").SelectSingleElement("max_reconnects").SetAttribute("value", value);
                    Save();
                }
            }

        }

        public int Port
        {
            get
            {
                lock (aso[12])
                {
                    return m_port;
                }
            }

            set
            {
                lock (Document)
                {
                    m_port = value;
                    Document.RootElement.SelectSingleElement("bot").SelectSingleElement("port").SetAttribute("value", value);
                    Save();
                }
            }

        }


        public string Language
        {
            get
            {
                lock (aso[13])
                {
                    return m_lang;
                }
            }

            set
            {
                lock (Document)
                {
                    m_lang = value;
                    Document.RootElement.SelectSingleElement("bot").SelectSingleElement("lang").SetAttribute("value", value);
                    Save();
                }
            }

        }

    }
}
