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


            if (iq.Vcard != null)
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
                    Vcard vc = iq.Vcard as Vcard;
                    string name = "";
                    string family = "";
                    if (vc.Name != null)
                    {
                    	if (vc.Name.Given != null) 
                    	    if (vc.Name.Given.Trim() != "")
                            name += vc.Name.Given + " ";
                        if (vc.Name.Middle != null)
                        	if (vc.Name.Middle.Trim() != "")
                            name += vc.Name.Middle + " ";
                        if (vc.Name.Family != null)
                        	if (vc.Name.Family.Trim() != "")
                            family = vc.Name.Family + "\n";
                    }
                    if (vc.Fullname != null)
                    	if (vc.Fullname.Trim() != "")
                        answer += "Name:          " + vc.Fullname + "\n";
                    if (name != "")
                        answer += "Given:         " + name + "\n";
                    if (family != "")
                        answer += "Family:        " + family + "\n";
                    if (vc.Nickname != null)
                    	if (vc.Nickname.Trim() != "")
                        answer += "NickName:      " + vc.Nickname + "\n";
                    if (vc.HasTag("BDAY"))
                    	if (vc.GetTag("BDAY").Trim() != "")
                        answer += "Birthday:      " + vc.GetTag("BDAY") + "\n";
                    Address[] adds = vc.GetAddresses();
                    if (adds != null)
                        if (adds.Length != 0)
                        {
                            foreach (Address add in adds)
                            {
                                if (add.Country != null)
                                	if (add.Country.Trim() != "")
                                    answer += "Country:       " + add.Country + "\n";
                                if (add.Region != null)
                                	if (add.Region.Trim() != "")
                                    answer += "Region:        " + add.Region+"\n";
                                if (add.PostalCode != null)
                                	if (add.PostalCode.Trim() != "")
                                    answer += "PostalCode:    " + add.PostalCode+"\n";
                                if (add.Locality != null)
                                	 	if (add.Locality.Trim() != "")
                                    answer += "Locality:      " + add.Locality+"\n";
                                if (add.Street != null)
                                	if (add.Street.Trim() != "")
                                    answer += "Street:        " + add.Street+"\n";

                            }
                        }

                    Email[] ms = vc.GetEmailAddresses();
                    if (ms != null)
                        if (ms.Length != 0)
                        {
                            foreach (Email ml in ms)
                            {
                                if (ml.UserId != null)
                                	if (ml.UserId.Trim() != "")
                                    answer += "E-mail:        " + ml.UserId + "\n";
                            }
                        }
                    Telephone[] tn = vc.GetTelephoneNumbers();
                    if (tn != null)
                        if (tn.Length != 0)
                        {
                            foreach (Telephone tl in tn)
                            {
                                if (tl.Number != null)
                                		if (tl.Number.Trim() != "")
                                    answer += "Telephone:     " + tl.Number + "\n";
                            }
                        }
                    if (vc.Organization != null)
                    {
                        if (vc.Organization.Name != null)
                        	if (vc.Organization.Name.Trim() != "")
                            answer += "Organization:  " + vc.Organization.Name + "\n";
                        if (vc.Organization.Unit != null)
                        	if (vc.Organization.Unit.Trim() != "")
                            answer += "Unit:          " + vc.Organization.Unit + "\n";
                    }
                    if (vc.Url != null)
                    	if (vc.Url.Trim() != "")
                        answer += "URL:            " + vc.Url + "\n";
                    if (vc.Description != null)
                    		if (vc.Description.Trim() != "")
                        answer += "About:         " + vc.Description + "\n";
                    answer = answer == "\n" ? m_r.f("vcard_empty") : m_r.f("vcard_info")+answer.TrimEnd('\n');
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
