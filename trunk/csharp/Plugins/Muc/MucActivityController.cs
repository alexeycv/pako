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
using agsXMPP;
using agsXMPP.protocol.client;
using Core.Client;
using Core.Conference;
using Core.Manager;

namespace Plugin
{
    public class MucActivityController
    {
       Jid m_jid;
       Response m_r;
       MUC m_muc;

       public MucActivityController(Response r, MUC muc)
       {
           m_jid = muc.Jid;
           m_r = r;
           m_muc = muc;
           m_r.Connection.OnPresence += new Core.Client.PresenceHandler(Connection_OnPresence);
           muc.Join();
       }

       void Connection_OnPresence(object sender, Presence pres)
       {
           if (pres.From.Bare == m_jid.Bare)
           {
               if (pres.Type != PresenceType.error)
               {
                   if (pres.MucUser != null)
                   {
                       m_r.Sh.S.AutoMucManager.AddMuc(m_muc.Jid, m_muc.MyNick, m_muc.MyStatus, m_muc.Language);
                       m_r.Reply(m_r.FormatPattern("muc_join_success", m_jid.Bare, m_muc.MyNick));
                   }
               }
               else
               {
                  m_r.Reply(m_r.FormatPattern("muc_join_failed", m_jid.Bare, pres.Error.GetAttribute("code") + " - " +pres.Error.Condition.ToString()));
               }
               m_r.Connection.OnPresence -= new Core.Client.PresenceHandler(this.Connection_OnPresence);
           }
       }






    }
}
