using System;
using agsXMPP;
using Core.Conference;
using Core.Client;
using agsXMPP.Xml.Dom;
using agsXMPP.protocol.client;

namespace Core.Client
{


  
    /// <summary>
    /// The class, which provides a specifid response with exact language, using [*\lang.pack], [*\help.pack] files 
    /// </summary>
    public class Response : XMLContainer
    {

        string m_lang;
        int m_access;
        string alias;
        MUser m_user;
        MUC m_muc;
        Message m_msg;
        string del;
        bool has_alias;      
        XmppClientConnection m_con;
        int m_msglimit;
        XMLContainer m_help;
        SessionHandler m_sh;
        

        /// <summary>
        /// Creates The Response instance
        /// </summary>
        /// <param name="Con"></param>
        /// <param name="Folder"></param>
        /// <param name="Lang"></param>
        /// <param name="Msglimit"></param>
        public Response(XmppClientConnection Con, string Folder, string Lang, int Msglimit)
        {
            Open(Folder + Utils.d + "lang.pack", 36);
            m_con = Con;
            m_msglimit = Msglimit;
            m_lang = Lang;
            m_access = -1;
            m_help = new XMLContainer();
            m_help.Open(Folder + Utils.d + "help.pack", 10);         
        }


        public Response(Response basic)
        {
            Open(35);
            m_con = basic.Connection;
            Document = basic.Document;
            File = basic.File;
            m_access = basic.m_access;
            m_msglimit = basic.m_msglimit;
            m_lang = basic.m_lang;
            m_msg = basic.m_msg;
            m_help = basic.m_help;
            m_sh = basic.m_sh;

        }

        /// <summary>
        /// The language of responding
        /// </summary>
        public string Language
        {
            get
            {
                lock (aso[2]) { return m_lang; }
            }
            set
            {
                lock (aso[2]) { m_lang = value; }
            }
        }
        /// <summary>
        /// The current Connection, which is used by class
        /// </summary>
         public XmppClientConnection Connection
        {
            get
            {
                lock (aso[0]) { return m_con; }
            }
            set
            {
                lock (aso[0]) { m_con = value; }
            }
        }


  



        public int Access
        {
            get { lock (aso[4]) { return m_access; } }
            set { lock (aso[4]) { m_access = value; } }
        }

        public int MSGLimit
        {
            get { lock (aso[8]) { return m_msglimit; } }
            set { lock (aso[8]) { m_msglimit = value; } }
        }

        public Document HelpDoc
        {
            get { lock (aso[27]) { return m_help.Document; } }
            set { lock (aso[27]) { m_help.Document = value; } }
        }

        public SessionHandler Sh
        {
            get { lock (aso[35]) { return m_sh; } }
            set { lock (aso[35]) { m_sh = value; } }
        }
        public Message Msg
        {
            get { lock (aso[26]) { return m_msg; } }
            set { lock (aso[26]) { m_msg = value; } }
        }

        public bool HasAlias
        {
            get { lock (aso[11]) { return has_alias; } }
            set { lock (aso[11]) { has_alias = value; } }
        }

        public string Alias
        {
            get { lock (aso[12]) { return alias; } }
            set { lock (aso[12]) { alias = value; } }
        }

        public string Delimiter
        {
            get { lock (aso[25]) { return del; } }
            set { lock (aso[25]) { del = value; } }
        }

        public MUC  MUC
        {
            get { lock (aso[15]) { return m_muc; } }
            set { lock (aso[15]) { m_muc = value; } }
        }

        public MUser MUser
        {
            get { lock (aso[16]) { return m_user; } }
            set { lock (aso[16]) { m_user = value; } }
        }

        public void SetPattern(string name, string value)
        {
            lock (aso[21]) 
             {
                 Document.RootElement.SelectSingleElement("patterns").SetAttribute(name, value);
                 Save();
             }
        }

        public bool PatternExists(string Pattern)
        {
            return Document.RootElement.SelectSingleElement("patterns").HasAttribute(Pattern);
        }


