﻿/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
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
using System.Threading;
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
        Presence pres;
        SessionHandler Sh;

        public PresenceHandler(Presence m_pres, SessionHandler sh)
        {
            pres = m_pres;
            Sh = sh;
            Handle();
        }
        public void Handle()
        {
            try
            {
                _Handle();
            }
            catch (Exception ex)
            {

                string clrf = "\n";
                string msg_source = pres.ToString();

                if (Utils.OS == Platform.Windows)
                {
                    clrf = "\r\n";
                    msg_source = pres.ToString().Replace("\n", "\r\n");

                }
                string data = "====== [" + DateTime.Now.ToString() + "] ===================================================>" + clrf +
                                        "   Stanza:" + clrf + msg_source + clrf + clrf + ex.ToString() + clrf + clrf + clrf;

                Sh.S.ErrorLoger.Write(data);
                Message _msg = new Message();
                _msg.To = Sh.S.Config.Administartion()[0];
                _msg.Type = MessageType.chat;
                _msg.Body = "ERROR:   " + ex.ToString();
                Sh.S.C.Send(_msg);
            }
        }

        public void _Handle()
        {


            pres.From = new Jid(pres.From.Bare.ToLower() + (pres.From.Resource != "" ? "/" + pres.From.Resource : ""));
            if (pres.MucUser != null)
                if (pres.MucUser.Item.Jid != null)
                    pres.MucUser.Item.Jid = new Jid(pres.MucUser.Item.Jid.Bare.ToLower() + (pres.MucUser.Item.Jid.Resource != "" ? "/" + pres.MucUser.Item.Jid.Resource : ""));
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
                    if (Sh.S.Config.AutoSubscribe)
                    {
                        Presence pr = new Presence();
                        pr.To = pres.From;
                        pr.Type = PresenceType.subscribed;
                        pr.Nickname = new agsXMPP.protocol.extensions.nickname.Nickname(Sh.S.Config.Nick);
                        Sh.S.C.Send(pr);
                        pr.Type = PresenceType.subscribe;
                        Sh.S.C.Send(pr);
                    }
                    else
                    {
                        Presence pr = new Presence();
                        pr.To = pres.From;
                        pr.Type = PresenceType.unsubscribed;
                        pr.Nickname = new agsXMPP.protocol.extensions.nickname.Nickname(Sh.S.Config.Nick);
                        Sh.S.C.Send(pr);
                    }
                    break;

                case PresenceType.unsubscribe:
                    Presence _pr = new Presence();
                    _pr.To = pres.From;
                    _pr.Type = PresenceType.unsubscribed;
                    _pr.Nickname = new agsXMPP.protocol.extensions.nickname.Nickname(Sh.S.Config.Nick);
                    Sh.S.C.Send(_pr);
                    _pr.Type = PresenceType.unsubscribe;
                    Sh.S.C.Send(_pr);
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
                    string vl = Sh.S.VipLang.GetLang(Jid);
                    if ((vl == null) && (m_muc != null))
                        vl = m_muc.VipLang.GetLang(Jid);
                    string lng =
                                 vl != null ?
                                 vl :
                                 m_muc != null ?
                                 m_muc.Language :
                                 Sh.S.Config.Language;



                    if (pres.MucUser != null)
                    {





                        m_user = m_muc != null ? m_muc.GetUser(pres.From.Resource) : null;
                        Jid calcjid = m_muc != null ? pres.From : new Jid(pres.From.Bare);


                        Sh.S.CalcHandler.AddHandle(calcjid);
                        int? access = Sh.S.GetAccess(pres, m_muc);

                        long time;
                        if (m_muc != null)
                            @out.exe(m_user != null ? "[" + p_jid.User + "]*** " + p_jid.Resource + " is now " + pres.Show.ToString().Replace("NONE", "Online") : "[" + p_jid.User + "]*** " + p_jid.Resource + " enters the room as " + pres.MucUser.Item.Affiliation + "/" + pres.MucUser.Item.Role);
                        time = m_user != null ? m_user.EnterTime : DateTime.Now.Ticks;
                        MUser user = new MUser(
                        pres.From.Resource,
                        Jid,
                        pres.MucUser.Item.Role,
                        pres.MucUser.Item.Affiliation,
                        pres.Status,
                        pres.Show,
                        lng,
                        time,
                        access,
                        m_user != null ? m_user.Idle : DateTime.Now.Ticks
                        );

                        Sh.S.GetMUC(p_jid).SetUser(m_user, user);

                        if (m_muc != null)
                        {
                            string ak;
                            m_muc = Sh.S.GetMUC(p_jid);
                            if (m_muc.OptionsHandler.GetOption("akick") == "+")
                            {
                                ak = Sh.S.Tempdb.IsAutoKick(Jid, p_jid.Resource, p_jid, Sh);
                                @out.exe("censored: " + (ak ?? "NULL"));
                                if (ak != null)
                                {

                                    if (m_muc.KickableForCensored(user))
                                    {
                                        @out.exe("censored: yes");
                                        m_muc.Kick(null, user.Nick, Utils.FormatEnvironmentVariables(ak, m_muc, user));
                                        return;
                                    }
                                }
                            }
                            if (m_muc.OptionsHandler.GetOption("avisitor") == "+")
                            {
                                ak = Sh.S.Tempdb.IsAutoVisitor(Jid, p_jid.Resource, p_jid, Sh);
                                if (ak != null)
                                {
                                    if (m_muc.KickableForCensored(user))
                                    {
                                        m_muc.Devoice(null, user.Nick, Utils.FormatEnvironmentVariables(ak, m_muc, user));
                                        return;
                                    }

                                }
                            }
                            if (m_muc.OptionsHandler.GetOption("amoderator") == "+")
                            {
                                if (Sh.S.Tempdb.AutoModerator(Jid, m_muc.Jid))
                                    m_muc.Moderator(null, user.Nick, null);
                            }
                            Response r = new Response(Sh.S.Rg.GetResponse(lng));
                            r.MUC = m_muc;
                            r.MUser = user;
                            if ((pres.Status != null) && (user.Nick != m_muc.MyNick))
                            {
                                string censored = Sh.S.GetMUC(p_jid).IsCensored(pres.Status, m_muc.OptionsHandler.GetOption("global_censor") == "+");
                                if (censored != null)
                                {

                                    @out.exe(m_muc.KickableForCensored(user).ToString());
                                    switch (m_muc.OptionsHandler.GetOption("censor_result"))
                                    {
                                        case "kick":
                                            {
                                                if (m_muc.KickableForCensored(user))
                                                { m_muc.Kick(null, user.Nick, censored); return; }
                                                else
                                                {
                                                    Message msg = new Message();
                                                    r.Msg = new Message();
                                                    r.Msg.Body = pres.Status;
                                                    r.Msg.From = pres.From;
                                                    r.Msg.Type = MessageType.groupchat;
                                                    r.Reply(censored);
                                                }
                                            } break;
                                        case "devoice":
                                            {
                                                if (m_muc.KickableForCensored(user))
                                                { m_muc.Devoice(null, user.Nick, censored); return; }
                                                else
                                                {
                                                    Message msg = new Message();
                                                    r.Msg = new Message();
                                                    r.Msg.Body = pres.Status;
                                                    r.Msg.From = pres.From;
                                                    r.Msg.Type = MessageType.groupchat;
                                                    r.Reply(censored);
                                                }

                                            }
                                            break;
                                        case "ban":
                                            {
                                                if (m_muc.KickableForCensored(user))
                                                { m_muc.Ban(null, user.Nick, censored); return; }
                                                else
                                                {
                                                    Message msg = new Message();
                                                    r.Msg = new Message();
                                                    r.Msg.Body = pres.Status;
                                                    r.Msg.From = pres.From;
                                                    r.Msg.Type = MessageType.groupchat;
                                                    r.Reply(censored);
                                                }


                                            }
                                            break;
                                        case "warn":
                                            {
                                                Message msg = new Message();
                                                r.Msg = new Message();
                                                r.Msg.Body = pres.Status;
                                                r.Msg.From = pres.From;
                                                r.Msg.Type = MessageType.groupchat;
                                                r.Reply(censored);
                                            }
                                            break;
                                        default:
                                            break;

                                    }
                                }
                            }
                            if (m_user == null)
                            {
                                string data = Sh.S.Tempdb.Greet(Jid, m_muc.Jid);
                                if (data != null)
                                {
                                    @out.exe(">> " + data);

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

                                    r.Reply(r.f("said_to_tell", s_sender, phrase[1], phrase[3]));
                                    r.Msg.Type = MessageType.groupchat;
                                    r.Reply(r.f("private_notify"));
                                    Thread.Sleep(Convert.ToInt32(Sh.S.Config.GetTag("interval")));

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
                    m_user = m_muc != null ? m_muc.GetUser(pres.From.Resource) : null;
                    Jid _calcjid = m_muc != null ? pres.From : new Jid(pres.From.Bare);
                    Sh.S.CalcHandler.DelHandle(_calcjid);
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
