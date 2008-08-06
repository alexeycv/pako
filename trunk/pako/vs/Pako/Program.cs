﻿/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
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
using System.Text;
using agsXMPP;
using agsXMPP.protocol.x.muc;
using agsXMPP.protocol.client;
using agsXMPP.protocol.iq.version;
using agsXMPP.protocol.iq.roster;
using Core.Kernel;
using Core.Other;
using Core.Conference;
using Core.Plugins;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading;
using Mono.Data.SqliteClient;


namespace Pako
{
    static class Bot
    {     
        static SessionHandler _S;
        static string m_dir;
        static object async_session = new object();
        static DirBuilder db = new DirBuilder();
        static string version;
        static string[] _args;

        static void Main(string[] args)
        {
            _args = args;
            MainConnect();
        }

        static public SessionHandler S
        {
            get { lock (async_session) { return _S; } }
            set { lock (async_session) { _S = value; } }
        }

        static public void MainConnect()
        {
           
            version = "8.8.6";
            Assembly assem = Assembly.GetExecutingAssembly();
            m_dir = Path.GetDirectoryName(assem.Location);
            Utils.CD = m_dir;
            _S = new SessionHandler();
            _S.Open(version,_args);
            for (;;)
            {
               Console.Read();
            }
            }
    }
}
