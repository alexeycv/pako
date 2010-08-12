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
using System.Text.RegularExpressions;
using Mono.Data.SqliteClient;
using System.Threading;
using System.IO;
using agsXMPP;
using Core.Kernel;
using Core.Other;



namespace Core.DataBase
{

    public enum AutoKickType
    {
        AKICK_JID_REGEXP,
        AKICK_NICK_REGEXP,
        AKICK_JID
    }


    public class Tempdb
    {
        object[] sobjs = new object[30];
        SqliteConnection sqlite_conn;
        string db_file;
        int ver;
        string m_dir;
        SessionHandler Sh;

        public SqliteConnection SQLiteConnection
        {
            get { lock (sobjs[4]) { return sqlite_conn; } }
            set { lock (sobjs[4]) { sqlite_conn = value; } }
        }

        public void LoadBase()
        {
            bool to_create = !File.Exists(db_file);
            m_dir = db_file;
            sqlite_conn = new SqliteConnection("URI=file:" + db_file.Replace("\\", "/") + ",version=" + ver.ToString());
            sqlite_conn.Open();
            if (to_create)
            {
                SqliteCommand cmd = new SqliteCommand(@"
                    CREATE 
                          TABLE  tell (jid varchar, phrase varchar, author varchar, date varchar);
                                      ", sqlite_conn);

                cmd.ExecuteNonQuery();

                cmd = new SqliteCommand(@"
                    CREATE 
                          TABLE  greet (jid varchar, room varchar, phrase varchar);
                                      ", sqlite_conn);
                cmd.ExecuteNonQuery();

                cmd = new SqliteCommand(@"
                    CREATE 
                          TABLE  automoderator (jid varchar, room varchar, updated bigint, period bigint);
                                      ", sqlite_conn);
                cmd.ExecuteNonQuery();

                cmd = new SqliteCommand(@"
                    CREATE 
                          TABLE  autokick (determiner varchar, value varchar, updated bigint, period bigint, reason varchar, room varchar);
                                      ", sqlite_conn);
                cmd.ExecuteNonQuery();
              
                cmd = new SqliteCommand(@"
                    CREATE 
                          TABLE  autovisitor (determiner varchar, value varchar, updated bigint, period bigint, reason varchar, room varchar);
                                      ", sqlite_conn);
                cmd.ExecuteNonQuery();

            }
            else // Check wheter that tables exists and recreate if not
            {
                SqliteCommand cmd = new SqliteCommand(@"
                    CREATE 
                          TABLE IF NOT EXISTS tell (jid varchar, phrase varchar, author varchar, date varchar);
                                      ", sqlite_conn);

                cmd.ExecuteNonQuery();

                cmd = new SqliteCommand(@"
                    CREATE 
                          TABLE IF NOT EXISTS greet (jid varchar, room varchar, phrase varchar);
                                      ", sqlite_conn);
                cmd.ExecuteNonQuery();

                cmd = new SqliteCommand(@"
                    CREATE 
                          TABLE IF NOT EXISTS automoderator (jid varchar, room varchar, updated bigint, period bigint);
                                      ", sqlite_conn);
                cmd.ExecuteNonQuery();

                cmd = new SqliteCommand(@"
                    CREATE 
                          TABLE IF NOT EXISTS autokick (determiner varchar, value varchar, updated bigint, period bigint, reason varchar, room varchar);
                                      ", sqlite_conn);
                cmd.ExecuteNonQuery();
              
                cmd = new SqliteCommand(@"
                    CREATE 
                          TABLE IF NOT EXISTS autovisitor (determiner varchar, value varchar, updated bigint, period bigint, reason varchar, room varchar);
                                      ", sqlite_conn);
                cmd.ExecuteNonQuery();
            }

        }

        public Tempdb(string DBfile, int version)
        {
            for (int i = 0; i < 30; i++)
            {
                sobjs[i] = new object();
            }
            ver = version;
            db_file = DBfile;

            this.Sh = Core.Other.Utils.Sh;

            LoadBase();
        }




        public ArrayList CheckAndAnswer(Jid Jid)
        {
            lock (sobjs[21])
            {
                string nick;
                ArrayList al = new ArrayList();
                SqliteCommand cmd;
                SqliteDataReader sqr;


                nick = Jid.ToString().Replace("'", "''");
                cmd = new SqliteCommand(String.Format(@"
                        SELECT *   
                               FROM tell 
                               WHERE (jid = '{0}')
                               ORDER BY phrase DESC
                                      ",
                                           nick), sqlite_conn);
                //cmd.ExecuteNonQuery();

                try
                {
                    sqr = cmd.ExecuteReader();
                }
                catch (SqliteSyntaxException ex)
                {
                    string clrf = "\n";
                    string _sqlText = cmd.CommandText;

                    if (Core.Other.Utils.OS == Platform.Windows)
                    {
                        clrf = "\r\n";
                    }
                    string data = "====== [" + DateTime.Now.ToString() + "] ===================================================>" + clrf + _sqlText;
                    Sh.S.ErrorLoger.Write(data);

                    return al;
                }

                while (sqr.Read())
                {
                    al.Add(new string[] { sqr.GetString(0), sqr.GetString(1), sqr.GetString(2), sqr.GetString(3) });
                }

                cmd = new SqliteCommand(String.Format(@"
                        DELETE    
                               FROM tell 
                               WHERE (jid = '{0}')
                                      ",
                                        nick), sqlite_conn);
                cmd.ExecuteNonQuery();

                return al;
            }
        }



        public bool AutoModerator(Jid Jid, Jid Room)
        {
            lock (sobjs[22])
            {
                SqliteCommand cmd;
                SqliteDataReader sqr;
                string jid = Jid.Bare.ToLower().Replace("'", "''");
                string room = Room.Bare.ToLower().Replace("'", "''");
                cmd = new SqliteCommand(String.Format(@"
                        SELECT *   
                               FROM automoderator 
                               WHERE (jid = '{0}' and room = '{1}')
                               ORDER BY jid DESC
                                      ",
                                           jid, room), sqlite_conn);
                //cmd.ExecuteNonQuery();
                try
                {
                    sqr = cmd.ExecuteReader();
                }
                catch (SqliteSyntaxException ex)
                {
                    string clrf = "\n";
                    string _sqlText = cmd.CommandText;

                    if (Core.Other.Utils.OS == Platform.Windows)
                    {
                        clrf = "\r\n";
                    }
                    string data = "====== [" + DateTime.Now.ToString() + "] ===================================================>" + clrf + _sqlText;
                    Sh.S.ErrorLoger.Write(data);

                    return false;
                }

                if (sqr.Read())
                {
                    long period = sqr.GetInt64(3);
                    long updated = sqr.GetInt64(2);
                    if (period == 0)
                        return true;
                    if (DateTime.Now.Ticks < updated + period)
                    {
                        return true;
                    }
                    else
                    {
                        cmd = new SqliteCommand(String.Format(@"
                                     DELETE    
                                            FROM automoderator 
                                            WHERE (jid = '{0}' and room ='{1}')
                                      ",
                                  jid, room), sqlite_conn);
                        cmd.ExecuteNonQuery();
                    }
                }
                return false;
            }
        }




        public bool GreetExists(Jid Jid, Jid Room)
        {
            lock (sobjs[20])
            {
                string jid = Jid.Bare.ToLower().Replace("'", "''");
                string room = Room.Bare.ToLower().Replace("'", "''");
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                     SELECT *   
                               FROM greet 
                               WHERE (jid = '{0}' and room = '{1}')
                                      ",
                                            jid, room), sqlite_conn);
                //cmd.ExecuteNonQuery();
                SqliteDataReader sqr = cmd.ExecuteReader();
                return sqr.Read();

            }
        }


        public bool AddAutoKick(string Value, Jid Room, string Type, string Reason, long Period)
        {
            lock (sobjs[19])
            {
                string type = Type.Replace("'", "''");
                string value = Value.Replace("'", "''");
                string reason = Reason.Replace("'", "''");
                string room = Room.Bare.Replace("'", "''");
                if (AutoKickExists(Value, Room, Type, Reason))
                    return false;
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                      INSERT 
                           INTO autokick (determiner, value, updated, period, reason, room) 
                           VALUES ('{0}', '{1}', {2}, {3}, '{4}', '{5}');
                                      ",
                                            type, value, DateTime.Now.Ticks.ToString(), Period.ToString(), reason, room), sqlite_conn);
                cmd.ExecuteNonQuery();
                return true;

            }
        }

        public bool AddAutoVisitor(string Value, Jid Room, string Type, string Reason, long Period)
        {
            lock (sobjs[18])
            {
                string type = Type.Replace("'", "''");
                string value = Value.Replace("'", "''");
                string reason = Reason.Replace("'", "''");
                string room = Room.Bare.Replace("'", "''");
                if (AutoVisitorExists(Value, Room, Type, Reason))
                    return false;
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                      INSERT 
                           INTO autovisitor (determiner, value, updated, period, reason, room) 
                           VALUES ('{0}', '{1}', {2}, {3}, '{4}', '{5}');
                                      ",
                                            type, value, DateTime.Now.Ticks.ToString(), Period.ToString(), reason, room), sqlite_conn);
                cmd.ExecuteNonQuery();
                return true;

            }
        }


        public bool AutoKickExists(string Value, Jid Room, string Type, string Reason)
        {
            lock (sobjs[17])
            {
                string type = Type.Replace("'", "''");
                string value = Value.Replace("'", "''");
                string reason = Reason.Replace("'", "''");
                string room = Room.Bare.Replace("'", "''");
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                      SELECT * 
                           FROM autokick 
                           WHERE (determiner = '{0}' and value = '{1}' and reason = '{2}' and room = '{3}')
                                      ",
                                            type, value, reason, room), sqlite_conn);
//                cmd.ExecuteNonQuery();
                SqliteDataReader sqr = cmd.ExecuteReader();
                return sqr.Read();

            }
        }



        public bool AutoVisitorExists(string Value, Jid Room, string Type, string Reason)
        {
            lock (sobjs[16])
            {
                string type = Type.Replace("'", "''");
                string value = Value.Replace("'", "''");
                string reason = Reason.Replace("'", "''");
                string room = Room.Bare.Replace("'", "''");
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                      SELECT * 
                           FROM autovisitor
                           WHERE (determiner = '{0}' and value = '{1}' and reason = '{2}' and room = '{3}')
                                      ",
                                            type, value, reason, room), sqlite_conn);
//                cmd.ExecuteNonQuery();
                SqliteDataReader sqr = cmd.ExecuteReader();
                return sqr.Read();

            }
        }

