using System;
using System.Collections.Generic;
using System.Text;
using agsXMPP;
using agsXMPP.protocol.iq.disco;
using agsXMPP.protocol;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;
using Core.Client;

namespace Plugin
{
    public class DiscoCB
    {
        Response m_r;
        Jid m_jid;
        int num;



        public DiscoCB(Response r, Jid Jid, int number)
        {
            m_r = r;
            m_jid = Jid;
            DiscoItemsIq ti = new DiscoItemsIq();
            ti.Type = IqType.get;
            ti.To = m_jid;
            ti.GenerateId();
            num = number;
            m_r.Connection.IqGrabber.SendIq(ti, new IqCB(DiscoExtractor), null);

        }




        private void DiscoExtractor(object obj, IQ iq, object arg)
        {

            Console.WriteLine(" before translate  =>  ");
            DiscoItems d_items = iq.Query as DiscoItems;
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

            
            if (d_items != null)
            {
                if (iq.Type == IqType.error)
                {
                    answer = m_r.FormatPattern("version_error", jid);
                }
                else
                {



                    DiscoItem[] items = d_items.GetDiscoItems();

                    if (items.Length == 0)
                    {
                        answer = m_r.FormatPattern("disco_empty", jid);
                    }
                    else
                    {

                        string data = "";

                        int i = 1;
                        num = num < 1 ? items.Length : num;
                        int all = 0;

                        foreach (DiscoItem item in items)
                        {
                            if (i <= num)
                            {
                                data += "\n" + i.ToString() + ") " + item.Jid;
                                if (item.Name != "")
                                    data += " [" + item.Name+"]";
                            }
                            else
                            {
                                all = items.Length;
                                break;
                            }
                               
                            i++;
                        }

                        answer = m_r.FormatPattern("disco_result") + data + (all != 0 ? "\n-- "+all.ToString()+" --" : "") ;

                    }

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
