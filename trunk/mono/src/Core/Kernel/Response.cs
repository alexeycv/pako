/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Pako Jabber-bot. Bbodio's Lab.                                                *
 * Copyright. All rights reserved � 2007-2008 by Klichuk Bogdan (Bbodio's Lab)   *
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
using agsXMPP;
using Core.Conference;
using Core.Xml;
using agsXMPP.Xml.Dom;
using agsXMPP.protocol.client;
using Core.Other;

namespace Core.Kernel
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
        Jid emulation;
        

        /// <summary>
        /// Creates The Response instance
        /// </summary>
        /// <param name="Con"></param>
        /// <param name="Folder"></param>
        /// <param name="Lang"></param>
        /// <param name="Msglimit"></param>
        public Response(XmppClientConnection Con, string Folder, string Lang, int Msglimit)
        {
            Open(Folder + Utils.d + "lang.pack", 37);
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

        public Jid Emulation
        {
            get { lock (aso[30]) { return emulation; } }
            set { lock (aso[30]) { emulation = value; } }
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


        public string f(string Pattern, params string[] args)
        {
            try
            {
                if (Document.RootElement.SelectSingleElement("patterns").HasAttribute(Pattern))
                {
                    string source = Document.RootElement.SelectSingleElement("patterns").GetAttribute(Pattern);
                    int num = 1;
                    foreach (string s in args)
                    {
                        source = source.Replace("{" + num.ToString() + "}", "\"" + args[num - 1] + "\"");
                        num++;
                    }
                    return source;
                }
                else
                    return "Can not format pattern <" + Pattern + "> in [" + Language + Utils.d + "lang.pack]";
            }
            catch
            {
                return "Packet [" + Language + Utils.d + "lang.pack] is broken, please pass this message to bot administrator";
            }
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
                return this.f("help_not_found", HelpCmd);
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
            return this.f("agreed");
        }

        public string Deny()
        {
            return this.f("denied");
        }


        public string _se
        {
            get
            {
                return this.f("syntax_error");
            }
        }

        public void se(string command)
        {     
            Reply(this.f("syntax_error", this.Delimiter+"help "+command));    
        }

        public void Reply(string Body)
        {
            if ((this.MUC != null) && (this.MUser != null))
                Body = Utils.FormatEnvironmentVariables(Body, this.MUC, this.MUser);
            if (Emulation != null)
            {
                m_msg.Type = MessageType.chat;
                m_msg.From = Emulation;
            }
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
                    m_notify.Body = m_jid.Resource + ": " + f("private_notify");
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
                r_msg.Body = Emulation != null ? "Re: "+ Body : Body;
                r_msg.Type = MessageType.chat;
            }
            Connection.Send(r_msg);
         

        }


    }
}
