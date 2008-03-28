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

namespace Core.Conference
{


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
        SqliteConnection sqlite_conn;
        string acc_file;
        string al_file;
        LocalAccess la;
        Dictionary m_defs;
        MucManager m_manager;
        VipAccess va;
        VipLang vl;
        ShowType m_show;
        string m_subject;
        static object[] sobjs = new object[47];
        
        XmppClientConnection m_con;

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

        public string Subject
        {
            get { lock (sobjs[13]) { return m_subject; } }
            set { lock (sobjs[13]) { m_subject = value; } }
        }

        public LocalAccess AccessManager
        {
            get { lock (sobjs[42]) { return la; } }
            set { lock (sobjs[42]) { la = value; } }
        }
        public VipAccess VipAccess
        {
            get { lock (sobjs[45]) { return va; } }
            set { lock (sobjs[45]) { va = value; } }
        }
        public VipLang VipLang
        {
            get { lock (sobjs[46]) { return vl; } }
            set { lock (sobjs[46]) { vl = value; } }
        }
        public Dictionary Dictionary
        {
            get { lock (sobjs[35]) {
                @out.exe("_____-near defs_hnd");
            return m_defs; } }
            set { lock (sobjs[35]) { m_defs = value; } }
        }

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
        public Hashtable Users
        {
            get { return m_users; }
        }

        public string Name
        {
            get { return m_name; }
        }

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



        public void Changer(Role role, Jid room, Jid user, string nickname, string reason, IqCB cb, object cbArg)
        {
            AdminIq aIq = new AdminIq();
            aIq.To = room;
            aIq.Type = IqType.set;

            agsXMPP.protocol.x.muc.iq.admin.Item itm = new agsXMPP.protocol.x.muc.iq.admin.Item();
            itm.Role = role;

            if (user != null)
                itm.Jid = user;
            if (nickname != null)
                itm.Nickname = nickname;

            if (reason != null)
                itm.Reason = reason;

            aIq.Query.AddItem(itm);

            if (cb == null)
                Connection.Send(aIq);
            else
                Connection.IqGrabber.SendIq(aIq, cb, cbArg);
        }


        public void Changea(Affiliation affiliation,  Jid room, Jid user, string nickname, string reason, IqCB cb, object cbArg)
        {
            AdminIq aIq = new AdminIq();
            aIq.To = room;
            aIq.Type = IqType.set;

            agsXMPP.protocol.x.muc.iq.admin.Item itm = new agsXMPP.protocol.x.muc.iq.admin.Item();
            itm.Affiliation = affiliation;

            if (user != null)
                itm.Jid = user;
            if (nickname != null)
                itm.Nickname = nickname;

            if (reason != null)
                itm.Reason = reason;

            aIq.Query.AddItem(itm);

            if (cb == null)
                Connection.Send(aIq);
            else
                Connection.IqGrabber.SendIq(aIq, cb, cbArg);
        }


        public void Changear(Affiliation affiliation, Role role, Jid room, Jid user, string nickname, string reason, IqCB cb, object cbArg)
        {
            AdminIq aIq = new AdminIq();
            aIq.To = room;
            aIq.Type = IqType.set;

            agsXMPP.protocol.x.muc.iq.admin.Item itm = new agsXMPP.protocol.x.muc.iq.admin.Item();
            itm.Affiliation = affiliation;

            if (user != null)
                itm.Jid = user;
            if (nickname != null)
                itm.Nickname = nickname;

            if (reason != null)
                itm.Reason = reason;
            aIq.Query.AddItem(itm);

            agsXMPP.protocol.x.muc.iq.admin.Item itm2 = new agsXMPP.protocol.x.muc.iq.admin.Item();
            itm2.Role = role;

            if (user != null)
                itm2.Jid = user;
            if (nickname != null)
                itm2.Nickname = nickname;
            if (reason != null)
                itm2.Reason = reason;

            aIq.Query.AddItem(itm2);

            if (cb == null)
                Connection.Send(aIq);
            else
                Connection.IqGrabber.SendIq(aIq, cb, cbArg);
        }

            public ActionResult Kick(string Nick, string Reason)
        {
            if (UserExists(Nick))
            {
                if (KickableForCensored(GetUser(Nick)))
                {
                    Manager.KickOccupant(Jid, Nick, Reason);
                    return ActionResult.Done;
                }else
                    return ActionResult.NotAble;
            }
            return ActionResult.UserNotFound;        
        }


