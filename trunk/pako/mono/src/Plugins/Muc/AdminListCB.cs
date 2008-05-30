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
using agsXMPP.protocol.x.muc.iq.admin;
using agsXMPP.protocol;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;
using Core.Kernel;

namespace Plugin
{
    public class AdminListCB
    {
        Response m_r;
        Jid m_jid;



        public AdminListCB(Response r, Jid Room)
        {
            m_r = r;
            m_jid = Room;
            AdminIq ti = new AdminIq();
            ti.Type = IqType.get;
            ti.To = m_jid;
            ti.GenerateId();
            m_r.Connection.IqGrabber.SendIq(ti, new IqCB(AdminListExtractor), null);

        }




        private void AdminListExtractor(object obj, IQ iq, object arg)
        {

            Admin list = iq.Query as Admin;

           // string answer;
            string jid = m_jid.ToString();
            bool muc = m_r.MUC != null;
           
      
            /*
            if (d_items != null)
            {
                if (iq.Type == IqType.error)
                {
                    answer = m_r.f("version_error", jid);
                }
                else
                {



                    DiscoItem[] items = d_items.GetDiscoItems();

                    if (items.Length == 0)
                    {
                        answer = m_r.f("disco_muc_empty", jid);
                    }
                    else
                    {

                        string data = "";

                        int i = 1;

                        foreach (DiscoItem item in items)
                        {
                                if (item.Name != null)
                                data += "\n" + i.ToString() + ") " + item.Name;
                            i++;
                        }

                        answer = m_r.f("disco_muc_result", jid) + data + ("\n-- "+items.Length.ToString()+" --") ;

                    }

                }
            }
            else
            {
                answer = m_r.f("admin_iq_list_error");
            }

            m_r.Reply(answer);
            
            */
            
        
        }





    }
}
