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

#region Namespaces

using System.Collections.Generic;
using System.Collections;
using System.Threading;
using System.Text;
using System;
using agsXMPP;
using Core.Kernel;
using Core.Xml;
using Core.DataBase;
using agsXMPP.Xml.Dom;
using System.IO;
using agsXMPP.protocol.client;
using agsXMPP.protocol.x.muc;
using Mono.Data.SqliteClient;
using System.Reflection;
using System.Diagnostics;
using System.Text.RegularExpressions;
using agsXMPP.protocol.x.muc.iq.admin;
using agsXMPP.protocol.x.muc.iq.owner;
using Core.Other;

#endregion Namespaces


namespace Core.Conference
{



    /// <summary>
    /// A class for containing chat-room information: users (roes, nicknames, statuses), MUC-specific data
    /// </summary>
    public class MUC
    {

        Jid m_jid;
        string m_name;
        string m_server;
        Hashtable m_users;
        Document m_aliases;
        string m_mynick;
        string m_mystatus;
        string m_lang;
        string m_dir;
        string vl_dir;
        string va_dir;
        string m_password;
        SqliteConnection sqlite_conn;
        string acc_file;
        string al_file;
        LocalAccess la;
        Dictionary m_defs;
        MucManager m_manager;
        VipAccess va;
        VipLang vl;
        ShowType m_show;
        OptionsHandler opth;
        string m_subject;
        static object[] sobjs = new object[60];

        XmppClientConnection m_con;

        //Whowas collector (StringBuilder)
        StringBuilder _whowas;


        /// <summary>
        /// Jid of a conference
        /// </summary>
        public Jid Jid
        {
            get
            {
                lock (sobjs[3])
                {
                    return m_jid;
                }
            }
        }


        /// <summary>
        /// Current subject of a conference
        /// </summary>
        public string Subject
        {
            get { lock (sobjs[13]) { return m_subject; } }
            set { lock (sobjs[13]) { m_subject = value; } }
        }

        /// <summary>
        /// Manager for commands access-level, but local ( ony for this conference)
        /// </summary>
        public LocalAccess AccessManager
        {
            get { lock (sobjs[42]) { return la; } }
            set { lock (sobjs[42]) { la = value; } }
        }

        /// <summary>
        /// Manager for options
        /// </summary>
        public OptionsHandler OptionsHandler
        {
            get { lock (sobjs[47]) { return opth; } }
            set { lock (sobjs[47]) { opth = value; } }
        }

        /// <summary>
        /// A passowrd fro entering the MUC. If unneeded set it to null.
        /// </summary>
        public string Password
        {
            get { lock (sobjs[14]) { return m_password; } }
            set { lock (sobjs[14]) { m_password = value; } }
        }


        /// <summary>
        /// Manager for ruling users jid-spesific, but local (only for this conference) access-level
        /// </summary>
        public VipAccess VipAccess
        {
            get { lock (sobjs[45]) { return va; } }
            set { lock (sobjs[45]) { va = value; } }
        }

        /// <summary>
        /// Manager for ruling users jid-spesific, but local (only for this conference) language of reply
        /// </summary>
        public VipLang VipLang
        {
            get { lock (sobjs[46]) { return vl; } }
            set { lock (sobjs[46]) { vl = value; } }
        }

        /// <summary>
        /// A database, having a similar-to-dictionary structure [entity -> definition (c) author]
        /// </summary>
        public Dictionary Dictionary
        {
            get { lock (sobjs[35]) { return m_defs; } }
            set { lock (sobjs[35]) { m_defs = value; } }
        }

        /// <summary>
        /// Manager for a conference to manage users' role/affiliation, also  join/leave conference.
        /// </summary>
        public MucManager Manager
        {
            get
            {
                lock (sobjs[12])
                {
                    return m_manager;
                }
            }
        }


        /// <summary>
        /// Current connection to the server 
        /// </summary>
        public XmppClientConnection Connection
        {
            get
            {
                lock (sobjs[11])
                {
                    return m_con;
                }
            }
        }