        public void Ban(string Nick, string Reason)
        {
            if (UserExists(Nick))
            {
                Manager.BanUser(Jid, this.GetUser(Nick).Jid , Reason);
            }
            else
            {
                Manager.BanUser(Jid, new Jid(Nick), Reason);
            }
      
        }

        public bool Voice(string Nick, string Reason)
        {
            if (UserExists(Nick))
            {
                Manager.GrantVoice(Jid, Nick, Reason);
                return true;
            }
            return false;
        }


        public SqliteConnection SQLiteConnection
        {
            get { lock (sobjs[38]) { return sqlite_conn; } }
            set { lock (sobjs[38]) { sqlite_conn = value; } }
        }

        public bool Devoice(string Nick, string Reason)
        {
            if (UserExists(Nick))
            {
                Manager.RevokeVoice(Jid, Nick, Reason);
                return true;
            }
            return false;
        }


        public void ChangeSubject(string NewSubject)
        {    
             Manager.ChangeSubject(Jid, NewSubject);
        }

        public void Leave()
        {
            Manager.LeaveRoom(Jid, MyNick);
        }


        public void Participant(string Nick)
        {
            if (UserExists(Nick))
            {
                Changea(Affiliation.none, Jid, null, Nick, null, null, null);
                Changer( Role.participant, Jid, null, Nick, null, null, null);
            }
            else
            {
                Changea(Affiliation.none, Jid, new Jid(Nick), null, null, null, null);
                Changer(Role.participant, Jid, new Jid(Nick), null, null, null, null);
            }
        }


        public void MemberShip(string Nick)
        {
            if (UserExists(Nick))
            {
                Changer(Role.participant, Jid, null, Nick, null, null, null);
                Changea(Affiliation.member, Jid, null, Nick, null, null, null);
            }
            else
            {
                Changer(Role.participant, Jid, new Jid(Nick), null, null, null, null);
                Changea(Affiliation.member,Jid, new Jid(Nick), null, null, null, null);
            }
        }


        public void Admin(string Nick, string Reason)
        {
            if (UserExists(Nick))
            {
                Manager.GrantAdminPrivileges(Jid, this.GetUser(Nick).Jid);
            }
            else
            {
               Manager.GrantAdminPrivileges(Jid, new Jid(Nick));
            }
        }



        public void Owner(string Nick, string Reason)
        {
            if (UserExists(Nick))
            {
                Manager.GrantOwnershipPrivileges(Jid, this.GetUser(Nick).Jid);
            }
            else
            {
                Manager.GrantOwnershipPrivileges(Jid, new Jid(Nick));
            }
        }


        public bool Moderator(string Nick, string Reason)
        {
            if (UserExists(Nick))
            {
                Changer(Role.moderator, Jid, null, Nick, null, null, null);
                return true;
            }
            return false;
        }

     

        public bool ChangeNick(string Nick)
        {
            if (!UserExists(Nick))
            {
                Manager.ChangeNickname(Jid, Nick);
                return true;
            }
            return false;
        }


        public bool UserExists(string Nick)
        {
            return GetUser(Nick) != null ? true : false;
        }

        public ShowType MyShow
        {
            get
            {
                lock (sobjs[7])
                {
                    return m_show;
                }
            }
            set
            {
                lock (sobjs[7])
                {
                    m_show = value;
                }
            }
        }

        public string MyStatus
        {
            get
            {
                lock (sobjs[2])
                {
                    return m_mystatus;
                }
            }
            set
            {
                lock (sobjs[2])
                {
                    m_mystatus = value;
                }
            }
        }

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

        public string Server
        {
            get { return m_server; }
        }

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

