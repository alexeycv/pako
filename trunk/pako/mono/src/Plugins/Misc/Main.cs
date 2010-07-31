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
using Core.Plugins;
using Core.Kernel;
using Core.Conference;
using agsXMPP;
using agsXMPP.protocol.client;

namespace Plugin
{
 
    public class Main : IPlugin
    {
        SessionHandler _session = null;

        public string File
        {
            get
            {
                return "Misc.dll";
            }
        }

        public string Name
        {
            get
            {
                return "Misc";
            }
        }

        public string Comment
        {
            get
            {
                return "Miscelaneous commands for bot and users" ;
            }
        }

        public SessionHandler Session
        {
            get
            {
                return _session;
            }
            set
            {
                _session = value;
            }
        } 

        public bool SubscribePresence 
        { 
            get
            {
                return true;
            }
        }

        public bool SubscribeMessages 
        { 
            get
            {
                return false;
            }
        }
        
        public bool SubscribeIq 
        { 
            get
            {
                return false;
            }
        }

        public void PerformAction(IPluginData d)
        {

            MiscHandler ph = new MiscHandler(d.r, Name);

        }

        // IPlugin implementation

        // Plugin initialization and shut down
        public void Start(SessionHandler sh)
        {
        }

        public void Stop()
        {
        }

        // Handlers
        public void CommandHandler(agsXMPP.protocol.client.Message msg, SessionHandler s, Message emulation, CmdhState signed, int level)
        {
        }

        public void PresenceHandler(Presence m_pres, SessionHandler sh)
        {
            Presence pres = m_pres;
            Response m_r = null;
            Jid m_jid = null;
            MUC m_muc = null;
           
            if (sh.S.MUCJustJoined.Contains(pres.From.Bare))
            {
                m_r = (Response)sh.S.MUCJustJoined[pres.From.Bare];
                m_jid = pres.From;

                if (sh.S.MUCJustJoined_Mucs.Contains(pres.From.Bare)) 
                    m_muc = (MUC)sh.S.MUCJustJoined_Mucs[pres.From.Bare];
            }

            if (m_r != null && m_muc != null)
            {
                if (pres.Type != PresenceType.error)
                {
                    if (pres.MucUser != null)
                    {
                         m_r.Sh.S.AutoMucManager.AddMuc(m_muc.Jid, m_muc.MyNick, m_muc.MyStatus, m_muc.Language, m_muc.Password);
                         Jid querer = m_r.Msg.From;
                         m_r.Reply(m_r.f("muc_join_success", m_jid.Bare, m_muc.MyNick));
                         foreach (Jid j in m_r.Sh.S.Config.Administartion())
                         {
                             m_r.Msg.From = j;
                             m_r.Msg.Type = MessageType.chat;
                             m_r.MUC = null;
                             m_r.Reply("Re NEW(" + querer.ToString() + ": misc join):\n" + m_r.f("muc_join_success", m_jid.Bare, m_muc.MyNick));
                         }
                    }

                }
                else
                {
                   m_r.Reply(m_r.f("muc_join_failed", m_jid.Bare, pres.Error.GetAttribute("code") + " - " +pres.Error.Condition.ToString()));
                   Jid querer = m_r.Msg.From;
                    foreach (Jid j in m_r.Sh.S.Config.Administartion())
                   {
                       m_r.Msg.From = j;
                       m_r.Msg.Type = MessageType.chat;
                       m_r.MUC = null;
                       m_r.Reply("Re("+querer.ToString()+": misc join):\n"+m_r.f("muc_join_failed", m_jid.Bare, pres.Error.GetAttribute("code") + " - " + pres.Error.Condition.ToString()));
                   }
                }
                //m_r.Connection.OnPresence -= new agsXMPP.protocol.client.PresenceHandler(this.Connection_OnPresence);

                //Clearing
                sh.S.MUCJustJoined.Remove(pres.From.Bare);
                sh.S.MUCJustJoined_Mucs.Remove(pres.From.Bare);
            }
        }

        public void IqHandler(IQ iq, XmppClientConnection Con)
        {
        }
    }



}
