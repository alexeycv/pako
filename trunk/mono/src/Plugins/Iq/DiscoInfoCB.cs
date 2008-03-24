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
using agsXMPP.protocol.iq.disco;
using agsXMPP.protocol;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;
using Core.Kernel;

namespace Plugin
{
    public class DiscoInfoCB
    {
        Response m_r;
        Jid m_jid;



        public DiscoInfoCB(Response r, Jid Jid)
        {
            m_r = r;
            m_jid = Jid;
            DiscoInfoIq ti = new DiscoInfoIq();
            ti.Type = IqType.get;
            ti.To = m_jid;
            ti.GenerateId();
            m_r.Connection.IqGrabber.SendIq(ti, new IqCB(DiscoExtractor), null);
        }

        private void DiscoExtractor(object obj, IQ iq, object arg)
        {

            DiscoInfo d_items = iq.Query as DiscoInfo;

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


            if (d_items != null)
            {
                if (iq.Type == IqType.error)
                {
                    answer = m_r.f("version_error", jid);
                }
                else
                {



                    DiscoFeature[] features = d_items.GetFeatures();
                    DiscoIdentity[] identities = d_items.GetIdentities();

                    if ((features.Length == 0) && (identities.Length == 0))
                    {
                        answer = m_r.f("disco_info_empty", jid);
                    }
                    else
                    {

                        string data1 = "";
                        string data2 = "";
                        string pattern = m_r.f("disco_info");


                        int ii = 1;
                        foreach (DiscoIdentity identity in d_items.GetIdentities())
                        {
                            data1 += ii.ToString() + ") " + identity.Type+ " - "+ identity.Category+ ": "+ identity.Name + "\n";
                            ii++;
                        }

                        int i = 1;

                        foreach (DiscoFeature feature in features)
                        {
                            data2 += i.ToString() + ") " + feature.Var + "\n";
                            i++;
                        }

                        data1 = data1.TrimEnd('\n');
                        data2 = data2.TrimEnd('\n');
                        answer = pattern.Replace("{1}", data1).Replace("{2}", data2);

                    }

                }
            }
            else
            {
                answer = m_r.f("version_error", jid);
            }

            m_r.Reply(answer);
        }





    }
}
