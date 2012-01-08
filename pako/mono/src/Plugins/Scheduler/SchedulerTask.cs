/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Pako Jabber-bot.                                                              *
 * Copyright. All rights reserved © 2007-2008 by Klichuk Bogdan (Bbodio's Lab)   *
 * Copyright. All rights reserved © 2009-2012 by Alexey Bryohov                  *
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
using Core.Plugins;
using Core.Kernel;
using Core.Conference;
using agsXMPP;
using agsXMPP.protocol.client;

using System.Data;

namespace Plugin
{

	public class SchedulerTask
	{
		public SchedulerTask ()
		{
			
		}

		#region Properties

		public Jid JID { get; set; }
		public String Name { get; set; }
		public Jid Muc { get; set; }

		public DateTime AddDate { get; set; }

		public DateTime ScheduleDate { get; set; }
		public TimeSpan ScheduleTime { get; set; }
		public SchedulerTaskPeriod SchedulePeriod { get; set; }

		public bool IsComplete { get; set; }

		public String ScheduleCommands { get; set; }

		public DateTime ExecuteDateTime { get; set; }
		
		
		public SessionHandler Sh {get; set;}

		#endregion

		#region Methods

		public void Execute ()
		{
			string[] _cmds = ScheduleCommands.Split (new string[] { "&&" }, StringSplitOptions.RemoveEmptyEntries);
			
			@out.write ("SchedulerTask. debug : SchedullerTask.Execute(). Commands : " + ScheduleCommands);
			
			try
			{
			foreach (string _cmd in _cmds) {
				agsXMPP.protocol.client.Message msg = new agsXMPP.protocol.client.Message ();
				msg.From = this.JID;
					
					if (this.Sh == null)						
						@out.write ("sh null");
					
					if (this.Sh.S == null)
						@out.write ("sh.S null");
					
					if (this.Sh.S.Config == null)
						@out.write ("sh.S.Config null");
					
					if (this.Sh.S.Config.Jid == null)
						@out.write ("sh.S.Config.Jid null");
					
				msg.To = this.Sh.S.Config.Jid;
				msg.Type = MessageType.chat;
				msg.Body = _cmd.Trim (' ', '\n');
				
				// Set jid to reply
				//Jid _jid = null;
				//if (m_r.MUC != null)
				//	_jid = m_r.MUC.GetUser (msg.From.Resource).Jid;
				//else
				//	_jid = msg.From;
				
				//@out.write ("Launching: " + _cmd);
				CommandHandler cmd_handler = new CommandHandler (msg, Sh, null, CmdhState.PREFIX_NOT_POSSIBLE, 1);
				Sh.S.Sleep ();
				//@out.write ("Scheduler debug : msg.From : " + msg.From.Resource + " - " + _jid.ToString() +"");
				
			}
			}
			catch (Exception _ex)
			{
				@out.write ("SchedulerTask.Execute() exception: \n" + _ex.ToString());
			}
		}
		
		#endregion
	}
}

