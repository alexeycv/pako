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

        public string File
        {
            get
            {
                return "aiml.dll";
            }
        }


        public bool MucOnly
        {
            get
            {
                return false;
            }
        }


        public string Name
        {
            get
            {
                return "AIML";
            }
        }

        public string Comment
        {
            get
            {
                return "AIML Chatbot" ;
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
                return false;
            }
        }

        public bool SubscribeMessages 
        { 
            get
            {
                return true;
            }
        }
        
        public bool SubscribeIq 
        { 
            get
            {
                return false;
            }
        }

        #region CommandHandler variables

        Message c_m_msg;
        Jid c_s_jid;
        string c_s_nick;
        MUser c_m_user;
        string c_original;
        Jid c_m_jid;
        MUC c_m_muc;
        SessionHandler c_Sh;
//        Message emulate;
        CmdhState c_signed;
        int c_level;

        #endregion;

        /// <summary>
        /// Handle a command insede of the plug-in
        /// </summary>
        /// <param name="d"></param>
        public void PerformAction(IPluginData d)
        {

        	if (d.r.AccessType == AccessType.None)
            { 
        		if (d.r.Access >= 100)
        		{
        			ConfigHandler ph = new ConfigHandler(d.r);
        		}
                   else
                   d.r.Reply(d.r.f("access_not_enough","100"));
        	}else
        	{
        		ConfigHandler ph = new ConfigHandler(d.r);
        	}

        }

        // IPlugin implementation

        // Plugin initialization and shut down
        public void Start(SessionHandler sh)
        {
            @out.write("===> AIML initialization start.");

            @out.write("===> AIML initialization end.");
        }

        public void Stop()
        {
        }

        // Handlers
        public void CommandHandler(agsXMPP.protocol.client.Message msg, SessionHandler s, Message emulation, CmdhState signed, int level)
        {
            // === Initializing ===
            msg.From = new Jid(msg.From.Bare.ToLower() + (msg.From.Resource != "" ? "/" + msg.From.Resource : ""));
            c_m_msg = msg;
            c_s_jid = msg.From;
            c_Sh = s;
            if (msg.Body == null || msg.Body == "")
                return;
            if (c_s_jid.Bare == c_Sh.S.C.MyJID.Bare)
                return;
           // emulate = emulation;
            c_signed = signed;
            c_level = level;
            // === End Init ===

            Thread thr = new Thread(new ThreadStart(CommandHandleThread));
            thr.Start();
        }

        public void PresenceHandler(Presence m_pres, SessionHandler sh)
        {
        }

        public void IqHandler(IQ iq, XmppClientConnection Con)
        {
        }

        // CommandHandler Thread Method
        public void CommandHandleThread()
        {
            // Get MUC and mucuser
            c_m_muc = c_Sh.S.GetMUC(c_s_jid);
            c_m_user = null;
            if (c_m_muc != null)
            {
                if (c_s_jid.Resource == null)
                    return;
                c_m_user = c_m_muc.GetUser(c_s_jid.Resource);
            }

            // Parse message
            //m_msg.Body
        }
    }



}