        public void ClearAutoKick(Jid Room)
        {
            lock (sobjs[15])
            {
                string room = Room.Bare.Replace("'", "''");
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                                     DELETE    
                                          FROM autokick 
                                          WHERE (room = '{0}')
                                      ", room),
                                          sqlite_conn);
                cmd.ExecuteNonQuery();


            }
        }
        public void ClearGreet(Jid Room)
        {
            lock (sobjs[14])
            {
                string room = Room.Bare.Replace("'", "''");
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                                     DELETE    
                                          FROM greet 
                                          WHERE (room = '{0}')
                                      ", room),
                                          sqlite_conn);
                cmd.ExecuteNonQuery();


            }
        }
        public void ClearAutoVisitor(Jid Room)
        {
            lock (sobjs[13])
            {
                string room = Room.Bare.Replace("'", "''");
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                                     DELETE    
                                          FROM autovisitor
                                          WHERE (room = '{0}')
                                      ", room),
                                          sqlite_conn);
                cmd.ExecuteNonQuery();


            }
        }

        public void ClearAutoModerator(Jid Room)
        {
            lock (sobjs[25])
            {
                string room = Room.Bare.Replace("'", "''");
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                                     DELETE    
                                          FROM automoderator
                                          WHERE (room = '{0}')
                                      ", room),
                                          sqlite_conn);
                cmd.ExecuteNonQuery();


            }
        }



        public void CleanAutoVisitor(Jid Room)
        {
            lock (sqlite_conn)
            {
                SqliteCommand cmd;
                SqliteDataReader sqr;
                string room = Room.Bare.Replace("'", "''");
                cmd = new SqliteCommand(String.Format(@"
                        SELECT *   
                               FROM autovisitor
                               WHERE (room = '{0}')
                                      ",
                                         room), sqlite_conn);
//                cmd.ExecuteNonQuery();
                sqr = cmd.ExecuteReader();

                while (sqr.Read())
                {
                    long period = sqr.GetInt64(3);
                    long updated = sqr.GetInt64(2);
                    string jid = sqr.GetString(1).Replace("'", "''");
                    if (period == 0)
                        continue;
                    if (DateTime.Now.Ticks < updated + period)
                    {
                        continue;
                    }
                    else
                    {
                        cmd = new SqliteCommand(String.Format(@"
                                     DELETE    
                                            FROM autovisitor 
                                            WHERE (value = '{0}' and updated = {1} and period = {2} and room ='{3}')
                                      ",
                                  jid, updated.ToString(), period.ToString(), room), sqlite_conn);
                        cmd.ExecuteNonQuery();
                    }
                }


            }
        }



        public void CleanAutoKick(Jid Room)
        {
            lock (sqlite_conn)
            {
                SqliteCommand cmd;
                SqliteDataReader sqr;
                string room = Room.Bare.Replace("'", "''");
                cmd = new SqliteCommand(String.Format(@"
                        SELECT *   
                               FROM autokick
                               WHERE (room = '{0}')
                                      ",
                                         room), sqlite_conn);
//                cmd.ExecuteNonQuery();
                sqr = cmd.ExecuteReader();

                while (sqr.Read())
                {
                    long period = sqr.GetInt64(3);
                    long updated = sqr.GetInt64(2);
                    string jid = sqr.GetString(1).Replace("'", "''");
                    if (period == 0)
                        continue;
                    if (DateTime.Now.Ticks < updated + period)
                    {
                        continue;
                    }
                    else
                    {
                        cmd = new SqliteCommand(String.Format(@"
                                     DELETE    
                                            FROM autokick 
                                            WHERE (value = '{0}' and updated = {1} and period = {2} and room ='{3}')
                                      ",
                                   jid, updated.ToString(), period.ToString(), room), sqlite_conn);
                        cmd.ExecuteNonQuery();
                    }
                }


            }
        }

        public void CleanAutoModerator(Jid Room)
        {
            lock (sqlite_conn)
            {
                    SqliteCommand cmd;
                    SqliteDataReader sqr;
                    string room = Room.Bare.Replace("'", "''");
                    cmd = new SqliteCommand(String.Format(@"
                        SELECT *   
                               FROM automoderator 
                               WHERE (room = '{0}')
                                      ",
                                             room), sqlite_conn);
//                    cmd.ExecuteNonQuery();
                    sqr = cmd.ExecuteReader();

                    while (sqr.Read())
                    {
                        long period = sqr.GetInt64(3);
                        long updated = sqr.GetInt64(2);
                        string jid = sqr.GetString(0).Replace("'", "''");
                        if (period == 0)
                            continue;
                        if (DateTime.Now.Ticks < updated + period)
                        {
                            continue;
                        }
                        else
                        {
                            cmd = new SqliteCommand(String.Format(@"
                                     DELETE    
                                            FROM automoderator 
                                            WHERE (jid = '{0}' and room ='{1}')
                                      ",
                                      jid, room), sqlite_conn);
                            cmd.ExecuteNonQuery();
                        }
                    }


            }
        }




        public bool DelAutoKick(Jid Room, int num)
        {
            lock (sobjs[12])
            {
                string room = Room.Bare.ToLower().Replace("'", "''");
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                      SELECT * 
                           FROM autokick 
                           WHERE (room = '{0}')
                                      ",
                                       room), sqlite_conn);
