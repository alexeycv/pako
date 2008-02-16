﻿/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
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
using agsXMPP.protocol.iq.version;
using agsXMPP.protocol.client;
using Core.Client;

namespace Plugin
{
    public class PingCB
    {
        Response m_r;
        Jid m_jid;
        long ticks;


        public PingCB(Response r, Jid Jid)
        {

            m_r = r;
            m_jid = Jid;
            VersionIq vi = new VersionIq();
            vi.Type = IqType.get;
            vi.To = m_jid;
            vi.GenerateId();
            m_r.Connection.IqGrabber.SendIq(vi, new IqCB(VersionExtractor), null);
            ticks = DateTime.Now.Ticks;

        }




        private void VersionExtractor(object obj, IQ iq, object arg)
        {

            long tt = DateTime.Now.Ticks - ticks;
            TimeSpan ts = TimeSpan.FromTicks(tt);
            string data =
                (ts.Days > 31 ? Convert.ToString(System.Math.Truncate((double)ts.Days / 31)) + m_r.FormatPattern("month") + " " : "") +
                (ts.Days > 0 ? ts.Days.ToString() + " " + m_r.FormatPattern("day") + " " : "") +
                (ts.Hours > 0 ? ts.Hours.ToString() + " " + m_r.FormatPattern("hour") + " " : "") +
                (ts.Minutes > 0 ? ts.Minutes.ToString() + " " + m_r.FormatPattern("minute") + " " : "");

            int mili = ts.Milliseconds;
            string s_mili = mili < 100 ? "0" + mili.ToString() : mili.ToString();

               if (ts.Seconds > 0)
               {
                       if (ts.Milliseconds > 0)
                           data += ts.Seconds + "," + s_mili + " " + m_r.FormatPattern("second");
                       else
                           data += ts.Seconds + " " + m_r.FormatPattern("second");
               }
            else
               {
                         if (ts.Milliseconds > 0)
                             data += "0," + s_mili + " " + m_r.FormatPattern("second");
               }

           


            agsXMPP.protocol.iq.version.Version vi = iq.Query as agsXMPP.protocol.iq.version.Version;


            string answer;
            string jid = m_jid.ToString();
            bool muc = m_r.MUC != null;
            if (muc)
            {
                muc = m_jid.Resource != null;
                if (muc)
                {
                    muc = m_r.MUC.UserExists(m_jid.Resource);
                    if (muc)
                        jid = m_jid.Resource;
                }
            }
            @out.exe("1");
            if (vi != null)
           {
               @out.exe("2");
                if (iq.Error != null)
               if (iq.Error.HasTag("remote-server-not-found"))
               {
                   answer = m_r.FormatPattern("version_error", jid);
                   return;
               }

               @out.exe("3");

               


                    if (muc)
                    {
                        @out.exe("4");
                       // @out.exe(" version muc  =>  ");
                        if (m_r.Msg.From.ToString() != m_jid.ToString())
                            answer = m_r.FormatPattern("ping_muc", jid) + " " + data;
                        else
                            answer = m_r.FormatPattern("ping_muc_self") + " " + data;

                    }
                    else
                    {
                        //@out.exe(" version  muc =>  ");
                        answer = m_r.FormatPattern("ping_server", jid) + " " + data;
                    }
               

            }
            else
            {   
                @out.exe("5");
                answer = m_r.FormatPattern("version_error", jid);
            }
         
            @out.exe("6");
            m_r.Reply(answer);

        }




    }
}