        /// <summary>
        /// The Hashtable of users, located in the conference 
        /// </summary>
        public Hashtable Users
        {
            get { return m_users; }
        }

        /// <summary>
        /// The name of conference: user part of a Jid
        /// </summary>
        public string Name
        {
            get { return Jid.User; }
        }

        /// <summary>
        /// The current Nickname of a bot in the conference
        /// </summary>
        public string MyNick
        {
            get
            {
                lock (sobjs[1])
                {
                    return m_mynick;
                }
            }
            set
            {
                lock (sobjs[1])
                {
                    m_mynick = value;
                }
            }
        }

        /// <summary>
        /// A Whowas collector
        /// </summary>
        public StringBuilder WhoWas
        {
            get
            {
                lock (sobjs[51])
                {
                    return _whowas;
                }
            }
            set
            {
                lock (sobjs[51])
                {
                    _whowas = value;
                }
            }
        }


        /// <summary>
        /// Tryes to kick user with nickname "Nick", if Response is not null, messages the querer of a result
        /// </summary>
        /// <param name="r"></param>
        /// <param name="Nick"></param>
        /// <param name="Reason"></param>
        public void Kick(Response r, string Nick, string Reason)
        {
            if (UserExists(Nick))
            {
                if (KickableForCensored(GetUser(Nick)))
                {
                    AdminIQCB aic = new AdminIQCB();
                    aic.ChangeRole(this.Connection, r, Role.none, this.Jid, null, Nick, Reason);
                }
                else
                    if (r != null) r.Reply(r.Deny());
            }
            else
                if (r != null) r.Reply(r.f("user_not_found", Nick));
        }


        /// <summary>
        /// Tryes to ban user with nickname "Nick", if Response is not null, messages the querer of a result
        /// </summary>
        /// <param name="r"></param>
        /// <param name="Nick"></param>
        /// <param name="Reason"></param>
        public void Ban(Response r, string Nick, string Reason)
        {
            AdminIQCB aic = new AdminIQCB();
            if (UserExists(Nick))
            {
                aic.ChangeAffiliation(this.Connection, r, Affiliation.outcast, this.Jid, this.GetUser(Nick).Jid, null, Reason);
            }
            else
            {
                aic.ChangeAffiliation(this.Connection, r, Affiliation.outcast, this.Jid, new Jid(Nick), null, Reason);
            }

        }

        /// <summary>
        /// Tryes to give voice to an user with nickname "Nick", if Response is not null, messages the querer of a result
        /// </summary>
        /// <param name="r"></param>
        /// <param name="Nick"></param>
        /// <param name="Reason"></param>
        public void Voice(Response r, string Nick, string Reason)
        {
            if (UserExists(Nick))
            {
                AdminIQCB aic = new AdminIQCB();
                aic.ChangeRole(this.Connection, r, Role.participant, this.Jid, null, Nick, Reason);
            }
            else
                if (r != null) r.Reply(r.f("user_not_found", Nick));

        }

        /// <summary>
        /// The SQLite conenction, given by main Censor manager.
        /// </summary>
        public SqliteConnection SQLiteConnection
        {
            get { lock (sobjs[38]) { return sqlite_conn; } }
            set { lock (sobjs[38]) { sqlite_conn = value; } }
        }

        /// <summary>
        /// Tryes to give away voice to an user with nickname "Nick", if Response is not null, messages the querer of a result
        /// </summary>
        /// <param name="r"></param>
        /// <param name="Nick"></param>
        /// <param name="Reason"></param>
        public void Devoice(Response r, string Nick, string Reason)
        {
            if (UserExists(Nick))
            {
                AdminIQCB aic = new AdminIQCB();
                aic.ChangeRole(this.Connection, r, Role.visitor, this.Jid, null, Nick, Reason);
            }
            else
                if (r != null) r.Reply(r.f("user_not_found", Nick));
        }

        /// <summary>
        /// Change to subject of a current conference
        /// </summary>
        /// <param name="NewSubject"></param>
        public void ChangeSubject(string NewSubject)
        {
            Manager.ChangeSubject(Jid, NewSubject);
        }

