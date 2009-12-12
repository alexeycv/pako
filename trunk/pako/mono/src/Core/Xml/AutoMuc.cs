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
using agsXMPP.Xml.Dom;

namespace Core.Xml
{
   public  class AutoMuc
    {
       Jid m_jid;
       string m_nick;
       string m_status;
       string m_lang;
       string m_pass;
       object[] sobjs = new object[10];

       /// <summary>
       /// Create an instance of AutoMuc - a simple container, dedicated to hold information, which
       /// is used to join the multi-user-chat
       /// </summary>
       /// <param name="Jid"></param>
       /// <param name="Nick"></param>
       /// <param name="Status"></param>
       /// <param name="Language"></param>
       public AutoMuc(Jid Jid, string Nick, string Status, string Language, string Password)
       {
           for (int i = 0; i < 10; i++)
           {
               sobjs[i] = new object();
           }

           m_jid = Jid;
           m_status = Status;
           m_nick = Nick;
           m_lang = Language;
           m_pass = Password;

       }
       static public implicit operator Jid(AutoMuc am)
       {
           return am.Jid;
       }


       public Jid Jid
       {
           get { lock (sobjs[0]) { return m_jid; } }
           set { lock (sobjs[0]) { m_jid = value; } }
       }

       public string Status
       {
           get { lock (sobjs[1]) { return m_status; } }
           set { lock (sobjs[1]) { m_status = value; } }
       }

       public string Password
       {
           get { lock (sobjs[4]) { return m_pass; } }
           set { lock (sobjs[4]) { m_pass = value; } }
       }

       public string  Nick
       {
           get { lock (sobjs[2]) { return m_nick; } }
           set { lock (sobjs[2]) { m_nick = value; } }
       }

       public string Language
       {
           get { lock (sobjs[3]) { return m_lang; } }
           set { lock (sobjs[3]) { m_lang = value; } }
       }
  


    }
}
