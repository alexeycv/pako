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
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.protocol.iq.roster;
using agsXMPP.protocol.iq.version;
using agsXMPP.Xml.Dom;
using agsXMPP.protocol.x.muc;
using Core.Plugins;
using Core.DataBase;
using Core.Conference;
using System.Threading;
using Core.Other;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using Mono.Data.SqliteClient;
using agsXMPP.protocol.iq.time;


namespace Core.Kernel
{
    public class SessionHandler
    {

         public int m_rects;
         public bool arg_nomucs = false; 
         Session _S;
         static  object async_session = new object();
         static object async_defs = new object();
         static object async_rfc = new object();
         static object async_ticks = new object();
         Dictionary m_defs;
         DirBuilder db = new DirBuilder();
         RfcManager m_rfc;
         long ticks = DateTime.Now.Ticks;
         // GC Thread
         Thread _cleaner;

         public Session S
        {
            get { lock (async_session) { return _S; } }
            set { lock (async_session) { _S = value; } }
        }


         public Dictionary Defs
        {
            get { lock (async_defs) { return m_defs; } }
            set { lock (async_defs) { m_defs = value; } }
        }


         public long Ticks
        {
            get { lock (async_ticks) { return ticks; } }
            set { lock (async_ticks) { ticks = value; } }
        }

         public RfcManager RFCHandler
        {
            get { lock (async_rfc) { return m_rfc; } }
            set { lock (async_rfc) { m_rfc = value; } }
        }



        public void Open()
        {
            this.Register();
        	@out.log_get_ready();
            Environment.SetEnvironmentVariable("PAKODIR", Utils.CD);
            try
            {
                Process p = Process.GetCurrentProcess();
                Console.Title = "Pako " + p.MainModule.FileVersionInfo.FileVersion;
            }
            catch (Exception err)
            {
            }
            m_rects = 0;
            Thread.Sleep(1000);
            MainConnect();
        }



         public void LoadBase(int sqliteversion)
        {
            m_defs = new Dictionary(db.b(Utils.CD, "Dynamic", "Dictionary.db"), sqliteversion);
            m_rfc = new RfcManager(db.b(Utils.CD, "Static", "rfc3921.txt"));
        }


         public void Register()
         {
             Utils.Sh = this;
         }

         public void CleanerWorker()
         {
            while (true)
            {
                GC.Collect();
                Thread.Sleep(5000);
            }
         }

         public void MainConnect()
        {
                // init cleaner thread
                if (_cleaner == null)
                {
                    _cleaner = new Thread(this.CleanerWorker);
                    _cleaner.Start();
                }

                m_rects++;
                Console.Clear();
                if (m_defs != null)
                {
                    m_defs.SQLiteConnection.Close();
                }
                /*=============================================================*/
                /* Main class , which contains all the bot settings and xmpp resources, 
                 * all the muc and roster data.
                 */


                S = new Session(m_rects, this);

                /* Actions, needed for binding socket with specified actions :
                 * presence handler, messages handler, iq handler, bot handler
                 * everything - those actions bind socket with the program */
                /*Socket opens the connection - the internet connection parameters are setted*/
                S.Open();
        }

          
        }
    }
