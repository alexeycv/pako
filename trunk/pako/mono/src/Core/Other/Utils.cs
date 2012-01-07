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
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using agsXMPP;
using agsXMPP.Xml.Dom;
using agsXMPP.protocol.client;
using System.Diagnostics;
using Core.Conference;
using Core.Kernel;
using System.Reflection;

namespace Core.Other
{

	/// <summary>
	/// Represents current OSes, which can support C# .NET/Mono
	/// </summary>
	public enum Platform
	{
		Windows,
		Mac,
		Unix,
		Linux
	}



	public static class Utils
	{


		static object obj = new object ();
		static object obj2 = new object ();
		static object obj3 = new object ();
		static object obj4 = new object ();
		static object obj5 = new object ();
		static List<string> vars;
		static Document paths;
		static SessionHandler session;
		static string m_dir;


		/// <summary>
		/// Is needed to create all needed instances in the class 
		/// </summary>
		public static void Register ()
		{
			vars = new List<string> ();
			vars.Add ("NICK");
			vars.Add ("BOTSERVER");
			vars.Add ("ROOM");
			vars.Add ("JID");
			vars.Add ("STATUS");
			vars.Add ("LANG");
			vars.Add ("ACCESS");
			vars.Add ("DATE");
			vars.Add ("TIME");
			vars.Add ("DAY");
			vars.Add ("MINUTE");
			vars.Add ("SECOND");
			vars.Add ("YEAR");
			vars.Add ("MONTH");
			vars.Add ("DAYOFWEEK");
			vars.Add ("DATETIME");
			vars.Add ("BOTNICK");			//16
			vars.Add ("SYSTEM");
			vars.Add ("BOTVERSION");
			vars.Add ("SUBJECT");
			vars.Add ("ENVIRONMENT");
			vars.Add ("MONTHSHORT");			// Some users need short format of month like 3 instead of 03
			vars.Add ("DAYSHORT");			// Some users need short format of month like 3 instead of 03
			Assembly assem = Assembly.GetExecutingAssembly ();
			m_dir = Path.GetDirectoryName (assem.Location);
			CD = m_dir;
			paths = new Document ();
			paths.LoadFile (dir (CD, "Paths.cfg"));
		}


		/// <summary>
		/// Directory builder for any OS
		/// </summary>
		/// <param name="parts"></param>
		/// <returns></returns>
		public static string dir (params string[] parts)
		{
			string data = "";
			int i = 0;
			foreach (string part in parts) {
				i++;
				if (i != 1)
					data += Path.DirectorySeparatorChar + part;
				else
					data += part;
			}
			return data;
		}

		/// <summary>
		/// Current directory of the process
		/// </summary>
		public static string CD {
			get {
				lock (obj2) {
					return m_dir;
				}
			}
			set {
				lock (obj2) {
					m_dir = value;
				}
			}
		}

		/// <summary>
		/// The platform-specific sign, used to separate directory/path units
		/// </summary>
		public static char d {
			get { return System.IO.Path.DirectorySeparatorChar; }
		}


		public static SessionHandler Sh {
			get {
				lock (obj4) {
					return session;
				}
			}
			set {
				lock (obj4) {
					session = value;
				}
			}
		}
		public static List<string> Variables {
			get {
				lock (obj5) {
					return vars;
				}
			}
			set {
				lock (obj5) {
					vars = value;
				}
			}
		}


		public static Document Paths {
			get {
				lock (obj3) {
					return paths;
				}
			}
			set {
				lock (obj3) {
					paths = value;
				}
			}
		}

		/// <summary>
		/// Gets current machine's Operating System's base
		/// </summary>
		public static Platform OS {
			get {
				string os = Environment.OSVersion.ToString ().ToLower ();
				return (os.IndexOf ("unix") > -1) ? Platform.Unix : (os.IndexOf ("windows") > -1) ? Platform.Windows : (os.IndexOf ("linux") > -1) ? Platform.Linux : (os.IndexOf ("mac") > -1) ? Platform.Mac : Platform.Unix;
				
			}
		}




