/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Pako Jabber-bot.                                                              *
 * Copyright. All rights reserved � 2007-2008 by Klichuk Bogdan (Bbodio's Lab)   *
 * Copyright. All rights reserved � 2009-2012 by Alexey Bryohov                  *
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
using agsXMPP.protocol.iq;
using agsXMPP.protocol.iq.version;
using agsXMPP.protocol.iq.time;
using agsXMPP.protocol.extensions.ping;
using System.IO;
using System.Net;
using Core.Kernel;
using Core.Conference;
using Core.Xml;
using Core.Other;

namespace Core.Plugins
{
    public interface IPlugin
    {
        string File { get;}
        string Name { get;}
        string Comment { get;}

        SessionHandler Session {get; set;}

        bool SubscribePresence { get;}
        bool SubscribeMessages { get;}
        bool SubscribeIq { get;}

        // Command handler
        void PerformAction(IPluginData d);

        //; Plugin initialization and shut down
        void Start(SessionHandler sh);
        void Stop();

        // Handlers
        void CommandHandler(agsXMPP.protocol.client.Message msg, SessionHandler s, Message emulation, CmdhState signed, int level);
        void PresenceHandler(Presence m_pres, SessionHandler sh);
        void IqHandler(IQ iq, XmppClientConnection Con);
    }
}
       
