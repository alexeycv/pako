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
using agsXMPP.protocol.iq.time;
using agsXMPP.protocol.client;
using Core.Kernel;

namespace Plugin
{
    public class TimeCB
    {
        Response m_r;
        Jid m_jid;



        public TimeCB(Response r, Jid Jid)
        {
            m_r = r;
            m_jid = Jid;
            TimeIq ti = new TimeIq();
            ti.Type = IqType.get;
            ti.To = m_jid;
            ti.GenerateId();
            m_r.Connection.IqGrabber.SendIq(ti, new IqCB(TimeExtractor), null);

        }




        private void TimeExtractor(object obj, IQ iq, object arg)
        {
           
                @out.exe(" before translate  =>  ");
                agsXMPP.protocol.iq.time.Time vi = iq.Query as agsXMPP.protocol.iq.time.Time;
                @out.exe(" after translate  =>  ");

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
                if (vi != null)
                {
                    if (iq.Type == IqType.error)
                    {
                        if (iq.Error.HasTag("feature-not-implemented"))
                        {
                            if (m_r.Msg.From.ToString() != m_jid.ToString())
                                answer = m_r.f("iq_not_implemented", jid);
                            else
                                answer = m_r.f("iq_not_implemented_self");
                        }
                        else
                            answer = m_r.f("version_error", jid);
                    }
                    else
                    {

                        string Tz = vi.Tz;
                        Tz = String.IsNullOrEmpty(Tz) ? "" : Tz;

                        string display = vi.Display;
                        display = String.IsNullOrEmpty(display) ? "" : display;

                        string utc = vi.Utc;
                        utc = String.IsNullOrEmpty(utc) ? " unknown " : utc;

     
                        string full = display + " " + Tz;
                        if (full.TrimStart() == "")
                            full = utc;

                        if (muc)
                        {
                            // @out.exe(" time muc  =>  ");
                            if (m_r.Msg.From.ToString() != m_jid.ToString())
                                answer = m_r.f("time_muc", jid) + " " + full;
                            else
                                answer = m_r.f("time_muc_self") + " " + full;
                        }
                        else
                        {
                            // @out.exe(" version  muc =>  ");
                            answer = m_r.f("time_server", jid) + " " + full;
                        }
                    }
                }
                else
                {
                    @out.exe(" version null  =>  ");
                    answer = m_r.f("version_error", jid);
                }

                m_r.Reply(answer);
            }
        




    }
}
