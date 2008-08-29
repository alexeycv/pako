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
using System.Collections;
using System.Text.RegularExpressions;
using System.Text;
using agsXMPP;
using agsXMPP.protocol.iq.disco;
using agsXMPP.protocol;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;
using Core.Kernel;

namespace Plugin
{
    public class DiscoCB
    {
        Response m_r;
        Jid m_jid;
        int num;
        bool conferences = false;


        public DiscoCB(Response r, Jid Jid, int number)
        {
        	conferences = Jid.ToString().StartsWith("conference.");
            m_r = r;
            m_jid = Jid;
            DiscoItemsIq ti = new DiscoItemsIq();
            ti.Type = IqType.get;
            ti.To = m_jid;
            ti.GenerateId();
            num = number;
            m_r.Connection.IqGrabber.SendIq(ti, new IqCB(DiscoExtractor), null);

        }


      public int GetCount(string muc_review)
      {
       Regex reg = new Regex(@"\([0-9]+\)*[^\(]");
       MatchCollection mc = reg.Matches(muc_review);
       if (mc.Count == 0) return 0;
       string str = mc[mc.Count - 1].ToString();
       str = str.Substring(1,str.Length - 2);
       try {return Convert.ToInt32(str);} catch {return 0;}
      }

        private void DiscoExtractor(object obj, IQ iq, object arg)
        {

            DiscoItems d_items = iq.Query as DiscoItems;

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



                    DiscoItem[] items = d_items.GetDiscoItems();
                    int all = items.Length;
                    if (items.Length == 0)
                    {
                        answer = m_r.f("disco_empty", jid);
                    }
                    else
                    {
                       if (!conferences)
                       {
                        string data = "";

                        int i = 1;
                        num = num < 1 ? items.Length : num;

                        foreach (DiscoItem item in items)
                        {
                            if (i <= num)
                            {
                                data += "\n" + i.ToString() + ") " + item.Jid;
                                if (!String.IsNullOrEmpty(item.Name))
                                    data += " [" + item.Name+"]";
                            }
                            else
                            {
                                break;
                            }
                               
                            i++;
                        }

                        answer = m_r.f("disco_result") + data + (all != 0 ? "\n-- "+all.ToString()+" --" : "") ;
					}else
					{
						 
						 @out.exe("disco_conference");
						  num = num < 1 ? items.Length : num;
                          List<object[]> hash = new List<object[]>(); 
                          foreach (DiscoItem item in items) { 
                          	   int users_count = item.Name != null ? GetCount(item.Name) : 0;
                          	   hash.Add(new object[]{ users_count, item});
						  }
						   @out.exe("disco_conference_ready_to_group");
						 for (int i = 1; i < hash.Count; i++) {
						      for (int j = 0; j < i;j++)
						      { int c1 = ((int)((object[])hash[i])[0]);
						        int c2 = ((int)((object[])hash[j])[0]);
						      	if (c2 <= c1)
						      	{ object[] s = hash[i]; hash[i] = hash[j]; hash[j] = s;  }
							  }
						  }
						   @out.exe("disco_conference_grouping_finished");
						  string _data=""; int l = 0;
						  foreach (object[] _i in hash)
						  {
						  	l++; if (l <= num) {
						  	 DiscoItem item = ((DiscoItem)(_i[1]));
						  	 _data += "\n" + l.ToString() + ") " + item.Jid;
                             if (!String.IsNullOrEmpty(item.Name))
                                 _data += " [" + item.Name+"]";
						} else break;	
					      }	
					       @out.exe("disco_conference_items ready");
						  answer = m_r.f("disco_result") + _data + (all != 0 ? "\n-- "+all.ToString()+" --" : "") ; 
					    }

                    }

                }
            }
            else
            {
                answer = m_r.f("version_error", jid);
            }
            @out.exe("disco_answering");
            m_r.Reply(answer);
        }





    }
}
