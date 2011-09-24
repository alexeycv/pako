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
					rs = m_r.f ("volume_list", n) + "\nlist, poke";
					break;
				}

			
			case "poke":
				
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

