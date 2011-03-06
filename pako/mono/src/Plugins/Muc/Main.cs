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
using System.Threading;

namespace Plugin
{

	public class Main : IPlugin
	{
		SessionHandler _session = null;

		public string File {
			get { return "Muc.dll"; }
		}


		public string Name {
			get { return "Muc"; }
		}

		public string Comment {
			get { return "Can serve muc users"; }
		}

		public SessionHandler Session {
			get { return _session; }
			set { _session = value; }
		}

		public bool SubscribePresence {
			get { return false; }
		}

		public bool SubscribeMessages {
			get { return true; }
		}

		public bool SubscribeIq {
			get { return false; }
		}

		public void PerformAction (IPluginData d)
		{
			
			MucHandler ph = new MucHandler (d.r, Name);
			
		}

		// IPlugin implementation

		// Plugin initialization and shut down
		public void Start (SessionHandler sh)
		{
			_session = sh;
		}

		public void Stop ()
		{
		}

		// Handlers
		public void CommandHandler (agsXMPP.protocol.client.Message msg, SessionHandler s, Message emulation, CmdhState signed, int level)
		{
			MessageHandler _handler = new MessageHandler(msg, s, emulation, signed, level);
			Thread thr = new Thread (new ThreadStart (_handler.Handle));
			thr.Start ();
		}

		public void PresenceHandler (Presence m_pres, SessionHandler sh)
		{
		}

		public void IqHandler (IQ iq, XmppClientConnection Con)
		{
		}
	}
	
	
	
}
