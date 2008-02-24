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

using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System;
using agsXMPP;
using System.Web;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;
using System.IO;
using System.Net;
using Core.Kernel;
using Core.Conference;
using Core.Plugins;
using System.Threading;
using Core.Xml;
using Core.Other;

namespace Core.Kernel
{

    public class CommandHandler
    {

        #region Variables

        Message m_msg;
        bool alias_exists;
        Jid s_jid;
        string s_nick;
        MUser m_user;
        string original;
        Jid m_jid;
        MUC m_muc;
        Response r;
        SessionHandler Sh;

        #endregion;



        public CommandHandler(agsXMPP.protocol.client.Message msg, SessionHandler s)
        {
            m_msg = msg;

            s_jid = msg.From;
            Sh = s;
            if (s_jid.Bare == Sh.S.C.MyJID.Bare)
                return;
            Thread thr = new Thread(new ThreadStart(Handle));
            thr.Start();

        }

        public void Handle()
        {

            try
            {
                _Handle();
            }
            catch (Exception ex)
            {

                string data = "======> [" + DateTime.Now.ToString() + "]   \n" +
                                        "   Stanza: '" + m_msg.ToString() + "'\n" + ex.ToString() + "\n\n";
                Sh.S.ErrorLoger.Write(data);
                Message _msg = new Message();
                _msg.To = Sh.S.Config.Administartion()[0];
                _msg.Type = MessageType.chat;
                _msg.Body = "ERROR:" + ex.Message;
                Sh.S.C.Send(_msg);
            }
        }


        public void _Handle()
        {



            if (m_msg.Subject != null)
            {
                Sh.S.GetMUC(s_jid).Subject = m_msg.Subject;
            }

            m_muc = Sh.S.GetMUC(s_jid);
            m_user = null;

            if (m_muc != null)
            {
                if (s_jid.Resource == null)
                    return;
                m_user = m_muc.GetUser(s_jid.Resource);

            }

            string d = Sh.S.Config.Delimiter;

            s_nick = s_jid.Resource;
            msgType m_type = Sh.S.GetTypeOfMsg(m_msg, m_user);
            bool is_muser = m_type == msgType.MUC;
            m_jid = is_muser ? m_user.Jid : s_jid;

            if ((m_type == msgType.MUC) || (m_type == msgType.Roster))
            {
             
                m_msg.Body = m_msg.Body.TrimStart(' ');
                original = m_msg.Body;
                string m_body = m_msg.Body;
                Vipuser ud = Sh.S.VipManager.GetUserData(m_jid);
                bool ud_lang = ud != null;
                if (ud_lang)
                    ud_lang = ud.Language != null;
                r = new Response(Sh.S.Rg.
                    GetResponse(
                                 ud_lang ?
                                 ud.Language :
                                 is_muser ?
                                 m_user.Language :
                                 Sh.S.Config.Language
                            ));


                int access = Sh.S.GetAccess(m_msg, m_user);
                r.Access = access;
                r.Msg = m_msg;
                r.MSGLimit = Sh.S.Config.MSGLimit;
                string aliasb = String.Empty;
                string aliase = String.Empty;

                r.MUC = m_muc;
                r.MUser = m_user;
                r.Delimiter = d;
                r.Sh = Sh;

                int naccess = -1;

                if (is_muser)
                {
                    @out.exe("[" + s_jid.User + "] " + s_jid.Resource + "> " + m_msg.Body);

                    if (m_user.Nick == m_muc.MyNick)
                        return;
                    string found_censored = Sh.S.GetMUC(s_jid).IsCensored(m_msg.Body);
                    if (found_censored != null)
                    {
                        if (m_muc.KickableForCensored(m_user))
                        {
                            m_muc.Kick(m_user.Nick, found_censored);
                            return;
                        }
                        else
                        {
                            r.Reply(found_censored);
                            Thread.Sleep(1500);
                        }

                    }
                    naccess = m_muc.AccessManager.GetAccess(m_body);

                    alias_exists = m_muc.HasAlias(m_body);
                    r.HasAlias = alias_exists;
                    while (m_muc.HasAlias(m_body))
                    {
                  
                        m_body = m_muc.GetAlias(m_body, ref aliasb, ref aliase);
                      
                    }
                    if (alias_exists)
                        m_body = Utils.FormatEnvironmentVariables(m_body, m_muc, m_user);
                    @out.exe(m_body);
                    m_msg.Body = m_body;
                    r.Alias = aliasb;
                }

                string[] args = Utils.SplitEx(m_body, 2);

                if (!alias_exists)
                {
                    if (!m_msg.Body.StartsWith(d))
                        return;
                    else
                    {
                        m_msg.Body = m_msg.Body.Substring(d.Length);

                        m_body = m_msg.Body;
                        if (m_msg.Body.Trim() == "")
                            return;
                    }
                }



                args = Utils.SplitEx(m_body, 2);
                string m_cmd = args[0];

                string m_retort = null;
   

                @out.exe(m_body);

                   if ((Sh.S.PluginHandler.Handles(m_cmd)) || (alias_exists))
                   {


                       if (naccess == -1)
                       {
                           @out.exe("ALIAS");
                           naccess = Sh.S.AccessManager.GetAccess(m_body);
                       }
                       if (access < naccess)
                       {
                           r.Reply(r.FormatPattern("access_not_enough", naccess.ToString()));
                           return;
                       }
                   }
               

              


                if (Sh.S.PluginHandler.Handles(m_cmd))
                {



                    try
                    {
                        object obj = Sh.S.PluginHandler.Execute(m_cmd);
                        if (obj != null)
                        {
                            IPlugin plugin = (IPlugin)obj;
                            PluginTransfer pt = new PluginTransfer(r);
                            @out.exe("EXE");
                            plugin.PerformAction(pt);

                        }
                    }
                    catch (Exception ex)
                    {
                        Sh.S.ErrorLoger.Write(ex.ToString());
                        return;
                    }
                    return;
                }

          

                switch (m_cmd)
                {
                    case "help":
                        {
                            args = Utils.SplitEx(m_body, 1);

                            if (args.Length > 1)
                            {
                                string data = args[1].Trim();
                                while (data.IndexOf("  ") > -1)
                                    data = data.Replace("  ", " ");
                                m_retort = r.GetHelp(data);
                            }
                            else
                                m_retort = r.FormatPattern("help_info", Sh.S.PluginHandler.GetPluginsList(), d + "<volume> <command>");
                            break;
                        }

                   
                }

  

                    if (m_retort != null)
                        r.Reply(m_retort);
                    else
                        if (alias_exists)
                        {
                            r.Reply(aliase);
                        }
            }








        }

    }
}
