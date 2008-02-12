using System;
using System.Collections.Generic;
using System.Text;
using agsXMPP;
using agsXMPP.protocol.x.muc;
using agsXMPP.protocol.client;
using agsXMPP.protocol.iq.version;
using agsXMPP.protocol.iq.roster;
using Core.Client;
using Core;
using Core.Conference;
using Core.Plugins;
using Core.Special;
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
        static bool debug = false;

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                debug = args[0] == "--debug";
            }

            MainConnect();
        }

        static public SessionHandler S
        {
            get { lock (async_session) { return _S; } }
            set { lock (async_session) { _S = value; } }
        }



 

        static public void MainConnect()
        {
                /*=============================================================*/
                /* Main class , which contains all the bot settings and xmpp resources, 
                 * all the muc and roster data.
            
                 * */


          
            _S = new SessionHandler();
            version = "20080211";
                /*=============================================================*/

                   m_dir = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

            _S.Open(
                /*====================Main_Settings_-_Verify_and_identify_all_the_settings========================*/
                /*========*/db.b(m_dir,"Lang"),
                /*========*/db.b(m_dir,"Plugins"),
                /*========*/db.b(m_dir,"Pako.cfg"),
                /*========*/db.b(m_dir,"Dynamic","Rooms.base"),
                /*========*/db.b(m_dir,"Dynamic","Censor.db"),
                /*========*/db.b(m_dir,"Dynamic","Users.base"),
                /*========*/db.b(m_dir,"Dynamic","Access.base"),
                /*========*/db.b(m_dir,"Error.log"),
                /*========*/db.b(m_dir,"Dynamic","Temp.db"),
                /*=============================================================*/
                 version,
                 debug
                 );
            for (; ; )
            {
               Console.Read();
            }
            }








    }
}