//                cmd.ExecuteNonQuery();
                SqliteDataReader sqr = cmd.ExecuteReader();
                int i = 0;
                while (sqr.Read())
                {
                    i++;
                    if (i == num)
                    {
                        string determiner = sqr.GetString(0);
                        string value = sqr.GetString(1);
                        long updated = sqr.GetInt64(2);
                        long period = sqr.GetInt64(3);
                        string reason = sqr.GetString(4);
                        cmd = new SqliteCommand(String.Format(@"
                                     DELETE    
                                            FROM autokick 
                                            WHERE (determiner = '{0}' and value = '{1}' and updated = {2} and period = {3} and reason = '{4}' and room ='{5}')
                                      ",
                                               determiner.Replace("'", "''"), value.Replace("'", "''"), updated.ToString(), period.ToString(), reason.Replace("'", "''"), room.Replace("'", "''")), sqlite_conn);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
                return false;
          

            }
        }
        public string GetAutoKickList(Jid Room, string pattern, Response r)
        {
            lock (sobjs[9])
            {
                string room = Room.Bare.ToLower().Replace("'", "''");
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                      SELECT * 
                           FROM autokick 
                           WHERE (room = '{0}')
                                      ",
                                       room), sqlite_conn);
//                cmd.ExecuteNonQuery();
                SqliteDataReader sqr = cmd.ExecuteReader();
                string data = "";
                int num = 0;
                while (sqr.Read())
                {
                   num++;
                   data += pattern
                       .Replace("{1}", num.ToString())
                       .Replace("{2}", sqr.GetString(0)
                                       .Replace("AKICK_JID_REGEXP", "jid-exp")
                                       .Replace("AKICK_JID", "jid")
                                       .Replace("AKICK_NICK_REGEXP", "nick-exp")
                                       .Replace("AKICK_NICK", "nick"))
                       .Replace("{3}", sqr.GetString(1))
                       .Replace("{4}", sqr.GetInt64(3) == 0 ? "&" : sqr.GetInt64(3) + sqr.GetInt64(2) - DateTime.Now.Ticks < 0 ? "0" : Utils.FormatTimeSpan(sqr.GetInt64(3) + sqr.GetInt64(2) - DateTime.Now.Ticks, r))
                       .Replace("{5}", sqr.GetString(4)) + "\n";
                }
                return data == "" ? null : r.f("akick_list")+"\n"+ data.Trim('\n');

            }
        }

        public string IsAutoKick(Jid Jid, string Nick, Jid Room, SessionHandler sh)
        {
            lock (sobjs[7])
            {
                @out.exe("akick_regex_starting");
                string jid = Jid.ToString().ToLower().Replace("'", "''").ToLower();
                string nick = Nick.Replace("'", "''").ToLower();
                string room = Room.Bare.Replace("'", "''");
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                     SELECT *   
                               FROM autokick 
                               WHERE (room = '{0}')
                               ORDER BY updated DESC
                                      ",
                                            room), sqlite_conn);
