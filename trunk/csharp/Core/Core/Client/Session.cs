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
using Core.Manager;
using Core.Conference;
using System.Threading;
using Core.Special;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Mono.Data.SqliteClient;
using agsXMPP.protocol.iq.time;

namespace Core.Client
{
   public class Session
    {
       string dir_langs;
       string dir_config;
       string dir_ams;
       string dir_plugs;
       string dir_censor;
       string dir_usersdata;
       string dir_access;
       XmppClientConnection m_con;
       ReplyGenerator m_resphnd;
       Config m_config;
       Hashtable m_mucs;
       AcessManager m_acsh;
       AutoMucManager m_ams;
       PHandler m_plugs;
       Censor m_censor;
       int mucs_count;
       VipManager m_ud;
       CalcHandler m_calch;
       string m_version;
       ArrayList m_roster = new ArrayList();
       Tempdb temp;
       int m_rects;
       string dir_error;
       string os_version;
       ErrorLoger m_el;
       string m_dir;
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
       public Session(string LanguageFolder, string PluginsFolder, string ConfigFile, string AutoMucsFile, string CensorFile, string UsersDataFile, string AccessFile,string ErrorFile, string TempDBFile, string botversion, int reconnects,  SessionHandler sh)
       {
           for (int i = 0; i < 60; i++)
           {
               sobjs[i] = new object();
           }

           Sh = sh;
           m_version = botversion;
           m_dir = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
           Console.WriteLine("Location : " + m_dir + "\n");
           Console.WriteLine("Working set: " + (Environment.WorkingSet).ToString() + " bytes\n");
           Console.WriteLine("DebugMode " + (Sh.arg_debug ? "ON" : "OFF"));
           Console.WriteLine("NoMucsMode " + (Sh.arg_nomucs ? "ON" : "OFF"));
           Console.WriteLine("===============================================================================");
           Console.WriteLine("                                PAKO " + m_version + "                         ");
           Console.WriteLine("====================================STARTING===================================\n");
           dir_usersdata = UsersDataFile;
           dir_config = ConfigFile;
           dir_ams = AutoMucsFile;       
           dir_langs = LanguageFolder;
           dir_censor = CensorFile;
           dir_plugs = PluginsFolder;
           dir_error = ErrorFile;
           dir_access = AccessFile;
           m_rects = reconnects;
          
           m_ud = new VipManager(dir_usersdata);
           m_censor = new Censor(CensorFile);
           m_config = new Config(dir_config);
           m_el = new ErrorLoger(dir_error);
           m_ams = new AutoMucManager(dir_ams);
           m_acsh = new AcessManager(AccessFile);
           temp = new Tempdb(TempDBFile);
           m_con = new XmppClientConnection();
           m_calch = new CalcHandler();
           @out.exe("Jid: <" + S.Config.Jid + ">\n");
           m_con.Resource = S.Config.Jid.Resource;
           m_con.Priority = 100;
           m_con.Port = S.Config.Port;
           m_con.Server = S.Config.Jid.Server;
           m_con.Username = S.Config.Jid.User;
           m_con.Password = S.Config.Password;
           m_con.AutoResolveConnectServer = true;
           m_con.UseSSL = S.Config.UseSSL;
           m_con.UseStartTLS = S.Config.UseStartTls;
           m_con.UseCompression = S.Config.UseCompression;
           m_con.SocketConnectionType = agsXMPP.net.SocketConnectionType.Direct;
           m_con.ConnectServer = null;
           m_resphnd = new ReplyGenerator(m_con, dir_langs, S.Config.MSGLimit);
           m_mucs = new Hashtable();
           mucs_count = 0;
          

           os_version = GetOSVersion()
               .Replace("\n\r", "")
               .Replace("\n", "")
               .Replace("\r", "");

           m_con.ClientSocket.OnDisconnect += delegate(object o)
           {
               Console.WriteLine("Socket disconnected");
               m_rects++;
               if (S.Config.MaxReconnects >= m_rects)
               {
                   Console.WriteLine("Reconnect in " + S.Config.ReconnectTime + " seconds (" + m_rects + "/" + S.Config.MaxReconnects + ")\n");
                   Thread.Sleep(S.Config.ReconnectTime * 1000);
                   Sh.MainConnect();
               }
               else
                   Console.WriteLine("Stopped...");
           };





           m_con.ClientSocket.OnError += delegate(object o, Exception ex)
           {
               Console.WriteLine("Socket error occured:\n");
               Console.WriteLine(ex.ToString() + "\n");
               m_rects++;
               if (S.Config.MaxReconnects >= m_rects)
               {
                   Console.WriteLine("Reconnect in " + S.Config.ReconnectTime + " seconds (" + m_rects + "/" + S.Config.MaxReconnects + ")\n");
                   Thread.Sleep(S.Config.ReconnectTime * 1000);
                   Sh.MainConnect();
               }
               else
                   Console.WriteLine("Stopped...");

           };

           m_con.OnMessage += delegate(object o, agsXMPP.protocol.client.Message msg)
           {
               CommandHandler cmdh = new CommandHandler(msg, Sh);
           };
         
           m_con.OnRosterItem += new XmppClientConnection.RosterHandler(OnItem);
         
           m_con.OnXmppConnectionStateChanged += new XmppConnectionStateHandler(OnXmppConnectionStateChanged);
           if (Sh.arg_debug)
           {
               m_con.OnWriteXml += new XmlHandler(OnWriteXml);
               m_con.OnReadXml += new XmlHandler(OnReadXml);
           }
           m_con.OnPresence += new PresenceHandler(OnPresence);
           m_con.OnIq += new IqHandler(OnIQ);
           m_con.OnLogin += new ObjectHandler(OnLogin);
           m_plugs = new PHandler(dir_plugs);
         


       }







