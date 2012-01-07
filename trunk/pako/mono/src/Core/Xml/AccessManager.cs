/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Pako Jabber-bot.                                                              *
 * Copyright. All rights reserved © 2007-2008 by Klichuk Bogdan (Bbodio's Lab)   *
 * Copyright. All rights reserved © 2009-2012 by Alexey Bryohov                  *
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
using System.Collections.Specialized;
using agsXMPP.Xml.Dom;
using Core.Kernel;
using Core.Other;

namespace Core.Xml
{


    public class AccessManager : XMLContainer
    {

        int m_count;

        public AccessManager(string ACSFile)
        {

            if (!System.IO.File.Exists(ACSFile))
            {
                Document doc = new Document();
                doc.LoadXml("<Access></Access>");
                doc.Save(ACSFile);
            }
            m_count = 0;
            Open(ACSFile, 5);
            foreach (Element el in Document.RootElement.SelectElements("command"))
            {
                m_count++;
            }

        }




        public int Count
        {
            get { lock (aso[3]) { return m_count; } }
            set { lock (aso[3]) { m_count = value; } }
        }

        public bool DelCommand(string source)
        {
            lock (Document)
            {
                source = source.ToLower();
                while (source.Contains("  ")) source = source.Replace("  ", " ");
                if (Document.RootElement.SelectElements("command") != null)
                {
                    foreach (Element el in Document.RootElement.SelectElements("command"))
                    {
                        if (source == el.GetAttribute("name"))
                        {
                            string m_source = Document.ToString();
                            m_source = m_source.Replace(el.ToString(), "");
                            Document.Clear();
                            Document.LoadXml(m_source);
                            Save();
                            return true;
                        }
                    }
                    return false;

                }
                return false;
            }
        }


        public bool CommandExists(string source)
        {
            lock (Document)
            {
                source = source.ToLower();
                while (source.Contains("  ")) source = source.Replace("  ", " ");
                if (Document.RootElement.SelectElements("command") != null)
                {
                    foreach (Element el in Document.RootElement.SelectElements("command"))
                    {
                        if (source == el.GetAttribute("name"))
                        {
                            return true;
                        }

                    }
                    return false;

                }
                return false;
            }

        }

        public ListDictionary GetCommands()
        {
            ListDictionary list = new ListDictionary();
            foreach (Element el in Document.RootElement.SelectElements("command"))
            {
                string cmd = el.GetAttribute("name");
                int access = el.GetAttributeInt("access");
                if (list[cmd] == null)
                    list.Add(cmd, access);
                else
                    @out.exe("access_manager_getcommands(): key \"" + cmd + "\" already exists");
            }
            return list;
        }


        public int? GetAccess(string source, string local_cmd, int? local_acc)
        {

            lock (Document)
            {


                source = source.ToLower();
                @out.exe("getting_global_access");
                string temp_cmd = local_cmd ?? "";
                int? temp_acc = null;

                foreach (Element el in Document.RootElement.SelectElements("command"))
                {
                    string cmd = el.GetAttribute("name").ToLower();
                    string[] ws = Utils.SplitEx(source, source.Length);
                    string[] rs = Utils.SplitEx(cmd, cmd.Length);
                    int index = 0;
                    int count = 0;
                    foreach (string _ws in ws)
                    {
                        if (index < rs.Length)
                        {
                            if (_ws == rs[index])
                                count++;
                            else
                                break;
                        }
                        index++;
                    }

                    if (count == rs.Length)
                    {
                        @out.exe("#gaccess: " + el.GetAttribute("access"));
                        int temp = el.GetAttributeInt("access");
                        if (cmd.StartsWith(temp_cmd) && cmd != temp_cmd)
                        {
                            @out.exe("rc_found: '" + cmd + "'");
                            temp_cmd = cmd;
                            temp_acc = temp;
                        }
                    }
                }
                @out.exe("#access: " + temp_acc ?? "null");

                if (temp_acc == null)
                {
                    if (Utils.SplitEx(source, 2)[0].ToLower() == "admin" && local_cmd == null)
                    {
                        @out.exe("$admin_cmd");
                        return 100;
                    }
                    else
                        return local_acc;
                }
                else
                    return temp_acc;


            }
        }




        public int? GetAccess(string source, string local_cmd, int? local_acc, ref AccessType acctype)
        {

            lock (Document)
            {


                source = source.ToLower();
                @out.exe("getting_global_access");
                string temp_cmd = local_cmd ?? "";
                int? temp_acc = null;

                foreach (Element el in Document.RootElement.SelectElements("command"))
                {
                    string cmd = el.GetAttribute("name").ToLower();
                    string[] ws = Utils.SplitEx(source, source.Length);
                    string[] rs = Utils.SplitEx(cmd, cmd.Length);
                    int index = 0;
                    int count = 0;
                    foreach (string _ws in ws)
                    {
                        if (index < rs.Length)
                        {
                            if (_ws == rs[index])
                                count++;
                            else
                                break;
                        }
                        index++;
                    }

                    if (count == rs.Length)
                    {
                        @out.exe("#gaccess: " + el.GetAttribute("access"));
                        int temp = el.GetAttributeInt("access");
                        if (cmd.StartsWith(temp_cmd) && cmd != temp_cmd)
                        {
                            @out.exe("rc_found: '" + cmd + "'");
                            temp_cmd = cmd;
                            temp_acc = temp;
                        }
                    }
                }
                @out.exe("#access: " + temp_acc ?? "null");

                if (temp_acc == null)
                {
                    if (Utils.SplitEx(source, 2)[0].ToLower() == "admin" && local_cmd == null)
                    {
                        @out.exe("$admin_cmd");
                        acctype = AccessType.SetByAdmin;
                        return 100;
                    }
                    else
                    {
                        acctype = AccessType.SetByMuc;
                        return local_acc;
                    }
                }
                else
                {
                    acctype = AccessType.SetByAdmin;
                    return temp_acc;
                }


            }
        }


        public void SetAccess(string cmd, int access)
        {
            lock (Document)
            {
                cmd = cmd.ToLower();
                access = access > 100 ? 100 : access;
                access = access < 0 ? 0 : access;
                while (cmd.Contains("  ")) cmd = cmd.Replace("  ", " ");
                foreach (Element el in Document.RootElement.SelectElements("command"))
                {
                    if (el.GetAttribute("name") == cmd)
                    {
                        el.SetAttribute("access", access);
                        Save();
                        return;
                    }

                }

                Document.RootElement.AddTag("command");

                foreach (Element el in Document.RootElement.SelectElements("command"))
                {
                    if (!el.HasAttribute("name"))
                    {
                        el.SetAttribute("name", cmd);
                        el.SetAttribute("access", access);
                        Save();
                        return;
                    }
                }
            }

        }



    }
}
