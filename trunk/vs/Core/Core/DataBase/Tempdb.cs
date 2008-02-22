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
using Mono.Data.SqliteClient;
using System.Threading;
using System.IO;
using agsXMPP;
using Core.Kernel;



namespace Core.DataBase
{
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


        /* * * * * * * * * * * * * * * * * * * * * *
         * roso@conference.jabber.ru/bbodio        *
         * bbodio@jabber.ru/bbodio                 *
         * * * * * * * * * * * * * * * * * * * * * */
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
                       // @out.exe("----" + sqr.GetString(0));
                        al.Add(new string[] { sqr.GetString(0), sqr.GetString(1), sqr.GetString(2) });
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


                 cmd = new SqliteCommand(String.Format(@"
                        SELECT *   
                               FROM greet 
                               ORDER BY phrase DESC
                                      ",
                                     jid, room), sqlite_conn);

                cmd.ExecuteNonQuery();

                SqliteDataReader sqr = cmd.ExecuteReader();
                while (sqr.Read())
                {
                    @out.exe("$$$ " + sqr.GetString(0) + " " + sqr.GetString(1) + " " + sqr.GetString(2));
                }

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
                        SELECT *   
                               FROM greet 
                               WHERE (jid = '{0}' and room = '{1}')
                               ORDER BY phrase DESC
                                      ",
                                            jid, room), sqlite_conn);

                 cmd.ExecuteNonQuery();

                   SqliteDataReader sqr = cmd.ExecuteReader();
                       while (sqr.Read())
                       {
                           @out.exe("$$$ " + sqr.GetString(0) + " " + sqr.GetString(1) + " " + sqr.GetString(2));
                       }


            

                 cmd = new SqliteCommand(String.Format(@"
                   DELETE 
                         FROM greet 
                         WHERE (jid = '{0}' and room = '{1}')
                                      ",
                                            jid, room), sqlite_conn);
                 cmd.ExecuteNonQuery();

                cmd = new SqliteCommand(String.Format(@"
                        SELECT *   
                               FROM greet 
                               WHERE (jid = '{0}' and room = '{1}')
                               ORDER BY phrase DESC
                                      ",
                                     jid, room), sqlite_conn);

                cmd.ExecuteNonQuery();

                sqr = cmd.ExecuteReader();
                while (sqr.Read())
                {
                    @out.exe("%%% " + sqr.GetString(0) + " " + sqr.GetString(1) + " " + sqr.GetString(2));
                }


               // sqlite_conn.Close();
               // sqlite_conn.Open();
                return true;

            }
        }


        public string Greet(Jid Jid, Jid Room)
        {
            lock (sobjs[2])
            {
                string jid = Jid.Bare.Replace("'", "''");

                string room = Room.Bare.Replace("'", "''");


                SqliteCommand  cmd = new SqliteCommand(String.Format(@"
                        SELECT *   
                               FROM greet 
                               ORDER BY phrase DESC
                                      ",
                                 jid, room), sqlite_conn);

                cmd.ExecuteNonQuery();

                SqliteDataReader sqr = cmd.ExecuteReader();
                while (sqr.Read())
                {
                    @out.exe("$$$ " + sqr.GetString(0) + " " + sqr.GetString(1) + " " + sqr.GetString(2));
                }

                 cmd = new SqliteCommand(String.Format(@"
                        SELECT *   
                               FROM greet 
                               WHERE (jid = '{0}' and room = '{1}')
                               ORDER BY phrase DESC
                               LIMIT 1
                                      ",
                                            jid, room), sqlite_conn);
                cmd.ExecuteNonQuery();
                 sqr = cmd.ExecuteReader();
                if (sqr.Read())
                {
                 
                    return sqr.GetString(2);
                }
                return null;

            }
        }


        public void AddTell(Jid Jid, string phrase, Jid author)
        {
            lock (sobjs[3])
            {
                   string jid = Jid.ToString().Replace("'", "''");
                   
                phrase = phrase.Replace("'", "''");
                string auth = author.ToString().Replace("'", "''");
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                   INSERT 
                         INTO tell (jid, phrase, author) 
                         VALUES ('{0}', '{1}', '{2}');
                                                         ",
                                                           jid,
                                                           phrase,
                                                           auth
                                                           ), sqlite_conn);

                cmd.ExecuteNonQuery();

            }
        }


    }
}
