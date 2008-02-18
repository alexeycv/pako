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
using agsXMPP.protocol.x.muc;
using Core.Plugins;
using Core.Conference;
using Core.Special;

namespace Core.Client
{
    public class Vipuser
    {

        string m_lang;
        object m_access;
        Jid m_jid;
        object[] sobjs = new object[4];
        public Vipuser(Jid Jid,string Lang, object Access)
        {
            for (int i = 0; i < 4; i++)
            {
                sobjs[i] = new object();
            }
            m_lang = Lang;
            m_jid = Jid;
            m_access = Access;
        }

        public string Language
        {
            get { lock (sobjs[0]) { return m_lang; } }
            set { lock (sobjs[0]) { m_lang = value; } }
        }

        public Jid Jid
        {
            get { lock (sobjs[3]) { return m_jid; } }
            set { lock (sobjs[3]) { m_jid = value; } }
        }


        public object Access
        {
            get { lock (sobjs[1]) { return m_access; } }
            set { lock (sobjs[1]) { m_access = (int)value; } }
        }

    }
}
