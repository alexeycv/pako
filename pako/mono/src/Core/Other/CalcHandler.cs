/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Pako Jabber-bot.                                                              *
 * Copyright. All rights reserved © 2007-2008 by Klichuk Bogdan (Bbodio's Lab)   *
 * Copyright. All rights reserved © 2009-2012 by Alexey Bryohov                  *
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
using agsXMPP.protocol.iq.roster;
using agsXMPP.Xml.Dom;
using agsXMPP.protocol.x.muc;
using Core.Plugins;
using Core.Conference;
using Core.Other;

namespace Core.Other
{
    public class CalcHandler
    {
        Hashtable m_calcs;
        object[] sobjs = new object[10];

        public CalcHandler()
        {
            for (int i = 0; i < 10; i++)
            {
                sobjs[i] = new object();
            }
            m_calcs = new Hashtable();
        }

        public Hashtable Calculates
        {
            get { lock (sobjs[0]) { return m_calcs; } }
            set { lock (sobjs[0]) { m_calcs = value; } }
        }


        public bool Exists(Jid Jid)
        {
            lock (sobjs[0])
            {
                return Calculates[Jid.ToString()] != null ? true : false;
            }
        }

        public bool AddHandle(Jid Jid)
        {
            lock (sobjs[0])
            {
                if (Exists(Jid))
                    return false;

                Calculates.Add(Jid.ToString(), new Calculate(Jid));
                return true;
            }
        }

        public void DelHandle(Jid Jid)
        {
            lock (sobjs[0])
            {
                if (Calculates.ContainsKey(Jid.ToString()))
                    Calculates.Remove(Jid.ToString());
            }

        }


        public Calculate GetHandle(Jid Jid)
        {
            return Exists(Jid) ? (Calculate)Calculates[Jid.ToString()] : new Calculate(Jid);
        }


    }
}
