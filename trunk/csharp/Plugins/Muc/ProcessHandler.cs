using System;
using System.Collections.Generic;
using System.Text;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;
using System.Threading;
using System.IO;
using Core.Client;
using Core.Conference;
using Core.Manager;
using agsXMPP.protocol.x.muc;
using agsXMPP.protocol.x.muc.iq.admin;
using agsXMPP.protocol.x.muc.iq.owner;
using Mono.Data.SqliteClient;


namespace Plugin
{
    public class MucHandler
    {
        string[] ws;
        Message m_msg;
        Response m_r;
        string self;
        bool syntax_error = false;
        Jid s_jid;
        string s_nick;
        string d;
        string n;
        string m_b;
        SessionHandler Sh;

        public MucHandler(Response r, string Name)
        {
            if (r.MUser == null)
            {
                r.Reply(r.FormatPattern("muconly"));
                return;
            }
            Sh = r.Sh;

            m_b = r.Msg.Body;
            ws = Utils.SplitEx(m_b, 2);
            m_msg = r.Msg;
            m_r = r;
            s_jid = r.Msg.From;
            s_nick = r.Msg.From.Resource;
            d = r.Delimiter;
            n = Name;
         
            if (ws.Length < 2)
            {
                r.Reply(r.FormatPattern("volume_info", n, d + n.ToLower() + " list"));
                return;
            }

         


            self = ws[0] + " " + ws[1];
            Handle();
        }




 

