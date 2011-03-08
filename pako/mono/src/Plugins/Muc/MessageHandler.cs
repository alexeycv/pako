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
using Core.Other;

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
		
		public void HandleMessage()
		{
			try{
				this.Handle();
				//@out.write ("Init OK"); 
			}
			catch (Exception ex) {
				
				string clrf = "\n";
				string msg_source = msg.ToString ();
				
				if (Utils.OS == Platform.Windows) {
					clrf = "\r\n";
					msg_source = msg.ToString ().Replace ("\n", "\r\n");
					
				}
				string data = "====== [" + DateTime.Now.ToString () + "] ===================================================>" + clrf + "   Stanza:" + clrf + msg_source + clrf + clrf + ex.ToString () + clrf + clrf + clrf;
				
				Session.S.ErrorLoger.Write (data);
				
				if (Session.S.Config.SendErrorMessagesToAdmin) {
					foreach (Jid j in Session.S.Config.Administartion ()) {
						Message _msg = new Message ();
						_msg.To = j;
						_msg.Type = MessageType.chat;
						_msg.Body = "ERROR:   " + ex.ToString () + "\n\nStack trace: \n" + ex.StackTrace + "\nSource:\n" + msg_source;
						Session.S.C.Send (_msg);
					}
				}
			}
		}

		public void Handle ()
		{
			//@out.write ("Handle in OK");
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
			
			if (msg.Body == null || msg.Body == "")
				return;
			if (_mucUser != null && _mucUser.Jid.Bare == Session.S.C.MyJID.Bare)
				return;
			
			#region Response
			Jid m_jid = _mucUser != null ? _mucUser.Jid : _fromJid;
			
			string vl = null;
			if (_muc != null)
				vl = _muc.VipLang.GetLang (m_jid);
			
			if (vl == null)
				vl = Session.S.VipLang.GetLang (m_jid);
			
			Response r = new Response (Session.S.Rg[vl != null ? vl : _mucUser != null ? _mucUser.Language : Session.S.Config.Language]);
			
			// Get user access
			int? access = 0;
			access = Session.S.GetAccess (msg, _mucUser, _muc);
			
			if (access != null)
				r.Access = access;
			else
				r.Access = 0;
			
			r.Msg = msg;
			r.MSGLimit = Session.S.Config.MucMSGLimit;
			r.MUC = _muc;
			r.Level = level;
			r.MUser = _mucUser;
			r.Delimiter = Session.S.Config.Delimiter;
			;
			r.Sh = Session;
			
			//@out.write ("Response OK");
			
			#endregion			
			
			// AntiFrood
			#region AntiFlood
			
			if (_mType == MessageType.groupchat && Session.S.Config.EnableLogging && _muc.OptionsHandler.GetOption ("antiflood") == "+" || true) {
				if (_mucUser != null) {
					Collection<MessageStorage> _messages = null;
					
					if (_mucUser.CustomObjects["MessageStorage"] != null) {
						_messages = (Collection<MessageStorage>)_mucUser.CustomObjects["MessageStorage"];
					} else {
						_messages = new Collection<MessageStorage> ();
						_mucUser.CustomObjects.Add ("MessageStorage", _messages);
					}
					
					Int32 _messagesCount = 3;
					if (_muc.OptionsHandler.GetOption ("antiflood_messages_count") != null)
						_messagesCount = Convert.ToInt32 (_muc.OptionsHandler.GetOption ("antiflood_messages_count"));
					
					// Add current message
					MessageStorage _messageStorage = new MessageStorage (DateTime.Now, msg.Body);
					_messages.Add (_messageStorage);
					
					int _similarMessagesCount = 0;
					
					if (_messagesCount > 2) {
						if (_messages.Count >= _messagesCount) {
							long _curentTicks = DateTime.Now.Ticks;
							
							// Find similar messages							
							foreach (MessageStorage _ms in _messages) {
								TimeSpan _tS = TimeSpan.FromTicks (_curentTicks - _ms._timestamp.Ticks);
								// Equals
								if (_messageStorage._message == _ms._message && _tS.Seconds <= 15) {
									_similarMessagesCount++;
								}
							}
							
							if (_similarMessagesCount >= _messagesCount) {
								//@out.write ("Simirar \n" + _messageStorage._message + "\n"+_similarMessagesCount + "\n messages in storage: "+ _messages.Count);
								//Now we can kick flooder or ban him
								
								#region Action
								
								string censored = "You repeat you message more than " + _messagesCount + " times. Its flood.";
								switch (_muc.OptionsHandler.GetOption ("antiflood_result")) {
								case "kick":
									
									
									{
										@out.exe ("censor_next_kick");
										if (_muc.KickableForCensored (_mucUser)) {
											_muc.Kick (null, _mucUser, censored);
											return;
										} else {
											//MessageType original_type = r.Msg.Type;
											r.Msg.Type = MessageType.groupchat;
											r.Reply (censored);
											//r.Msg.Type = original_type;
											@out.exe ("censor_next_sleeping");
											Session.S.Sleep ();
											
											@out.exe ("censor_next_slept");
										}
										break;
									}

								
								case "devoice":
									
									
									{
										@out.exe ("censor_next_devoice");
										if (_muc.KickableForCensored (_mucUser)) {
											_muc.Devoice (null, _mucUser, censored);
											return;
										} else {
											//MessageType original_type = r.Msg.Type;
											r.Msg.Type = MessageType.groupchat;
											r.Reply (censored);
											//r.Msg.Type = original_type;
											@out.exe ("censor_next_sleeping");
											Session.S.Sleep ();
											@out.exe ("censor_next_slept");
										}
										break;
									}

								
								case "ban":
									
									
									{
										@out.exe ("censor_next_ban");
										if (_muc.KickableForCensored (_mucUser)) {
											_muc.Ban (null, _mucUser, censored);
											return;
										} else {
											//MessageType original_type = r.Msg.Type;
											r.Msg.Type = MessageType.groupchat;
											r.Reply (censored);
											//r.Msg.Type = original_type;
											@out.exe ("censor_next_sleeping");
											Session.S.Sleep ();
											@out.exe ("censor_next_slept");
										}
										break;
									}

								
								case "warn":
									
									
									{
										@out.exe ("censor_next_warn");
										//MessageType original_type = r.Msg.Type;
										r.Msg.Type = MessageType.groupchat;
										r.Reply (censored);
										//r.Msg.Type = original_type;
										@out.exe ("censor_next_sleeping");
										Session.S.Sleep ();
										@out.exe ("censor_next_slept");
										
									}

									
									break;
								default:
									break;
								}
								//switch
								#endregion
							}
							
							//Remove first message
							if (_messages.Count > _messagesCount + 2)
								_messages.RemoveAt (0);
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
