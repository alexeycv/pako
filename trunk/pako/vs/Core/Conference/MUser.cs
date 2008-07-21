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

using System.Collections.Generic;
using System.Collections;
using System.Text;
using System;
using agsXMPP;
using agsXMPP.Xml.Dom;
using System.IO;
using agsXMPP.protocol.client;
using agsXMPP.protocol.extensions.nickname;
using agsXMPP.protocol.x.muc;
using Core.Other;

namespace Core.Conference
{
    /// <summary>
    /// Container for all the chat room's user's data.
    /// </summary>
    public class MUser
    {
       
        Jid m_jid;
        string m_nick;
        string m_status;
        Role m_role;
        long m_enter;
        Affiliation m_affiliation;
        ShowType m_show;
        int? m_access;
        string m_lang;
        object[] sobjs = new object[12];
        long msg_time;


        /// <summary>
        /// Generates the user's bot-access 
        /// </summary>
        /// <returns></returns>
        public int? GetAccess()
        {
            int? res = 0;
            switch (m_role)
            {
                case Role.participant:
                    res += 25;
                    break;
                case Role.moderator:
                    res += 35;
                    break;
                case Role.visitor:
                    res += 5;
                    break;
                case Role.none:
                    res = 0;
                    break;
            }

            switch (m_affiliation)
            {
                case Affiliation.member:
                    res += 25;
                    break;
                case Affiliation.admin:
                    res += 45;
                    break;
                case Affiliation.none:
                    res += 0;
                    break;
                case Affiliation.owner:
                    res = 100;
                    break;
            }

            return res;

        }
       
      /// <summary>
      ///  Creates the MUser container for all the chat room's user's data.
      /// </summary>
      /// <param name="UserNick"></param>
      /// <param name="UserJid"></param>
      /// <param name="UserRole"></param>
      /// <param name="UserAffiliation"></param>
      /// <param name="UserStatus"></param>
      /// <param name="UserShow"></param>
      /// <param name="IsBotAdmin"></param>
      /// <param name="Lang"></param>
      /// <param name="EnterTime"></param>
      /// <param name="args"></param>
  
        public MUser(string UserNick, Jid UserJid, Role UserRole, Affiliation UserAffiliation, string UserStatus, ShowType UserShow, string Lang, long EnterTime, int? access, long Idle)
        {

            for (int i = 0; i < 12; i++ )
            {
                sobjs[i] = new object();
            }

            m_jid = UserJid;
            m_nick = UserNick;
            m_role = UserRole;
            m_affiliation = UserAffiliation;
            m_show = UserShow;
            m_status = UserStatus;
            m_access = access;
            m_lang = Lang;
            m_enter = EnterTime;
            msg_time = Idle;

        }
        /// <summary>
        /// Nick of the user
        /// </summary>
        public string Nick
        {
            get { lock (sobjs[0]) { return m_nick; } }
            set { lock (sobjs[0]) { m_nick = value; } }
        }
        /// <summary>
        /// Current status of the user
        /// </summary>
        public string Status
        {
            get { lock (sobjs[1]) { return m_status; } }
            set { lock (sobjs[1]) { m_status = value; } }
        }
        /// <summary>
        /// The time, when user entered the chat room.
        /// </summary>
        public long EnterTime
        {
            get { lock (sobjs[10]) { return m_enter; } }
            set { lock (sobjs[10]) { m_enter = value; } }
        }
        /// <summary>
        /// The real jid of the user
        /// </summary>
        public Jid Jid
        {
            get { lock (sobjs[2]) { return m_jid; } }
            set { lock (sobjs[2]) { m_jid = value; } }
        }

        /// <summary>
        /// The language of the user
        /// </summary>
        public string Language
        {
            get { lock (sobjs[3]) { return m_lang; } }
            set { lock (sobjs[3]) { m_lang = value; } }
        }

        /// <summary>
        /// The chat room's specified affiliation of the user
        /// </summary>
        public Affiliation Affiliation
        {
            get { lock (sobjs[4]) { return m_affiliation; } }
            set { lock (sobjs[4]) { m_affiliation = value; } }
        }

        /// <summary>
        /// The chat room's specified role of the user
        /// </summary>
        public Role Role
        {
            get { lock (sobjs[5]) { return m_role; } }
            set { lock (sobjs[5]) { m_role = value; } }
        }


        /// <summary>
        /// Returns if the user is bot's admin
        /// </summary>


        /// <summary>
        /// Returns the Bot-access of the user
        /// </summary>
        public int? Access
        {
            get { lock (sobjs[7]) { return m_access; } }
            set { lock (sobjs[7]) { m_access = value == null ? 0 : value > 100 ? value < 1 ? 0 : value : value; } }
        }

        /// <summary>
        /// The show type of the user.
        /// Can be NONE, xa, away, dnd, chat.
        /// </summary>
        public ShowType Show
        {
            get { lock (sobjs[8]) { return m_show; } }
            set { lock (sobjs[8]) { m_show = value; } }
        }


        /// <summary>
        /// Returns a count in ticks of time, for which user has been idling (no messages)
        /// </summary>
        public long Idle
        {
            get { lock (sobjs[9]) { return msg_time; } }
            set { lock (sobjs[9]) { msg_time = value; } }
        }
     

    }

}