//                cmd.ExecuteNonQuery();
                SqliteDataReader sqr = cmd.ExecuteReader();
                @out.exe("akick_sqlite_executed");


                while (sqr.Read())
                {
                    string determiner = sqr.GetString(0);
                    string value = sqr.GetString(1).ToLower();
                    long updated = sqr.GetInt64(2);
                    long period = sqr.GetInt64(3);
                    string reason = sqr.GetString(4);
                    @out.exe("akick_type_determined");
                    switch (determiner)
                    {
                        case "AKICK_JID":
                            @out.exe("akick_type: JID");
                            if (Jid.Bare == value)
                            {
                                if (period == 0)
                                    return reason;
                                if (DateTime.Now.Ticks < updated + period)
                                {
                                    return reason;
                                }
                                else
                                {

                                    cmd = new SqliteCommand(String.Format(@"
                                     DELETE    
                                            FROM autokick 
                                            WHERE (determiner = '{0}' and value = '{1}' and updated = {2} and period = {3} and reason = '{4}' and room ='{5}')
                                      ",
                                              determiner.Replace("'", "''"), value.Replace("'", "''"), updated.ToString(), period.ToString(), reason.Replace("'", "''"), room.Replace("'", "''")), sqlite_conn);
                                    cmd.ExecuteNonQuery();


                                }
                            }
                            else
                            {
                                break;
                            }
                            break;

                        case "AKICK_JID_REGEXP":
                            @out.exe("akick_type: JID_REGEXP");
                            bool reged = false;
                            try
                            {
                                Regex regex = new Regex(value);
                                reged = regex.IsMatch(Jid.ToString().ToLower());
                            }
                            catch { }
                            if (reged)
                            {
                                if (period == 0)
                                    return reason;
                                if (DateTime.Now.Ticks < updated + period)
                                    return reason;
                                else
                                {
                                    cmd = new SqliteCommand(String.Format(@"
                                     DELETE    
                                            FROM autokick 
                                            WHERE (determiner = '{0}' and value = '{1}' and updated = '{2}' and period = '{3}' and reason = '{4}' and room ='{5}')
                                      ",
                                           determiner.Replace("'", "''"), value.Replace("'", "''"), updated.ToString(), period.ToString(), reason.Replace("'", "''"), room), sqlite_conn);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            else
                                break;
                            break;

                        case "AKICK_NICK_REGEXP":
                            @out.exe("akick_type: NICK_REGEXP");
                            bool reged1 = false;
                            try
                            {
                                Regex _regex = new Regex(value);
                                reged1 = _regex.IsMatch(Nick.ToLower());
                                @out.exe("akick_regex_success");
                            }
                            catch { @out.exe("akick_regex_fail"); }
                            if (reged1)
                            {
                                if (period == 0)
                                    return reason;
                                if (DateTime.Now.Ticks < updated + period)
                                    return reason;
                                else
                                {
                                    cmd = new SqliteCommand(String.Format(@"
                                     DELETE    
                                            FROM autokick 
                                            WHERE (determiner = '{0}' and value = '{1}' and updated = '{2}' and period = '{3}' and reason = '{4}' and room ='{5}')
                                      ",
                                           determiner.Replace("'", "''"), value.Replace("'", "''"), updated.ToString(), period.ToString(), reason.Replace("'", "''"), room), sqlite_conn);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            else
                                break;
                            break;

                        case "AKICK_NICK":
                            @out.exe("akick_type: NICK");
                            if (Nick == value)
                            {
                                if (period == 0)
                                    return reason;
                                if (DateTime.Now.Ticks < updated + period)
                                {
                                    return reason;
                                }
                                else
                                {
                                    cmd = new SqliteCommand(String.Format(@"
                                     DELETE    
                                            FROM autokick 
                                            WHERE (determiner = '{0}' and value = '{1}' and updated = {2} and period = {3} and reason = '{4}' and room ='{5}')
                                      ",
                                           determiner.Replace("'", "''"), value.Replace("'", "''"), updated.ToString(), period.ToString(), reason.Replace("'", "''"), room.Replace("'", "''")), sqlite_conn);
                                    cmd.ExecuteNonQuery();


                                }
                            }
                            else
                            {

                                break;
                            }
                            break;


                    }


                }
                return null;

            }
        }



        public bool DelAutoVisitor(Jid Room, int num)
        {
            lock (sobjs[10])
            {
                string room = Room.Bare.Replace("'", "''");
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                      SELECT * 
                           FROM autovisitor 
                           WHERE (room = '{0}')
                                      ",
                                       room), sqlite_conn);
//                cmd.ExecuteNonQuery();
                SqliteDataReader sqr = cmd.ExecuteReader();
                int i = 0;
                while (sqr.Read())
                {
                    i++;
                    if (i == num)
                    {
                        string determiner = sqr.GetString(0);
                        string value = sqr.GetString(1);
                        long updated = sqr.GetInt64(2);
                        long period = sqr.GetInt64(3);
                        string reason = sqr.GetString(4);
                        cmd = new SqliteCommand(String.Format(@"
                                     DELETE    
                                            FROM autovisitor 
                                            WHERE (determiner = '{0}' and value = '{1}' and updated = {2} and period = {3} and reason = '{4}' and room ='{5}')
                                      ",
                                               determiner.Replace("'", "''"), value.Replace("'", "''"), updated.ToString(), period.ToString(), reason.Replace("'", "''"), room.Replace("'", "''")), sqlite_conn);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
                return false;


            }
        }

        public string GetAutoVisitorList(Jid Room, string pattern, Response r)
        {
            lock (sobjs[9])
            {
                string room = Room.Bare.Replace("'", "''");
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                      SELECT * 
                           FROM autovisitor 
                           WHERE (room = '{0}')
                                      ",
                                       room), sqlite_conn);
//                cmd.ExecuteNonQuery();
                SqliteDataReader sqr = cmd.ExecuteReader();
                string data = "";
                int num = 0;
                while (sqr.Read())
                {
                    num++;
                    data += pattern
                        .Replace("{1}", num.ToString())
                        .Replace("{2}", sqr.GetString(0)
                                        .Replace("AKICK_JID_REGEXP", "jid-exp")
                                        .Replace("AKICK_JID", "jid")
                                        .Replace("AKICK_NICK_REGEXP", "nick-exp")
                                        .Replace("AKICK_NICK", "nick"))
                        .Replace("{3}", sqr.GetString(1))
                        .Replace("{4}", sqr.GetInt64(3) == 0 ? "&" : sqr.GetInt64(3) + sqr.GetInt64(2) - DateTime.Now.Ticks < 0 ? "0" : Utils.FormatTimeSpan(sqr.GetInt64(3) + sqr.GetInt64(2) - DateTime.Now.Ticks, r))
                        .Replace("{5}", sqr.GetString(4)) + "\n";
                }
                return data == "" ? null : r.f("avisitor_list") + "\n" + data.Trim('\n');

            }
        }


        public string IsAutoVisitor(Jid Jid, string Nick, Jid Room, SessionHandler sh)
        {
            lock (sobjs[6])
            {
                string jid = Jid.ToString().ToLower().Replace("'", "''").ToLower();
                string nick = Nick.Replace("'", "''").ToLower();
                string room = Room.Bare.Replace("'", "''");
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                     SELECT *   
                               FROM autovisitor
                               WHERE (room = '{0}')
                               ORDER BY updated DESC
                                      ",
                                            room), sqlite_conn);
