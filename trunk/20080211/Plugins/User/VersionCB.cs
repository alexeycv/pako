using System;
using System.Collections.Generic;
using System.Text;
using agsXMPP;
using agsXMPP.protocol.iq.version;
using agsXMPP.protocol.client;
using Core.Client;

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
           
            Console.WriteLine();

          
                Console.WriteLine(" before translate  =>  ");
                agsXMPP.protocol.iq.version.Version vi = iq.Query as agsXMPP.protocol.iq.version.Version;
                Console.WriteLine(" after translate  =>  ");

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
                Console.WriteLine(" version  =>  ");
                if (vi != null)
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


                        Console.WriteLine(" version not null  =>  ");
                        string os = vi.Os;
                        os = String.IsNullOrEmpty(os) ? "" :  os  ;

                        string name = vi.Name;
                        name = String.IsNullOrEmpty(name) ? "" : name;

                        string ver = vi.Ver;
                        ver = String.IsNullOrEmpty(ver) ? "" : ver;
                        string full = m_r.FormatPattern("version_packet").Replace("{1}",name)
                            .Replace("{2}",ver)
                            .Replace("{3}",os);

                        if (muc)
                        {
                            Console.WriteLine(" version muc  =>  ");
                            if (m_r.Msg.From.ToString() != m_jid.ToString())
                                answer = m_r.FormatPattern("version_muc", jid) + " " + full;
                            else
                                answer = m_r.FormatPattern("version_muc_self") + " " + full;

                        }
                        else
                        {
                            Console.WriteLine(" version  muc =>  ");
                            answer = m_r.FormatPattern("version_server", jid) + " " + full;
                        }
                    }

                }
                else
                {
                    Console.WriteLine(" version null  =>  ");
                    answer = m_r.FormatPattern("version_error", jid);
                }
                
                m_r.Reply(answer);
           
        }




    }
}
