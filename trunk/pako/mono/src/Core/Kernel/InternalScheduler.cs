/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Pako Jabber-bot. Bbodio's Lab.                                                *
 * Copyright. All rights reserved © 2007-2010 by Pako bot developers team        *
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
using agsXMPP;
using Core.Conference;
using Core.Xml;
using Core.DataBase;
using agsXMPP.Xml.Dom;
using agsXMPP.protocol.client;
using Core.Other;
using System.Timers;

namespace Core.Kernel
{
    /// <summary>
    /// Internal sheduller class that make internal events for bot like save timers
    /// </summary>
    public class InternalScheduler
    {
		Timer _saveTimer;
		
		#region Properties
		
		public Timer SaveTimer
		{
			get {return _saveTimer;}
			set {_saveTimer = value;}
		}
		
		#endregion
		
		#region Constructors
		
		InternalScheduler()
		{
			SaveTimer = new Timer();
			SaveTimer.Interval = 300;
			
			// Add an event handler
			SaveTimer.Elapsed += new ElapsedEventHandler(SaveTimerElapsed);
			
			SaveTimer.Enabled = true;
		}
		
		#endregion
		
		#region Methods
		
		public void SaveMethod()
		{
		}
		
		#endregion
		
		#region Event handlers
		
		private void SaveTimerElapsed(object source, ElapsedEventArgs e)
		{
			this.SaveMethod();
		}
		
		#endregion		
	}
	
}