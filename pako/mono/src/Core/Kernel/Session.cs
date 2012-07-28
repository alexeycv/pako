/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Pako Jabber-bot.                                                              *
 * Copyright. All rights reserved © 2007-2008 by Klichuk Bogdan (Bbodio's Lab)   *
 * Copyright. All rights reserved © 2009-2012 by Alexey Bryohov                  *
 * Patches: Mattias Aslund                                                       *
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
using agsXMPP.protocol.iq.disco;

namespace Core.Kernel
{
	public class Session
	{
		string dir_langs;
		string dir_config;
		string dir_ams;
		string dir_plugs;
		string dir_censor;
		string dir_vrcensor;
		//version and resource censor
		string dir_va;
		string dir_vl;
		string dir_temp;
		string dir_access;
		string dir_log;
		string dir_html_log;
		string dir_html_priv_log;
		public bool turn_off = false;
		XmppClientConnection m_con;
		
		// added by Mattias Aslund
		DiscoManager m_discoManager;
		//
		
		ReplyGenerator m_resphnd;
		Config m_config;
		Dictionary<Jid, MUC> m_mucs;
		AccessManager m_acsh;
		AutoMucManager m_ams;
		PHandler m_plugs;
		Censor m_censor;
		VrCensor m_vrcensor;
		//version and resource censor
		int mucs_count;
		VipAccess m_va;
		VipLang m_vl;
		CalcHandler m_calch;
		ArrayList m_roster = new ArrayList ();
		Tempdb temp;
		int m_rects;
		string dir_error;
		string os_version;
		ErrorLoger m_el;
		SessionHandler Sh;
		Logger _logger;
		Logger _seenlogger;
		Logger _htmllogger;
		Logger _htmlPrivlogger;

		Hashtable _justJoined;
		// Just joined MUCs. For misc.join handler.
		Hashtable _justJoined_Mucs;
		// Just joined MUCs. For misc.join handler.
		List<object> _messageTransformers = null;
		
		Hashtable _customObjects;

		object[] sobjs = new object[70];


		/// <summary>
		/// Private empty constructor
		/// </summary>
		private Session ()
		{
			
		}

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
		/// <param name="botversion"></param>           @out.write("Working set: " + (Environment.WorkingSet).ToString() + " bytes\n");
		/// <param name="reconnects"></param>
		public Session (int reconnects, SessionHandler sh) : this()
		{
			for (int i = 0; i < 70; i++) {
				sobjs[i] = new object ();
			}
			
			SessionHandler _sh_reserved = sh;
			
			_customObjects = new Hashtable();
			
			_messageTransformers = new List<object> ();
			
			_justJoined = new Hashtable ();
			_justJoined_Mucs = new Hashtable ();
			
			Sh = sh;
			dir_config = Utils.GetPath ("config");
			m_config = new Config (dir_config);
			@out.Debug = m_config.Debug;
			dir_va = Utils.GetPath ("vipaccess");
			dir_vl = Utils.GetPath ("viplang");
			dir_ams = Utils.GetPath ("rooms");
			dir_langs = Utils.GetPath ("lang");
			dir_censor = Utils.GetPath ("censor");
			dir_vrcensor = Utils.GetPath ("vrcensor");
			//version and resourse censor
			dir_plugs = Utils.GetPath ("plugins");
			dir_error = Utils.GetPath ("errlog");
			dir_access = Utils.GetPath ("accesses");
			dir_temp = Utils.GetPath ("tempdb");
			dir_log = Utils.GetPath ("logdb");
			dir_html_log = Utils.GetPath ("logmuc");
			dir_html_priv_log = Utils.GetPath ("logprivate");
			GetReady ();
			Utils.Sh = sh;
			Utils.Sh.S = this;
			@out.write ("Location : " + Utils.CD + "");
			@out.write ("Working set: " + (Environment.WorkingSet).ToString () + " bytes");
			@out.write ("Version: " + (Environment.Version).ToString () + "");
			@out.write ("NoMucsMode " + (Sh.arg_nomucs ? "ON" : "OFF\n"));
			@out.write ("===============================================================================");
			@out.write ("     PAKO " + Utils.Bot["version"] + "     ");
			@out.write ("====================================STARTING===================================\n");
			m_rects = reconnects;
			
			@out.write ("<" + DateTime.Now.Hour + " : " + DateTime.Now.Minute + " : " + DateTime.Now.Second + "> " + "Initializing databases and handlers...");
			
			int sqlv = int.Parse (S.Config.GetTag ("sqlite"));
			try {
				Sh.LoadBase (sqlv);
				m_va = new VipAccess (dir_va, sqlv);
				m_vl = new VipLang (dir_vl, sqlv);
				m_censor = new Censor (dir_censor, sqlv);
				m_vrcensor = new VrCensor (dir_vrcensor, sqlv);
				m_el = new ErrorLoger (dir_error);
				m_ams = new AutoMucManager (dir_ams);
				m_acsh = new AccessManager (dir_access);
				temp = new Tempdb (dir_temp, sqlv);
				m_con = new XmppClientConnection ();
				m_calch = new CalcHandler ();
				_logger = new Logger (dir_log + "log.db", sqlv, "chatlog");
				_seenlogger = new Logger (dir_log + "seen.db", sqlv, "seenlog");
				_htmllogger = new Logger (dir_html_log, sqlv, "muclog");
				_htmlPrivlogger = new Logger (dir_html_priv_log, sqlv, "muclog");
				
				@out.write ("<" + DateTime.Now.Hour + " : " + DateTime.Now.Minute + " : " + DateTime.Now.Second + "> " + "Handlers and databases ready.");
			} catch {
				@out.write ("<" + DateTime.Now.Hour + " : " + DateTime.Now.Minute + " : " + DateTime.Now.Second + "> " + "Handlers and databases NOT ready.");
			}
			
			@out.exe ("Jid: <" + S.Config.Jid + ">\n");
			m_con.Resource = S.Config.Jid.Resource;
			m_con.Priority = 100;
			m_con.Port = S.Config.Port;
			m_con.Server = S.Config.Jid.Server;
			m_con.Username = S.Config.Jid.User;
			m_con.Password = S.Config.Password;
			m_con.UseSSL = S.Config.UseSSL;
			m_con.UseStartTLS = S.Config.UseStartTls;
			m_con.UseCompression = S.Config.UseCompression;
			m_con.EnableCapabilities = true;
			m_con.Capabilities.Node = "http://pako.googlecode.com";
			m_con.KeepAlive = true;
			m_con.KeepAliveInterval = 60;
			m_con.SocketConnectionType = agsXMPP.net.SocketConnectionType.Direct;
			bool connect_server = S.Config.ConnectServer != "";
			m_con.AutoResolveConnectServer = !connect_server;
			m_con.ConnectServer = connect_server ? S.Config.ConnectServer : null;
			m_resphnd = new ReplyGenerator (m_con, dir_langs, S.Config.MucMSGLimit);
			m_mucs = new Dictionary<Jid, MUC> ();
			mucs_count = 0;
			
			// added by Mattias Aslund
			
			//Register a DiscoManager that will automatically respond to any
			//disco#info requests.
			//Plugins can add feature support by adding features in their Main.Start()
            m_con.DiscoInfo.AddIdentity(new DiscoIdentity("pc", "Pako", "client"));
            m_con.DiscoInfo.AddFeature(new DiscoFeature(agsXMPP.Uri.DISCO_INFO));
            //m_con.DiscoInfo.AddFeature(new DiscoFeature(agsXMPP.Uri.DISCO_ITEMS));
            m_con.DiscoInfo.AddFeature(new DiscoFeature(agsXMPP.Uri.MUC));
            m_discoManager = new DiscoManager(m_con);
			
			// end of code added by Mattias Aslund
			
			os_version = Utils.Bot["os"].Replace ("\n\r", "").Replace ("\n", "").Replace ("\r", "");
			
			
			m_con.OnClose += delegate(object o) { OnDisconnect (); };
			
			
//@out.write("SOCKET ERROR \n" + ex.Message);
			m_con.OnSocketError += delegate(object o, Exception ex) { OnDisconnect (); };
			
			m_con.OnMessage += delegate(object o, agsXMPP.protocol.client.Message msg) {
				CommandHandler cmdh = new CommandHandler (msg, Sh, null, CmdhState.PREFIX_NULL, 1);
				// Plugins handlers
				foreach (DictionaryEntry _d in Sh.S.PluginHandler.Plugins) {
					object _plugin = _d.Value;
					if (((IPlugin)_plugin).SubscribeMessages)
						((IPlugin)_plugin).CommandHandler (msg, Sh, null, CmdhState.PREFIX_NULL, 1);
				}
			};
			
			
			m_con.OnXmppConnectionStateChanged += new XmppConnectionStateHandler (OnXmppConnectionStateChanged);
			
			m_con.OnWriteXml += new XmlHandler (OnWriteXml);
			m_con.OnReadXml += new XmlHandler (OnReadXml);
			
			
			m_con.OnPresence += delegate(object obj, Presence pres) {
				PresenceHandler pr_handler = new PresenceHandler (pres, Sh);
				// Plugins handlers
				foreach (DictionaryEntry _d in Sh.S.PluginHandler.Plugins) {
					object _plugin = _d.Value;
					if (((IPlugin)_plugin).SubscribePresence) {
						((IPlugin)_plugin).PresenceHandler (pres, Sh);
					}
				}
			};
			
			m_con.OnIq += delegate(object obj, IQ iq) {
				IQHandler iq_handler = new IQHandler ();
				iq_handler.Handle (iq, S.C);
				// Plugins handlers
				foreach (DictionaryEntry _d in Sh.S.PluginHandler.Plugins) {
					object _plugin = _d.Value;
					if (((IPlugin)_plugin).SubscribeIq)
						((IPlugin)_plugin).IqHandler (iq, S.C);
				}
			};
			
			m_con.OnLogin += new ObjectHandler (OnLogin);
			m_plugs = new PHandler (dir_plugs, Sh);
		}

		void OnDisconnect ()
		{
			if (turn_off)
				Environment.Exit (0);
			try {
				@out.write ("Socket disconnected");
				Sh.Ticks = DateTime.Now.Ticks;
				if (S.Config.MaxReconnects >= m_rects || S.Config.MaxReconnects == 0) {
					@out.write ("Reconnect in " + S.Config.ReconnectTime + " seconds " + (S.Config.MaxReconnects != 0 ? " (" + m_rects + "/" + S.Config.MaxReconnects + ")" : "") + "\n");
					Thread.Sleep (S.Config.ReconnectTime * 1000);
					Sh.MainConnect ();
				} else
					@out.write ("Stopped...");
			} catch (Exception err) {
				Environment.Exit (0);
			}
		}





		public void GetReady ()
		{
			string error = null;
			if (!Directory.Exists (dir_langs))
				error = " *** Directory not found: \n  " + dir_langs;
			if (!Directory.Exists (dir_plugs))
				error = " *** Directory not found: \n  " + dir_plugs;
			if (!File.Exists (dir_config))
				error = " *** File not found: \n  " + dir_config;
			if (!File.Exists (dir_ams))
				error = " *** File not found: \n  " + dir_ams;
			if (!File.Exists (dir_access))
				error = " *** File not found: \n  " + dir_access;
			if (error != null) {
				@out.write (error);
				Console.Read ();
				Environment.Exit (0);
			}
		}

		void OnXmppConnectionStateChanged (object sender, XmppConnectionState state)
		{
			@out.write ("<" + DateTime.Now.Hour + " : " + DateTime.Now.Minute + " : " + DateTime.Now.Second + "> " + state.ToString () + "\n");
		}

		/// <summary>
		/// Main instance, which handles Session instance asynchronously by "lock"s
		/// </summary>
		public Session S {
			get {
				lock (sobjs[20]) {
					return this;
				}
			}
		}





		public void Session_DB_Close ()
		{
			lock (this) {
				foreach (MUC m in S.MUCs.Values) {
					m.SQLiteConnection.Close ();
					
				}
				
				S.Censor.SQLiteConnection.Close ();
				
			}
		}

		public void OnLogin (object sender)
		{
			@out.write ("<" + DateTime.Now.Hour + " : " + DateTime.Now.Minute + " : " + DateTime.Now.Second + "> " + "Login is OK.");
			if (!Sh.arg_nomucs) {
				@out.write ("<" + DateTime.Now.Hour + " : " + DateTime.Now.Minute + " : " + DateTime.Now.Second + "> Joining chat-rooms\n");
				S.C.SendMyPresence ();
				foreach (AutoMuc am in S.AutoMucManager.GetAMList ()) {
					if (!S.MUCs.ContainsKey (am.Jid)) {
						MUC m = new MUC (S.C, am.Jid, am.Nick, am.Status, am.Language, ShowType.NONE, Sh, am.Password);
						S.MUCs.Add (am.Jid, m);
					}
				}
				//lock (this)
				//{
				Int32 _time = S.Config.MucJoinTimeout;
				foreach (MUC m in S.MUCs.Values) {
					//m.Join();
					TimedJoiner _joiner = new TimedJoiner (m, _time);
					//Thread.Sleep(5000);
					_time = _time + 1;
				}
				//}
			}
			@out.write ("<" + DateTime.Now.Hour + " : " + DateTime.Now.Minute + " : " + DateTime.Now.Second + "> Ready for requests\n");
			
			
		}


		public void Sleep ()
		{
			try {
				Thread.Sleep (Convert.ToInt32 (Config.GetTag ("interval")));
			} catch {
			}
		}



		public int GetMucAccess (Role Role, Affiliation Affil)
		{
			int res = 0;
			try {
				switch (Role) {
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
				
				switch (Affil) {
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
			} catch (Exception err) {
				res = 0;
			}
			return res;
		}

		public int? GetAccess (Message m_msg, string entity, MUC muc)
		{
			try {
				MUser user = null;
				Message msg = new Message ();
				msg.From = m_msg.From;
				msg.To = m_msg.To;
				msg.Body = m_msg.Body;
				msg.Type = m_msg.Type;
				
				bool is_muser = muc != null;
				
				if (is_muser) {
					if (muc.UserExists (entity)) {
						user = muc.GetUser (entity);
						
					} else {
						is_muser = false;
						user = null;
						msg.From = new Jid (entity);
					}
					
				}
				
				Jid Jid = is_muser ? user.Jid : new Jid (msg.From.Bare);
				
				int? access = null;
				
				if (S.Config.BotAdmin (Jid)) {
					access = 100;
					return access;
				}
				int? va = Sh.S.VipAccess.GetAccess (Jid);
				if (va != null)
					return va;
				if (is_muser) {
					int? v_access = muc.VipAccess.GetAccess (Jid);
					if (v_access != null)
						return v_access;
				}
				if (is_muser)
					return GetMucAccess (user.Role, user.Affiliation);
				//return null;	
				return 0;
			} catch (Exception err) {
				return 0;
			}
		}


		public int? GetAccess (Message msg, MUser user, MUC muc)
		{
			try {
				bool is_muser = user != null;
				Jid Jid = is_muser ? user.Jid : new Jid (msg.From.Bare);
				
				int? access = null;
				
				if (S.Config.BotAdmin (Jid)) {
					access = 100;
					return access;
				}
				
				int? va = Sh.S.VipAccess.GetAccess (Jid);
				if (va != null)
					return va;
				if (muc != null) {
					int? v_access = muc.VipAccess.GetAccess (Jid);
					if (v_access != null)
						return v_access;
				}
				if (is_muser)
					return GetMucAccess (user.Role, user.Affiliation);
				//return null;
				return 0;
			} catch (Exception err) {
				return 0;
				//return null;
			}
			
		}

		/// <summary>
		/// Get MUC jid by a string representing a real numeric index (>= 1) of the MUC in the list.
		/// </summary>
		/// <param name="RealIndexString"></param>
		/// <returns></returns>
		public Jid GetMUCJid (string RealIndexString)
		{
			try {
				short index = Convert.ToInt16 (RealIndexString);
				@out.exe ("TRY_EMUL_CONVERT");
				return (index <= MUCs.Keys.Count && --index > -1) ? (Jid)new ArrayList (MUCs.Keys)[index] : null;
			} catch {
				return null;
			}
		}


		public int? GetAccess (Presence pres, MUC muc)
		{
			try {
				bool is_muser = pres.MucUser != null;
				bool jid_visible = false;
				if (is_muser)
					jid_visible = pres.MucUser.Item.Jid != null;
				
				Jid Jid = is_muser ? jid_visible ? pres.MucUser.Item.Jid : pres.From : new Jid (pres.From.Bare);
				
				
				int? access = null;
				
				if (S.Config.BotAdmin (Jid)) {
					access = 100;
					return access;
				}
				
				int? va = Sh.S.VipAccess.GetAccess (Jid);
				if (va != null)
					return va;
				if (is_muser) {
					int? v_access = muc.VipAccess.GetAccess (Jid);
					if (v_access != null)
						return v_access;
				}
				return is_muser ? GetMucAccess (pres.MucUser.Item.Role, pres.MucUser.Item.Affiliation) : 0;
			} catch (Exception err) {
				return 0;
			}
			
		}


		public void OnWriteXml (object sender, string xml)
		{
			if (@out.Debug) {
				@out.exe ("SEND: " + xml + "\n");
			}
		}


		public void OnReadXml (object sender, string xml)
		{
			if (@out.Debug) {
				@out.exe ("GOT: " + xml + "\n");
			}
		}

		public void Open ()
		{
			S.C.Open ();
		}

		public void Exit (Response resp, string message)
		{
			Presence pr = new Presence ();
			
			pr.Type = PresenceType.unavailable;
			if (resp.MUser != null)
				pr.Status = resp.MUser.Jid.User + " -> " + message;
			else
				pr.Status = resp.Msg.From.User + " -> " + message;
			foreach (MUC m in S.MUCs.Values) {
				pr.To = m.Jid;
				m.SQLiteConnection.Close ();
				S.C.Send (pr);
			}
			pr.To = null;
			S.C.Send (pr);
			S.C.Close ();
			Session_DB_Close ();
			Sh.Defs.SQLiteConnection.Close ();
			Sh.S.Censor.SQLiteConnection.Close ();
			Sh.S.Tempdb.SQLiteConnection.Close ();
		}


		public void Exit (Response resp)
		{
			Presence pr = new Presence ();
			pr.Type = PresenceType.unavailable;
			pr.Status = resp.f ("muc_leave");
			foreach (MUC m in S.MUCs.Values) {
				pr.To = m.Jid;
				m.SQLiteConnection.Close ();
				S.C.Send (pr);
			}
			pr.To = null;
			
			S.C.Send (pr);
			S.C.Close ();
			Session_DB_Close ();
			Sh.Defs.SQLiteConnection.Close ();
			Sh.S.Censor.SQLiteConnection.Close ();
			Sh.S.Tempdb.SQLiteConnection.Close ();
		}
		public Config Config {
			get {
				lock (sobjs[1]) {
					return S.m_config;
				}
			}
			set {
				lock (sobjs[1]) {
					S.m_config = value;
				}
			}
		}
		public VipAccess VipAccess {
			get {
				lock (sobjs[11]) {
					return S.m_va;
				}
			}
			set {
				lock (sobjs[11]) {
					S.m_va = value;
				}
			}
		}
		public VipLang VipLang {
			get {
				lock (sobjs[10]) {
					return S.m_vl;
				}
			}
			set {
				lock (sobjs[10]) {
					S.m_vl = value;
				}
			}
		}

		public PHandler PluginHandler {
			get {
				lock (sobjs[12]) {
					return m_plugs;
				}
			}
			set {
				lock (sobjs[12]) {
					m_plugs = value;
				}
			}
		}

		public Tempdb Tempdb {
			get {
				lock (temp) {
					return temp;
				}
			}
			set {
				lock (temp) {
					temp = value;
				}
			}
		}

		public CalcHandler CalcHandler {
			get {
				lock (sobjs[14]) {
					return S.m_calch;
				}
			}
			set {
				lock (sobjs[14]) {
					S.m_calch = value;
				}
			}
		}



		public string OSVersion {
			get {
				lock (sobjs[55]) {
					return S.os_version;
				}
			}
			set {
				lock (sobjs[55]) {
					S.os_version = value;
				}
			}
		}




		public AutoMucManager AutoMucManager {
			get {
				lock (sobjs[5]) {
					return S.m_ams;
				}
			}
			set {
				lock (sobjs[5]) {
					S.m_ams = value;
				}
			}
		}


		public AccessManager AccessManager {
			get {
				lock (sobjs[31]) {
					return S.m_acsh;
				}
			}
			set {
				lock (sobjs[31]) {
					S.m_acsh = value;
				}
			}
		}

		public XmppClientConnection C {
			get {
				lock (sobjs[7]) {
					return S.m_con;
				}
			}
			set {
				lock (sobjs[7]) {
					S.m_con = value;
				}
			}
		}

		public Censor Censor {
			get {
				lock (sobjs[2]) {
					return m_censor;
				}
			}
			set {
				lock (sobjs[2]) {
					m_censor = value;
				}
			}
		}

		public VrCensor VrCensor {
			get {
				lock (sobjs[64]) {
					return m_vrcensor;
				}
			}
			set {
				lock (sobjs[64]) {
					m_vrcensor = value;
				}
			}
		}

		public ErrorLoger ErrorLoger {
			get {
				lock (sobjs[49]) {
					return S.m_el;
				}
			}
			set {
				lock (sobjs[49]) {
					S.m_el = value;
				}
			}
		}

		public Logger BotLogger {
			get {
				lock (sobjs[60]) {
					return S._logger;
				}
			}
			set {
				lock (sobjs[60]) {
					S._logger = value;
				}
			}
		}

		public Logger SeenLogger {
			get {
				lock (sobjs[61]) {
					return S._seenlogger;
				}
			}
			set {
				lock (sobjs[61]) {
					S._seenlogger = value;
				}
			}
		}

		public Logger HtmlLogger {
			get {
				lock (sobjs[62]) {
					return S._htmllogger;
				}
			}
			set {
				lock (sobjs[62]) {
					S._htmllogger = value;
				}
			}
		}

		public Logger HtmlPrivLogger {
			get {
				lock (sobjs[63]) {
					return S._htmlPrivlogger;
				}
			}
			set {
				lock (sobjs[63]) {
					S._htmlPrivlogger = value;
				}
			}
		}

		public ReplyGenerator Rg {
			get {
				lock (sobjs[3]) {
					return S.m_resphnd;
				}
			}
			set {
				lock (sobjs[3]) {
					S.m_resphnd = value;
				}
			}
		}

		public ArrayList RosterJids {
			get {
				lock (sobjs[52]) {
					return S.m_roster;
				}
			}
			set {
				lock (sobjs[52]) {
					S.m_roster = value;
				}
			}
		}


		public Dictionary<Jid, MUC> MUCs {
			get {
				lock (sobjs[0]) {
					return S.m_mucs;
				}
			}
			set {
				lock (sobjs[0]) {
					S.m_mucs = value;
				}
			}
		}

		public MUC GetMUC (Jid jid)
		{
			lock (sobjs[1]) {
				if (jid != null) {
					Jid j = new Jid (jid.Bare);
					if (S.MUCs.ContainsKey (j))
						return S.MUCs[j];
					else {
						j = new Jid (jid.Bare.ToLower ());
						return S.MUCs.ContainsKey (j) ? S.MUCs[j] : null;
					}
				} else {
					return null;
				}
			}
		}

		public Hashtable MUCJustJoined {
			get {
				lock (sobjs[65]) {
					return _justJoined;
				}
			}
			set {
				lock (sobjs[65]) {
					_justJoined = value;
				}
			}
		}

		public Hashtable MUCJustJoined_Mucs {
			get {
				lock (sobjs[66]) {
					return _justJoined_Mucs;
				}
			}
			set {
				lock (sobjs[66]) {
					_justJoined_Mucs = value;
				}
			}
		}

		public bool AddMUC (MUC muc)
		{
			lock (sobjs[1]) {
				
				if (S.GetMUC (muc.Jid) == null) {
					S.MUCs.Add (muc.Jid, muc);
					mucs_count++;
					return true;
				}
				
				return false;
			}
		}


		public bool DelMUC (Jid Jid)
		{
			lock (sobjs[1]) {
				foreach (MUC muc in S.MUCs.Values) {
					if (muc.Jid.Bare == Jid.Bare) {
						mucs_count--;
						S.MUCs.Remove (muc.Jid);
						return true;
					}
				}
				return false;
			}
		}
		
		public Hashtable CustomObjects
        {
            get { lock (sobjs[67]) { return _customObjects; } }
            set { lock (sobjs[67]) { _customObjects = value; } }
        }
		
		
		
		
	}
}
