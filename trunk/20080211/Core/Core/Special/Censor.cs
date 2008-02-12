using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;
using agsXMPP.protocol.x.muc;
using Core.Client;
using System.Text.RegularExpressions;
using Mono.Data.SqliteClient;
using System.Threading;

namespace Core.Special
{
     public class Censor
    {
        object[] sobjs = new object[10];
        SqliteConnection sqlite_conn;
        string db_file;



        public SqliteConnection SQLiteConnection
        {
            get { lock (sobjs[4]) { return sqlite_conn; } }
            set { lock (sobjs[4]) { sqlite_conn = value; } }
        }

        public void LoadBase()
        {
            bool to_create = !File.Exists(db_file);
            sqlite_conn = new SqliteConnection("URI=file:" + db_file.Replace("\\", "/"));
            sqlite_conn.Open();
            if (to_create)
            {
                SqliteCommand cmd = new SqliteCommand(@"
                    CREATE 
                          TABLE  censor (muc varchar, censor_data varchar, reason varchar);
                                      ", sqlite_conn);
               
                cmd.ExecuteNonQuery();
            }

        

        }

        public Censor(string DBfile)
        {
            for (int i = 0; i < 10; i++)
            {
                sobjs[i] = new object();
            }

            db_file = DBfile;
            //Thread thr = new Thread(new ThreadStart(LoadBase));
            //thr.Start();
            LoadBase();
        }




        public bool DelRoomCensor(string source)
        {
            lock (sqlite_conn)
            {
                try
                {
                    int num = Convert.ToInt32(source);

                    source = source.Replace("'", "''");
                    SqliteCommand cmd = new SqliteCommand(@"
                     SELECT *
                         FROM censor
                         WHERE (muc = '*')",

                                          sqlite_conn);
                    cmd.ExecuteNonQuery();
                    SqliteDataReader sqlite_datareader = cmd.ExecuteReader();
                    int i = 0;
                    while (sqlite_datareader.Read())
                    {
                        i++;
                        if (num == i)
                        {
                            string catched = sqlite_datareader.GetString(1).Replace("'","''");

                            cmd = new SqliteCommand(String.Format(@"
                     DELETE 
                         FROM censor
                         WHERE  (censor_data = '{0}' and muc = '*') 
                                      ",
                                                                      catched),
                                                                      sqlite_conn);
                            cmd.ExecuteNonQuery();
                            return true;
                        }

                        return false;


                    }
                    cmd = new SqliteCommand(String.Format(@"
                     DELETE 
                         FROM censor
                         WHERE  (censor_data = '{0}' and muc = '*') 
                                      ",
                                              source),
                                              sqlite_conn);
                    cmd.ExecuteNonQuery();

                    return true;


                }
                catch
                {

                    source = source.Replace("'", "''");
                    SqliteCommand cmd = new SqliteCommand(String.Format(@"
                     SELECT *
                         FROM censor
                         WHERE (censor_data = '{0}' and muc = '*') 
                                      ",
                                           source),
                                          sqlite_conn);
                    cmd.ExecuteNonQuery();
                    SqliteDataReader sqlite_datareader = cmd.ExecuteReader();
                    if (!sqlite_datareader.Read())
                        return false;

                    //(muc = '{0}') &&
                    cmd = new SqliteCommand(String.Format(@"
                     DELETE 
                         FROM censor
                         WHERE  (censor_data = '{0}' and muc = '*') 
                                      ",
                                              source),
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
                SqliteCommand cmd = new SqliteCommand(@"
                     SELECT *
                         FROM censor
                         WHERE (muc = '*') 
                                      ",
                                      sqlite_conn);
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


        public void AddCensor(string source, string reason)
        {
            lock (sqlite_conn)
            {
                source = source.Replace("'", "''");
                reason = reason.Replace("'", "''");
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                     INSERT 
                         INTO censor (muc, censor_data, reason) 
                         VALUES ('*', '{0}', '{1}');
                                      ",
                                          source,
                                          reason),
                                          sqlite_conn);
                cmd.ExecuteNonQuery();

               // SqliteDataReader sqlite_datareader = cmd.ExecuteReader();
            }

        }

       // public bool DelRoomCensor(Jid Room, int 

      
    }
}