//                cmd.ExecuteNonQuery();
                SqliteDataReader sqr = cmd.ExecuteReader();

                while (sqr.Read())
                {
                    string determiner = sqr.GetString(0);
                    string value = sqr.GetString(1).ToLower();
                    long updated = sqr.GetInt64(2);
                    long period = sqr.GetInt64(3);
                    string reason = sqr.GetString(4);
                    switch (determiner)
                    {
                        case "AKICK_JID":
                            if (Jid.Bare == value)
                            {
                                if (period == 0)
                                    return reason;
                                if (DateTime.Now.Ticks < updated + period)
                                {
                                    return reason;
                                }
                                else
                                {


                                    cmd = new SqliteCommand(String.Format(@"
                                     DELETE    
                                            FROM autovisitor 
                                            WHERE (determiner = '{0}' and value = '{1}' and updated = {2} and period = {3} and reason = '{4}' and room ='{5}')
                                      ",
                                           determiner.Replace("'", "''"), value.Replace("'", "''"), updated.ToString(), period.ToString(), reason.Replace("'", "''"), room.Replace("'", "''")), sqlite_conn);
                                    cmd.ExecuteNonQuery();


                                }
                            }
                            else
                            {

                                break;
                            }
                            break;

                        case "AKICK_JID_REGEXP":
                          bool reged1 = false;
                           try{ Regex regex = new Regex(value);
                                reged1 = regex.IsMatch(Jid.ToString().ToLower()); } catch{}
                            if (reged1)
                            {
                                if (period == 0)
                                    return reason;
                                if (DateTime.Now.Ticks < updated + period)
                                    return reason;
                                else
                                {
                                    cmd = new SqliteCommand(String.Format(@"
                                     DELETE    
                                            FROM autovisitor 
                                            WHERE (determiner = '{0}' and value = '{1}' and updated = '{2}' and period = '{3}' and reason = '{4}' and room ='{5}')
                                      ",
                                           determiner.Replace("'", "''"), value.Replace("'", "''"), updated.ToString(), period.ToString(), reason.Replace("'", "''"), room), sqlite_conn);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            else
                                break;
                            break;

                        case "AKICK_NICK_REGEXP":
                          bool reged = false;
                           try{ Regex _regex = new Regex(value);
                           reged = _regex.IsMatch(Nick.ToLower());}
                           catch { }
                            if (reged)
                            {
                                if (period == 0)
                                    return reason;
                                if (DateTime.Now.Ticks < updated + period)
                                    return reason;
                                else
                                {
                                    cmd = new SqliteCommand(String.Format(@"
                                     DELETE    
                                            FROM autovisitor 
                                            WHERE (determiner = '{0}' and value = '{1}' and updated = '{2}' and period = '{3}' and reason = '{4}' and room ='{5}')
                                      ",
                                           determiner.Replace("'", "''"), value.Replace("'", "''"), updated.ToString(), period.ToString(), reason.Replace("'", "''"), room), sqlite_conn);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            else
                                break;
                            break;

                        case "AKICK_NICK":
                            if (Nick == value)
                            {
                                if (period == 0)
                                    return reason;
                                if (DateTime.Now.Ticks < updated + period)
                                {
                                    return reason;
                                }
                                else
                                {

                                    cmd = new SqliteCommand(String.Format(@"
                                     DELETE    
                                            FROM autovisitor 
                                            WHERE (determiner = '{0}' and value = '{1}' and updated = {2} and period = {3} and reason = '{4}' and room ='{5}')
                                      ",
                                           determiner.Replace("'", "''"), value.Replace("'", "''"), updated.ToString(), period.ToString(), reason.Replace("'", "''"), room.Replace("'", "''")), sqlite_conn);
                                    cmd.ExecuteNonQuery();


                                }
                            }
                            else
                            {

                                break;
                            }
                            break;
                    }
                }

                return null;

            }
        }


        public bool AddGreet(Jid Jid, Jid Room, string phrase)
        {
            lock (sobjs[5])
            {
                string jid = Jid.Bare.ToLower().Replace("'", "''");
                string room = Room.Bare.Replace("'", "''");
                phrase = phrase.Replace("'", "''");
                if (Greet(Jid, Room) != null)
                    return false;
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                   INSERT 
                         INTO greet (jid, room, phrase) 
                         VALUES ('{0}', '{1}', '{2}');
                                      ",
                                            jid, room, phrase), sqlite_conn);
                cmd.ExecuteNonQuery();
                return true;

            }
        }

        public bool AddAutoModerator(Jid Jid, Jid Room, long Period)
        {
            lock (sobjs[5])
            {
                string jid = Jid.Bare.ToLower().Replace("'", "''");
                string room = Room.Bare.Replace("'", "''");
                if (AutoModerator(Jid, Room) == true)
                    return false;
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                   INSERT 
                         INTO automoderator (jid, room, updated, period) 
                         VALUES ('{0}', '{1}', {2}, {3});
                                      ",
                                            jid, room, DateTime.Now.Ticks.ToString(), Period.ToString()), sqlite_conn);
                cmd.ExecuteNonQuery();
                return true;

            }
        }


        public bool DelGreet(Jid Jid, Jid Room)
        {
            lock (sqlite_conn)
            {
                if (Greet(Jid, Room) == null)
                    return false;
                string jid = Jid.Bare.ToLower().Replace("'", "''");
                string room = Room.Bare.Replace("'", "''");

                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                   DELETE 
                         FROM greet 
                         WHERE (jid = '{0}' and room = '{1}')
                                      ",
                                            jid, room), sqlite_conn);
                cmd.ExecuteNonQuery();
                return true;

            }
        }
  
 
        public bool DelGreet(Jid Room, int num)
        {
            lock (sqlite_conn)
            { 
                string room = Room.Bare.Replace("'", "''");
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                        SELECT *   
                               FROM greet 
                               WHERE (room = '{0}')
                               ORDER BY phrase DESC
                                      ",
                                        room), sqlite_conn);
