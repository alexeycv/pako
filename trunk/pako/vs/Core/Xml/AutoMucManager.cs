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
using System.Collections;
using System.Text;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;
using agsXMPP.protocol.x.muc;
using Core.Plugins;
using Core.Conference;
using Core.Other;

namespace Core.Xml
{
    public class AutoMucManager : XMLContainer
    {
        int m_count;

        public AutoMucManager(string AMFile)
        {
            
            m_count = 0;
            Open(AMFile,10);

            foreach (Element el in Document.RootElement.SelectSingleElement("rooms").SelectElements("room"))
            {
                m_count++;
            }

        }

        public bool SetLanguage(Jid Room, string lang)
        {
            lock (Document)
            {
                foreach (Element el in Document.RootElement.SelectSingleElement("rooms").SelectElements("room"))
                {
                    if (el.GetAttribute("jid") == Room.Bare)
                    {
                        el.SetAttribute("lang", lang);
                        Save();
                        return true;
                    }
                }
                return false;
            }
        }

        public bool SetStatus(Jid Room, string status)
        {
            lock (Document)
            {
                foreach (Element el in Document.RootElement.SelectSingleElement("rooms").SelectElements("room"))
                {
                    if (el.GetAttribute("jid") == Room.Bare)
                    {
                        el.SetAttribute("status", status);
                        Save();
                        return true;
                    }
                }
                return false;
            }
        }

        public bool AddMuc(Jid Room, string nick, string status, string language, string password)
        {
            lock (Document)
            {
                foreach (AutoMuc am in GetAMList())
                {
                    if (am.Jid.ToString() == Room.ToString())
                        return false;
                }

                Document.RootElement.SelectSingleElement("rooms").AddTag("room");
                foreach (Element el in Document.RootElement.SelectSingleElement("rooms").SelectElements("room"))
                {
                    if (!el.HasAttribute("jid"))
                    {
                        el.SetAttribute("status", status);
                        el.SetAttribute("jid",    Room.ToString());
                        el.SetAttribute("nick",   nick);
                        el.SetAttribute("lang",   language);
                        if (password != null)
                            el.SetAttribute("password", password);
                        Count++;
                        Save();
                        return true;
                    }
                }
                return false;
            }
        }
 
        public bool DelMuc(Jid Room)
        {
            lock (Document)
            {
               
                foreach (Element el in Document.RootElement.SelectSingleElement("rooms").SelectElements("room"))
                {
                    if (el.GetAttribute("jid") == Room.ToString())
                    {
                        string m_source = Document.ToString();
                        m_source = m_source.Replace(el.ToString(), "");
                        Document.Clear();
                        Document.LoadXml(m_source);
                        Count--;
                        Save();
                        return true;
                    }
                }
                return true;
            }
        }

        public bool SetNick(Jid Room, string nick)
        {
            lock (Document)
            {
                foreach (Element el in Document.RootElement.SelectSingleElement("rooms").SelectElements("room"))
                {
                    if (el.GetAttribute("jid") == Room.Bare)
                    {
                        el.SetAttribute("nick", nick);
                        Save();
                        return true;
                    }
                }
                return false;
            }
        }

        public int Count
        {
            get { lock (aso[3]) { return m_count; } }
            set { lock (aso[3]) { m_count = value; } }
        }

        public bool Exists(Jid Room)
        {
            lock (Document)
            {
                foreach (AutoMuc am in GetAMList())
                {
                    if (am.Jid.ToString() == Room.ToString())
                        return true;
                }
                return false;
       
            }
        }

        public List<AutoMuc> GetAMList()
        {
            lock (Document)
            {
                List<AutoMuc> am = new List<AutoMuc>();
                foreach (Element el in Document.RootElement.SelectSingleElement("rooms").SelectElements("room"))
                {

                    am.Add(new AutoMuc(new Jid(el.GetAttribute("jid")),
                        el.GetAttribute("nick"),
                        el.GetAttribute("status"),
                        el.GetAttribute("lang"),
                        el.HasAttribute("password") && el.GetAttribute("password").Trim() != "" ? 
                        el.GetAttribute("password") : null));

                }
                return am;
            }
        }
    }
}
