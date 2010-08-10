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
                return false;
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
            if (c_m_muc != null && c_m_msg.Body.IndexOf(c_m_muc.MyNick) >=0  && AIML_Bot != null)
            {
                @out.write("===> AIML GO.");
                AIMLbot.Request _request = new AIMLbot.Request(c_m_msg.Body, _aimlUser, AIML_Bot);
                AIMLbot.Result _reply = AIML_Bot.Chat(_request);

                msgType m_type = Utils.GetTypeOfMsg(c_m_msg, c_m_user);
                bool is_muser = m_type == msgType.MUC;
                string m_body = c_m_msg.Body;
                string vl = null;
                //if (c_m_muc != null)
                //    vl = c_m_muc.VipLang.GetLang(c_m_jid);
                //if (vl == null)
                //    vl = c_Sh.S.VipLang.GetLang(c_m_jid);

                Response r = new Response(c_Sh.S.Rg[
                              vl != null ?
                              vl :
                              is_muser ?
                              c_m_user.Language :
                              c_Sh.S.Config.Language
                         ]);

                int? access = c_Sh.S.GetAccess(c_m_msg, c_m_user, c_m_muc);

                if (access != null)
                    r.Access = access;
                else
                    r.Access = 0;

                r.Msg = c_m_msg;
                r.MSGLimit = c_Sh.S.Config.MucMSGLimit;

                r.MUC = c_m_muc;
                r.Level = c_level;
                r.MUser = c_m_user;
                //r.Delimiter = d;
                r.Sh = c_Sh;

                MessageType original_type = r.Msg.Type;
                //r.Msg.Type = MessageType.groupchat;
                r.Reply(_reply.Output);
                r.Msg.Type = original_type;

                //agsXMPP.protocol.client.Message msg = c_m_msg;
                //msg.From = c_m_msg.To;
                //msg.To = c_m_msg.From;
                //msg.Body = _reply.Output;
                //c_Sh.S.C.Send(msg);
            }
        }
    }



}
