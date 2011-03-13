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

namespace Plugin
{
 
    public class Scheduler		
    {
		Timer _mainTimer;
		Collection<SchedulerTask> _tasks;
		DateTime _initDateTime;
		
		public Scheduler()
		{
			this._initDateTime = DateTime.Now;
			_tasks = new Collection<SchedulerTask>();
			
			//load tsak collection
			_tasks = this.LoadTasksOnTheDay(DateTime.Now);
			
			TimerCallback _tcb = this.MainTimerHandler;
			
			this._mainTimer = new Timer(_tcb);
			this._mainTimer.Change(1000, 50000); // 50 seconds
		}
		
		public Collection<SchedulerTask> LoadTasksOnTheDay(DateTime currentDT)
		{
			Collection<SchedulerTask> retValue = new Collection<SchedulerTask>();
			
			return retValue;
		}
		
		internal void MainTimerHandler(object stateInfo)
		{
			//@out.write ("Scheduler: "+ DateTime.Now.ToString("hh:mm:ss:fff")); 
			if (this._initDateTime.Day != DateTime.Now.Day)
			{
				// reload tasks
				_tasks = this.LoadTasksOnTheDay(DateTime.Now);
				this._initDateTime = DateTime.Now;
			}
			
			foreach (SchedulerTask _st in this._tasks)
			{
			}
		}
	}
}
