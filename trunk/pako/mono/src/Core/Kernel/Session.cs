/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Pako Jabber-bot. Bbodio's Lab.                                                *
 * Copyright. All rights reserved � 2007-2008 by Klichuk Bogdan (Bbodio's Lab)   *
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
using Core.Xml;
using Core.Conference;
using Core.DataBase;
using System.Threading;
using Core.Other;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Mono.Data.SqliteClient;
using agsXMPP.protocol.iq.time;

namespace Core.Kernel
{
   public class Session
    {
       string dir_langs;
       string dir_config;
       string dir_ams;
       string dir_plugs;
       string dir_censor;
       string dir_va;
       string dir_vl;
       string dir_temp;
       string dir_access;
       public bool turn_off = false;
       XmppClientConnection m_con;
       ReplyGenerator m_resphnd;
       Config m_config;
       Hashtable m_mucs;
       AccessManager m_acsh;
       AutoMucManager m_ams;
       PHandler m_plugs;
       Censor m_censor;
       int mucs_count;
       VipAccess m_va;
       VipLang m_vl;
       CalcHandler m_calch;
       string m_version;
       ArrayList m_roster = new ArrayList();
       Tempdb temp;
       int m_rects;
       string dir_error;
       string os_version;
       ErrorLoger m_el;
       SessionHandler Sh;
       object[] sobjs = new object[60];


     
   /// <summary>
   /// The main class, which provides all the features of the bot, all the
   /// specification tools, connection and jabber managers, chat-rooms' managers etc.
   /// Use Session to connect and rule the bot... Good Luck ;-) 
   /// </summary>
   /// <param name="LanguageFolder"></param>
   /// <param name="PluginsFolder"></param>
   /// <param name="ConfigFile"></param>
   /// <param name="AutoMucsFile"></param>
   /// <param name="CensorFile"></param>
   /// <param name="UsersDataFile"></param>
   /// <param name="AccessFile"></param>
   /// <param name="botversion"></param>
   /// <param name="reconnects"></param>
       public Session(string botversion, int reconnects, SessionHandler sh)
       {
           for (int i = 0; i < 60; i++)
           {
               sobjs[i] = new object();
           }
           Sh = sh;
           dir_config = Utils.GetPath("config");
           m_config = new Config(dir_config);
           @out.Debug = m_config.Debug;
           dir_va = Utils.GetPath("vipaccess");
           dir_vl = Utils.GetPath("viplang");
           dir_ams = Utils.GetPath("rooms");
           dir_langs = Utils.GetPath("lang");
           dir_censor = Utils.GetPath("censor");
           dir_plugs = Utils.GetPath("plugins");
           dir_error = Utils.GetPath("errlog");
           dir_access = Utils.GetPath("accesses");
           dir_temp = Utils.GetPath("tempdb");
           GetReady();  
           m_version = botversion;
           @out.write("Location : " + Utils.CD + "\n");
           @out.write("Working set: " + (Environment.WorkingSet).ToString() + " bytes\n");
           @out.write("NoMucsMode " + (Sh.arg_nomucs ? "ON" : "OFF"));
           @out.write("===============================================================================");
           @out.write("                                PAKO " + m_version + "                         ");
           @out.write("====================================STARTING===================================\n");
           m_rects = reconnects;
           int sqlv = int.Parse(S.Config.GetTag("sqlite"));
           Sh.LoadBase(sqlv);
           
           m_va = new VipAccess(dir_va, sqlv);
           m_vl = new VipLang(dir_vl, sqlv);
           m_censor = new Censor(dir_censor, sqlv);
           m_el = new ErrorLoger(dir_error);
           m_ams = new AutoMucManager(dir_ams);
           m_acsh = new AccessManager(dir_access);
           temp = new Tempdb(dir_temp, sqlv);
           m_con = new XmppClientConnection();
           m_calch = new CalcHandler();
           @out.exe("Jid: <" + S.Config.Jid + ">\n");
           m_con.Resource = S.Config.Jid.Resource;
           m_con.Priority = 100;
           m_con.Port = S.Config.Port;
           m_con.Server = S.Config.Jid.Server;
           m_con.Username = S.Config.Jid.User;
           m_con.Password = S.Config.Password;
           m_con.UseSSL = S.Config.UseSSL;
           m_con.UseStartTLS = S.Config.UseStartTls;
           m_con.UseCompression = S.Config.UseCompression;
           m_con.SocketConnectionType = agsXMPP.net.SocketConnectionType.Direct;
           bool connect_server = S.Config.ConnectServer != "";
           m_con.AutoResolveConnectServer = !connect_server;
           m_con.ConnectServer = connect_server ? S.Config.ConnectServer : null;
           m_resphnd = new ReplyGenerator(m_con, dir_langs, S.Config.MucMSGLimit);
           m_mucs = new Hashtable();
           mucs_count = 0;
          
   
           os_version = Utils.GetOSVersion(this)
               .Replace("\n\r", "")
               .Replace("\n", "")
               .Replace("\r", "");

     

           m_con.OnClose += delegate(object o)
           {
               OnDisconnect();
           };


           m_con.OnSocketError += delegate(object o, Exception ex)
           {
               OnDisconnect();
           };

         
 
           m_con.OnMessage += delegate(object o, agsXMPP.protocol.client.Message msg)
           {
               CommandHandler cmdh = new CommandHandler(msg, Sh, null, CmdhState.PREFIX_NULL, 1);
           };
         
         
           m_con.OnXmppConnectionStateChanged += new XmppConnectionStateHandler(OnXmppConnectionStateChanged);
           if (@out.Debug)
           {
               m_con.OnWriteXml += new XmlHandler(OnWriteXml);
               m_con.OnReadXml += new XmlHandler(OnReadXml);
           }
           m_con.OnPresence += delegate(object obj, Presence pres)
           {
               PresenceHandler pr_handler = new PresenceHandler(pres,Sh);
           };

           m_con.OnIq += delegate(object obj, IQ iq)
           {
               IQHandler iq_handler = new IQHandler();
               iq_handler.Handle(iq, S.C, OSVersion, MyVersion);
           };

           m_con.OnLogin += new ObjectHandler(OnLogin);
           m_plugs = new PHandler(dir_plugs);
         


       }

       void OnDisconnect()
       {
           if (turn_off)
               Environment.Exit(0);
           @out.write("Socket disconnected");
           Sh.Ticks = DateTime.Now.Ticks;
           if (S.Config.MaxReconnects >= m_rects || S.Config.MaxReconnects == 0)
           {
               @out.write("Reconnect in " + S.Config.ReconnectTime + " seconds "+ (S.Config.MaxReconnects != 0 ? " (" + m_rects + "/" + S.Config.MaxReconnects + ")" : "") + "\n");
               Thread.Sleep(S.Config.ReconnectTime * 1000);
               Sh.MainConnect();
           }
           else
               @out.write("Stopped...");
       }

    
 


       public void GetReady()
       {
           string error = null;
           if (!Directory.Exists(dir_langs))
               error = " *** Directory not found: \n  " + dir_langs;
           if (!Directory.Exists(dir_plugs))
               error = " *** Directory not found: \n  " + dir_plugs;
           if (!File.Exists(dir_config))
               error = " *** File not found: \n  " + dir_config;
           if (!File.Exists(dir_ams))
               error = " *** File not found: \n  " + dir_ams;
           if (!File.Exists(dir_access))
               error = " *** File not found: \n  " + dir_access;
           if (error != null)
           {
               @out.write(error);
               Console.Read();
               Environment.Exit(0);
           }
       }







       void OnXmppConnectionStateChanged(object sender, XmppConnectionState state)
       {
           @out.write("<" + DateTime.Now.Hour+" : "+DateTime.Now.Minute + " : " + DateTime.Now.Second + "> " + state.ToString() + "\n");
       }

       /// <summary>
       /// Main instance, which handles Session instance asynchronously by "lock"s
       /// </summary>
       public Session S
       {
           get
           {
               lock (sobjs[20])
               {
                   return this;
               }
           }


       }



       public void Session_DB_Close()
       {
           lock (this)
           {
               foreach (MUC m in S.MUCs.Values)
               {
                   m.SQLiteConnection.Close();
              
               }

               S.Censor.SQLiteConnection.Close();

           }
       }

       public void OnLogin(object sender)
       {

           if (!Sh.arg_nomucs)
           {
               @out.write("<" + DateTime.Now.Hour + " : " + DateTime.Now.Minute + " : " + DateTime.Now.Second + "> Joining chat-rooms\n");
               S.C.SendMyPresence();
               foreach (AutoMuc am in S.AutoMucManager.GetAMList())
               {

                   if (S.MUCs[am.Jid] == null)
                   {
                       MUC m = new MUC(S.C, am.Jid, am.Nick, am.Status, am.Language, ShowType.NONE, Sh, am.Password);
                       S.MUCs.Add(am.Jid, m);
                   }
               }
               foreach (MUC m in S.MUCs.Values)
               {
                   m.Join();
               }
           }
           @out.write("<" + DateTime.Now.Hour + " : " + DateTime.Now.Minute + " : " + DateTime.Now.Second + "> Ready for requests\n");


       }


       public void Sleep()
       {
           try { Thread.Sleep(Convert.ToInt32(Config.GetTag("interval"))); }
           catch {}
       }

   

       public int GetMucAccess(Role Role, Affiliation Affil)
       {
           int res = 0;
           switch (Role)
           {
               case Role.participant:
                   res += 15;
                   break;
               case Role.moderator:
                   res += 25;
                   break;
               case Role.visitor:
                   res += 10;
                   break;
               case Role.none:
                   res = 0;
                   break;
           }

           switch (Affil)
           {
               case Affiliation.member:
                   res += 15;
                   break;
               case Affiliation.admin:
                   res += 35;
                   break;
               case Affiliation.none:
                   res += 0;
                   break;
               case Affiliation.owner:
                   res += 55;
                   break;
           }

           return res;

       }

      


       public int? GetAccess(Message m_msg, string entity, MUC muc)
       {
           MUser user = null;
           Message msg = new Message();
           msg.From = m_msg.From;
           msg.To = m_msg.To;
           msg.Body = m_msg.Body;
           msg.Type = m_msg.Type;

           bool is_muser = muc != null;

           if (is_muser)
           {
               if (muc.UserExists(entity))
                   {
                       user = muc.GetUser(entity);

                   }
                   else
                   {
                       is_muser = false;
                       user = null;
                       msg.From = new Jid(entity);
                   }

           }

           Jid Jid = is_muser ?
               user.Jid :
               new Jid(msg.From.Bare);

           int? access = null;

           if (S.Config.BotAdmin(Jid))
           {
               access = 100;
               return access;
           }
           int? va = Sh.S.VipAccess.GetAccess(Jid);
           if (va != null)
               return va;
           if (is_muser)
           {
               int? v_access = muc.VipAccess.GetAccess(Jid);
               if (v_access != null)
                   return v_access;
           } 
           if (is_muser)
               return  GetMucAccess(user.Role, user.Affiliation);
           return null;

 
       }


       public int? GetAccess(Message msg, MUser user, MUC muc)
       {

           bool is_muser = user != null;
           Jid Jid = is_muser ?
               user.Jid :
               new Jid(msg.From.Bare);

           int? access = null;

           if (S.Config.BotAdmin(Jid))
           {
               access = 100;
               return access;
           }

           int? va = Sh.S.VipAccess.GetAccess(Jid);
           if (va != null)
               return va;
           if (muc != null)
           {
               int? v_access = muc.VipAccess.GetAccess(Jid);
               if (v_access != null)
                   return v_access;
           }
           if (is_muser)
               return GetMucAccess(user.Role, user.Affiliation);
           return null;


       }

       /// <summary>
       /// Get MUC jid by a string representing a real numeric index (>= 1) of the MUC in the list.
       /// </summary>
       /// <param name="RealIndexString"></param>
       /// <returns></returns>
       public Jid GetMUCJid(string RealIndexString)
       {
           try
           {
               short index = Convert.ToInt16(RealIndexString);
               @out.exe("TRY_EMUL_CONVERT");
               return (index <= MUCs.Keys.Count && --index > -1) ?
                   (Jid)new ArrayList(MUCs.Keys)[index] : null;
           }
           catch { return null; } 
       }


       public int? GetAccess(Presence pres, MUC muc)
       {
          
           bool is_muser =  pres.MucUser != null;
           bool jid_visible = false;
           if (is_muser)
               jid_visible = pres.MucUser.Item.Jid != null;

           Jid Jid = is_muser ? 
               jid_visible ? 
               pres.MucUser.Item.Jid :
               pres.From :
               new Jid(pres.From.Bare);

 
           int? access = null;

           if (S.Config.BotAdmin(Jid))
           {
               access = 100;
               return access;
           }

           int? va = Sh.S.VipAccess.GetAccess(Jid);
           if (va != null)
               return va;
           if (is_muser)
           {
               int? v_access = muc.VipAccess.GetAccess(Jid);
               if (v_access != null)
                   return v_access;
           }
           return is_muser ? GetMucAccess(pres.MucUser.Item.Role, pres.MucUser.Item.Affiliation) : 0;


       }


       public void OnWriteXml(object sender, string xml)
       {
           @out.exe("SEND: " + xml+"\n");
       }


       public void OnReadXml(object sender, string xml)
       {
         @out.exe("GOT: " + xml+"\n");
       }

       public void Open()
       {
           S.C.Open();
       }
          



        public void Exit(Response resp, string message)
       {
           Presence pr = new Presence();

           pr.Type = PresenceType.unavailable;
           if (resp.MUser != null)
               pr.Status = resp.MUser.Jid.User + " -> " + message;
           else
               pr.Status = resp.Msg.From.User + " -> " + message;
           foreach (MUC m in S.MUCs.Values)
           {
               pr.To = m.Jid;
               m.SQLiteConnection.Close();
               S.C.Send(pr);
           }
           pr.To = null;
           S.C.Send(pr);
           S.C.Close();
           Session_DB_Close();
           Sh.Defs.SQLiteConnection.Close();
           Sh.S.Censor.SQLiteConnection.Close();
           Sh.S.Tempdb.SQLiteConnection.Close();



       }


       public void Exit(Response resp)
       {
           Presence pr = new Presence();
           pr.Type = PresenceType.unavailable;
           pr.Status = resp.f("muc_leave");
           foreach (MUC m in S.MUCs.Values)
           {
               pr.To = m.Jid;
               m.SQLiteConnection.Close();
               S.C.Send(pr);
           }
           pr.To = null;

           S.C.Send(pr);
           S.C.Close();
           Session_DB_Close();
           Sh.Defs.SQLiteConnection.Close();
           Sh.S.Censor.SQLiteConnection.Close();
           Sh.S.Tempdb.SQLiteConnection.Close();
       }
       public Config Config
       {
           get { lock (sobjs[1]) { return S.m_config; } }
           set { lock (sobjs[1]) { S.m_config = value; } }
       }
       public VipAccess VipAccess
       {
           get { lock (sobjs[11]) { return S.m_va; } }
           set { lock (sobjs[11]) { S.m_va = value; } }
       }
       public VipLang VipLang
       {
           get { lock (sobjs[10]) { return S.m_vl; } }
           set { lock (sobjs[10]) { S.m_vl = value; } }
       }

       public PHandler PluginHandler
       {
           get { lock (sobjs[12]) { return m_plugs; } }
           set { lock (sobjs[12]) { m_plugs = value; } }
       }

       public Tempdb Tempdb
       {
           get { lock (temp) { return temp; } }
           set { lock (temp) { temp = value; } }
       }

       public CalcHandler CalcHandler
       {
           get { lock (sobjs[14]) { return S.m_calch; } }
           set { lock (sobjs[14]) { S.m_calch = value; } }
       }



       public string OSVersion
       {
           get { lock (sobjs[55]) { return S.os_version; } }
           set { lock (sobjs[55]) { S.os_version = value; } }
       }

       public string MyVersion
       {
           get { lock (sobjs[56]) { return S.m_version; } }
           set { lock (sobjs[56]) { S.m_version = value; } }
       }


       public AutoMucManager AutoMucManager
       {
           get { lock (sobjs[5]) { return S.m_ams; } }
           set { lock (sobjs[5]) { S.m_ams = value; } }
       }


       public AccessManager AccessManager
       {
           get { lock (sobjs[31]) { return S.m_acsh; } }
           set { lock (sobjs[31]) { S.m_acsh = value; } }
       }

       public XmppClientConnection C
       {
           get { lock (sobjs[7]) { return S.m_con; } }
           set { lock (sobjs[7]) { S.m_con = value; } }
       }

       public Censor Censor
       {
           get { lock (sobjs[2]) { return m_censor; } }
           set { lock (sobjs[2]) { m_censor = value; } }
       }

       public ErrorLoger ErrorLoger
       {
           get { lock (sobjs[49]) { return S.m_el; } }
           set { lock (sobjs[49]) { S.m_el = value; } }
       }


       public ReplyGenerator Rg
       {
           get { lock (sobjs[3]) { return S.m_resphnd; } }
           set { lock (sobjs[3]) { S.m_resphnd = value; } }
       }

       public ArrayList RosterJids
       {
           get { lock (sobjs[52]) { return S.m_roster; } }
           set { lock (sobjs[52]) { S.m_roster = value; } }
       }


       public Hashtable MUCs
       {
           get { lock (sobjs[0]) { return S.m_mucs; } }
           set { lock (sobjs[0]) { S.m_mucs = value; } }
       }

       public MUC GetMUC(Jid Jid)
       {
           lock (sobjs[1])
           {
               return (MUC)S.MUCs[new Jid(Jid.Bare.ToLower())];
           }
       }


     




       public bool AddMUC(MUC muc)
       {
           lock (sobjs[1])
           {

               if (S.GetMUC(muc.Jid) == null)
                   {
                       S.MUCs.Add(muc, muc.Jid);
                       mucs_count++;
                       return true;
                   }
            
               return false;
           }
       }


       public bool DelMUC(Jid Jid)
       {
           lock (sobjs[1])
           {
               foreach (MUC muc in S.MUCs.Values)
               {
                   if (muc.Jid.Bare == Jid.Bare)
                   {
                       mucs_count--;
                       S.MUCs.Remove(muc.Jid);
                   return true;
                   }
               }
               return false;
           }
       }
       




    }
}
