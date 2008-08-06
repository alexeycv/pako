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
    public class Logger
    {

             object[] sobjs = new object[30];
        SqliteConnection sqlite_conn;
        string db_file;
        int ver;
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
            sqlite_conn = new SqliteConnection("URI=file:" + db_file.Replace("\\", "/") + ",version=" + ver.ToString());
            sqlite_conn.Open();
            if (to_create)
            {
                SqliteCommand cmd = new SqliteCommand(@"
                    CREATE 
                          TABLE  logs (jid varchar, phrase varchar, author varchar, date varchar);
                                      ", sqlite_conn);

                cmd.ExecuteNonQuery();


            }

        }

        public Logger(string DBfile, int version)
        {
            for (int i = 0; i < 30; i++)
            {
                sobjs[i] = new object();
            }
            ver = version;
            db_file = DBfile;
            LoadBase();
        }


    }
}
