/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Pako Jabber-bot.                                                              *
 * Copyright. All rights reserved © 2007-2008 by Klichuk Bogdan (Bbodio's Lab)   *
 * Copyright. All rights reserved © 2009-2012 by Alexey Bryohov                  *
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
    public class Logger
    {

        object[] sobjs = new object[30];
        SqliteConnection sqlite_conn;
        string db_file;
        int ver;
        string m_dir;
        Hashtable _hTable;



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
                          TABLE  logs (id varchar, jid varchar, action varchar, source varchar, phrase varchar, author varchar, date varchar, time varchar);
                                      ", sqlite_conn);

                cmd.ExecuteNonQuery();
            }
        }

	public void LoadSeenBase()
        {
            bool to_create = !File.Exists(db_file);
            m_dir = db_file;
            sqlite_conn = new SqliteConnection("URI=file:" + db_file.Replace("\\", "/") + ",version=" + ver.ToString());
            sqlite_conn.Open();
            if (to_create)
            {
                SqliteCommand cmd = new SqliteCommand(@"
                    CREATE 
                          TABLE  logs (jid varchar, room varchar, nick varchar, datetime varchar);
                                      ", sqlite_conn);

                cmd.ExecuteNonQuery();
            }
        }

	public void LoadIqBase()
        {
            bool to_create = !File.Exists(db_file);
            m_dir = db_file;
            sqlite_conn = new SqliteConnection("URI=file:" + db_file.Replace("\\", "/") + ",version=" + ver.ToString());
            sqlite_conn.Open();
            if (to_create)
            {
                SqliteCommand cmd = new SqliteCommand(@"
                    CREATE 
                          TABLE  iq (jid varchar, room varchar, nick varchar, version varchar, os varchar);
                                      ", sqlite_conn);

                cmd.ExecuteNonQuery();
            }
        }

        public Logger(string DBfile, int version, string logtype)
        {
            for (int i = 0; i < 30; i++)
            {
                sobjs[i] = new object();
            }
            ver = version;
            db_file = DBfile;
            if (logtype == "chatlog")
            	LoadBase();
            if (logtype == "seenlog")
                LoadSeenBase();
            if (logtype == "muclog")
            {
                _hTable = new Hashtable();
                if (!Directory.Exists(db_file))
                {
                    Directory.CreateDirectory(db_file);
                }
            }
        }

        public int EntitiesCount()
        {
            lock (sobjs[1])
            {
                SqliteCommand cmd = new SqliteCommand(@"
                   SELECT COUNT(id) 
                           FROM logs 
                                      ", sqlite_conn);
                cmd.ExecuteNonQuery();

                SqliteDataReader sqlite_datareader = cmd.ExecuteReader();
                sqlite_datareader.Read();
                return sqlite_datareader.GetInt32(0);
            }
        }

        public void Clear()
        {
            lock (sobjs[2])
            {
                SqliteCommand cmd = new SqliteCommand(@"
                   DELETE  
                         FROM logs 
                                      ", sqlite_conn);
                cmd.ExecuteNonQuery();
            }
        }

        public void AddEntry(string jid, string action, string source, string phrase, string author)
        {
            lock (sobjs[3])
            {
                string id = Guid.NewGuid().ToString("N");
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                   INSERT 
                         INTO logs (entity, def, author) 
                         VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
                                                         ", 
                                                           id,
                                                           jid,
                                                           action,
                                                           source,
                                                           phrase,
                                                           author,
                                                           "date",
                                                           "time"
                                                           ), sqlite_conn);
                cmd.ExecuteNonQuery();
            }
        }

        public void AddSeenEntry(string jid, string room, string nick)
        {
            lock (sobjs[5])
            {
                //string id = Guid.NewGuid().ToString("N");
                try
                {
                    // If the same jid+room is exisis
                    SqliteCommand delcmd = new SqliteCommand(String.Format(@"
                       DELETE FROM logs WHERE jid = '{0}' AND nick = '{1}' AND room = '{2}';",                                                            
                                                               jid, nick,
                                                               room
                                                               ), sqlite_conn);
                    //delcmd.ExecuteNonQuery();

                    SqliteCommand cmd = new SqliteCommand(String.Format(@"
                       INSERT 
                             INTO logs (jid, room, nick, datetime) 
                             VALUES ('{0}', '{1}', '{2}', '{3}');",                                                            
                                                               jid,
                                                               room,
                                                               nick,
                                                               DateTime.Now.ToString("yyyy.MM.dd - HH:mm:ss")
                                                               ), sqlite_conn);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception err)
                {
                }
            }
        }

        public string FindEntities(string jid)
        {
            lock (sobjs[2])
            {                
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                   SELECT  * 
                           FROM logs 
                           WHERE jid LIKE '%{0}%'
                           ORDER BY jid DESC;
                                      ",
                                          jid), sqlite_conn);
                cmd.ExecuteNonQuery();
                SqliteDataReader sqlite_datareader = cmd.ExecuteReader();
                string data = "";
                int count = 0;
                while (sqlite_datareader.Read())
                {
                    count++;
                    data += "\n" +sqlite_datareader.GetString(0) + " - " + sqlite_datareader.GetString(1) + " - " + sqlite_datareader.GetString(2) + " - " + sqlite_datareader.GetString(3);
                }

                return data != "" ? data+"\n"+"("+count.ToString()+")" : null; 
            }
        }

        public string FindEntities(string nick, string room)
        {
            lock (sobjs[6])
            {                
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                   SELECT  * 
                           FROM logs 
                           WHERE nick LIKE '%{0}%' AND room LIKE '%{1}%'
                           ORDER BY datetime DESC
                           LIMIT 1;
                                      ",
                                          nick, room), sqlite_conn);
                cmd.ExecuteNonQuery();
                SqliteDataReader sqlite_datareader = cmd.ExecuteReader();
                string data = "";
                int count = 0;
                while (sqlite_datareader.Read())
                {
                    count++;
                    data = sqlite_datareader.GetString(2) + " - " + sqlite_datareader.GetString(3);
                }

                return data;
            }
        }

        public string FindEntitiesLast(string room)
        {
            lock (sobjs[7])
            {                
                SqliteCommand cmd = new SqliteCommand(String.Format(@"
                   SELECT  * 
                           FROM logs 
                           WHERE room LIKE '%{0}%'
                           GROUP BY nick
                           ORDER BY datetime DESC                           
                           LIMIT 30;
                                      ",
                                          room), sqlite_conn);
                cmd.ExecuteNonQuery();
                SqliteDataReader sqlite_datareader = cmd.ExecuteReader();
                string data = "";
                int count = 0;
                while (sqlite_datareader.Read())
                {
                    count++;
                    data += count.ToString() + ") " + sqlite_datareader.GetString(2) + " - " + sqlite_datareader.GetString(3) + "\n";
                }

                return data;
            }
        }

        public void AddHtmlLog(string msgType, string subType, string jid, string nick, string message)
        {
            lock (sobjs[7])
            {
                try
                {
                    this.WriteHeader(jid);
                }
                catch (Exception err)
                {
                }

                string _ws = "";

                message = message.Replace("\n", "<br>").Replace("\"", "&quot;").Replace(" ", "&nbsp;");
                nick = nick.Replace("\n", "<br>").Replace("\"", "&quot;").Replace(" ", "&nbsp;");
                if (msgType == "groupchat")
                {
                    if (subType == "subscribe")
                    {
                       //message
                       _ws = "<span class=\"timestamp\">["+DateTime.Now.ToString("HH:mm:ss")+"] </span><span class=\"userjoin\">" + nick + "  " + message + "</span><br />\n";
                    }
  
                    if (subType == "chat")
                    {
                        //message
                       _ws = "<span class=\"timestamp\">["+DateTime.Now.ToString("HH:mm:ss")+"] </span><span class=\"normal\"><b>" + nick + "</b>: " + message + "</span><br />\n";
                    }

                   if (subType == "status")
                   {
                       //message
                       _ws = "<span class=\"timestamp\">["+DateTime.Now.ToString("HH:mm:ss")+"] </span><span class=\"statuschange\">" + nick + "  " + message + "</span><br />\n";
                   }

                   if (subType == "affiliation")
                   {
                       //message
                       _ws = "<span class=\"timestamp\">["+DateTime.Now.ToString("HH:mm:ss")+"] </span><span class=\"rachange\">" + nick + "  " + message + "</span><br />\n";
                   }

                   if (subType == "topic")
                   {
                       if (_hTable.Contains(jid))
                       {
                           if ((string)_hTable[jid] != message)
                           {
                               //message
                               _ws = "<span class=\"timestamp\">["+DateTime.Now.ToString("HH:mm:ss")+"] </span><span class=\"userjoin\"><h1>" + nick + message + "</h1></span><br />\n";
                               _hTable.Remove(jid);
                               _hTable.Add(jid, message);
                           }
                       }
                       else
                       {
                           //message
                           _ws = "<span class=\"timestamp\">["+DateTime.Now.ToString("HH:mm:ss")+"] </span><span class=\"userjoin\"><h1>" + nick + message + "</h1></span><br />\n";
                           _hTable.Add(jid, message);
                       }
                   }
               }

               if (_ws != "")
               {
                   //Step 0:Build a file and directory strings
                   try
                   {
                       string _dir = db_file.TrimEnd('/')+"/"+jid+"/"+DateTime.Now.Year.ToString()+"/"+DateTime.Now.Month.ToString()+"/";
                       string _file = _dir+DateTime.Now.Day.ToString()+".html";
                       StreamWriter _sw = new StreamWriter(_file, true);
                       _sw.Write(_ws);
                       _sw.Flush();
                       _sw.Close();
                   }
                   catch (Exception err)
                   {
                   }
               }
            }
        }

        void WriteHeader(string jid)
        {
            //Step 0:Build a file and directory strings
            string _dir = db_file.TrimEnd('/')+"/"+jid+"/"+DateTime.Now.Year.ToString()+"/"+DateTime.Now.Month.ToString()+"/";
            string _file = _dir+DateTime.Now.Day.ToString()+".html";
            
            //Step 1: Checking if file esists
            bool _fileExists = false;
            _fileExists = Directory.Exists(_dir);

            if (!_fileExists)
            {
                Directory.CreateDirectory(_dir);
                //create .htaccess file
                string _htdata = "AuthType Basic\nAuthName \"Ask room owner for the username/password\"\n"+
                                 "AuthUserFile .htpasswd\nrequire valid-user";
            }

            _fileExists = File.Exists(_file);

            if (!_fileExists)
            {
                //html header string
                string _data = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dt\">"+
                               "\n<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"en\" lang=\"en\">\n"+
                               "<head>\n"+
                               "<title>"+jid+"</title>\n"+
                               "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" /><style type=\"text/css\"><!--\n"+
                               ".userjoin {color: #009900; font-style: italic; font-weight: bold}\n"+
                               ".userleave {color: #dc143c; font-style: italic; font-weight: bold}\n"+
                               ".statuschange {color: #a52a2a; font-weight: bold}\n"+
                               ".rachange {color: #0000FF; font-weight: bold}\n"+
                               ".userkick {color: #FF7F50; font-weight: bold}\n"+
                               ".userban {color: #DAA520; font-weight: bold}\n"+
                               ".nickchange {color: #FF69B4; font-style: italic; font-weight: bold}\n"+
                               ".timestamp {color: #aaa;}\n"+
                               ".timestamp a {color: #aaa; text-decoration: none;}\n"+
                               ".system {color: #090; font-weight: bold;}\n"+
                               ".emote {color: #800080;}\n"+
                               ".self {color: #0000AA;}\n"+
                               ".selfmoder {color: #DC143C;}\n"+
                               ".normal {color: #483d8b;}\n"+
                               "#mark { color: #aaa; text-align: right; font-family: monospace; letter-spacing: 3px }\n"+
                               "h1 { color: #369; font-family: sans-serif; border-bottom: #246 solid 3pt; letter-spacing: 3px; margin-left: 20pt;}\n"+
                               "h2 { color: #639; font-family: sans-serif; letter-spacing: 2px; text-align: center }\n"+
                               "a.h1 {text-decoration: none;color: #369;}\n"+
                               "#//-->\n"+
                               "</style>\n"+
                               "</head>\n"+
                               "<body>\n"+
                               "<div id=\"mark\">Pako log</div>\n"+
                               "<h1><a class=\"h1\" href=\"xmpp:"+jid+"?join\" title=\"Join room\">"+jid+"</a></h1>\n"+
                               "<h2>"+jid+"</h2>\n"+
                               "<div>\n"+
                               "<tt>\n";

                StreamWriter _sw = new StreamWriter(_file, false);
                _sw.Write(_data);
                _sw.Flush();
                _sw.Close();
                
                //.htaccess
            }
        }
    }
}
