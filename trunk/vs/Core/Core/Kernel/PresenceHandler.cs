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
using System.Collections;
using System.Text;
using agsXMPP;
using agsXMPP.protocol.client;
using Core.Kernel;
using Core.Conference;
using Core.Other;
using Core.Xml;

namespace Core.Kernel
{
    public class PresenceHandler
    {
        public void Handle(Presence pres, SessionHandler Sh)
        {

      
            Jid p_jid = pres.From;
            MUC m_muc = Sh.S.GetMUC(p_jid);
            MUser m_user = null;
            Jid Jid = pres.From;
            if (pres.MucUser != null)
                if (pres.MucUser.Item.Jid != null)
                    Jid = pres.MucUser.Item.Jid;
            switch (pres.Type)
            {
                case PresenceType.subscribe:
                    PresenceManager pm = new PresenceManager(Sh.S.C);
                    pm.Subcribe(p_jid);
                    break;
                case PresenceType.error:
                    if (m_muc != null)
                    {

                        if (p_jid.Resource == m_muc.MyNick)
                        {
                            Sh.S.DelMUC(p_jid);
                            foreach (Jid j in Sh.S.Config.Administartion())
                            {
                                Message _msg = new Message();
                                _msg.To = j;
                                _msg.Type = MessageType.chat;
                                _msg.Body = p_jid.Bare + " => error: " + pres.Error.GetAttribute("code") + " - " + pres.Error.Condition.ToString();
                                Sh.S.C.Send(_msg);
                            }
                        }
                    }
                    break;
                case PresenceType.available:
                    Vipuser ud = Sh.S.VipManager.GetUserData(Jid);
                    bool ud_lang = ud != null;
                    if (ud_lang)
                        ud_lang = ud.Language != null;
                    string lng =
                                 ud_lang ?
                                 ud.Language :
                                 m_muc != null ?
                                 m_muc.Language :
                                 Sh.S.Config.Language;




                    if (pres.MucUser != null)
                    {



                        m_user = m_muc != null ? m_muc.GetUser(pres.From.Resource) : null;
                        Jid calcjid = m_muc != null ? pres.From : new Jid(pres.From.Bare);




                        Sh.S.CalcHandler.AddHandle(calcjid);
                        int access = Sh.S.GetAccess(pres);

                        string time;
                        if (m_muc != null)
                            @out.exe(m_user != null ? "[" + p_jid.User + "]*** " + p_jid.Resource + " is now " + pres.Show.ToString().Replace("NONE", "Online") : "[" + p_jid.User + "]*** " + p_jid.Resource + " enters the room as " + pres.MucUser.Item.Affiliation + "/" + pres.MucUser.Item.Role);
                        time = m_user != null ? m_user.EnterTime : DateTime.Now.ToString();
                        MUser user = new MUser(
                        pres.From.Resource,
                        Jid,
                        pres.MucUser.Item.Role,
                        pres.MucUser.Item.Affiliation,
                        pres.Status,
                        pres.Show,
                        lng,
                        time,
                        access
                        );

                        Sh.S.GetMUC(p_jid).SetUser(m_user, user);

                        if (m_muc != null)
                        {
                            Response r = new Response(Sh.S.Rg.GetResponse(lng));
                            r.MUC = m_muc;
                            r.MUser = user;
                            if (pres.Status != null)
                            {
                                string censored = Utils.FormatEnvironmentVariables(Sh.S.GetMUC(p_jid).IsCensored(pres.Status), m_muc, user);
                                if (censored != null)
                                {
                                    @out.exe(m_muc.KickableForCensored(user).ToString());
                                    if (m_muc.KickableForCensored(user))
                                        m_muc.Kick(user.Nick, censored);
                                    else
                                    {

                                        Message msg = new Message();
                                        r.Msg.Body = pres.Status;
                                        r.Msg.From = pres.From;
                                        r.Msg.Type = MessageType.groupchat;
                                        r.Reply(censored);
                                    }
                                }
                            }
                            if (m_user == null)
                            {
                                string data = Sh.S.Tempdb.Greet(Jid, m_muc.Jid);
                                if (data != null)
                                {
                                    @out.exe(">> "+data);
                                 
                                    r.Msg = new Message();
                                    r.Msg.From = pres.From;
                                    r.Msg.Type = MessageType.groupchat;
                                        r.Reply(Utils.FormatEnvironmentVariables(data, m_muc, user));
                                    
                                }



                            }


                            ArrayList ar = Sh.S.Tempdb.CheckAndAnswer(p_jid);
                            if (ar.Count > 0)
                            {
                                foreach (string[] phrase in ar)
                                {
                                    Jid _sender = new Jid(phrase[2]);
                                    string s_sender = phrase[2];
                                    s_sender = _sender.Resource;
                                    r.Msg = new Message();
                                    r.Msg.From = pres.From;
                                    r.Msg.Type = MessageType.chat;

                                    r.Reply(r.FormatPattern("said_to_tell", s_sender, phrase[1], phrase[3]));
                                    r.Msg.Type = MessageType.groupchat;
                                    r.Reply(r.FormatPattern("private_notify"));


                                }
                            }

                        }
                    }
                    else
                    {
                        Sh.S.RosterJids.Add(pres.From);

                    }
                    break;

                case PresenceType.unavailable:
                    if (pres.MucUser != null)
                    {
                        m_user = m_muc != null ? m_muc.GetUser(pres.From.Resource) : null;

                        if (m_user != null)
                        {
                            Sh.S.GetMUC(p_jid).DelUser(m_user);
                            @out.exe("[" + p_jid.User + "]*** " + p_jid.Resource + " leave the room");
                        }
                        else
                            return;
                        if (p_jid.Resource == m_muc.MyNick)
                        {
                            if (pres.MucUser != null)
                            {
                                if (pres.MucUser.Item.Nickname != null)
                                {
                                    if (pres.MucUser.Item.Nickname != p_jid.Resource)
                                        Sh.S.GetMUC(p_jid).MyNick = pres.MucUser.Item.Nickname;
                                }
                                else
                                {

                                    Sh.S.DelMUC(p_jid);
                                    foreach (Jid j in Sh.S.Config.Administartion())
                                    {


                                        Message _msg = new Message();
                                        _msg.To = j;
                                        _msg.Type = MessageType.chat;
                                        string data = p_jid.Bare + " => " + pres.Type.ToString();
                                        if (pres.HasTag("x"))
                                            if (pres.SelectSingleElement("x").HasTag("status"))
                                                if (pres.SelectSingleElement("x").SelectSingleElement("status").HasAttribute("code"))
                                                    data += " (" + pres.SelectSingleElement("x").SelectSingleElement("status").GetAttribute("code") + ")";
                                        _msg.Body = data;
                                        Sh.S.C.Send(_msg);
                                    }
                                }
                            }
                        }
                    }
                    break;
            }     
        }
    }
}
