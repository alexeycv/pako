using System;
using System.Collections.Generic;
using System.Text;
using agsXMPP;
using agsXMPP.protocol.iq.last;
using agsXMPP.protocol;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;
using Core.Client;

namespace Plugin
{
    public class LastCB
    {
        Response m_r;
        Jid m_jid;



        /// <summary>
        /// 
        /// <stat name='time/uptime'/>
	    /// <stat name='users/online'/>
	    /// <stat name='bandwidth/packets-in'/>
	    /// <stat name='bandwidth/packets-out'/>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="Jid"></param>
        /// <param name="number"></param>
        public LastCB(Response r, Jid Jid)
        {
            m_r = r;
            m_jid = Jid;
            LastIq iq = new LastIq();
            iq.Type = IqType.get;
            iq.To = m_jid;
            iq.GenerateId();
            m_r.Connection.IqGrabber.SendIq(iq, new IqCB(LastExtractor), null);

        }




        private void LastExtractor(object obj, IQ iq, object arg)
        {

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


            if (iq.Query != null)
            {
                if (iq.Type == IqType.error)
                {
                    if (iq.Error.HasTag("feature-not-implemented"))
                    {
                        if (m_r.Msg.From.ToString() != m_jid.ToString())
                            answer = m_r.FormatPattern("iq_not_implemented", jid);
                        else
                            answer = m_r.FormatPattern("iq_not_implemented_self");
                    }
                    else
                        answer = m_r.FormatPattern("version_error", jid);
                }
                else
                {
                    long ticks = iq.Query.GetAttributeInt("seconds");
                    @out.exe("<"+ticks.ToString());
                    ticks = ticks * 10000000;
                    @out.exe("<" + ticks.ToString());
                    answer = m_r.FormatPattern("uptime", jid) + " " + Utils.FormatTimeSpan(ticks, m_r);
                }
            }
            else
            {
                answer = m_r.FormatPattern("version_error", jid);
            }

            m_r.Reply(answer);
        }





    }
}
