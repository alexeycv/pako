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

#region Namespaces

using System.Collections.Generic;
using System.Collections;
using System.Threading;
using System.Text;
using System;
using agsXMPP;
using Core.Kernel;
using Core.Xml;
using Core.DataBase;
using agsXMPP.Xml.Dom;
using System.IO;
using agsXMPP.protocol.client;
using agsXMPP.protocol.x.muc;
using Mono.Data.SqliteClient;
using System.Reflection;
using System.Diagnostics;
using System.Text.RegularExpressions;
using agsXMPP.protocol.x.muc.iq.admin;
using agsXMPP.protocol.x.muc.iq.owner;
using Core.Other;

#endregion Namespaces


namespace Core.Conference
{

	public class TimedJoiner
	{
		MUC Muc;
		Timer _timer;
		
		public TimedJoiner(MUC _muc, Int32 _time)
		{
			this.Muc = _muc;
			
			TimerCallback _tcb = this.Joiner;
			
			this._timer = new Timer(_tcb);
			this._timer.Change(_time*5000, 600000);
		}
		
		internal void Joiner(object stateInfo)
		{
			if (this.Muc!= null)
			{
				this.Muc.Join();
				
				this.Muc = null;
			}
		}
	}
}
