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
using agsXMPP.protocol.iq.version;
using agsXMPP.protocol.client;
using Core.Kernel;

namespace Plugin
{
    public class VersionCB
    {
        Response m_r;
        Jid m_jid;



        public VersionCB(Response r, Jid Jid)
        {
            m_r = r;
            m_jid = Jid;
            VersionIq vi = new VersionIq();
            vi.Type = IqType.get;
            vi.To = m_jid;
            vi.GenerateId();
            m_r.Connection.IqGrabber.SendIq(vi, new IqCB(VersionExtractor), null); 
            
        }




        private void VersionExtractor(object obj, IQ iq, object arg)
        {
           

          
                @out.exe(" before translate  =>  ");
                agsXMPP.protocol.iq.version.Version vi = iq.Query as agsXMPP.protocol.iq.version.Version;
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
                @out.exe(" version  =>  ");
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


                        @out.exe(" version not null  =>  ");
                        string os = vi.Os;
                        os = String.IsNullOrEmpty(os) ? "" :  os  ;

                        string name = vi.Name;
                        name = String.IsNullOrEmpty(name) ? "" : name;

                        string ver = vi.Ver;
                        ver = String.IsNullOrEmpty(ver) ? "" : ver;
                        string full = m_r.f("version_packet").Replace("{1}",name)
                            .Replace("{2}",ver)
                            .Replace("{3}",os);

                        if (muc)
                        {
                            @out.exe(" version muc  =>  ");
                            if (m_r.Msg.From.ToString() != m_jid.ToString())
                                answer = m_r.f("version_muc", jid) + " " + full;
                            else
                                answer = m_r.f("version_muc_self") + " " + full;

                        }
                        else
                        {
                            @out.exe(" version  muc =>  ");
                            answer = m_r.f("version_server", jid) + " " + full;
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