		public static string ConsoleEscape (string Source)
		{
			return "\"" + Source.Replace (@"\", @"\\").Replace (@"\'", @"\\\'").Replace ("\"", "\\\"").Replace ("`", "\\`").Replace ("\n", "").Replace ("\r", "") + "\"";
		}

		/// <summary>
		///  Gets the substring in "Source" - the first entering of string, specified in "Pattern"( It's has to contain (.*) which will be used like regular expression to find the needed part)
		/// You can specify as an argument true/false - if you want to get all the pattern or just the matched by "Pattern" word, which was found for (.*)
		/// </summary>
		/// <param name="Source"></param>
		/// <param name="Pattern"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public static string GetValue (string Source, string Pattern, params object[] args)
		{
			string part1 = Pattern.Substring (0, Pattern.IndexOf ("(.*)"));
			string part2 = Pattern.Substring (Pattern.IndexOf ("(.*)") + 4);
			bool include;
			if (args.Length == 0)
				include = false;
			else
				try {
					include = (bool)args[0];
				} catch {
					include = false;
				}
			;
			int posb;
			int pose;
			if (part1 == "")
				posb = 0;
			else {
				posb = include ? Source.IndexOf (part1) : Source.IndexOf (part1) + part1.Length;
				if (posb == -1)
					return "";
			}
			
			if (part2 == "")
				pose = Source.Length;
			else {
				pose = include ? Source.IndexOf (part2, posb + 1) + part2.Length : Source.IndexOf (part2, posb);
				if (pose == -1)
					return "";
			}
			if (pose >= posb)
				return Source.Substring (posb, pose - posb);
			else
				return "";
			
		}

		/// <summary>
		/// Removes in string "Source" the first entering of string, specified in "Pattern"( It's has to contain (.*) which will be used like regular expression to find the needed part)
		/// You can specify as an argument true/false - if you want to remove al the pattern or just the matched by "Pattern" word, which was found for (.*)
		/// </summary>
		/// <example>
		/// string data = "Hello, my job is [programmer - ingeneer]
		/// RemoveValue(data, "[(.*)]", false)
		/// or
		/// RemoveValue(data, "[(.*)]") will return a string 
		/// "Hello, my job is []"
		/// RemoveValue(data, "[(.*)]", true) will return a string 
		/// "Hello, my job is "
		/// </example>
		/// <param name="Source"></param>
		/// <param name="Pattern"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public static string RemoveValue (string Source, string Pattern, params object[] args)
		{
			
			string part1 = Pattern.Substring (0, Pattern.IndexOf ("(.*)"));
			string part2 = Pattern.Substring (Pattern.IndexOf ("(.*)") + 4);
			string value = GetValue (Source, Pattern, args);
			if ((value.StartsWith (part1)) && (value.EndsWith (part2)))
				return Source.Remove (Source.IndexOf (value), value.Length);
			return Source.Remove (Source.IndexOf (part1 + value) + 1, value.Length);
		}


		/// <summary>
		/// Singularily replaces first entering of string "From" in string "Source" with "To" 
		/// </summary>
		/// <param name="Source"></param>
		/// <param name="From"></param>
		/// <param name="To"></param>
		/// <returns></returns>
		public static string ReplaceOnce (string Source, string From, string To)
		{
			string result = Source;
			if (string.IsNullOrEmpty (result))
				return "";
			if ((string.IsNullOrEmpty (To)) || (string.IsNullOrEmpty (To)))
				return result;
			int posb = result.IndexOf (From);
			if (posb == -1)
				return result;
			result = result.Remove (posb, From.Length).Insert (posb, To);
			return result;
			
		}

		/// <summary>
		/// This is SplitEx -> split words by empty-space, specifiyng maximum parts.
		/// </summary>
		/// <example>
		/// Let's take a look on this phrase:
		/// string data = "Hello, my job is programmer - ingeneer";
		/// A ussual split will return a string[6] massive with each part,delimited with emptyspace.
		/// When we use ParseWords(data,3) it will return string[3] massive, which parts will be:
		/// "Hello," "my" "job" "is programmer - ingeneer"
		/// For ParseWords(data, 2):
		/// "Hello," "my" "job is programmer - ingeneer"
		/// </example>
		/// <param name="Source"></param>
		/// <param name="Fixed"></param>
		/// <returns></returns>
		public static string[] SplitEx (string Source, int Fixed)
		{
			string[] ss = Source.Trim ().Split (' ');
			int i = 0;
			string[] res;
			string _source = Source;
			foreach (string s in ss) {
				if (s != "") {
					if (i + 1 <= Fixed) {
						i++;
						_source = _source.Remove (0, _source.IndexOf (s) + s.Length);
					
					} else {
						string rr;
						i++;
						rr = _source.Substring (_source.IndexOf (s)).Trim ();
						break;
					}
				}
				
			}
			_source = Source.Trim ();
			res = new string[i];
			i = 0;
			foreach (string s in ss) {
				if (s != "") {
					if (i + 1 <= Fixed) {
						res[i] = s.Trim ();
						i++;
						_source = _source.Remove (0, _source.IndexOf (s) + s.Length);
					
					} else {
						string rr;
						
						rr = _source.Substring (_source.IndexOf (s)).Trim ();
						res[i] = rr;
						i++;
						return res;
						
					}
				}
				
			}
			
			return res;
		}






		/// <summary>
		/// A formatter for a TimeSpan, which converts ticks count into a real data/time.
		/// </summary>
		/// <param name="Ticks"></param>
		/// <param name="r"></param>
		/// <returns></returns>
		public static string FormatTimeSpan (long Ticks, Response r)
		{
			@out.exe (Ticks.ToString ());
			TimeSpan ts = new TimeSpan (Ticks);
			double m = System.Math.Truncate ((double)ts.Days / 30);
			@out.exe (Ticks.ToString ());
			string res = (ts.Days > 30 ? Convert.ToString (m) + " " + r.f ("month") + " " : "") + (ts.Days - m * 30 > 0 ? (ts.Days - m * 30).ToString () + " " + r.f ("day") + " " : "") + (ts.Hours > 0 ? ts.Hours.ToString () + " " + r.f ("hour") + " " : "") + (ts.Minutes > 0 ? ts.Minutes.ToString () + " " + r.f ("minute") + " " : "") + (ts.Seconds > 0 ? ts.Seconds.ToString () + " " + r.f ("second") + " " : "");
			return res.Trim () == "" ? "0 " + r.f ("second") : res;
			
		}



		public static string ClearTags (string Source)
		{
			while (GetValue (Source, "<(.*)>", true) != "")
				Source = RemoveValue (Source, "<(.*)>", true);
			return Source;
		}

		//<summary>
		//D Delete all chars left
		//<s/ummary>
		public static string TrimLeft (string str, char ch)
		{
			if (str.Length > 0) {
				while (str[0] == ch) {
					str = str.Remove (0, 1);
				}
			}
			return str;
		}

		/// <summary>
		/// Formats escape sequences into phrases.
		/// </summary>
		/// <param name="Source"></param>
		/// <param name="muc"></param>
		/// <param name="user"></param>
		/// <returns></returns>
		public static string FormatEnvironmentVariables (string Source, Response r)
		{
			if (Source == null)
				return null;
			
			List<string> strs = Variables;
			string res = Source.Replace ("{" + strs[5] + "}", r.Language).Replace ("{" + strs[6] + "}", r.Access.ToString ()).Replace ("{" + strs[1] + "}", r.Connection.Server).Replace ("{" + strs[3] + "}", r.MUser != null ? r.MUser.Jid.Bare : r.Msg.From.Bare).Replace ("{" + strs[7] + "}", DateTime.Now.Date.ToString ("d")).Replace ("{" + strs[8] + "}", DateTime.Now.ToString ("HH:mm:ss")).Replace ("{" + strs[9] + "}", DateTime.Now.ToString ("dd")).Replace ("{" + strs[10] + "}", DateTime.Now.ToString ("mm")).Replace ("{" + strs[11] + "}", DateTime.Now.ToString ("ss")).Replace ("{" + strs[12] + "}", DateTime.Now.ToString ("yyyy")).Replace ("{" + strs[13] + "}", DateTime.Now.ToString ("MM")).Replace ("{" + strs[14] + "}", DateTime.Now.ToString ("dddd")).Replace ("{" + strs[15] + "}", DateTime.Now.ToString ()).Replace ("{" + strs[17] + "}", Bot["os"]).Replace ("{" + strs[18] + "}", Bot["version"]).Replace ("{" + strs[20] + "}", (Environment.Version).ToString ()).Replace ("{" + strs[21] + "}", TrimLeft (DateTime.Now.ToString ("MM"), '0')).Replace ("{" + strs[22] + "}", TrimLeft (DateTime.Now.ToString ("dd"), '0'));
			if (r.MUC != null && r.MUser != null)
				res = res.Replace ("{" + strs[0] + "}", r.MUser).Replace ("{" + strs[2] + "}", r.MUC.Jid.ToString ()).Replace ("{" + strs[4] + "}", r.MUser.Status).Replace ("{" + strs[16] + "}", r.MUC.MyNick);
			if (r.MUC != null && r.MUC.Subject != null)
				res = res.Replace ("{" + strs[19] + "}", r.MUC.Subject);
			
			return res;
			
			
		}

		/// <summary>
		/// Formats the string. Replaces in "pattern" all the numbered enterings of the "prefix" with words from 
		/// "msg" and returns formatted string.
		/// If you want to n
		/// yj 
		/// </summary>
		/// <param name="pattern"></param>
		/// <param name="msg"></param>
		/// <param name="prefix"></param>
		/// <param name="escape_prefix"></param>
		/// <param name="escape_sh"></param>
		/// <returns></returns>
		public static string FormatMsg (string pattern, string msg, string prefix, bool escape_prefix, bool escape_sh)
		{
			int max = 2;
			msg = msg.Trim ();
			pattern = pattern.Trim ();
			Regex r = new Regex ((escape_prefix ? "\\" : "") + prefix + "[1-9]+");
			foreach (Match m in r.Matches (pattern)) {
				@out.exe ("alias_reg_match: " + m.ToString ());
				int temp_num = Int32.Parse (m.ToString ().Substring (1));
				if (temp_num > max)
					max = temp_num;
			}
			string[] ws = Utils.SplitEx (msg, max - 1);
			
			pattern = r.Replace (pattern, new MatchEvaluator (delegate(Match reg) {
				int temp = Int32.Parse (reg.ToString ().Substring (1));
				try {
					return escape_sh ? Utils.ConsoleEscape (ws[temp - 1]) : ws[temp - 1];
				} catch {
					return "";
				}
			}));
			
			return pattern.Replace (prefix + "~", escape_sh ? Utils.ConsoleEscape (msg) : msg);
			
		}


		/// <summary>
		/// Detect if the Jid is valid
		/// </summary>
		/// <param name="Jid"></param>
		/// <returns></returns>
		public static bool JidValid (Jid Jid)
		{
			return Jid.ToString ().IndexOf ("@") > -1;
		}

		/// <summary>
		///  Detect if the Jid is valid
		/// </summary>
		/// <param name="Jid"></param>
		/// <returns></returns>
		public static bool JidValid (string Jid)
		{
			return Jid.IndexOf ("@") > -1;
		}



		public static msgType GetTypeOfMsg (Message msg, MUser user)
		{
			if (msg.Error == null) {
				if ((msg.XDelay == null) && (msg.From.Resource != null)) {
					if (msg.Body != null) {
						if (user != null) {
							return msgType.MUC;
						} else {
							return msgType.Roster;
						}
					} else {
						
						return msgType.Empty;
					}
				} else
					return msgType.None;
			} else
				return msgType.Error;
		}




		private static string GetOSVersion {
			get {
				Stdior std = new Stdior ();
				switch (Utils.OS) {
				case Platform.Windows:
					return std.Execute ("ver", Sh.S);
				case Platform.Unix:
					return std.Execute ("uname -a", Sh.S);
				case Platform.Linux:
					return std.Execute ("uname -a", Sh.S);
				default:
					return "<unknown>";
				}
			}
		}




		public static bool Self (MUC muc, MUser user, string entity)
		{
			if (user == null || muc == null)
				return false;
			return user.Jid.Bare == muc.Jid.Bare ? entity == user : muc.UserExists (entity) ? muc.GetUser (entity).Jid.Bare == user.Jid.Bare : new Jid (entity.ToLower ()).Bare == user.Jid.Bare;
			
		}





		public static string GetPath (string name)
		{
			string data = null;
			foreach (Element el in Paths.RootElement.SelectElements ("path")) {
				if (el.GetAttribute ("name") == name) {
					data = el.GetAttribute ("value");
					break;
				}
			}
			
			if (data != null) {
				foreach (Element el in Paths.RootElement.SelectElements ("path")) {
					string val = "%" + el.GetAttribute ("name") + "%";
					data = data.Replace (val, el.GetAttribute ("value"));
					data = data.Replace ("%pako%", CD).Replace ("/", d.ToString ());
				}
				return data;
			}
			return name;
			
		}
		
		public static string GetPath ()
		{
			return CD;			
		}


		public static string GetPath (string name, MUC muc)
		{
			string data = null;
			ElementList els = Paths.RootElement.SelectElements ("path");
			int index = 0;
			for (int i = 0; i < els.Count; i++) {
				Element el = els.Item (i);
				if (el.GetAttribute ("name") == name) {
					data = el.GetAttribute ("value");
					index = i;
					break;
				}
			}
			
			if (data != null) {
				
				for (int i = index - 1; i >= 0; i--) {
					Element el = els.Item (i);
					string val = "%" + el.GetAttribute ("name") + "%";
					data = data.Replace (val, el.GetAttribute ("value"));
				}
				
				data = data.Replace ("%pako%", CD).Replace ("%muc%", muc.Jid.Bare).Replace ("/", d.ToString ());
				return data;
			}
			return name;
			
		}



		public static Dictionary<string, string> Bot {
			get {
				Dictionary<string, string> dict = new Dictionary<string, string> ();
				dict.Add ("os", GetOSVersion);
				dict.Add ("name", "Pako bot");
				dict.Add ("version", Version);
				try {
					dict.Add ("file", Process.GetCurrentProcess ().MainModule.FileVersionInfo.ToString ());
				} catch (Exception err) {
					dict.Add ("file", "Pako");
				}
				dict.Add ("environment", "C# 2.0 .NET/Mono");
				return dict;
			}
		}




		private static string Version {
			get { return "12.01.07 (http://pako.googlecode.com developers team)"; }
		}
		
		
		
		
		
	}
}