        public string FormatPattern(string Pattern, params object[] args)
        {
            if (Document.RootElement.SelectSingleElement("patterns").HasAttribute(Pattern))
            {
                string source = Document.RootElement.SelectSingleElement("patterns").GetAttribute(Pattern);
                int num = 1;
                foreach (string s in args)
                {
                    source = source.Replace("{" + num.ToString() + "}", "\""+args[num - 1].ToString()+"\"");
                    num++;
                }
                return source;
            }
            else
                return "Can not format pattern <" + Pattern + "> in [" + Language + Utils.d+"lang.pack]";
        }

        public string Limit(string Source)
        {
            if (Source.Length > m_msglimit)
                return Source.Substring(0, m_msglimit);
            else
                return Source;

        }


        public bool OverFlow(string Source)
        {
            return Source.Length > m_msglimit;
        }

        public string GetHelp(string HelpCmd)
        {
            lock (aso[7])
            {
                foreach (Element el in HelpDoc.RootElement.SelectElements("command"))
                {
                    if (el.GetAttribute("name") == HelpCmd)
                    {

                        return el.GetAttribute("value").Replace("{1}", Delimiter);
                    }
                    
                }
                return this.FormatPattern("help_not_found", HelpCmd);
            }
        }

        public bool HelpExists(string HelpCmd)
        {
            lock (aso[22])
            {
                foreach (Element el in HelpDoc.RootElement.SelectElements("command"))
                {
                    if (el.GetAttribute("name") == HelpCmd)
                    {
                        return true;
                    }

                }
                return false;
            }
        }


        public void SetHelp(string HelpCmd, string value)
        {
            lock (aso[23])
            {

                foreach (Element el in HelpDoc.RootElement.SelectElements("command"))
                    {
                        if (el.GetAttribute("name") == HelpCmd)
                        {
                            el.SetAttribute("value", value);
                            m_help.Save();
                            return;
                        }

                    }

                    HelpDoc.RootElement.AddTag("command");

                    foreach (Element el in HelpDoc.RootElement.SelectElements("command"))
                    {
                        if (!el.HasAttribute("name"))
                        {
                            el.SetAttribute("name", HelpCmd);
                            el.SetAttribute("value", value);
                            m_help.Save();
                            return;
                        }
                    }
            }

        }

        public string Agree()
        {
            return this.FormatPattern("agreed");
        }

        public string Deny()
        {
            return this.FormatPattern("denied");
        }


        public string _se
        {
            get
            {
                return this.FormatPattern("syntax_error");
            }
        }

        public void se(string command)
        {     
            Reply(this.FormatPattern("syntax_error", this.Delimiter+"help "+command));    
        }

        public void Reply(string Body)
        {

            MessageType m_type = m_msg.Type;
            Jid m_jid = m_msg.From;
            Message r_msg = new Message();
            r_msg.Type = m_type;
          
            if (m_type == MessageType.groupchat)
            {
                if (OverFlow(Body))
                {
                    Message m_notify = new Message();
                    m_notify.Type = MessageType.groupchat;
                    m_notify.Body = m_jid.Resource + ": " + FormatPattern("private_notify");
                    m_notify.To = new Jid(m_jid.Bare);
                    Connection.Send(m_notify);
                    r_msg.To = m_jid;
                    Body = Body.Length > 30000 ? Body.Substring(0, 30000) + "[...]" : Body;
                    r_msg.Body = Body;
                    r_msg.Type = MessageType.chat;
                }
                else
                {

                    r_msg.To = new Jid(m_jid.Bare);
                    Body = Body.Length > 30000 ? Body.Substring(0, 30000) + "[...]" : Body;
                    r_msg.Body = m_jid.Resource + ": " + Body;
                    r_msg.Type = m_type;
                }
            }
            else
            {
                r_msg.To = m_jid;
                Body = Body.Length > 30000 ? Body.Substring(0, 30000) + "[...]" : Body;
                r_msg.Body = Body;
                r_msg.Type = MessageType.chat;
            }

            Connection.Send(r_msg);
         

        }


    }
}
