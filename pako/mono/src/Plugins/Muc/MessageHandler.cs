using System;
using System.Collections.Generic;
using System.Text;
using Core.Plugins;
using Core.Kernel;
using Core.Conference;
using agsXMPP;
using agsXMPP.protocol.client;

namespace Plugin
{

	public class MessageHandler
	{
		agsXMPP.protocol.client.Message msg;
		SessionHandler Session;
		Message emulation;
		CmdhState signed;
		int level;
		
		public MessageHandler(agsXMPP.protocol.client.Message msg, SessionHandler s, Message emulation, CmdhState signed, int level)
		{
			this.msg = msg;
			this.Session = s;
			this.emulation = emulation;
			this.signed = signed;
			this.level = level;
		}
		
		public void Handle()
		{
			MessageType _mType = msg.Type;
			Jid _fromJid = new Jid (msg.From.Bare.ToLower () + (msg.From.Resource != "" ? "/" + msg.From.Resource : ""));
			
			// AntiFrood
			if (_mType == MessageType.groupchat) {
				MUC _muc = null;
				MUser _mucUser = null;
				
				//@out.write (_mType.ToString () + " - " + _fromJid + " -");
				//@out.write (msg.Body + "\n");
				
				// Get MUC object
				if (Session.S.GetMUC (_fromJid) != null) {
					_muc = Session.S.GetMUC (_fromJid);
				}
				
				// Get MUC User object
				_mucUser = null;
				if (_muc != null) {
					if (_fromJid.Resource == null)
						return;
					// MUC mode woth a null MUC user. Stop process
					if (_muc.GetUser (_fromJid.Resource) != null)
						_mucUser = _muc.GetUser (_fromJid.Resource);
					else
						return;
					// Because of null MUC user in MUC mode
				}
				
				//if (_muc != null)					
				//	@out.write (_muc.Jid.ToString());
				
				//if (_mucUser != null)
				//	@out.write (_mucUser.Jid.ToString() + " MUC: "+ _mucUser.Jid.Bare + "/"+_mucUser.Jid.Resource);
				
				//Do a message check
			}
		}
	}
}
