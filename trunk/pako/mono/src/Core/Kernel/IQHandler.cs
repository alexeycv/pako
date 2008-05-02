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
using agsXMPP.protocol.iq;
using agsXMPP.protocol.iq.version;
using agsXMPP.protocol.iq.time;

namespace Core.Kernel
{
    public class IQHandler
    {

        public void Handle(IQ iq, XmppClientConnection Con, string os_version , string version)
        {

            
            if (iq.Query != null)
            {
                if (iq.Type == IqType.get)
                {
                    if (iq.Query.GetType() == typeof(agsXMPP.protocol.iq.version.Version))
                    {
                        VersionIq _iq = new VersionIq();
                        _iq.To = iq.From;
                        _iq.Id = iq.Id;
                        _iq.Type = IqType.result;
                        _iq.Query.Name = "Pako bot";
                        _iq.Query.Ver = version + " stable";
                        _iq.Query.Os = "(C# 2.0 .NET/Mono) on base: " + os_version;
                        Con.Send(_iq);
                    }
                    else
                        if (iq.Query.GetType() == typeof(agsXMPP.protocol.iq.time.Time))
                        {
                            TimeIq _iq = new TimeIq();
                            _iq.To = iq.From;
                            _iq.Id = iq.Id;
                            _iq.Type = IqType.result;
                            _iq.Query.Tz = DateTime.Now.ToString();
                            Con.Send(_iq);
                        }
                }
            }

        }

    }
}
