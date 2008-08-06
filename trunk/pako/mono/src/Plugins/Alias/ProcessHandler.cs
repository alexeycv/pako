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
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;
using System.IO;
using Core.Kernel;
using Core.Conference;
using Core.Other;

namespace Plugin
{
    /// <summary>
    /// An Alias plug-in handler
    /// </summary>
    public class AliasHandler
    {
        string[] ws;
        Message m_msg;
        Response m_r;
        bool syntax_error = false;
        string self;
        string d;
        Jid s_jid;
        string m_b;
        string n;
        SessionHandler Sh;

        public AliasHandler(Response r, string Name)
        {
            Sh = r.Sh;
            m_r = r;
            m_b = m_r.Msg.Body;
            ws = Utils.SplitEx(m_b, 2);
            m_msg = m_r.Msg;
            n = Name;
            d = r.Delimiter;
            s_jid = m_r.Msg.From;
            if (r.MUser == null)
            {
                r.Reply(r.f("muconly"));
                return;
            }
            @out.exe("inside: alias: " + m_b);
            if (ws.Length < 2)
            {
                r.Reply(r.f("volume_info", n, d + n.ToLower() + " list"));
                return;
            }
            self = ws[0] + " " + ws[1];
           
            Handle();

        }


        /// <summary>
        /// Handle plug-in
        /// </summary>
        public void Handle()
        {
            string cmd = ws[1].ToLower();
            string rs = null;
            switch (cmd)
            {
                case "add":
                    {
                        ws = Utils.SplitEx(m_b, 3);
                        if (ws.Length > 2)
                        {                
                                bool spec = false;
                                int access = 0;
                                if ((ws[2].StartsWith("$")) && (ws[2].Length > 1))
                                {
                                    try
                                    {
                                        access = Convert.ToInt32(ws[2].Substring(1));
                                        int index = 0;
                                        m_b = "";
                                           foreach (string word in ws)
                                           {
                                               if (index != 2)
                                               {
                                                   m_b += word + " ";
                                               }
                                                   index++;
                                           }
                                        spec = true;
                                    }
                                    catch
                                    {}
                                }

                                ws = Utils.SplitEx(m_b.Trim(), 2);
                                @out.exe(">"+m_b);
                                if (ws[2].IndexOf("=") > -1)
                                {
                                    string alias = ws[2].Substring(0, ws[2].IndexOf("=")).Trim();
                                    string value = ws[2].Substring(ws[2].IndexOf("=") + 1).Trim();
                                
                                    if ((value != "") && (alias != ""))
                                    {

                                        Cmd cm = Cmd.CreateInstance(value, m_r, null);
                                        if (m_r.Access < cm.CompleteAccess)
                                        {
                                            rs = m_r.f("cmd_access_restricted_too_high");
                                            break;
                                        }
                                        if (access > m_r.Access)
                                        {
                                            rs = m_r.f("cmd_new_access_restricted_too_low");
                                            break;

                                        }
                        	          
                                        if (spec)
                                            Sh.S.GetMUC(m_r.MUC.Jid).AccessManager.SetAccess(alias, access);

                                        if (Sh.S.GetMUC(s_jid).AddAlias(alias, value))
                                            rs = m_r.f("alias_added");
                                        else
                                            rs = m_r.f("alias_already_exists");

                                    }
                                    else
                                        syntax_error = true;
                                }
                                else
                                    syntax_error = true;
                        }
                        else
                            syntax_error = true;
                        break;
                    }

                case "list":
                    {
                        if (ws.Length == 2)
                        {
                            rs = m_r.f("volume_list", n) + "\nlist, show, add, del, count, clear";
                        }
                        break;
                    }

                case "show":
                    {
                    	m_r.Format = false;
                        if (ws.Length == 2)
                        {
                            string data = m_r.MUC.GetAliasList("{1}) {2} = {3}\n", null);
                            rs = data != null ? data.Trim('\n') : m_r.f("alias_list_empty");
                        }
                        else
                            if (ws.Length > 2)
                            {
                                string data = m_r.MUC.GetAliasList("", ws[2]);
                                rs = data != null ? ws[2] + " = " + data : m_r.f("alias_not_existing");
                            }
                            else
                                syntax_error = true;
                        break;
                    }


                case "count":
                    {
                        if (ws.Length == 2)
                        {
                            rs = m_r.MUC.AliasCount().ToString();
                        }
                        else
                            syntax_error = true;
                        break;
                    }
                case "clear":
                    {
                        if (ws.Length == 2)
                        {
                            Sh.S.GetMUC(s_jid).ClearAliases();
                            rs = m_r.f("aliases_cleared");
                        }else
                            syntax_error = true;
                        break;
                    }
                case "del":
                    {
                        if (ws.Length > 2)
                        {

                            if (Sh.S.GetMUC(s_jid).DelAlias(ws[2]))
                                rs = m_r.f("alias_deleted");
                            else
                                rs = m_r.f("alias_not_existing");
                        }else
                            syntax_error = true;
                        break;
                    }

                default:
                    {
                        rs = m_r.f("volume_cmd_not_found", n, ws[1], d + n.ToLower() + " list");
                        break;
                    }




            }

            if (syntax_error)
            {
                m_r.se(self);
            }
            else

                if (rs != null)
                    m_r.Reply(rs);

        }

    }
}