        /// <summary>
        /// Sends "unavailable" presence to a the current room.
        /// </summary>
        public void Leave()
        {
            Manager.LeaveRoom(Jid, MyNick);
            @out.write("MUC LEAVE >>> " + Jid.ToString());
        }

        /// <summary>
        /// Tryes to make a user with nickname "Nick" participant, if Response is not null, messages the querer of a result
        /// </summary>
        /// <param name="r"></param>
        /// <param name="Nick"></param>
        public void Participant(Response r, string Nick)
        {
            AdminIQCB aic = new AdminIQCB();
            if (UserExists(Nick))
            {
                aic.ChangeAffiliation(this.Connection, r, Affiliation.none, this.Jid, null, Nick, null);
                aic.ChangeRole(this.Connection, null, Role.participant, this.Jid, null, Nick, null);

            }
            else
            {
                aic.ChangeAffiliation(this.Connection, r, Affiliation.none, this.Jid, new Jid(Nick), null, null);
                aic.ChangeRole(this.Connection, null, Role.participant, this.Jid, new Jid(Nick), null, null);
            }
        }

        /// <summary>
        /// Tryes to make a user with nickname "Nick" member/participant, if Response is not null, messages the querer of a result
        /// </summary>
        /// <param name="r"></param>
        /// <param name="Nick"></param>
        public void MemberShip(Response r, string Nick)
        {
            AdminIQCB aic = new AdminIQCB();
            if (UserExists(Nick))
            {
                aic.ChangeRole(this.Connection, null, Role.participant, this.Jid, null, Nick, null);
                aic.ChangeAffiliation(this.Connection, r, Affiliation.member, this.Jid, null, Nick, null);
            }
            else
            {
                aic.ChangeRole(this.Connection, null, Role.participant, this.Jid, new Jid(Nick), null, null);
                aic.ChangeAffiliation(this.Connection, r, Affiliation.member, this.Jid, new Jid(Nick), null, null);
            }
        }

        /// <summary>
        /// Tryes to make a user with nickname "Nick" to be admin/moderator, if Response is not null, messages the querer of a result
        /// </summary>
        /// <param name="r"></param>
        /// <param name="Nick"></param>
        /// <param name="Reason"></param>
        public void Admin(Response r, string Nick, string Reason)
        {
            AdminIQCB aic = new AdminIQCB();
            if (UserExists(Nick))
            {
                aic.ChangeRole(this.Connection, null, Role.moderator, this.Jid, this.GetUser(Nick).Jid, null, Reason);
                aic.ChangeAffiliation(this.Connection, r, Affiliation.admin, this.Jid, this.GetUser(Nick).Jid, null, Reason);
            }
            else
            {
                aic.ChangeRole(this.Connection, null, Role.moderator, this.Jid, new Jid(Nick), null, Reason);
                aic.ChangeAffiliation(this.Connection, r, Affiliation.admin, this.Jid, new Jid(Nick), null, Reason);
            }
        }


        /// <summary>
        /// MUser instance, where bot's user is hidden.
        /// </summary>
        public MUser Me
        {
            get
            {
                return this.GetUser(this.MyNick);
            }
        }


        /// <summary>
        /// Tryes to make a user with nickname "Nick" owner/moderator, if Response is not null, messages the querer of a result
        /// </summary>
        /// <param name="r"></param>
        /// <param name="Nick"></param>
        /// <param name="Reason"></param>
        public void Owner(Response r, string Nick, string Reason)
        {
            AdminIQCB aic = new AdminIQCB();
            if (UserExists(Nick))
            {
                aic.ChangeRole(this.Connection, null, Role.moderator, this.Jid, this.GetUser(Nick).Jid, null, Reason);
                aic.ChangeAffiliation(this.Connection, r, Affiliation.owner, this.Jid, this.GetUser(Nick).Jid, null, Reason);
            }
            else
            {
                aic.ChangeRole(this.Connection, null, Role.moderator, this.Jid, new Jid(Nick), null, Reason);
                aic.ChangeAffiliation(this.Connection, r, Affiliation.owner, this.Jid, new Jid(Nick), null, Reason);
            }
        }

