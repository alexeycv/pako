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
using System.Threading;
using System.Collections.ObjectModel;

using Mono.Data.SqliteClient;
using System.Data;
using Core.API.Data;

using Core.Other;

namespace Plugin
{

	public class Scheduler
	{
		SessionHandler _sh;

		Timer _mainTimer;
		Collection<SchedulerTask> _tasks;
		DateTime _initDateTime;
		
		DataController _database;

		public Scheduler (SessionHandler sh)
		{
			this._sh = sh;
			
			// init database
			try {
				@out.write ("Scheduler : Init dayabase /Dynamic/Scheduler.db .");
				int sqlv = int.Parse (sh.S.Config.GetTag ("sqlite"));
				_database = new DataController (Utils.GetPath () + "/Dynamic/Scheduler.db", sqlv.ToString (), true);
				if (_database.JustCreated) {
					_database.ExecuteNonQuery ("CREATE TABLE  tasks (jid text, name text, muc text, add_date text, sch_date text, sch_time text, sch_period text, iscompleted integer, execute_datetime text, sch_commands text);");
				}
			} catch (Exception exx) {
				@out.write ("Scheduler: Exception: \n" + exx.Message + "\n\n" + exx.Source + "\n\n" + exx.StackTrace + "\n\n Inner:\n\n");
				_database = null;
			}
			
			this._initDateTime = DateTime.Now;
			_tasks = new Collection<SchedulerTask> ();
			
			//load tsak collection
			_tasks = this.LoadTasksOnTheDay (DateTime.Now);
			
			TimerCallback _tcb = this.MainTimerHandler;
			
			this._mainTimer = new Timer (_tcb);
			this._mainTimer.Change (1000, 50000);
			// 50 seconds
		}

		public Collection<SchedulerTask> LoadTasksOnTheDay (DateTime currentDT)
		{
			Collection<SchedulerTask> retValue = new Collection<SchedulerTask> ();
			
			DataTable _dt = _database.ExecuteDALoad("SELECT * FROM tasks WHERE sch_date = '"+currentDT.ToString("yyyy.MM.dd")+"'");
			if (_dt.Rows.Count > 0)
			{
				for (int i=0; i< _dt.Rows.Count; i++)
				{
					SchedulerTask _task = new SchedulerTask();
					_task.JID = new Jid((String)_dt.Rows[i]["jid"]);
					_task.Name = (String)_dt.Rows[i]["name"];
					_task.Muc = new Jid((String)_dt.Rows[i]["muc"]);
					_task.AddDate = Convert.ToDateTime((String)_dt.Rows[i]["add_date"]);					
					_task.ScheduleDate = Convert.ToDateTime((String)_dt.Rows[i]["sch_date"]);
					_task.ScheduleTime = TimeSpan.Parse((String)_dt.Rows[i]["sch_time"]);
					_task.SchedulePeriod = (SchedulerTaskPeriod)Enum.Parse(typeof(SchedulerTaskPeriod), (String)_dt.Rows[i]["name"]);
					_task.IsComplete = (int)_dt.Rows[i]["iscompleted"] == 1 ? true : false;
					_task.ScheduleCommands = (String)_dt.Rows[i]["sch_commands"];
					
					_task.ExecuteDateTime = Convert.ToDateTime((String)_dt.Rows[i]["execute_datetime"]);
					
					retValue.Add(_task);
				}
			}
			
			return retValue;
		}

		public void Reload ()
		{
			@out.write ("Scheduler. debug : Scheduller.Reload() entry point.");
		}

		public void AddTask (string jid, string name, string muc, string sch_date, string sch_time, string sch_period, string sch_commands)
		{
			if (this._database != null)
			{
				StringBuilder _sb = new StringBuilder();
				_sb.Append("INSERT INTO tasks VALUES (");
				_sb.Append("'"+jid+"', ");
				_sb.Append("'"+name+"', ");
				_sb.Append("'"+muc+"', ");
				_sb.Append("'"+DateTime.Now.ToString("yyyy.MM.dd")+"',");
				_sb.Append("'"+sch_date+"', ");
				_sb.Append("'"+sch_time+"', ");
				_sb.Append("'"+sch_period+"', ");
				_sb.Append("0, ");
				_sb.Append("'"+sch_commands+"'");
				_sb.Append(" );");
				
				_database.ExecuteNonQuery (_sb.ToString());
			}
		}

		internal void MainTimerHandler (object stateInfo)
		{
			//@out.write ("Scheduler: "+ DateTime.Now.ToString("hh:mm:ss:fff")); 
			if (this._initDateTime.Day != DateTime.Now.Day) {
				// reload tasks
				_tasks = this.LoadTasksOnTheDay (DateTime.Now);
				this._initDateTime = DateTime.Now;
			}
			
			foreach (SchedulerTask _st in this._tasks) {
			}
		}
	}
}