       public string GetOSVersion()
       {
           switch(Utils.OS)
           {
               case Platform.Windows:
                    Stdior std = new Stdior(); return std.Execute("ver");
               case Platform.Unix:
                    std = new Stdior(); return std.Execute("uname -sr");
               case Platform.Linux:
                    std = new Stdior(); return std.Execute("uname -a");
               default:
                    return "<unknown>";
           }
            
       }



       void OnXmppConnectionStateChanged(object sender, XmppConnectionState state)
       {
           Console.WriteLine("<" + DateTime.Now.Hour+":"+DateTime.Now.Minute + ":" + DateTime.Now.Second + "> " + state.ToString() + "\n");
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
               Console.WriteLine("<" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "> Joining chat-rooms\n");
               S.C.SendMyPresence();
               foreach (AutoMuc am in S.AutoMucManager.GetAMList())
               {

                   if (S.MUCs[am.Jid] == null)
                   {
                       MUC m = new MUC(S.C, am.Jid, am.Nick, am.Status, am.Language, ShowType.NONE, S.Censor.SQLiteConnection);
                       S.MUCs.Add(am.Jid, m);
                   }
               }
               foreach (MUC m in S.MUCs.Values)
               {
                   m.Join();
               }
           }
           Console.WriteLine("<" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "> Ready for requests\n");


       }



       public void OnIQ(object sender, IQ iq)
       {

           if (iq.Query != null)
           {
               if (iq.Type == IqType.get)
               {
                   if (iq.Query.GetType() == typeof(agsXMPP.protocol.iq.version.Version))
                   {
                       VersionIq _iq = new VersionIq();
                       _iq.To = iq.From;
                       _iq.Id = iq.Id;
                       _iq.Type = IqType.result;
                       _iq.Query.Name = "Pako bot";
                       _iq.Query.Ver = m_version + " stable";
                       _iq.Query.Os = "(C# .NET/Mono) on base: " + os_version;
                       C.Send(_iq);
                   }
                   else
                       if (iq.Query.GetType() == typeof(agsXMPP.protocol.iq.time.Time))
                       {
                           TimeIq _iq = new TimeIq();
                           _iq.To = iq.From;
                           _iq.Id = iq.Id;
                           _iq.Type = IqType.result;
                           _iq.Query.Tz = DateTime.Now.ToString();
                           C.Send(_iq);
                       }
               }
           }

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
                   res += 35;
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
                   res += 45;
                   break;
           }

