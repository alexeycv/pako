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

		public Scheduler (SessionHandler sh)
		{
			this._sh = sh;
			
			// init database
			DataController _dc;
			try {
				@out.write ("Scheduler : Init dayabase /Dynamic/Scheduler.db .");
				int sqlv = int.Parse (sh.S.Config.GetTag ("sqlite"));
				_dc = new DataController (Utils.GetPath () + "/Dynamic/Scheduler.db", sqlv.ToString (), true);
				if (_dc.JustCreated) {
					_dc.ExecuteNonQuery ("CREATE TABLE  tasks (jid text, name text, muc text, add_date text, sch_date text, sch_time text, sch_period text, iscompleted integer ,sch_commands text);");
				}
			} catch (Exception exx) {
				@out.write ("Scheduler: Exception: \n" + exx.Message + "\n\n" + exx.Source + "\n\n" + exx.StackTrace + "\n\n Inner:\n\n");
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
			
			return retValue;
		}

		public void Reload ()
		{
			@out.write ("Scheduler. debug : Scheduller.Reload() entry point.");
		}

		public void AddTask ()
		{
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