        /// <summary>
        /// tryes to change user's role to moderator
        /// </summary>
        /// <param name="r"></param>
        /// <param name="Nick"></param>
        /// <param name="Reason"></param>
        public void Moderator(Response r, string Nick, string Reason)
        {
            if (UserExists(Nick))
            {
                AdminIQCB aic = new AdminIQCB();
                aic.ChangeRole(this.Connection, null, Role.moderator, this.Jid, null, Nick, Reason);
            }
            else
                if (r != null) r.Reply(r.f("user_not_found", Nick));
        }


        /// <summary>
        /// Changes bot's nickname in current conference to "Nick" 
        /// </summary>
        /// <param name="Nick"></param>
        /// <returns></returns>
        public bool ChangeNick(string Nick)
        {
            if (!UserExists(Nick))
            {
                Manager.ChangeNickname(Jid, Nick);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check if the user is present in the MUC 
        /// </summary>
        /// <param name="Nick"></param>
        /// <returns>A boolean value indicating, if the user is present in the MUC</returns>
        public bool UserExists(string Nick)
        {
            return GetUser(Nick) != null;
        }

        /// <summary>
        /// Show-status of the bot in the current MUC
        /// </summary>
        public ShowType MyShow
        {
            get
            {
                lock (sobjs[7])
                {
                    return Me != null ? Me.Show : m_show;
                }
            }
            set
            {
                lock (sobjs[7])
                {
                    if (Me != null) Me.Show = value;
                    else
                        m_show = value;
                }
            }
        }

        /// <summary>
        /// Status message of the bot in the current MUC
        /// </summary>
        public string MyStatus
        {
            get
            {
                lock (sobjs[2])
                {
                    return Me != null ? Me.Status : m_mystatus;
                }
            }
            set
            {
                lock (sobjs[2])
                {
                    if (Me != null) Me.Status = value;
                    else
                        m_mystatus = value;
                }
            }
        }

        /// <summary>
        /// Basic language for all users in the current MUC
        /// </summary>
        public string Language
        {
            get
            {
                lock (sobjs[0])
                {
                    return m_lang;
                }
            }

            set
            {
                lock (Users)
                {
                    foreach (MUser u in Users.Values)
                    {
                        u.Language = value;
                    }
                    m_lang = value;
                }
            }
        }

        /// <summary>
        /// Server-part of the Jid of the MUC
        /// </summary>
        public string Server
        {
            get { return m_server; }
        }

        /// <summary>
        /// Document, a container of the aliases file
        /// </summary>
        public Document Aliases
        {
            get
            {
                lock (sobjs[23])
                {
                    return m_aliases;
                }
            }
        }

        /// <summary>
        /// The count of aliases in the current MUC
        /// </summary>
        /// <returns></returns>
        public int AliasCount()
        {
            int res = 0;
            {
                foreach (Element el in Aliases.RootElement.SelectElements("alias"))
                {
                    res++;
                }
            }
            return res;
        }

       
        /// <summary>
        /// Retrieves an alias by the original phrase, user, who called the command,
        /// writes to "from" a name of the alias, "val" - a value of the last alias, "to" - formatted last alias value
        /// </summary>
        /// <param name="phrase"></param>
        /// <param name="from"></param>
        /// <param name="val"></param>
        /// <param name="to"></param>
        /// <param name="muc"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public string GetAlias(string phrase, ref string from, ref string val, ref string to, Response r)
        {
            lock (sobjs[22])
            {
      
                if (OptionsHandler.GetOption("aliases") == "-") return phrase;
                foreach (Element el in Aliases.RootElement.SelectElements("alias"))
                {

                    string name = el.GetAttribute("name");
                    string value = el.GetAttribute("value");
                    value = Utils.FormatEnvironmentVariables(value, r);
                    val = value;
                    from = name;

                    if ((phrase.IndexOf(name + " ") == 0) || (phrase == name))
                    {
                        string basic = phrase.Remove(0, name.Length);
                        value = Utils.FormatMsg(value, basic, "#", true, false);
                        value = Utils.FormatMsg(value, basic, "%", true, true);
                        to = value.Trim();
                        return to;
                    }

                }
                return phrase;
            }
        }


        /// <summary>
        /// Check if it is possible to kick the user "muser" from the MUC
        /// </summary>
        /// <param name="muser"></param>
        /// <returns></returns>
        public bool KickableForCensored(MUser muser)
        {
            MUser me = this.Me;
            @out.exe("IQ_affiliation_role_change_step 1");
            if (muser.Role == Role.moderator || muser.Access == 100)
                return false;
            @out.exe("IQ_affiliation_role_change_step 2");
            if (me == null) return true;
            @out.exe("IQ_affiliation_role_change_step 3");
            switch (me.Role)
            {
                case Role.moderator:
                    switch (me.Affiliation)
                    {
                        case Affiliation.none:
                            switch (muser.Affiliation)
                            {
                                case Affiliation.none:
                                    return true;
                                case Affiliation.member:
                                    goto case Affiliation.none;
                                default:
                                    return false;
                            }

                        case Affiliation.member:
                            switch (muser.Affiliation)
                            {
                                case Affiliation.none:
                                    return true;
                                case Affiliation.member:
                                    return true;
                                default:
                                    return false;
                            }
                        case Affiliation.admin:
                            switch (muser.Affiliation)
                            {
                                case Affiliation.none:
                                    return true;
                                case Affiliation.member:
                                    return true;
                                case Affiliation.outcast:
                                    return true;
                                default:
                                    return false;
                            }
                        case Affiliation.owner:
                            @out.exe("IQ_affiliation_role_change_step 4");
                            switch (muser.Affiliation)
                            {
                                case Affiliation.none:
                                    return true;
                                case Affiliation.member:
                                    return true;
                                case Affiliation.outcast:
                                    return true;
                                case Affiliation.admin:
                                    return true;
                                default:
                                    return false;
                            }

                        default:
                            return false;

                    }
                default:
                    return false;
            }



        }

        /// <summary>
        /// Check if the room handles an alias "phrase"
        /// </summary>
        /// <param name="phrase"></param>
        /// <returns></returns>
        public bool HasAlias(string phrase)
        {

            lock (Aliases)
            {
                if (OptionsHandler.GetOption("aliases") == "-") return false;
                foreach (Element el in Aliases.RootElement.SelectElements("alias"))
                {
                    string name = el.GetAttribute("name");
                    string value = el.GetAttribute("value");

                    if ((phrase.IndexOf(name + " ") == 0) || (phrase == name))
                        return true;
                }
                return false;


            }
        }

        /// <summary>
        /// Get alias list of the current MUc
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="find"></param>
        /// <returns></returns>
        public string GetAliasList(string pattern, string find)
        {
            lock (Aliases)
            {
                string res = "";
                int count = 0;

                foreach (Element el in Aliases.RootElement.SelectElements("alias"))
                {
                    count++;
                    if (find != null)
                    {
                        if (el.GetAttribute("name") == find)
                            return el.GetAttribute("value");
                    }
                    else
                    {
                        string name = el.GetAttribute("name");
                        string value = el.GetAttribute("value");
                        res += pattern.Replace("{1}", count.ToString()).Replace("{2}", name).Replace("{3}", value);
                    }
                }

                if (find != null)
                    return null;
                if (res != "")
                    return res;
                else
                    return null;
            }


        }

        /// <summary>
        /// Clears all the aliases in the current MUC (deletes each)
        /// </summary>
        public void ClearAliases()
        {
            lock (Aliases)
            {

                Aliases.Clear();
                Aliases.LoadXml("<Aliases></Aliases>");
                Aliases.Save(al_file);
            }
        }

        /// <summary>
        /// Delete the single specified alias by name 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public bool DelAlias(string source)
        {
            lock (Aliases)
            {
                try
                {
                    int number = Convert.ToInt32(source);
                    int count = 0;
                    if (Aliases.RootElement.SelectElements("alias") != null)
                    {
                        foreach (Element el in Aliases.RootElement.SelectElements("alias"))
                        {
                            count++;
                            if (count == number)
                            {
                                string m_source = Aliases.ToString();
                                m_source = m_source.Replace(el.ToString(), "");
                                Aliases.Clear();
                                Aliases.LoadXml(m_source);
                                Aliases.Save(al_file);
                                return true;
                            }
                        }
                    }
                }
                catch
                {
                    if (Aliases.RootElement.SelectElements("alias") != null)
                    {
                        foreach (Element el in Aliases.RootElement.SelectElements("alias"))
                        {
                            if (source == el.GetAttribute("name"))
                            {
                                string m_source = Aliases.ToString();
                                m_source = m_source.Replace(el.ToString(), "");
                                Aliases.Clear();
                                Aliases.LoadXml(m_source);
                                Aliases.Save(al_file);
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// now you can use MUC instance as a string - it will return it's bare jid
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        static public implicit operator Jid(MUC m)
        {
            return m.Jid;
        }



        //the method below is looking pretty cool, ye? :)
        //ye, but it determines if the alias is not following the recursion :)
        public bool chkal_rec(string alias_name, int curr_level, int max_level, Response r)
        {
            @out.exe(curr_level.ToString() + "  " + alias_name);
            if (curr_level >= max_level) return true;
            string b = "", c = "", a = "";
            while (HasAlias(alias_name))
            {
                if (curr_level >= max_level) return true;
                alias_name = GetAlias(alias_name, ref b, ref c, ref a,r); curr_level++;
            }
            @out.exe(curr_level.ToString() + "  " + alias_name);
            string[] ws = Utils.SplitEx(alias_name, 2);
            if (ws.Length > 2) if (ws[0].ToLower() == "misc" && ws[1] == "make")
                {
                    string[] ps = ws[2].Split(new string[] { "&&" }, StringSplitOptions.RemoveEmptyEntries);
                    curr_level += ps.Length - 1; foreach (string p in ps)
                        if (chkal_rec(p, curr_level, max_level, r)) 
                            return true;
                        return false;
                }
            return false;
        }

        /// <summary>
        /// Add the alias by its name and value
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public bool AddAlias(string alias, string cmd)
        {
            lock (Aliases)
            {
                if (!HasAlias(alias))
                {
                    foreach (Element el in Aliases.RootElement.SelectElements("alias"))
                        if (el.GetAttribute("name") == alias)
                            return false;

                    Aliases.RootElement.AddTag("alias");
                    foreach (Element el in Aliases.RootElement.SelectElements("alias"))
                    {
                        if (!el.HasAttribute("name"))
                        {
                            el.SetAttribute("name", alias);
                            el.SetAttribute("value", cmd);
                            Aliases.Save(al_file);
                            return true;
                        }
                    }
                    return false;
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// Get MUser instance of the user by nickname
        /// </summary>
        /// <param name="Nick"></param>
        /// <returns></returns>
        public MUser GetUser(string Nick)
        {
            lock (Users)
            {
                MUser mu = (MUser)Users[Nick];
                return mu;
            }
        }

        /// <summary>
        /// Set user's data: erase previous, write the new one.
        /// </summary>
        /// <param name="olduser"></param>
        /// <param name="newuser"></param>
        public void SetUser(MUser olduser, MUser newuser)
        {
            lock (Users)
            {
                if (olduser != null)
                    Users.Remove(olduser.Nick);
                Users.Add(newuser.Nick, newuser);
            }
        }

        /// <summary>
        /// Delete data of the user (delete the instance from the users' list)
        /// </summary>
        /// <param name="user"></param>
        public void DelUser(MUser user)
        {
            lock (sobjs[5])
            {
                if (user != null)
                    Users.Remove(user.Nick);
            }
        }

        /// <summary>
        /// Add the expression, which will be writen to the local censor
        /// </summary>
        /// <param name="source"></param>
        /// <param name="reason"></param>
        public void AddRoomCensor(string source, string reason)
        {
            lock (sqlite_conn)
            {
                source = source.Replace("'", "''");
                reason = reason.Replace("'", "''");
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                     INSERT 
                         INTO censor (muc, censor_data, reason) 
                         VALUES ('{0}', '{1}', '{2}');
                                      ",
                                          Jid.Bare, source, reason),
                                          sqlite_conn);
                cmd.ExecuteNonQuery();
            }

        }

        /// <summary>
        /// Clear the censor expressions of the local censor
        /// </summary>
        public void ClearCensor()
        {
            lock (sqlite_conn)
            {
                string room = Jid.Bare.Replace("'", "''");
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                     DELETE 
                         FROM censor 
                         WHERE (muc = '{0}')
                                      ", room),
                                          sqlite_conn);
                cmd.ExecuteNonQuery();
            }

        }

        /// <summary>
        /// Delete the specified expression (or number in the list) from the list of local expressions 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public bool DelRoomCensor(string source)
        {
            lock (sqlite_conn)
            {
                source = source.ToLower();
                try
                {
                    int num = Convert.ToInt32(source);

                    source = source.Replace("'", "''");
                    SqliteCommand cmd = new SqliteCommand(String.Format(@"
                     SELECT *
                         FROM censor
                         WHERE (muc = '{0}') 
                                      ",
                                          Jid.Bare),
                                          sqlite_conn);
                    cmd.ExecuteNonQuery();
                    SqliteDataReader sqlite_datareader = cmd.ExecuteReader();
                    int i = 0;
                    while (sqlite_datareader.Read())
                    {
                        i++;
                        if (num == i)
                        {
                            string caught = sqlite_datareader.GetString(1).Replace("'", "''");
                            string reason = sqlite_datareader.GetString(2).Replace("'", "''");
                            cmd = new SqliteCommand(String.Format(@"
                     DELETE 
                         FROM censor
                         WHERE  (censor_data = '{1}' and muc = '{0}' and reason = '{2}') 
                                      ",
                                                                      Jid.Bare, caught, reason),
                                                                      sqlite_conn);
                            cmd.ExecuteNonQuery();
                            return true;
                        }
                    }
                    return false;
                }
                catch
                {

                    source = source.Replace("'", "''");
                    SqliteCommand cmd = new SqliteCommand(String.Format(@"
                     SELECT *
                         FROM censor
                         WHERE (censor_data = '{1}' and muc = '{0}') 
                                      ",
                                          Jid.Bare, source),
                                          sqlite_conn);
                    cmd.ExecuteNonQuery();
                    SqliteDataReader sqlite_datareader = cmd.ExecuteReader();
                    if (!sqlite_datareader.Read())
                        return false;

                    //(muc = '{0}') &&
                    cmd = new SqliteCommand(String.Format(@"
                     DELETE 
                         FROM censor
                         WHERE  (censor_data = '{1}' and muc = '{0}') 
                                      ",
                                              Jid.Bare, source),
                                              sqlite_conn);
                    cmd.ExecuteNonQuery();

                    return true;


                }

            }

        }

        /// <summary>
        /// Get the list of local censor expressions  
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public string GetRoomCensorList(string pattern)
        {
            lock (sqlite_conn)
            {//(muc = '{0}')  && 
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                     SELECT *
                         FROM censor
                         WHERE (muc = '{0}') 
                                      ",
                                      Jid.Bare),
                                      SQLiteConnection);
                cmd.ExecuteNonQuery();
                SqliteDataReader sqlite_datareader = cmd.ExecuteReader();
                string data = "";
                int i = 0;
                while (sqlite_datareader.Read())
                {
                    i++;
                    data += "\n" + pattern.Replace("{1}", i.ToString()).Replace("{2}", sqlite_datareader.GetString(1)).Replace("{3}", sqlite_datareader.GetString(2));

                }

                return data != "" ? data : null;

            }

        }

        /// <summary>
        /// Check if the "source" is caught by censor (full or only local according to the options of the MUC)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="global"></param>
        /// <returns>If the string "source" os caught by censor - returns the string-reaction</returns>
        public string IsCensored(string source, bool global)
        {

            lock (SQLiteConnection)
            {
                Regex reg;
                source = source.ToLower().Replace("'", "''");
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                   SELECT  * 
                           FROM censor 
                           WHERE (muc = '{0}')" + (global ? " or (muc = '*')" : "") + @"
                                      ",
                                          Jid.Bare),
                                          SQLiteConnection);
                cmd.ExecuteNonQuery();

                SqliteDataReader sqlite_datareader = cmd.ExecuteReader();
                while (sqlite_datareader.Read())
                {
                    string data = sqlite_datareader.GetString(1);
                    try
                    {
                        reg = new Regex(data);
                        if (reg.IsMatch(source))
                            return sqlite_datareader.GetString(2);

                    }
                    catch
                    {

                    }
                }
                return null;
            }
        }

        

        /// <summary>
        /// Join the current MUC (send presence)
        /// </summary>
        public void Join()
        {
            Presence pres = new Presence();
            pres.To = new Jid(m_jid + "/" + m_mynick);
            pres.Status = m_mystatus;
            pres.Show = MyShow;
            Muc x = new Muc();
            if (this.Password != null)
                x.Password = this.Password;
            //<history maxstanzas='0'/>
            History h = new History(0);
            x.AddChild(h);
            pres.AddChild(x);
            m_con.Send(pres);
            @out.write("MUC JOIN >>> " + Jid.ToString());
            //init whowas
            //this.WhoWas = new StringBuilder();
        }

        /// <summary>
        /// A constructor: please fill all the filed of the new conference's data
        /// </summary>
        /// <param name="con"></param>
        /// <param name="Jid"></param>
        /// <param name="Nick"></param>
        /// <param name="Status"></param>
        /// <param name="Lang"></param>
        /// <param name="Show"></param>
        /// <param name="sh"></param>
        /// <param name="Password"></param>
        public MUC(XmppClientConnection con, Jid Jid, string Nick, string Status, string Lang, ShowType Show, SessionHandler sh, string Password)
        {
            for (int i = 0; i < 60; i++)
            {
                sobjs[i] = new object();
            }

            DirBuilder db = new DirBuilder();
            m_con = con;
            m_jid = Jid;
            m_show = Show;
            m_name = m_jid.Bare;
            m_server = m_jid.Server;
            m_mynick = Nick;
            m_mystatus = Status;
            sqlite_conn = sh.S.Censor.SQLiteConnection;
            m_lang = Lang;
            m_password = Password;
            m_users = new Hashtable();
            al_file = Utils.GetPath("aliases",this);
            acc_file = Utils.GetPath("local_accesses", this);
            vl_dir = Utils.GetPath("local_viplang", this);
            va_dir = Utils.GetPath("local_vipaccess", this);
            @out.exe("options_hnd_starting");
            m_dir = Utils.GetPath("mucdata", this);
            @out.exe("options_hnd_fnished");
            m_aliases = new Document();
            if (!Directory.Exists(m_dir))
                m_aliases.LoadFile(al_file);
            Directory.CreateDirectory(m_dir);
 
            if (File.Exists(al_file))
                m_aliases.LoadFile(al_file);
            else
            {
                m_aliases.LoadXml("<Aliases></Aliases>");
                m_aliases.Save(al_file);
            }
            int sqlv = int.Parse(sh.S.Config.GetTag("sqlite"));
            opth = new OptionsHandler(Utils.GetPath("local_options", this));
            la = new LocalAccess(acc_file);
            VipLang = new VipLang(vl_dir, sqlv);
            VipAccess = new VipAccess(va_dir, sqlv);
            m_defs = new Dictionary(Utils.GetPath("def", this), sqlv);
            m_manager = new MucManager(m_con);
            this._whowas = new StringBuilder();


        }



    }
}

