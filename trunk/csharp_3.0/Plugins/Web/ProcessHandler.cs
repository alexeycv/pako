using System;
using System.Collections.Generic;
using System.Text;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;
using System.Threading;
using System.IO;
using Core.Client;
using Core.Special;
using Core.Conference;
using Core.Manager;
using agsXMPP.protocol.x.muc;
using System.Net;
using System.Web;


namespace www
{
    public class WwwHandler
    {
        string[] ws;
        Message m_msg;
        Response m_r;
        string self;
        bool syntax_error = false;
        Jid s_jid;
        string s_nick;
        string m_b;
        string n;
        string d;
        SessionHandler Sh;

        public WwwHandler(Response r,string Name)
        {

            Sh = r.Sh;
            m_b = r.Msg.Body;
            ws = m_b.SplitEx(2);
            m_msg = r.Msg;
            m_r = r;
            s_jid = r.Msg.From;
            s_nick = r.Msg.From.Resource;
            d = r.Delimiter;
            n = Name;
            if (ws.Length < 2)
            {
                r.Reply(r.FormatPattern("volume_info", n, d + n.ToLower() + " list"));
                return;
            }

         


            self = ws[0] + " " + ws[1];
           // Handle();
            Handle();
        }

     


        public void Handle()
        {


    
            string cmd = ws[1];
            string rs = null;

         //   @out.exe("ppppppppppppppppp");
            switch (cmd)
            {






       

                case "dns":
                    {
                        if (ws.Length > 2)
                        {
                           /* try
                            {
                                IPHostEntry ip_host_entry = Dns.GetHostByAddress(ws[2]);
                                rs = ip_host_entry.HostName;
                            }
                            catch
                            {
*/
                        //    int i = Convert.ToInt32("rtr");
                                try
                                {
                                    IPHostEntry ip_host_entry = Dns.GetHostEntry(ws[2]);
                                    string ip_data = "";
                                    bool _host = false;
                                    foreach (IPAddress s in ip_host_entry.AddressList)
                                    {
                                        ip_data += s.ToString() + " ";
                                        if (s.ToString().Contains(ws[2]))
                                        {
                                            _host = true;
                                            break;
                                        }

                                    }

                                    if (_host)
                                        ip_data = ip_host_entry.HostName;
                                    else
                                    ip_data = ip_data.Trim().Replace(" ", ", ");
       
                                   
                                    rs = ip_data;
                                }
                                catch
                                {
                                    rs = m_r.FormatPattern("resolve_fail");
                                }
                           // }
                        }else
                            syntax_error = true;
                        break;
                    }

                case "whois":
                    {
                        if (ws.Length > 2)
                        {
                            try
                            {
                                string data = WhoisResolver.Whois(ws[2].Trim().ToLower());
                                if (data != null)
                                    rs = data;
                                else
                                    rs = m_r.FormatPattern("whois_fail");                         
                            }
                            catch
                            {
                                rs = m_r.FormatPattern("whois_fail");
                            }
                        }else
                            syntax_error = true;
                        break;
                    }

                case "google":
                    {
                        ws = m_b.SplitEx(3);
                        if (ws.Length > 3)
                        {
                            int number = 1;
                            try
                            {
                                number = Convert.ToInt32(ws[2]);

                            }
                            catch
                            {
                                syntax_error = true;
                            }
                            if ((number <= 10) && (number > 0))
                            {

                                Google g = new Google(ws[3], number, m_r);
                                return;
                            }
                            else syntax_error = true;
                        }
                        else
                            syntax_error = true;
                        break;
                    }

                case "gtlangs":
                    {
                        if (ws.Length == 2)
                        {
                            TranslateUtil tr = new TranslateUtil();
                            string data = m_r.FormatPattern("lang_pairs");
                            foreach (string key in tr.Modes.Keys)
                            {
                                data += "\n" + key + " : " + ((string[])tr.Modes[key])[1];
                            }
                            rs = data;
                        }
                        else
                            syntax_error = true;
                        break;
                    }

                case "gt":
                    {
                        ws = m_msg.Body.Trim().SplitEx(3);

                        if (ws.Length > 3)
                        {
                            TranslateUtil tr = new TranslateUtil();
                            tr.Execute(ws[3], ws[2], m_r);
                            return;
                        }
                        else
                            syntax_error = true;
                        break;
                    }

                case "curr":
                    {
                        ws = m_msg.Body.SplitEx(5);

                        if (ws.Length > 3)
                        {
                            if (ws[2] == "find")
                            {
                                ws = m_msg.Body.SplitEx(3);
                                Currency c = new Currency();
                                string data = c.Find(ws[3]);
                                if (data != null)
                                    rs = data;
                                else

                                {
  
                                    rs = m_r.FormatPattern("currency_search_not_found");
                                }
                                break;
                            }
                        }

                        if (ws.Length == 5)
                        {
                            Currency c = new Currency();
                            try
                            {
                                rs = c.Handle(ws[3], ws[4], Convert.ToInt32(ws[2]));
                            }
                            catch
                            {
                                rs = m_r.FormatPattern("curr_name_not_found");
                            }
                        }
                        else
                        if (ws.Length == 3)
                        {
                            if (ws[2] == "list")
                            {
                                Currency c = new Currency();
                                rs = c.GetList();
                            }
                            else
                                syntax_error = true;
                        }
                        else
                            syntax_error = true;
                        break;
                    }
                case "xep":
                    {
                
                        if (ws.Length > 2)
                        {

                            XEPS xeps = new XEPS();
                            string data = xeps.GetXep(ws[2]);
                            rs = data != null ? data : m_r.FormatPattern("xep_not_found", ws[2]);
                        }
                        else
                            syntax_error = true;
                        break;
                    }

                case "link":
                    {

                        if (ws.Length == 3)
                        {
                            if (Utils.OS == Platform.Windows)
                            {
                                Stdior std = new Stdior();
                                rs = std.Execute("links " + ws[2]);
                            }
                            else
                            {
                                Stdior std = new Stdior();
                                rs = std.Execute("links -dump" + ws[2]); 
 
                            }
                        }
                        else
                            syntax_error = true;
                        break;
                    }

                case "rfc":
                    {
                        if (ws.Length > 2)
                        {
                            string data = Sh.RFCHandler.GetState(ws[2]);
                            rs = data != null ? data.Trim().Trim('\n') : m_r.FormatPattern("rfc_not_found");
                        }
                        else
                            syntax_error = true;
                        break;
                    }


                case "list":
                    {
                        if (ws.Length == 2)
                        {
                            rs = m_r.FormatPattern("volume_list", n) + "\nlist, dns, whois, google, gt, gtlangs, xep, rfc, curr, link";
                        }
                        break;
                    }
          
                default:
                    {
                        m_r.Reply(m_r.FormatPattern("volume_cmd_not_found", n, ws[1], d + n.ToLower() + " list"));
                        break;
                    }

            }


            if (syntax_error)
                m_r.se(self);
            else
            if (rs != null)
                m_r.Reply( rs);
               
        }

      

    }
}