//                cmd.ExecuteNonQuery();
                SqliteDataReader sqr = cmd.ExecuteReader();
                string phrase = "";
                Jid Jid = new Jid("");
                int i = 0;
                while (sqr.Read())
                {
                    i++;
                    if (i == num)
                    {
                        phrase = sqr.GetString(2);
                        Jid = new Jid(sqr.GetString(0));
                    }
                }
               
               if (phrase == "")
                   return false;
                cmd = new SqliteCommand(String.Format(@"
                   DELETE 
                         FROM greet 
                         WHERE (jid ='{0}' and room = '{1}')
                                      ",
                                           Jid.Bare.Replace("'","''"),  room), sqlite_conn);
                cmd.ExecuteNonQuery();
                return true;

            }
        }


        public bool DelAutoModerator(Jid Room, int num)
        {
            lock (sqlite_conn)
            {
                string room = Room.Bare.Replace("'", "''");
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                        SELECT *   
                               FROM automoderator 
                               WHERE (room = '{0}')
                               ORDER BY jid DESC
                                      ",
                                        room), sqlite_conn);
//                cmd.ExecuteNonQuery();
                SqliteDataReader sqr = cmd.ExecuteReader();
                string jid = "";
                int i = 0;
                while (sqr.Read())
                {
                    i++;
                    if (i == num)
                    {
                        jid = sqr.GetString(0);
                    }
                }

                if (jid == "")
                    return false;
                cmd = new SqliteCommand(String.Format(@"
                   DELETE 
                         FROM automoderator 
                         WHERE (jid ='{0}' and room = '{1}')
                                      ",
                                           jid.Replace("'", "''"), room), sqlite_conn);
                cmd.ExecuteNonQuery();
                return true;

            }
        }


        public string Greet(Jid Jid, Jid Room)
        {
            lock (sobjs[2])
            {
                string jid = Jid.Bare.ToLower().Replace("'", "''");
                string room = Room.Bare.Replace("'", "''");

                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                        SELECT *   
                               FROM greet 
                               WHERE (jid = '{0}' and room = '{1}')
                               ORDER BY phrase DESC
                               LIMIT 1
                                      ",
                                            jid, room), sqlite_conn);
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SqliteSyntaxException ex)
                {
                    string clrf = "\n";
                    string _sqlText = cmd.CommandText;

                    if (Core.Other.Utils.OS == Platform.Windows)
                    {
                        clrf = "\r\n";
                    }
                    string data = "====== [" + DateTime.Now.ToString() + "] ===================================================>" + clrf + _sqlText;
                    Sh.S.ErrorLoger.Write(data);

                    return null;
                }

                SqliteDataReader sqr = cmd.ExecuteReader();
                if (sqr.Read())
                {
                    return sqr.GetString(2);
                }
                return null;

            }
        }

        public string GetAutoModeratorList(Jid Room, string pattern, Response r)
        {
            lock (sobjs[24])
            {
                string room = Room.Bare.Replace("'", "''");

                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                        SELECT *   
                               FROM automoderator 
                               WHERE (room = '{0}')
                               ORDER BY jid DESC
                                      ",
                                            room), sqlite_conn);
