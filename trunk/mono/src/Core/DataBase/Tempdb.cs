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
        object[] sobjs = new object[10];
        SqliteConnection sqlite_conn;
        string db_file;
        string m_dir;



        public SqliteConnection SQLiteConnection
        {
            get { lock (sobjs[4]) { return sqlite_conn; } }
            set { lock (sobjs[4]) { sqlite_conn = value; } }
        }

        public void LoadBase()
        {
            bool to_create = !File.Exists(db_file);
            m_dir = db_file;
            sqlite_conn = new SqliteConnection("URI=file:" + db_file.Replace("\\", "/"));
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
                          TABLE  autokick (determiner varchar, value varchar, updated bigint, period bigint, reason varchar, room varchar);
                                      ", sqlite_conn);
                cmd.ExecuteNonQuery();
            
            }

        } 

        public Tempdb(string DBfile)
        {
            for (int i = 0; i < 10; i++)
            {
                sobjs[i] = new object();
            }

            db_file = DBfile;
            LoadBase();
        }




        public ArrayList CheckAndAnswer(Jid Jid)
        {
            lock (sobjs[0])
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
                    cmd.ExecuteNonQuery();

              
                    sqr = cmd.ExecuteReader();

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





        public bool GreetExists(Jid Jid, Jid Room)
        {
            lock (sobjs[1])
            {
                string jid = Jid.Bare.Replace("'", "''");
                string room = Room.Bare.Replace("'", "''");
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                     SELECT *   
                               FROM greet 
                               WHERE (jid = '{0}' and room = '{1}')
                                      ",
                                            jid, room), sqlite_conn);
                cmd.ExecuteNonQuery();
                SqliteDataReader sqr = cmd.ExecuteReader();
                return sqr.Read();

            }
        }


        public bool AddAutoKick(string Value, Jid Room, string Type, string Reason, long Period)
        {
          lock (sobjs[7])
            {
                string type  = Type.Replace("'", "''");
                string value = Value.Replace("'", "''");
                string reason = Reason.Replace("'", "''");
                string room = Room.Bare.Replace("'", "''");
                if (AutoKickExists(Value, Room, Type, Reason, Period))
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

       public bool AutoKickExists(string Value, Jid Room, string Type, string Reason, long Period)
        {
          lock (sobjs[8])
            {
                string type  = Type.Replace("'", "''");
                string value = Value.Replace("'", "''");
                string reason = Reason.Replace("'", "''");
                string room = Room.Bare.Replace("'", "''");
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                      SELECT * 
                           FROM autokick 
                           WHERE (determiner = '{0}' and value = '{1}' and updated = '{2}' and period = '{3}' and reason = '{4}' and room ='{5}')
                                      ",
                                            type, value, DateTime.Now.Ticks.ToString(), Period.ToString(), reason, room), sqlite_conn);
                cmd.ExecuteNonQuery();
		SqliteDataReader sqr = cmd.ExecuteReader();
              return sqr.Read();

            }
        } 
        public string IsAutoKick(Jid Jid, string Nick, Jid Room)
        {
            lock (sobjs[6])
            {
                string jid = Jid.Bare.Replace("'", "''");
                string nick = Nick.Replace("'", "''");
                string room = Room.Bare.Replace("'", "''");
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                     SELECT *   
                               FROM autokick 
                               WHERE (room = '{0}')
                               ORDER BY updated DESC
                                      ",
                                            room), sqlite_conn);
                cmd.ExecuteNonQuery();
                SqliteDataReader sqr = cmd.ExecuteReader();
                while  (sqr.Read())
                   {
                     string determiner = sqr.GetString(0);
                     string value = sqr.GetString(1);
                     long updated = sqr.GetInt64(2);
                     long period = sqr.GetInt64(3);
                     string reason = sqr.GetString(4);
                     switch (determiner)
                     {
                        case "AKICK_JID":
                            if (jid == value)
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
                                          determiner, value, updated.ToString(), period.ToString(), reason, room), sqlite_conn);
                                   cmd.ExecuteNonQuery();
				 }
                               }
                                else 
                                    return null; 

                             break;
                        case "AKICK_JID_REGEXP":
 			    Regex regex = new Regex(value);
                            if (regex.IsMatch(jid))
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
                                          determiner, value, updated.ToString(), period.ToString(), reason, room), sqlite_conn);
                                   cmd.ExecuteNonQuery();
				 }
                               }
                                else 
                                    return null; 

                           break;
                        case "AKICK_NICK_REGEXP":
                            Regex _regex = new Regex(value);
                            if (_regex.IsMatch(nick))
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
                                          determiner, value, updated.ToString(), period.ToString(), reason, room), sqlite_conn);
                                   cmd.ExecuteNonQuery();
				 }
                               }
                                else 
                                    return null; 
                           break;
 	             }
                   }
                   return null;

            }
        }



        public bool AddGreet(Jid Jid, Jid Room, string phrase)
        {
            lock (sobjs[4])
            {
                string jid = Jid.Bare.Replace("'", "''");
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

        public bool DelGreet(Jid Jid, Jid Room)
        {
            lock (sqlite_conn)
            {
                if (Greet(Jid,Room) == null)
                    return false;
                string jid = Jid.Bare.Replace("'", "''");
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


        public string Greet(Jid Jid, Jid Room)
        {
            lock (sobjs[2])
            {
                string jid = Jid.Bare.Replace("'", "''");
                string room = Room.Bare.Replace("'", "''");

                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                        SELECT *   
                               FROM greet 
                               WHERE (jid = '{0}' and room = '{1}')
                               ORDER BY phrase DESC
                               LIMIT 1
                                      ",
                                            jid, room), sqlite_conn);
                cmd.ExecuteNonQuery();
                SqliteDataReader  sqr = cmd.ExecuteReader();
                if (sqr.Read())
                {
                 
                    return sqr.GetString(2);
                }
                return null;

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
                cmd.ExecuteNonQuery();


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
