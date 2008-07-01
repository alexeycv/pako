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

         public string dir_langs;
         public string dir_config;
         public string dir_ams;
         public string dir_plugs;
         public string dir_censor;
         public string dir_va;
         public string dir_vl;
         public string dir_access;
         public string m_version;
         public string dir_tempdb;
         public int m_rects;
         public string dir_error;
         public bool arg_debug  = false;
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



        public void Open(string LanguageFolder, string PluginsFolder, string ConfigFile, string AutoMucsFile, string CensorFile, string VipAccessFile, string VipLangFile, string AccessFile, string ErrorFile, string TempDBFile, string botversion, string[] args)
        {

        	@out.log_get_ready();

            if (args != null)
            foreach (string _arg in args)
            {
                string arg = _arg.Replace("--","-").ToLower();

               switch (arg) 
               {
                   case "-debug":
                       arg_debug = !arg_debug;
                       break;
                   case "-help":
                       @out.write(@"
    This is a Pako jabber-bot, based on .NET technologies (C# 2.0)
    Visit our project home page: http://code.google.com/p/pako
    Leave your comments and add new ideas.
    Parameters available:

          --help      See the major help for a a list of parameters.
          --debug     Turn in debugging mode: open xml console and Plug-ins debugging.
          --nomucs    Bot will work only in roster mode. No conference support.
          --version   Shows the current version of bot
   
    Mail me <bbodio at i.ua> if you've got a bug report or some questions.
");

                       return;
                   case "-nomucs":
                       arg_nomucs = !arg_nomucs;
                       break;
                   case "-version":
                       @out.write("\nPako bot " + Process.GetCurrentProcess().MainModule.FileVersionInfo.FileVersion + " " + botversion+"\n");
                       return;
                   default:
                       @out.write("\n"+arg + " : command not found");
                       return;
               }


            }

            
            Environment.SetEnvironmentVariable("PAKODIR", Utils.CD);
            Process p = Process.GetCurrentProcess();
            m_version = botversion;
            dir_va = VipAccessFile;
            dir_vl = VipLangFile;
            dir_config = ConfigFile;
            dir_ams = AutoMucsFile;     
            dir_langs = LanguageFolder;
            dir_censor = CensorFile;
            dir_plugs = PluginsFolder;
            dir_tempdb = TempDBFile;
            dir_error = ErrorFile;
            dir_access = AccessFile;
            Console.Title = "Pako " + p.MainModule.FileVersionInfo.FileVersion;
            m_rects = 0;
            Thread.Sleep(1000);
            MainConnect();
        }



         public void LoadBase()
        {
            m_defs = new Dictionary(db.b(Utils.CD, "Dynamic", "Dictionary.db"));
            m_rfc = new RfcManager(db.b(Utils.CD, "Static", "rfc3921.txt"));
        }



         public void MainConnect()
        {
                m_rects++;
                Console.Clear();
                if (m_defs != null)
                {
                    m_defs.SQLiteConnection.Close();
                }

                Thread thr = new Thread(new ThreadStart(LoadBase));
                thr.Start();
                /*=============================================================*/
                /* Main class , which contains all the bot settings and xmpp resources, 
                 * all the muc and roster data.
                 */


                S = new Session(dir_langs, dir_plugs, dir_config, dir_ams, dir_censor, dir_va, dir_vl, dir_access, dir_error, dir_tempdb, m_version, m_rects, this);

                /* Actions, needed for binding socket with specified actions :
                 * presence handler, messages handler, iq handler, bot handler
                 * everything - those actions bind socket with the program */
                /*Socket opens the connection - the internet connection parameters are setted*/
                S.Open();
        }

          
        }
    }
