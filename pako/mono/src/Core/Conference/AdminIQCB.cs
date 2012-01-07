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

using System.Collections.Generic;
using System.Collections;
using System.Threading;
using System.Text;
using System;
using agsXMPP;
using Core.Kernel;
using Core.Xml;
using Core.DataBase;
using System.IO;
using agsXMPP.protocol.x.muc.iq.owner;
using agsXMPP.protocol.x.muc.iq.admin;
using agsXMPP.protocol.iq.disco;
using agsXMPP.protocol.client;
using agsXMPP.protocol.x.muc;
using agsXMPP.protocol;

namespace Core.Conference
{
	/// <summary>
	/// A class for handling users' role/affiliation ine the chat-room
	/// </summary>
	public class AdminIQCB
	{
		
		#region Variable list
	
		string                _nick;
		Jid                   _jid;
		XmppClientConnection  _con;
		string                _reason;
		
		#endregion Variable list
		
		/// <summary>
		/// A constructor
		/// </summary>
		public AdminIQCB()
		{			
		  //TODO: Insert TODO list here				
		}
		
       /// <summary>
       /// Change user's role, specifiing his nickname or Jid (leave the unneeded to be null)
       /// </summary>
       /// <param name="Response"></param>
       /// <param name="role"></param>
       /// <param name="room"></param>
       /// <param name="user"></param>
       /// <param name="nickname"></param>
       /// <param name="reason"></param>
       /// <param name="cb"></param>
       /// <param name="cbArg"></param>
        public void ChangeRole(XmppClientConnection con, Response Response, Role role, Jid Room, Jid user, string nickname, string reason)
        {
			_con = con;
			
            AdminIq aIq = new AdminIq();
            aIq.To = Room;
            aIq.Type = IqType.set;

            agsXMPP.protocol.x.muc.iq.admin.Item itm = new agsXMPP.protocol.x.muc.iq.admin.Item();
            itm.Role = role;

            if (user != null)
            {
                itm.Jid = user;
                _jid = user;
            }
            if (nickname != null)
            {
            	_nick = nickname;
                itm.Nickname = nickname;
            }
            if (reason != null)
            {
            	_reason = reason;
                itm.Reason = reason;
            }

            aIq.Query.AddItem(itm);
            _con.IqGrabber.SendIq(aIq, new IqCB(Handle_IQ_result), Response);
      
        }

       /// <summary>
        ///  Change user's affiliation, specifiing his nickname or Jid (leave the unneeded to be null)
       /// </summary>
       /// <param name="Response"></param>
       /// <param name="affiliation"></param>
       /// <param name="room"></param>
       /// <param name="user"></param>
       /// <param name="nickname"></param>
       /// <param name="reason"></param>
       /// <param name="cb"></param>
       /// <param name="cbArg"></param>
        public void ChangeAffiliation(XmppClientConnection con, Response Response, Affiliation affiliation, Jid Room, Jid user, string nickname, string reason)
        {
            _con = con;
            AdminIq aIq = new AdminIq();
            aIq.To = Room;
            aIq.Type = IqType.set;

            agsXMPP.protocol.x.muc.iq.admin.Item itm = new agsXMPP.protocol.x.muc.iq.admin.Item();
            itm.Affiliation = affiliation;

            
            if (user != null)
            {
                itm.Jid = user;
                _jid = user;
            }
            if (nickname != null)
            {
            	_nick = nickname;
                itm.Nickname = nickname;
            }

            if (reason != null)
            {
            	_reason = reason;
                itm.Reason = reason;
            }
            aIq.Query.AddItem(itm);
            _con.IqGrabber.SendIq(aIq, new IqCB(Handle_IQ_result), Response);
        }

        
        /// <summary>
        /// Handles the callback and answers according to fail/success.
        /// </summary>
        private void Handle_IQ_result(object obj, IQ iq, object arg)
        {
            if (arg != null)
            {
                if (iq.Type == IqType.error)
                {
                    ((Response)arg).Reply(((Response)arg).Deny());
                }
                else
                    ((Response)arg).Reply(((Response)arg).Agree());
            }

        }
        
      
		
		
		
	}
}