        public void Handle()
        {


       
            int myaccess = m_r.MUC.GetUser(m_r.MUC.MyNick).Access;
            string cmd = ws[1];
            string rs = null;




            switch (cmd)
            {

                case "kick":
                    {
                        if (ws.Length > 2)
                        {
                            switch (m_r.MUC.Kick(ws[2], m_r.FormatPattern("kick_reason")))
                            {
                                case ActionResult.Done:
                                    rs = m_r.Agree();
                                    break;
                                case ActionResult.NotAble:
                                    rs = m_r.Deny();
                                    break;
                                case ActionResult.UserNotFound:
                                    rs = m_r.FormatPattern("user_not_found", ws[2]);
                                    break;
                            }

                        }
                        else
                            syntax_error = true;
                        break;
                    }

                case "setsubject":
                    {
                        if (myaccess >= 50)
                        {
                            if (ws.Length > 2)
                            {
                                m_r.MUC.ChangeSubject(ws[2]);
                                rs = m_r.Agree();
                            }
                            else
                                rs = m_r.MUC.Subject;
                        }
                        else
                            rs = m_r.Deny();
                        break;
                    }



                case "nicks":
                    {
                        string data = "\n";
                        foreach (string nick in m_r.MUC.Users.Keys)
                        {
                            data += nick + ", ";
                        }

                        rs = m_r.FormatPattern("nicks_list") + data + "\n(" + m_r.MUC.Users.Count.ToString() + ")";

                        break;
                    }

                case "censor":
                    {
                        ws = Utils.SplitEx(m_b, 3);
                        if ((ws.Length > 2))
                        {
                            string reason = m_r.FormatPattern("kick_censored_reason");
                            if (ws.Length == 4)
                                reason = ws[3];
                            m_r.MUC.AddRoomCensor(ws[2], reason);
                            rs = m_r.Agree();
                        }
                        else
                            syntax_error = true;
                        break;
                    }
                case "allcensor":
                    {

                        if ((ws.Length == 2))
                        {
                            rs = m_r.MUC.GetRoomCensorList("{1}) {2}");
                        }
                        else
                            syntax_error = true;
                        break;
                    }

                case "uncensor":
                    {

                        if ((ws.Length > 2))
                        {
                            if (m_r.MUC.DelRoomCensor(ws[2]))
                                rs = m_r.Agree();
                            else
                                rs = m_r.Deny();
                        }
                        else
                            syntax_error = true;
                        break;
                    }


                case "tryme":
                    {
                        if (myaccess >= 50)
                        {
                            if (m_r.Access < myaccess)
                            {
                                Random rand = new Random();
                                switch (rand.Next(2))
                                {
                                    case 0:
                                        {
                                            rs = m_r.FormatPattern("tryme_fail");
                                            break;
                                        }
                                    default:
                                        {

                                            m_r.MUC.Kick(m_r.MUser.Nick, m_r.FormatPattern("tryme_reason"));
                                            Message msg = new Message();
                                            msg.To = m_r.MUser.Jid;
                                            msg.Type = MessageType.groupchat;
                                            msg.Body = m_r.FormatPattern("tryme_done");
                                            m_r.Connection.Send(msg);
                                            break;
                                        }
                                }

                            }
                            else
                                rs = m_r.FormatPattern("tryme_fail");
                        }
                        else
                            rs = m_r.FormatPattern("tryme_fail");
                        break;
                    }


                case "me":
                    {
                        rs = m_r.FormatPattern("self");
                        break;
                    }





                case "mylang":
                    {

                        if (ws.Length > 2)
                        {
                            string lang = ws[2].Trim();
                            if (Sh.S.Rg.GetResponse(lang) != null)
                            {
                                if (Sh.S.AutoMucManager.SetLanguage(m_r.MUC.Jid, lang))
                                {
                                    Sh.S.GetMUC(m_r.MUC.Jid).Language = lang;
                                    m_r.Document = Sh.S.Rg.GetResponse(lang).Document;
                                    m_r.Language = lang;
                                    rs = m_r.Agree();
                                }
                                else
                                    syntax_error = true;
                            }
                            else
                                rs = m_r.FormatPattern("lang_pack_not_found", lang);

                        }
                        else
                            syntax_error = true;

                        break;
                    }

                case "mystatus":
                    {

                        if (ws.Length > 2)
                        {
                            string status = ws[2].Trim();
                            if (Sh.S.AutoMucManager.SetStatus(m_r.MUC.Jid, status))
                            {
                                Presence pres = new Presence();
                                pres.To = m_r.MUC.Jid;
                                pres.Status = status;
                                pres.Show = m_r.MUC.MyShow;
                                m_r.Connection.Send(pres);

                                rs = m_r.Agree();
                            }
                            else
                                syntax_error = true;

                        }
                        else
                            syntax_error = true;

                        break;
                    }


                case "mynick":
                    {
                        if (ws.Length > 2)
                        {
                            string nick = ws[2].Trim();
                            if (Sh.S.AutoMucManager.SetNick(m_r.MUC.Jid, nick))
                            {
                                Sh.S.GetMUC(m_r.MUC.Jid).ChangeNick(ws[2]);


                                rs = m_r.Agree();
                            }
                            else
                                syntax_error = true;

                        }
                        else
                            syntax_error = true;
                        break;
                    }


                case "subject":
                    {
                        if (ws.Length == 2)
                        {
                            if (m_r.MUC.Subject != null)
                                rs = m_r.MUC.Subject;
                            else
                                rs = m_r.FormatPattern("muc_no_subject");
                        }
                        else
                            syntax_error = true;
                        break;

                    }

                case "show":
                    {

                        string data = "";
                        foreach (Jid j in Sh.S.MUCs.Keys)
                        {
                            data += "\n" + j.ToString() + ",";
                        }

                        rs = m_r.FormatPattern("mucs_list") + data + "\n-- " + Sh.S.MUCs.Count.ToString() + " --";

                        break;
                    }

                case "join":
                    {

                        ws = Utils.SplitEx(m_b, 3);

                        if (ws.Length == 3)
                        {
                            if (!Sh.S.AutoMucManager.Exists(new Jid(ws[2].Trim())))
                            {
                                MUC m = new MUC(Sh.S.C, new Jid(ws[2].Trim()), Sh.S.Config.Nick, Sh.S.Config.Status, Sh.S.Config.Language, ShowType.NONE, Sh.S.Censor.SQLiteConnection);
                                Sh.S.MUCs.Add(new Jid(ws[2].Trim()), m);
                                MucActivityController mac = new MucActivityController(m_r, m);
                                return;
                            }
                            else
                                rs = m_r.FormatPattern("muc_already_in");

                        }
                        else
                            if (ws.Length == 4)
                            {
                                if (!Sh.S.AutoMucManager.Exists(new Jid(ws[2].Trim())))
                                {
                                    MUC m = new MUC(Sh.S.C, new Jid(ws[2].Trim()), ws[3].Trim(), Sh.S.Config.Status, Sh.S.Config.Language, ShowType.NONE, Sh.S.Censor.SQLiteConnection);
                                    Sh.S.MUCs.Add(new Jid(ws[2].Trim()), m);
                                    MucActivityController mac = new MucActivityController(m_r, m);
                                    return;
                                   
                                }
                                else
                                    rs = m_r.FormatPattern("muc_already_in");

                            }
                            else
                                syntax_error = true;
                        break;
                    }

                case "leave":
                    {
                        if (ws.Length > 2)
                        {
                            Jid room = new Jid(ws[2].Trim());

                            if (Sh.S.AutoMucManager.Exists(room))
                            {


                                    m_r.Reply(m_r.Agree());
                                    if (Sh.S.GetMUC(room) != null)
                                    {
                                        Message msg = new Message();
                                        msg.To = room;
                                        msg.Body = m_r.FormatPattern("muc_leave");
                                        msg.Type = MessageType.groupchat;
                                        m_r.Connection.Send(msg);
                                        Presence pr = new Presence();
                                        pr.To = room;
                                        pr.Type = PresenceType.unavailable;
                                        m_r.Connection.Send(pr);
                                    }

                                    Sh.S.AutoMucManager.DelMuc(room);
                                   
                            }
                            else
                                rs = m_r.FormatPattern("muc_not_in");


                        }
                        else
                            syntax_error = true;
                        break;
                    }




                case "tell":
                    {
                        string word = Utils.GetValue(m_b, "[(.*)]").Trim();
                        m_b = Utils.RemoveValue(m_b, "[(.*)]", true);
                        ws = Utils.SplitEx(m_b, 2);


                        if ((ws.Length > 2) && (word != ""))
                        {
                            Jid sender = m_r.Msg.From;
                            Jid tell_jid = new Jid(m_r.MUC.Jid.Bare + "/" + word);


                            if (m_r.Msg.From.ToString() == tell_jid.ToString())
                            {
                                rs = m_r.FormatPattern("tell_fail_yourself");
                                break;
                            }

                            if (m_r.MUC.UserExists(word))
                            {

                                rs = m_r.FormatPattern("tell_fail_nick_here", word);
                                break;
                            }

                            Sh.S.Tempdb.AddTell(tell_jid, ws[2], sender);
                            rs = m_r.Agree();

                        }
                        else
                            syntax_error = true;
                        break;
                    }


                case "ban":
                    {
                        if (myaccess >= 70)
                        {
                            if (ws.Length > 2)
                            {
                                m_r.MUC.Ban(ws[2], m_r.FormatPattern("ban_reason"));
                                rs = m_r.Agree();
                            }
                            else
                                syntax_error = true;
                        }

                        else
                            rs = m_r.Deny();
                        break;
                    }

                case "admin":
                    {
                        if (myaccess >= 80)
                        {
                            if (ws.Length > 2)
                            {
                                m_r.MUC.Admin(ws[2], m_r.FormatPattern("admin_reason"));
                                rs = m_r.Agree();
                            }
                            else
                                syntax_error = true;
                        }
                        else
                            rs = m_r.Deny();
                        break;
                    }

                case "owner":
                    {
                        if (myaccess >= 80)
                        {
                            if (ws.Length > 2)
                            {
                                m_r.MUC.Owner(ws[2], m_r.FormatPattern("owner_reason"));
                                rs = m_r.Agree();
                            }
                            else
                                syntax_error = true;
                        }
                        else
                            rs = m_r.Deny();
                        break;
                    }

                case "none":
                    {
                        if (myaccess >= 80)
                        {
                            if (ws.Length > 2)
                            {
                                m_r.MUC.Participant(ws[2]);
                                rs = m_r.Agree();
                            }
                            else
                                syntax_error = true;
                        }
                        else
                            rs = m_r.Deny();
                        break;
                    }
                case "clean":
                    {
                        int number = 20;
                        if (ws.Length > 2)
                        {
                            try
                            {
                                number = Convert.ToInt32(ws[2]);


                                rs = m_r.FormatPattern("clean_up_done");
                            }
                            catch
                            {
                                syntax_error = true;
                            }
                        }
                        for (int i = 0; i < number; i++)
                        {
                            Message clm = new Message();
                            clm.To = m_r.MUC.Jid;
                            clm.Type = MessageType.groupchat;
                            clm.Body = " ";
                            m_r.Connection.Send(clm);
                            Thread.Sleep(1500);

                        }
                        break;

                    }
                case "member":
                    {
                        if (myaccess >= 70)
                        {
                            if (ws.Length > 2)
                            {

                                    m_r.MUC.MemberShip(ws[2]);
                                    rs = m_r.Agree();
                            }
                            else
                                syntax_error = true;

                        }
                        else
                            rs = m_r.Deny();
                        break;
                    }

                case "voice":
                    {
                        if (myaccess >= 50)
                        {
                            if (ws.Length > 2)
                            {
                                if (m_r.MUC.UserExists(ws[2]))
                                {
                                    m_r.MUC.Voice(ws[2], m_r.FormatPattern("voice_reason"));
                                    rs = m_r.Agree();
                                }
                                else
                                {
                                    rs = m_r.FormatPattern("user_not_found", ws[2]);
                                    break;
                                }

                            }
                            else
                                syntax_error = true;
                        }
                        else
                            rs = m_r.Deny();
                        break;
                    }


                case "devoice":
                    {
                        if (myaccess >= 50)
                        {
                            if (ws.Length > 2)
                            {
                                if (m_r.MUC.UserExists(ws[2]))
                                {
                                    m_r.MUC.Devoice(ws[2], m_r.FormatPattern("devoice_reason"));
                                    rs = m_r.Agree();
                                }
                                else
                                {
                                    rs = m_r.FormatPattern("user_not_found", ws[2]);
                                    break;
                                }
                            }
                            else
                                syntax_error = true;
                        }
                        else
                            rs = m_r.Deny();
                        break;
                    }

                case "name":
                    {
                        rs = m_r.MUC.Name;
                        break;
                    }


                case "jid":
                    {

                        MUser m_user = null;
                        if (ws.Length > 2)
                        {
                            if (m_r.MUC.UserExists(ws[2]))
                            {
                                m_user = m_r.MUC.GetUser(ws[2]);
                            }
                            else
                            {
                                rs = m_r.FormatPattern("user_not_found", ws[2]);
                                break;
                            }
                        }
                        else
                            m_user = m_r.MUser;

                        if (m_user != null)
                        {
                            m_r.Reply(m_r.FormatPattern("private_notify"));
                            m_r.Msg.Type = MessageType.chat;
                            rs = m_r.FormatPattern("real_jid", m_user.Nick, m_user.Jid.ToString());
                        }
                        break;
                    }

                case "echo":
                    {
                        if (ws.Length > 2)
                        {
                            Message msg = new Message();
                            msg.To = m_r.MUC.Jid;
                            msg.Type = MessageType.groupchat;
                            msg.Body = ws[2];
                            m_r.Connection.Send(msg);
                            if (m_msg.Type == MessageType.chat)
                                rs = m_r.Agree();
                        }
                        else
                            syntax_error = true;
                        break;
                    }

                case "status":
                    {
                        MUser m_user = null;
                        if (ws.Length > 2)
                        {
                            if (m_r.MUC.UserExists(ws[2]))
                            {
                                m_user = m_r.MUC.GetUser(ws[2]);
                            }
                            else
                            {
                                rs = m_r.FormatPattern("user_not_found", ws[2]);
                                break;
                            }

                        }
                        else
                            m_user = m_r.MUser;


                        rs = m_user.Status;
                        break;
                    }

                case "entered":
                    {
                        MUser m_user = null;
                        if (ws.Length > 2)
                        {
                            if (m_r.MUC.UserExists(ws[2]))
                            {
                                m_user = m_r.MUC.GetUser(ws[2]);
                            }
                            else
                            {
                                rs = m_r.FormatPattern("user_not_found", ws[2]);
                                break;
                            }

                        }
                        else
                            m_user = m_r.MUser;


                        rs = m_user.EnterTime;
                        break;
                    }


                case "role":
                    {
                        MUser m_user = null;
                        if (ws.Length > 2)
                        {
                            if (m_r.MUC.UserExists(ws[2]))
                            {
                                m_user = m_r.MUC.GetUser(ws[2]);
                            }
                            else
                            {
                                rs = m_r.FormatPattern("user_not_found", ws[2]);
                                break;
                            }

                        }
                        else
                            m_user = m_r.MUser;


                        rs = m_user.Affiliation + "/" + m_user.Role;
                        break;
                    }

                case "info":
                    {
                        MUser m_user = null;
                        if (ws.Length > 2)
                        {
                            if (m_r.MUC.UserExists(ws[2]))
                            {
                                m_user = m_r.MUC.GetUser(ws[2]);
                            }
                            else
                            {
                                rs = m_r.FormatPattern("user_not_found", ws[2]);
                                break;
                            }
                        }
                        else
                            m_user = m_r.MUser;



                        if (m_user != null)
                        {
                            Vipuser ud = Sh.S.VipManager.GetUserData(m_user.Jid);
                            bool ud_lang = ud != null;
                            if (ud_lang)
                                ud_lang = ud.Language != null;
                            string lng = ud_lang ?
                                               ud.Language :
                                               m_user.Language;

                            MUser user = m_r.MUser;
                            Message msg = new Message();
                            msg.From = m_user.Jid;
                            msg.To = m_msg.To;
                            msg.Body = m_msg.Body;
                            msg.Type = m_msg.Type;

                            string ShowJid = m_user.Jid.ToString();
                            if (m_r.Msg.Type == MessageType.groupchat)
                                ShowJid = "???";
                            else
                            if (m_r.Access <= 50)
                                    ShowJid = "???";

                            int access = Sh.S.GetAccess(msg, m_user);
                            rs = m_r.FormatPattern("muc_user_info",
                                   m_user.Nick,
                                   m_user.Affiliation + "/" + m_user.Role,
                                   lng,
                                   access.ToString(),
                                   m_user.Show + " (" + m_user.Status + ")",
                                   m_user.EnterTime,
                                   ShowJid);
                        }
                        break;
                    }

                case "list":
                    {
                        if (ws.Length == 2)
                        {
                            rs = m_r.FormatPattern("volume_list", n) + "\nlist, mynick, kick, ban, admin, tell, owner, tryme, member, voice, devoice, name,  status, role, entered, me, info, nicks, subject, censor, uncensor, allcensor, setsubject, join, leave, mystatus, mylang, show , disco";
                        }
                        break;
                    }

                default:
                    {
                        rs = m_r.FormatPattern("volume_cmd_not_found", n, ws[1], d + n.ToLower() + " list");
                        break;
                    }

            }

            if (syntax_error)
                m_r.se(self);
            else

            if (rs != null)
                m_r.Reply(rs);
               
        }

    }
}
