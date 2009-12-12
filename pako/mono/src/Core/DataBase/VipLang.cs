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
using Core.Kernel;
using agsXMPP;



namespace Core.DataBase
{
    public class VipLang
    {
        object[] sobjs = new object[10];
        SqliteConnection sqlite_conn;
        string db_file;
        string m_dir;
        int ver;



        public SqliteConnection SQLiteConnection
        {
            get { lock (sobjs[4]) { return sqlite_conn; } }
            set { lock (sobjs[4]) { sqlite_conn = value; } }
        }

        public void LoadBase()
        {

            
            bool to_create = !File.Exists(db_file);
            m_dir = db_file;
            sqlite_conn = new SqliteConnection("URI=file:" + db_file.Replace("\\", "/") + ",version="+ver.ToString());
            sqlite_conn.Open();
            if (to_create)
            {
                SqliteCommand cmd = new SqliteCommand(@"
                    CREATE 
                          TABLE  viplang (jid varchar, lang varchar);
                                      ", sqlite_conn);

                cmd.ExecuteNonQuery();
            }
 

        }

        public VipLang(string DBfile, int version )
        {
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    sobjs[i] = new object();
                }
                ver = version;
                db_file = DBfile;
                LoadBase();
            }
            catch (Exception ex)
            {
                @out.write(ex.ToString());
            }
        }


        public string GetLang(Jid Jid)
        {
            lock (sobjs[0])
            {
                string jid = Jid.Bare.ToLower().Replace("'", "''");
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                   SELECT  * 
                           FROM viplang 
                           WHERE (jid = '{0}') 
                                      ",
                                          jid), sqlite_conn);
                cmd.ExecuteNonQuery();

                SqliteDataReader sqlite_datareader = cmd.ExecuteReader();
                if (sqlite_datareader.Read())
                {
                    return sqlite_datareader.GetString(1);
                }
                return null;

 
            }
        }


        public string GetAllVips(string pattern)
        {
            lock (sobjs[1])
            {
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                   SELECT  * 
                           FROM viplang 
                                      "), sqlite_conn);
                cmd.ExecuteNonQuery();

                SqliteDataReader sqlite_datareader = cmd.ExecuteReader();
                string data = "";
                int count = 0;

                while (sqlite_datareader.Read())
                {
                    count++;
                    data += "\n" + pattern.
                                  Replace("{1}", count.ToString()).
                                  Replace("{2}",sqlite_datareader.GetString(0)).
                                  Replace("{3}", sqlite_datareader.GetString(1));
                }


                return data != "" ? data : null;

            }
        }



        public int Count()
        {
            lock (sobjs[4])
            {
                SqliteCommand cmd = new SqliteCommand(@"
                   SELECT COUNT(jid) 
                           FROM viplang 
                                      ", sqlite_conn);
                cmd.ExecuteNonQuery();

                SqliteDataReader sqlite_datareader = cmd.ExecuteReader();
                sqlite_datareader.Read();
                return sqlite_datareader.GetInt32(0);
            }
        }



        public void Clear()
        {
            lock (sobjs[5])
            {
                SqliteCommand cmd = new SqliteCommand(@"
                   DELETE  
                         FROM viplang
                                      ", sqlite_conn);
                cmd.ExecuteNonQuery();
            }
        }

        public bool DelVip(Jid Jid)
        {
            lock (sobjs[2])
            {
                string jid = Jid.Bare.ToLower().Replace("'", "''");
                if (GetLang(Jid) == null)
                    return false;
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                   DELETE 
                         FROM viplang 
                         WHERE (jid ='{0}');
                                                         ", jid
                                                           ), sqlite_conn);
                cmd.ExecuteNonQuery();
                return true;
            }
        }
        public bool DelVip(int number)
        {
            lock (sobjs[6])
            {

                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                      SELECT * 
                           FROM viplang 
                                      "
                                       ), sqlite_conn);
                cmd.ExecuteNonQuery();
                SqliteDataReader sqr = cmd.ExecuteReader();
                int i = 0;
                while (sqr.Read())
                {
                    i++;
                    if (i == number)
                    {
                        string jid = sqr.GetString(0).Replace("'", "''");
                        string value = sqr.GetString(1);
                        cmd = new SqliteCommand(String.Format(@"
                                     DELETE    
                                            FROM viplang 
                                            WHERE (jid = '{0}' and lang = '{1}')
                                      ",
                                               jid, value), sqlite_conn);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
                return false;

            }
        }

        public void AddVip(Jid Jid, string language)
        {
            lock (sobjs[3])
            {

                string jid = Jid.Bare.ToLower().Replace("'", "''");
                language = language.Trim().ToLower().Replace("'", "''");
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                   DELETE 
                         FROM viplang 
                         WHERE (jid ='{0}');
                                                         ", jid
                                                           ), sqlite_conn);
                cmd.ExecuteNonQuery();

                cmd = new SqliteCommand(String.Format(@"
                   INSERT 
                         INTO viplang (jid, lang) 
                         VALUES ('{0}', '{1}');
                                                         ",
                                                          jid,
                                                          language
                                                          ), sqlite_conn);


                cmd.ExecuteNonQuery();
            }
        }


    }
}
