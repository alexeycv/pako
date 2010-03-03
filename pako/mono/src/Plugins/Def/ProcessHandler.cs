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
    public class DefsHandler
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
        SessionHandler Sh;

        public DefsHandler(Response r, string Name)
        {
            Sh = r.Sh;
            m_b = r.Msg.Body;
            ws = Utils.SplitEx(m_b, 2);
            m_msg = r.Msg;
            m_r = r;
            n = Name;
            s_jid = r.Msg.From;

            d = r.Delimiter;
            if (r.MUser == null)
            {
                r.Reply(r.f("muconly"));
                return;
            }
            if (ws.Length < 2)
            {
                r.Reply(r.f("volume_info", n, d + n.ToLower() + " list"));
                return;
            }
            self = ws[0] + " " + ws[1];

            Handle();
        }



        public void Handle()
        {

            string cmd = ws[1].ToLower();
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
                                    m_r.MUC.Dictionary.AddEntity(word, value, s_jid.Resource);
                                    rs = m_r.f("defs_added");
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
                            rs = m_r.f("volume_list", n) + "\nlist, show, showto, showall, add, find, count, clear";
                        }
                        else
                            syntax_error = true;
                        break;
                    }

                case "showto":
                    {
                        string word = "";
                        try
                        {
                            word = Utils.GetValue(m_b, "[(.*)]").Trim();
                            m_b = Utils.RemoveValue(m_b, "[(.*)]", true);
                            ws = Utils.SplitEx(m_b, 2);

                        }
                        catch (Exception ex)
                        {
                            @out.exe("ERROR:\n\n" + ex.Message + "\n\n" + ex.StackTrace);
                        }

                        if (word != "")
                        {
                            string def = m_r.MUC.Dictionary.GetEntity(word, "{2}");
                            rs = def != null ? m_r.Agree() : m_r.f("defs_not_found");
                            if (def != null) m_r.Format = false;
					        string nick = m_r.MUser;
					        if (ws.Length > 2)
						       nick = ws[2];
                            if (m_r.MUC.UserExists(nick))
                            {
                                if (def != null)
                                {
                                    //Message msg = new Message();
                                    //msg.To = new Jid(m_msg.From.Bare + "/" + nick);
                                    //msg.Body = def;
                                    //msg.Type = MessageType.chat;
                                    //msg.From = m_msg.From;
                                    //m_r.Connection.Send(msg);    
                                    rs = def;// + " " + nick;
                                    m_r.MUser = m_r.MUC.GetUser(nick);
                                    m_r.Msg.From = new Jid(m_msg.From.Bare + "/" + nick);
                                }
                            }
                            else
                                rs = m_r.f("user_not_found", nick);

                        }
                        else
                            syntax_error = true;

                        break;
                    }

                case "show":
                    {
                        if (ws.Length > 2)
                        {
                            rs = m_r.MUC.Dictionary.GetEntity(ws[2], m_r.f("defs_single_said_by"));
                            if (rs == null)
                                rs = m_r.f("defs_not_found", ws[2]);
                                if (rs != null) m_r.Format = false;
                        }
                        else
                            syntax_error = true;

                        break;
                    }

                case "find":
                    {
                        if (ws.Length > 2)
                        {
                            rs = m_r.MUC.Dictionary.FindEntities(ws[2], m_r.f("defs_single_said_by"));
                            if (rs == null)
                                rs = m_r.f("defs_not_found", ws[2]);
                             if (rs != null) m_r.Format = false;
                        }
                        else
                            syntax_error = true;

                        break;
                    }

                case "count":
                    {
                        if (ws.Length == 2)
                        {
                            rs = m_r.MUC.Dictionary.EntitiesCount().ToString();
                        }
                        else
                            syntax_error = true;

                        break;
                    }

                case "showall":
                    {
                        if (ws.Length > 2)
                        {
                            rs = m_r.MUC.Dictionary.GetAllEntities(ws[2], m_r.f("defs_multi_said_by"));
                            if (rs == null)
                                rs = m_r.f("defs_not_found");
                            else
                                rs = rs.Trim('\n');
                                if (rs != null) m_r.Format = false;
                        }
                        else
                            syntax_error = true;
                        break;
                    }
                case "clear":
                    {
                        if (ws.Length == 2)
                        {
                            m_r.MUC.Dictionary.Clear();
                            rs = m_r.f("defs_cleared");
                        }
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