           return res;

       }


       public int GetAccess(Message msg, MUser user)
       {

           bool is_muser = user != null;


           Jid Jid = is_muser ?
               user.Jid :
               new Jid(msg.From.Bare);


           int access = 0;

           if (S.Config.BotAdmin(Jid))
           {
               access = 100;
               return access;
           }

           Vipuser ud = S.VipManager.GetUserData(Jid);
           if (ud != null)
           {
               if (ud.Access != null)
                   access = (int)ud.Access;
               return access;
           }
           if (is_muser)
           {
               return  GetMucAccess(user.Role, user.Affiliation);
           }
           return 0;


       }
  

       public int GetAccess(Presence pres)
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

 
           int access = 0;

           if (S.Config.BotAdmin(Jid))
           {
               access = 100;
               return access;
           }

           Vipuser ud = S.VipManager.GetUserData(Jid);
           if (ud != null)
           {
               if (ud.Access != null)
                   access = (int)ud.Access;
               return access;
           }

           access = is_muser ? GetMucAccess(pres.MucUser.Item.Role, pres.MucUser.Item.Affiliation) : 0;
           return access;


       }



       public void OnPresence(object sender,Presence pres)
       {
           Jid p_jid = pres.From;
           MUC m_muc = S.GetMUC(p_jid);
           MUser m_user = null;
           Jid Jid = pres.From;
           if (pres.MucUser != null)
               if (pres.MucUser.Item.Jid != null)
                   Jid = pres.MucUser.Item.Jid;

           switch (pres.Type)
           {
               case PresenceType.subscribe:
                   PresenceManager pm = new PresenceManager(S.C);
                   pm.Subcribe(p_jid);
                   break;
               case PresenceType.error:
                   if (m_muc != null)
                   {
               
                       if (p_jid.Resource == m_muc.MyNick)
                       {
                           S.DelMUC(p_jid);
                           foreach (Jid j in S.Config.Administartion())
                           {
                               Message _msg = new Message();
                               _msg.To = j;
                               _msg.Type = MessageType.chat;
                               _msg.Body = p_jid.Bare + " => error: " + pres.Error.GetAttribute("code") + " - " + pres.Error.Condition.ToString(); 
                               Sh.S.C.Send(_msg);
                           }
                       }
                   }
                   break;
               case PresenceType.available:



                   Vipuser ud = Sh.S.VipManager.GetUserData(Jid);
                   bool ud_lang = ud != null;
                   if (ud_lang)
                       ud_lang = ud.Language != null;
                   string lng =
                                ud_lang ?
                                ud.Language :
                                m_muc != null ?
                                m_muc.Language :
                                S.Config.Language;




                   if (pres.MucUser != null)
                   {



                       m_user = m_muc != null ? m_muc.GetUser(pres.From.Resource) : null;
                       Jid calcjid = m_muc != null ? pres.From : new Jid(pres.From.Bare);




                       S.CalcHandler.AddHandle(calcjid);
                       int access = GetAccess(pres);

                       string time;
                       if (m_muc != null)
                           @out.exe(m_user != null ? "[" + p_jid.User + "]*** " + p_jid.Resource + " is now " + pres.Show.ToString().Replace("NONE", "Online") : "[" + p_jid.User + "]*** " + p_jid.Resource + " enters the room as " + pres.MucUser.Item.Affiliation + "/" + pres.MucUser.Item.Role);
                       time = m_user != null ? m_user.EnterTime : DateTime.Now.ToString();
                       MUser user = new MUser(
                       pres.From.Resource,
                       Jid,
                       pres.MucUser.Item.Role,
                       pres.MucUser.Item.Affiliation,
                       pres.Status,
                       pres.Show,
                       lng,
                       time,
                       access
                       );

                       S.GetMUC(p_jid).SetUser(m_user, user);

                       if (m_muc != null)
                       {
                           Response r = new Response(Sh.S.Rg.GetResponse(lng));
                           if (pres.Status != null)
                           {
                               string censored = S.GetMUC(p_jid).IsCensored(pres.Status);
                               if (censored != null)
                               {
                                   @out.exe(m_muc.KickableForCensored(user).ToString());
                                   if (m_muc.KickableForCensored(user))
                                       m_muc.Kick(user.Nick, censored);
                                   else
                                   {

                                       Message msg = new Message();
                                       r.Msg.Body = pres.Status;
                                       r.Msg.From = pres.From;
                                       r.Msg.Type = MessageType.groupchat;
                                       r.Reply(r.FormatPattern("kick_censored_moderator"));
                                   }
                               }
                           }

                           if (m_user == null)
                           {

                               string data = Tempdb.Greet(Jid, m_muc.Jid);
                               if (data != null)
                               {
                                   r.Msg = new Message();
                                   r.Msg.From = pres.From;
                                   r.Msg.Type = MessageType.groupchat;
                                   r.Reply(data);
                               }



                           }


                           ArrayList ar = S.Tempdb.CheckAndAnswer(p_jid);
                           if (ar.Count > 0)
                           {
                               foreach (string[] phrase in ar)
                               {
                                   Jid _sender = new Jid(phrase[2]);
                                   string s_sender = phrase[2];
                                   s_sender = _sender.Resource;
                                   r.Msg = new Message();
                                   r.Msg.From = pres.From;
                                   r.Msg.Type = MessageType.chat;

                                   r.Reply(r.FormatPattern("said_to_tell", s_sender, phrase[1]));
                                   r.Msg.Type = MessageType.groupchat;
                                   r.Reply(r.FormatPattern("private_notify"));


                               }
                           }

                       }
                   }
                   else
                   {
                       RosterJids.Add(pres.From);

                   }
                   break;

               case PresenceType.unavailable:
                   if (pres.MucUser != null)
                   {
                       m_user = m_muc != null ? m_muc.GetUser(pres.From.Resource) : null;
                       
                       if (m_user != null)
                       {
                           S.GetMUC(p_jid).DelUser(m_user);
                           @out.exe("[" + p_jid.User + "]*** " + p_jid.Resource + " leave the room");
                       }
                       else
                           return;
                       if (p_jid.Resource == m_muc.MyNick)
                       {
                           if (pres.MucUser != null)
                           {
                               if (pres.MucUser.Item.Nickname != null)
                               {
                                   if (pres.MucUser.Item.Nickname != p_jid.Resource)
                                       S.GetMUC(p_jid).MyNick = pres.MucUser.Item.Nickname;
                               }
                               else
                               {
                     
                                   S.DelMUC(p_jid);
                                   foreach (Jid j in S.Config.Administartion())
                                   {
                                     
                               
                                       Message _msg = new Message();
                                       _msg.To = j;
                                       _msg.Type = MessageType.chat;
                                       string data = p_jid.Bare + " => " + pres.Type.ToString();
                                      if (pres.HasTag("x"))
                                       if (pres.SelectSingleElement("x").HasTag("status"))
                                           if (pres.SelectSingleElement("x").SelectSingleElement("status").HasAttribute("code"))
                                               data += " (" + pres.SelectSingleElement("x").SelectSingleElement("status").GetAttribute("code") + ")";
                                       _msg.Body = data;
                                       Sh.S.C.Send(_msg);
                                   }
                               }
                           }
                       }
                   }
                   break;
           }     
       }

        public void OnItem(object sender, RosterItem item)
        {
           S.CalcHandler.AddHandle(item.Jid);
           Presence pres = new Presence();
           pres.To = item.Jid;
           pres.Status = S.Config.Status;
           pres.Show = ShowType.NONE;
           pres.Type = PresenceType.available;
           S.C.Send(pres);

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
           pr.Status = resp.FormatPattern("muc_leave");
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
       public VipManager VipManager
       {
           get { lock (sobjs[10]) { return S.m_ud; } }
           set { lock (sobjs[10]) { S.m_ud = value; } }
       }

     /*  public Definitions Defs
       {
           get { lock (sobjs[26]) { return S.m_defs; } }
           set { lock (sobjs[26]) { S.m_defs = value; } }
       }
       */

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


       public AcessManager AccessManager
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
               foreach (MUC muc in S.MUCs.Values)
               {
                   if (muc.Jid.ToString() == Jid.Bare)
                       return muc;
               }
               return null;
           }
       }


     

       public msgType GetTypeOfMsg(Message msg, MUser user)
       {
           if (msg.Error == null)
           {
               if  ((msg.XDelay == null) && (msg.From.Resource != null))
               {
                   if (msg.Body != null)
                   {
                       if (user != null)
                       {
                           return msgType.MUC;
                       }
                       else
                       {
                           return msgType.Roster;
                       }
                   }
                   else
                   {
  
                       return msgType.Empty;
                   }
               }
               else
                   return msgType.None;
           }
           else
               return msgType.Error;
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
