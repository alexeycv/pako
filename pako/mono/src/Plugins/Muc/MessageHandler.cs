using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
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

		public MessageHandler (agsXMPP.protocol.client.Message msg, SessionHandler s, Message emulation, CmdhState signed, int level)
		{
			this.msg = msg;
			this.Session = s;
			this.emulation = emulation;
			this.signed = signed;
			this.level = level;
		}

		public void Handle ()
		{
			MUC _muc = null;
			MUser _mucUser = null;
			
			MessageType _mType = msg.Type;
			Jid _fromJid = new Jid (msg.From.Bare.ToLower () + (msg.From.Resource != "" ? "/" + msg.From.Resource : ""));
			
			#region Init message
			
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
			
			#endregion
			
			// AntiFrood
			#region AntiFlood
			
			if (_mType == MessageType.groupchat && Session.S.Config.EnableLogging && _muc.OptionsHandler.GetOption ("antiflood") == "+" || true) {
				if (_mucUser != null)
				{
					Collection<MessageStorage> _messages = null;
					
					if (_mucUser.CustomObjects["MessageStorage"] != null)
					{
						_messages = (Collection<MessageStorage>)_mucUser.CustomObjects["MessageStorage"];
					}
					else{
						_messages = new Collection<MessageStorage>();
						_mucUser.CustomObjects.Add("MessageStorage", _messages);
					}
					
					Int32 _messagesCount = 3;
					if (_muc.OptionsHandler.GetOption ("antiflood_messages_count") != null)
						_messagesCount = Convert.ToInt32(_muc.OptionsHandler.GetOption ("antiflood_messages_count"));
					
					if (_messagesCount > 2)
					{
						if (_messages.Count >= _messagesCount)
						{
							
							//Remove first message and add current
							_messages.RemoveAt(0);
							MessageStorage _messageStorage = new MessageStorage();
							_messages.Add(_messageStorage);
						}
					}
				}
				//@out.write (_mType.ToString () + " - " + _fromJid + " -");
				//@out.write (msg.Body + "\n");
				
				//if (_muc != null)					
				//	@out.write (_muc.Jid.ToString());
				
				//if (_mucUser != null)
				//	@out.write (_mucUser.Jid.ToString() + " MUC: "+ _mucUser.Jid.Bare + "/"+_mucUser.Jid.Resource);
				
				//Do a message check
			}
			
			#endregion
		}
	}
}
