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
using agsXMPP.protocol.iq.version;
using agsXMPP.protocol.client;
using Core.Kernel;
using Core.Conference;

namespace Core.Kernel
{
    public class VersionCB
    {
        Jid m_jid;
        MUser _mUser;



        public VersionCB(MUser user, XmppClientConnection conn)
        {
            _mUser = user;
            m_jid = _mUser.Jid;
            VersionIq vi = new VersionIq();
            vi.Type = IqType.get;
            vi.To = m_jid;
            vi.GenerateId();
            conn.IqGrabber.SendIq(vi, new IqCB(VersionExtractor), null); 
            
        }




        private void VersionExtractor(object obj, IQ iq, object arg)
        {
           

          
                @out.exe(" before translate  =>  ");
                agsXMPP.protocol.iq.version.Version vi = iq.Query as agsXMPP.protocol.iq.version.Version;
                @out.exe(" after translate  =>  ");

                string answer;
                string jid = m_jid.ToString();

		//Return a version to mucuser object
		_mUser.Version = vi.Name + " " + vi.Ver + " " + vi.Os;

                // Implementing version censor
           
        }




    }
}