//                cmd.ExecuteNonQuery();
                SqliteDataReader sqr = cmd.ExecuteReader();
                int i = 0;
                string data = "";
                while (sqr.Read())
                {
                    i++;
                    data += pattern.Replace("{1}", i.ToString()).Replace("{2}", sqr.GetInt64(3) == 0 ? "" : sqr.GetInt64(3) + sqr.GetInt64(2) - DateTime.Now.Ticks < 0 ? "0" : Utils.FormatTimeSpan(sqr.GetInt64(3) + sqr.GetInt64(2) - DateTime.Now.Ticks, r)).Replace("{3}", sqr.GetString(0)) + "\n";
                }
                return data != "" ? r.f("moderator_list") + "\n" + data.Trim('\n') : null;

            }
        }


        public string GetGreetList(Jid Room, string pattern)
        {
            lock (sobjs[4])
            {
                string room = Room.Bare.Replace("'", "''");

                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                        SELECT *   
                               FROM greet 
                               WHERE (room = '{0}')
                               ORDER BY phrase DESC
                                      ",
                                            room), sqlite_conn);
//                cmd.ExecuteNonQuery();
                SqliteDataReader sqr = cmd.ExecuteReader();
                int i = 0;
                string data = "";
                while (sqr.Read())
                {
                    i++;
                    data += pattern.Replace("{1}", i.ToString()).Replace("{2}", sqr.GetString(0)).Replace("{3}", sqr.GetString(2))+"\n";
                }
                return data != "" ? "\n"+data.Trim('\n') : null;

            }
        }


        public ArrayList GetActiveTransactions(Jid Jid)
        {
            lock (sobjs[5])
            {
                string nick;
                ArrayList al = new ArrayList();
                SqliteCommand cmd;
                SqliteDataReader sqr;
                nick = Jid.ToString().Replace("'", "''");
                cmd = new SqliteCommand(String.Format(@"
                        SELECT *   
                               FROM tell 
                               WHERE (author = '{0}')
                               ORDER BY phrase DESC
                                      ",
                                           nick), sqlite_conn);
//                cmd.ExecuteNonQuery();
                sqr = cmd.ExecuteReader();

                while (sqr.Read())
                {
                    al.Add(new string[] { sqr.GetString(0), sqr.GetString(1), sqr.GetString(2) });
                }
                return al;
            }
        }

        public bool AddTell(Jid Jid, string phrase, Jid author)
        {
            lock (sobjs[3])
            {
                if (GetActiveTransactions(author).Count < 6)
                {
                    string jid = Jid.ToString().Replace("'", "''");
                    phrase = phrase.Replace("'", "''");
                    string auth = author.ToString().Replace("'", "''");
                    SqliteCommand cmd = new SqliteCommand(String.Format(@"
                    INSERT 
                         INTO tell (jid, phrase, author, date) 
                         VALUES ('{0}', '{1}', '{2}', '{3}');
                                                         ",
                                                               jid,
                                                               phrase,
                                                               auth,
                                                               DateTime.Now.ToString()
                                                               ), sqlite_conn);

                    cmd.ExecuteNonQuery();
                    return true;
                }
                return false;
            }
        }


    }
}
