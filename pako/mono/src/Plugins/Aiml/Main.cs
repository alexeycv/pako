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
using Core.Other;
using agsXMPP;
using agsXMPP.protocol.client;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Reflection;

using AIMLbot;

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


        #region AIML Bot specific variables
        object[] sobjs = new object[5];
        AIMLbot.Bot _aimlbot;
        AIMLbot.User _aimlUser;

        // Propertiees
        public AIMLbot.Bot AIML_Bot
        {
            get {lock (sobjs[1]) {return _aimlbot;} }
            set {lock (sobjs[1]) {_aimlbot = value;} }
        }
        #endregion

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

            for (int i = 0; i < 5; i++)
           {
               sobjs[i] = new object();
           }

/*
            _aimlbot = new AIMLbot.Bot();
            _aimlUser = new AIMLbot.User("Default", _aimlbot);
            Assembly assem = Assembly.GetExecutingAssembly();
            String _basedir =  Path.GetDirectoryName(assem.Location).Replace("Plugins","");
//            _basedir = _basedir.Remove(_basedir.Length-3, 2);

            // Loading bot config
            @out.write("===> AIML CurrentConfig : " + _basedir + "AIML/config/Settings.xml");
            try
            {
                AIML_Bot.loadSettings(_basedir + "AIML/config/Settings.xml");
            }
            catch (Exception exx)
            { 
                @out.write("EX: " + exx.Message + "\nTraceback:\n"+ exx.StackTrace); 
                @out.write("===> AIML initialization FAILED.");
                _aimlbot = null;
                return;
            }

            // Loading AIML Dictionary. V1.0 supports english dict.
            @out.write("===> AIML Dictionary : " + _basedir + "AIML/en/");
            AIMLbot.Utils.AIMLLoader loader = new AIMLbot.Utils.AIMLLoader(_aimlbot);
            _aimlbot.isAcceptingUserInput = false;
            //loader.loadAIML(_basedir + "AIML/en/");
            loader.loadAIML(_aimlbot.PathToAIML);
            _aimlbot.isAcceptingUserInput = true;
*/

            @out.write("===> AIML initialization end.");
        }

        public void Stop()
        {
        }

        // Handlers
        public void CommandHandler (agsXMPP.protocol.client.Message msg, SessionHandler s, Message emulation, CmdhState signed, int level)
		{
			MessageHandler _handler = new MessageHandler(msg, s, emulation, signed, level);
			Thread thr = new Thread (new ThreadStart (_handler.HandleMessage));
			thr.Start ();
		}

        public void PresenceHandler(Presence m_pres, SessionHandler sh)
        {
        }

        public void IqHandler(IQ iq, XmppClientConnection Con)
        {
        }

    }



}
