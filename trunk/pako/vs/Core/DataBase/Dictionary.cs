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



namespace Core.DataBase
{
    public class Dictionary
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
                @out.exe("dict1");
                sqlite_conn.Open();
                if (to_create)
                {
                    SqliteCommand cmd = new SqliteCommand(@"
                    CREATE 
                          TABLE  defs (entity varchar, def varchar, author varchar);
                                      ", sqlite_conn);

                    cmd.ExecuteNonQuery();
                }
                @out.exe("dict2");
           
        }

        public Dictionary( string DBfile, int version)
        {
            for (int i = 0; i < 10; i++)
               {
                   sobjs[i] = new object();
               }
               ver = version;
               db_file = DBfile;
               LoadBase();
        }


        public string GetEntity(string entity, string pattern)
        {
            lock (sobjs[0])
            {
                entity = entity.Trim().Replace("'", "''");
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                   SELECT  * 
                           FROM defs 
                           WHERE (entity = '{0}') 
                                      ",
                                          entity),sqlite_conn);
                cmd.ExecuteNonQuery();

                SqliteDataReader sqlite_datareader = cmd.ExecuteReader();
                string dfn = null;
                string aut = null;
                string wtf = null;
                while (sqlite_datareader.Read())
                {
                    dfn = sqlite_datareader.GetString(1);
                    aut = sqlite_datareader.GetString(2);
                    wtf = sqlite_datareader.GetString(0);
                }


                if (dfn != null)
                {
                    return pattern
                                    .Replace("{1}", wtf)
                                    .Replace("{2}", dfn)
                                    .Replace("{3}", aut);
                }

                return  null;

            }
        }


        public string GetAllEntities(string entity, string pattern)
        {
            lock (sobjs[1])
            {
                entity = entity.Trim().Replace("'", "''");
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                   SELECT  * 
                           FROM defs 
                           WHERE (entity = '{0}') 
                                      ",
                                          entity), sqlite_conn);
                cmd.ExecuteNonQuery();

                SqliteDataReader sqlite_datareader = cmd.ExecuteReader();
                string data = "";
                int count = 0;

                while (sqlite_datareader.Read())
                {
                    count++;
                    data += "\n"+ pattern.
                                  Replace("{1}", count.ToString()).
                                  Replace("{2}", sqlite_datareader.GetString(0)).
                                  Replace("{3}", sqlite_datareader.GetString(1)).
                                  Replace("{4}", sqlite_datareader.GetString(2));
                }


                return data != "" ? data : null;

            }
        }

        public string FindEntities(string tip, string pattern)
        {
            lock (sobjs[2])
            {
                tip = tip.Trim().Replace("'", "''");
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                   SELECT  * 
                           FROM defs 
                           WHERE entity LIKE '%{0}%'
                           ORDER BY entity DESC
                           LIMIT 20;
                                      ",
                                          tip), sqlite_conn);
                cmd.ExecuteNonQuery();
                SqliteDataReader sqlite_datareader = cmd.ExecuteReader();
                string data = "";
                int count = 0;
                while (sqlite_datareader.Read())
                {
                    count++;
                    data += "\n" +sqlite_datareader.GetString(0);
                }

                return data != "" ? data+"\n"+"("+count.ToString()+")" : null; 
            }
        }


        public int EntitiesCount()
        {
            lock (sobjs[4])
            {
                SqliteCommand cmd = new SqliteCommand(@"
                   SELECT COUNT(entity) 
                           FROM defs 
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
                         FROM defs 
                                      ", sqlite_conn);
                cmd.ExecuteNonQuery();
            }
        }

        public void AddEntity(string entity, string meaning, string author)
        {
            lock (sobjs[3])
            {

                entity = entity.Trim().Replace("'", "''");
                meaning = meaning.Trim().Replace("'", "''");
                author = author.Trim().Replace("'", "''");
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                   DELETE 
                         FROM defs 
                         WHERE (author ='{0}' and entity = '{1}');
                                                         ",author
                                                          ,entity
                                                           ), sqlite_conn);
                cmd.ExecuteNonQuery();

                 cmd = new SqliteCommand(String.Format(@"
                   INSERT 
                         INTO defs (entity, def, author) 
                         VALUES ('{0}', '{1}', '{2}');
                                                         ", 
                                                           entity,
                                                           meaning,
                                                           author
                                                           ), sqlite_conn);


                cmd.ExecuteNonQuery();
            }
        }
       

    }
}
