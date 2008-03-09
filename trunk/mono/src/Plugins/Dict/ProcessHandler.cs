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
    public class DictHandler
    {
        string[] ws;
        bool syntax_error = false;
        Response m_r;
        string self;
        Jid s_jid;
        string d;
        Message m_msg;
        string m_b;
        string n;
        Jid m_jid;
        SessionHandler Sh;

        public DictHandler(Response r, string Name)
        {
            m_b = r.Msg.Body;
            ws = Utils.SplitEx(m_b, 2);
            m_msg = r.Msg;
            m_r = r;
            Sh = r.Sh;
            n = Name;
            s_jid = r.Msg.From;

            d = r.Delimiter;

            if (ws.Length < 2)
            {
                r.Reply(r.FormatPattern("volume_info", n, d + n.ToLower() + " list"));
                return;
            }
            self = ws[0] + " " + ws[1];
            //@out.exe(m_r.MUC.Jid.ToString());
            m_jid = r.MUser != null ? r.MUser.Jid.ToString() != r.Msg.From.ToString() ? r.MUser.Jid : null : r.Msg.From;
            if (m_jid == null)
            {
                m_r.Reply(m_r.FormatPattern("jid_visible_only"));
                return;
            }
            Handle();


        }



        public void Handle()
        {





            string cmd = ws[1];
            string rs = null;

            switch (cmd)
            {
                case "add":
                    {

                        if (ws.Length > 2)
                        {
                            if (ws[2].IndexOf("=") > -1)
                            {
                                string word = ws[2].Substring(0, ws[2].IndexOf("=")).Trim();
                                string value = ws[2].Substring(ws[2].IndexOf("=") + 1).Trim();
                                if ((value != "") && (word != ""))
                                {

                                   Sh.Defs.AddEntity(word, value, m_jid.Bare);
                                    rs = m_r.FormatPattern("defs_added");
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
                            rs = m_r.FormatPattern("volume_list", n) + "\nlist, show, showall, add, find, count, clear";
                        }
                        else
                            syntax_error = true;
                        break;
                    }

                case "show":
                    {
                        if (ws.Length > 2)
                        {
                            rs = Sh.Defs.GetEntity(ws[2], m_r.FormatPattern("defs_single_said_by"));
                            if (rs == null)
                                rs = m_r.FormatPattern("defs_not_found", ws[2]);
                        }
                        else
                            syntax_error = true;

                        break;
                    }
                case "showto":
                    {
                        if (m_r.MUC != null)
                        {
                            string word = Utils.GetValue(m_b, "[(.*)]");
                            m_b = Utils.RemoveValue(m_b, "[(.*)]", true);
                            ws = Utils.SplitEx(m_b, 2);


                            if ((ws.Length > 2) && (word != ""))
                            {
                                string def = Sh.Defs.GetEntity(word, "{2}");
                                rs = def != null ? m_r.Agree() : m_r.FormatPattern("defs_not_found");

                                if (m_r.MUC.UserExists(ws[2]))
                                {
                                    if (def != null)
                                    {
                                        Message msg = new Message();
                                        msg.To = new Jid(m_msg.From.Bare + "/" + ws[2]);
                                        msg.Body = def;
                                        msg.Type = MessageType.chat;
                                        msg.From = m_msg.From;
                                        m_r.Connection.Send(msg);
                                    }
                                }
                                else
                                    rs = m_r.FormatPattern("user_not_found", ws[2]);

                            }
                            else
                                syntax_error = true;
                        }
                        else
                            rs = m_r.FormatPattern("muconly");
                        break;
                    }

                case "find":
                    {
                        if (ws.Length > 2)
                        {
                            rs = Sh.Defs.FindEntities(ws[2], m_r.FormatPattern("defs_single_said_by"));
                            if (rs == null)
                                rs = m_r.FormatPattern("defs_not_found", ws[2]);
                        }
                        else
                            syntax_error = true;

                        break;
                    }

                case "count":
                    {
                        if (ws.Length == 2)
                        {
                            rs = Sh.Defs.EntitiesCount().ToString();
                        }
                        else
                            syntax_error = true;

                        break;
                    }

                case "showall":
                    {
                        if (ws.Length > 2)
                        {
                            rs = Sh.Defs.GetAllEntities(ws[2], m_r.FormatPattern("defs_multi_said_by"));
                            if (rs == null)
                                rs = m_r.FormatPattern("defs_not_found");
                        }
                        else
                            syntax_error = true;
                        break;
                    }
                case "clear":
                    {
                        if (ws.Length == 2)
                        {
                            Sh.Defs.Clear();
                            rs = m_r.FormatPattern("defs_cleared");
                        }
                        break;
                    }

                default:
                    {
                        rs = m_r.FormatPattern("volume_cmd_not_found", n, ws[1], d + n.ToLower() + " list");
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