        public string GetAlias(string phrase, ref string from, ref string to)
        {
            lock (sobjs[22])
            {
                foreach (Element el in Aliases.RootElement.SelectElements("alias"))
                {
                    string name = el.GetAttribute("name");
                    string value = el.GetAttribute("value");
                    from = name;

                    if ((phrase.IndexOf(name + " ") == 0) || (phrase == name))
                    {
                        string basic = phrase.Remove(0, name.Length);

                        bool esc = true;
                        int index = 1;

                        while (esc)
                        {
                            string part = "#" + index.ToString();
                            if (value.IndexOf(part) > -1)
                                index++;
                            else
                                break;
                        }
                        index--;
                        if (index == 0)
                            if (value.IndexOf("#~") < 0)
                                value = value.Trim() + " #~";
                        value = value.Replace("#~", basic.TrimStart(' '));
                        if (index > 0)
                        {
                            index = index == 1 ? 2 : index;
                            string[] ws = Utils.SplitEx(basic, index - 1);
                            int _index = 1;
                            foreach (string s in ws)
                            {
                                value = value.Replace("#" + _index.ToString(), s);
                                _index++;
                            }

                            for (int i = 1; i <= index; i++)
                            {
                                value = value.Replace("#" + i.ToString(), "");
                            }
                        }
                        phrase = value.Trim();
                        to = phrase;
                        return phrase;
                    }

                }
                return phrase;
            }
        }

        public bool KickableForCensored(MUser muser)
        {
            MUser me = GetUser(MyNick);
            if (me == null)
            {
                return true;
            }
            if (muser.Access >= 50)
                return false;
            switch (me.Role)
            {
                case Role.moderator:
                    {
                        switch (muser.Affiliation)
                        {
                            case Affiliation.none:
                                return true;
                            case Affiliation.outcast:
                                return true;
                            case Affiliation.member:
                                return true;
                            default:
                                return false;
                        }
                    }
                default:
                    return false;
            }

        }

        public bool HasAlias(string phrase)
        {
          
            lock (Aliases)
            {
               
                    foreach (Element el in Aliases.RootElement.SelectElements("alias"))
                    {
                        string name = el.GetAttribute("name");
                        string value = el.GetAttribute("value");

                        if ((phrase.IndexOf(name + " ") == 0) || (phrase ==  name))
                            return true;
                    }
                    return false;
               
  
            }
        }

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

        public void ClearAliases()
        {
            lock (Aliases)
            {             

                        Aliases.Clear();
                        Aliases.LoadXml("<Aliases></Aliases>");
                        Aliases.Save(al_file);
            }
        }

        public bool DelAlias(string  source)
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


        public bool AddAlias(string alias, string cmd)
        {
            lock (Aliases)
            {
                if (!HasAlias(alias))
                {
                    foreach (Element el in Aliases.RootElement.SelectElements("alias"))
                    {
                        if (((el.GetAttribute("value") == alias) && (el.GetAttribute("name") == cmd)) || (el.GetAttribute("name") == alias))
                            return false;
                    }
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


        public MUser GetUser(string Nick)
        {
            lock (Users)
            {
                MUser mu =  (MUser)Users[Nick];
                return mu;
            }
        }


        public void SetUser(MUser olduser, MUser newuser)
        {
            lock (Users)
            {
                if (olduser != null)
                Users.Remove(olduser.Nick);
                Users.Add(newuser.Nick,newuser);
            }
        }

        public void DelUser(MUser user)
        {
            lock (sobjs[5])
            {
                if (user != null)
                Users.Remove(user.Nick);
            }
        }

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


        public  string IsCensored(string source)
        {

            lock (sobjs[40])
            {
                Regex reg;
                source = source.ToLower().Replace("'", "''");
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                   SELECT  * 
                           FROM censor 
                           WHERE (muc = '{0}') or (muc = '*')
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


        public void Join()
        {
            Presence pres = new Presence();
            pres.To = new Jid(m_jid + "/" + m_mynick);
            pres.Status = m_mystatus;
            pres.Show = MyShow;
            m_con.Send(pres);
        }

        public MUC(XmppClientConnection con, Jid Jid, string Nick, string Status, string Lang, ShowType Show, SqliteConnection sqc)
        {
            for (int i = 0; i < 47; i++)
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
            sqlite_conn = sqc;
            m_lang = Lang;
            m_users = new Hashtable();
            Assembly assem = Assembly.GetExecutingAssembly();
            m_dir = db.b(Path.GetDirectoryName(assem.Location), "Data", m_jid.ToString());
            al_file = db.b(m_dir,"Aliases.xml");
            acc_file = db.b(m_dir, "Access.base");
            vl_dir = db.b(m_dir, "VipLang.db");
            va_dir = db.b(m_dir, "VipAccess.db");
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

            la = new LocalAccess(acc_file);
            VipLang = new VipLang(vl_dir);
            VipAccess = new VipAccess(va_dir);
            m_defs = new Dictionary(db.b(m_dir, "Def.db"));
            m_manager = new MucManager(m_con);
           

        }



    }
}

