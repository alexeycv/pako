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

	public class MessageStorage
	{
		public string _message;
		public DateTime _timestamp;
		
		public MessageStorage(DateTime timestamp, String message)
		{
			this._message = message;
			this._timestamp = timestamp;
		}
	}
}
