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
using agsXMPP.protocol.iq.vcard;
using agsXMPP.protocol;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;
using Core.Kernel;
using Core.Other;

namespace Plugin
{
    public class VCardCB
    {
        Response m_r;
        Jid m_jid;



        /// <summary>
        /// </summary>
        /// <param name="r"></param>
        /// <param name="Jid"></param>
        /// <param name="number"></param>
        public VCardCB(Response r, Jid Jid)
        {
            m_r = r;
            m_jid = Jid;
            VcardIq iq = new VcardIq();
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
                            answer = m_r.f("iq_not_implemented", jid);
                        else
                            answer = m_r.f("iq_not_implemented_self");
                    }
                    else
                        answer = m_r.f("version_error", jid);
                }
                else
                {
                    answer = "\n";
                    Vcard vc = iq.Query as Vcard;
                    string name = "";
                    string family = "";
                    if (vc.Name != null)
                    {
                        if (vc.Name.Given != null)
                            name += vc.Name.Given + " ";
                        if (vc.Name.Middle != null)
                            name += vc.Name.Middle + " ";
                        if (vc.Name.Family != null)
                            family = vc.Name.Family + "\n";
                    }
                    if (vc.Fullname != null)
                        answer += "Name: " + vc.Fullname + "\n";
                    if (name != "")
                        answer += "Given: " + name + "\n";
                    if (family != "")
                        answer += "Family: " + family + "\n";
                    if (vc.Nickname != null)
                        answer += "NickName: " + vc.Nickname + "\n";
                    if (vc.JabberId != null)
                        answer += "JID: " + vc.JabberId.ToString() + "\n";
                    Address[] adds = vc.GetAddresses();
                    if (adds != null)
                        if (adds.Length != 0)
                        {
                            answer += "Addresses:\n";
                            foreach (Address add in adds)
                            {
                                answer += "    ~~~\n";
                                if (add.Region != null)
                                    answer += "       Region: " + add.Region+"\n";
                                if (add.PostalCode != null)
                                    answer += "       PostalCode: " + add.PostalCode+"\n";
                                if (add.Locality != null)
                                    answer += "       Locality: " + add.Locality+"\n";
                                if (add.Street != null)
                                    answer += "       Street: " + add.Street+"\n";

                            }
                        }

                    Email[] ms = vc.GetEmailAddresses();
                    if (ms != null)
                        if (ms.Length != 0)
                        {
                            answer += "E-Mails:\n";
                            foreach (Email ml in ms)
                            {
                                answer += "    ~~~\n";
                                if (ml.UserId != null)
                                    answer += "       UderId: " + ml.UserId + "\n";
                            }
                        }
                    Telephone[] tn = vc.GetTelephoneNumbers();
                    if (tn != null)
                        if (tn.Length != 0)
                        {
                            answer += "PhoneNumbers:\n";
                            foreach (Telephone tl in tn)
                            {
                                answer += "    ~~~\n";
                                if (tl.Number != null)
                                    answer += "       Number: " + tl.Number + "\n";
                            }
                        }
                    if (vc.Organization != null)
                    {
                        if (vc.Organization.Name != null)
                            answer += "Organization: " + vc.Organization.Name + "\n";
                        if (vc.Organization.Unit != null)
                            answer += "Unit: " + vc.Organization.Unit + "\n";
                    }
                    if (vc.Url != null)
                        answer += "URL: " + vc.Url + "\n";

                    answer = answer == "\n" ? m_r.f("vcard_empty") : answer;
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
