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


using System;
using System.Collections.Generic;
using System.Text;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.protocol;
using agsXMPP.protocol.x.muc;
using agsXMPP.protocol.x.muc.iq.admin;
using Core.Kernel;
using Core.Conference;
using agsXMPP.Xml.Dom;

namespace Core.Conference
{
    public class MucConfig
    {
        /*
         *  <iq type="get" to="room@conference.jabber.dom" id="agsXMPP_03" >
         *  <query xmlns="http://jabber.org/protocol/muc#admin">
         *  <item affiliation="outcast" />
         *  </query>
         *  </iq>
         */

        Jid m_jid;
        MucManager manager;
        XmppClientConnection con;

        /// <summary>
        /// A class for managing the conference: see the configurator, see the administrators list, ban list, members list, owners list
        /// </summary>
        /// <param name="room"></param>
        /// <param name="connection"></param>
        public MucConfig(Jid room, XmppClientConnection connection)
        {
            m_jid = room;
            con = connection;
            manager = new MucManager(con);
        }


        /// <summary>
        /// Get the list of the outcasts list
        /// </summary>
        /// <param name="res"></param>
        /// <param name="count"></param>
        public void GetOutcastlist(Response res, int? count)
        {
            manager.RequestBanList(m_jid, new IqCB(get_list), new object[] { res, count, AdminQueryType.OUTCAST_LIST});
        }

        /// <summary>
        /// Get the list of the members list
        /// </summary>
        /// <param name="res"></param>
        /// <param name="count"></param>
        public void GetMemberlist(Response res, int? count)
        {
            manager.RequestMemberList(m_jid, new IqCB(get_list), new object[] { res, count, AdminQueryType.MEMBER_LIST });
        }

        /// <summary>
        /// Get the list of the administrators list
        /// </summary>
        /// <param name="res"></param>
        /// <param name="count"></param>
        public void GetAdminlist(Response res, int? count)
        {
            manager.RequestAdminList(m_jid, new IqCB(get_list), new object[] { res, count, AdminQueryType.ADMIN_LIST });
        }

        /// <summary>
        /// Get the list of the owners list
        /// </summary>
        /// <param name="res"></param>
        /// <param name="count"></param>
        public void GetOwnerlist(Response res, int? count)
        {
            manager.RequestOwnerList(m_jid, new IqCB(get_list), new object[] { res, count, AdminQueryType.OWNER_LIST });
        }


        /// <summary>
        /// A private callback method for an "admin" iq, which 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="iq"></param>
        /// <param name="arg"></param>
        private void get_list(object o, IQ iq, object arg)
        {
            object[] args = (object[])arg;
            Response r = (Response)args[0];
            int? count = (int?)args[1];
            AdminQueryType type = (AdminQueryType)args[2];
            if (iq.Error != null)
            {
                r.Reply(r.f("version_error"));
                return;
            }

            string data = null;
            string error = null;
            string empty = null;
            switch (type)
            {

                case AdminQueryType.OUTCAST_LIST:
                    data =  "ban_list";
                    empty = "ban_list_empty";
                    error = "ban_list_error";
                    break;

                case AdminQueryType.MEMBER_LIST:
                    data =  "member_list";
                    empty = "member_list_empty";
                    error = "member_list_error";
                    break;

                case AdminQueryType.ADMIN_LIST:
                    data =  "admin_list";
                    empty = "admin_list_empty";
                    error = "admin_list_error";
                    break;
                
                case AdminQueryType.OWNER_LIST:
                    data =  "owner_list";
                    empty = "owner_list_empty";
                    error = "owner_list_error";
                    break;
            }
           
            if (iq.Query != null)
            {
                ElementList els = iq.Query.SelectElements("item");
                if (els.Count == 0)
                {
                    r.Reply(r.f(empty));
                    return;
                }
              
                 
                int i = 0; 
                int all = els.Count;
                data = r.f(data);
                foreach (Element el in els)
                {
                   i++;
                   data += "\n" + i.ToString() + ") " + el.GetAttribute("jid") + (el.HasTag("resaon") && el.GetTag("reason") != "" ? "     '"+el.GetTag("reason")+"' " : "");
                   if (count != null && i == count)
                       break;
                }
                data += "\n-- " + all.ToString() +" --";

                r.Reply(data);
                return;
            }

            r.Reply(r.f(error));

        }

    }
}
