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
using System.Collections.Specialized;
using System.Collections;
using System.Text;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;
using System.Threading;
using System.IO;
using Core.Kernel;
using Core.Conference;
using Core.Other;
using Core.Plugins;
using agsXMPP.protocol.x.muc;
using agsXMPP.protocol.x.muc.iq.admin;
using agsXMPP.protocol.x.muc.iq.owner;
using System.Diagnostics;
using Mono.Data.SqliteClient;
using System.Data;
using Core.API.Data;

namespace Plugin
{

	/// <summary>
	/// An Admin plugin handler
	/// </summary>
	public class FunHandler
	{
		string[] ws;
		static Message m_msg;
		static Response m_r;
		string self;
		string d;
		bool syntax_error = false;
		Jid s_jid;
		string m_b;
		string n = "Mucfilter";
		SessionHandler Sh;
		bool _isInAdminList = false;


		public FunHandler (Response r)
		{
			Sh = r.Sh;
			m_b = r.Msg.Body;
			ws = Utils.SplitEx (m_b, 2);
			m_msg = r.Msg;
			m_r = r;
			s_jid = r.Msg.From;
			d = r.Delimiter;
			
			if (ws.Length < 2) {
				r.Reply (r.f ("volume_info", n, d + n.ToLower () + " list"));
				return;
			}
			
			self = ws[0] + " " + ws[1];
			Handle ();
			
			
			
		}

		/// <summary>
		/// Handles a plug-in
		/// </summary>
		public void Handle ()
		{
			
			string cmd = ws[1].ToLower ();
			string rs = null;
			switch (cmd) {
			case "list":
				
				{
					rs = m_r.f ("volume_list", n) + "\nlist, poke, poke_add, bottle";
					break;
				}

			
			case "poke":
				
				{
					rs = "Error.";
					// Data layer
					int sqlv = int.Parse (Sh.S.Config.GetTag ("sqlite"));					
				
					DataController _dc;
					try
					{
						_dc = new DataController(Utils.GetPath()+"/Dynamic/Pokes.db", sqlv.ToString(), true);
						if (_dc.JustCreated)
						{
							_dc.ExecuteNonQuery("CREATE TABLE  poke (id varchar, muc varchar, poke_data varchar);");
						}
					
						if (ws.Length > 2) {
							DataTable _dt = _dc.ExecuteDALoad("SELECT * FROM poke WHERE muc='*'");
						
							if (_dt != null)
							{
								if (_dt.Rows.Count > 0)
								{
									Random r = new Random();
									int _index = r.Next(0, _dt.Rows.Count);
								
									string _pokeString = "/me " + _dt.Rows[_index]["poke_data"];
									rs = _pokeString.Replace ("%NICK%", ws[2]);
								}
								else
								{
									rs = "No pokes avaliable.";
								}
							}
							else
							{
								rs = "Error";
							}
						
							//rs = "";
						} else {
							rs = "Ты чаго это?";
						}
					
						_dc.Close();
					}
					catch (Exception exx)
					{
						@out.write ("Exception: \n" + exx.Message + "\n\n" + exx.Source + "\n\n" + exx.StackTrace + "\n\n Inner:\n\n");
					}
					
					break;
				}
				
			case "poke_add":
				
				{
					rs = "Error.";
					// Data layer
					int sqlv = int.Parse (Sh.S.Config.GetTag ("sqlite"));					
				
					DataController _dc;
					try
					{
						_dc = new DataController(Utils.GetPath()+"/Dynamic/Pokes.db", sqlv.ToString(), true);
						if (_dc.JustCreated)
						{
							_dc.ExecuteNonQuery("CREATE TABLE  poke (id varchar, muc varchar, poke_data varchar);");
						}
					
						if (ws.Length > 2) {
							string _data = ws[2];
							
							if (!_data.Contains("%NICK%"))
							{
								rs = "No %NICK% was defined. Saving aborted.";
							}
							else
							{
								rs = "Ok.";
							
								Guid _id = Guid.NewGuid();
							
								//DataTable _dt = new DataTable("poke");
								//_dt.Columns.Add("id", typeof(string));
								//_dt.Columns.Add("muc", typeof(string));
								//_dt.Columns.Add("poke_data", typeof(string));
							
								//DataRow _dr = _dt.NewRow();
								//_dr["id"] = _id.ToString();
								//_dr["muc"] = "*";
								//_dr["poke_data"] = _data;
								
								//_dt.Rows.Add(_dr);
							
								//_dc.ExecuteDASave("select * from poke where id='"+_id.ToString()+"'", _dr);
								_dc.ExecuteNonQuery("INSERT INTO poke VALUES ('"+_id.ToString()+"', '*', '"+_data+"')");
							}
							
						} else {
							rs = "No data was saved.";
						}
					
						_dc.Close();
					}
					catch (Exception exx)
					{
						@out.write ("Exception: \n" + exx.Message + "\n\n" + exx.Source + "\n\n" + exx.StackTrace + "\n\n Inner:\n\n");
					}                 
					
					break;
				}
				
			case "bott;e":
				
				{
					if (ws.Length > 2) {
						string _pokeAtring = "/me" + " тыкнул %NICK% палочкой";
						rs = _pokeAtring.Replace ("%NICK%", ws[2]);
						
						//rs = "";
					} else {
						rs = "Ты чаго это?";
					}
					
					break;
				}

			default:
				
				
				{
					rs = m_r.f ("volume_cmd_not_found", n, ws[1], d + n.ToLower () + " list");
					break;
				}

				
			}
			
			if (syntax_error)
				m_r.se (self); else if (rs != null)
				
			m_r.Reply (rs);
			
		}
		
	}
}

