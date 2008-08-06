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
using System.Collections.Specialized;
using System.Text;
using Core.Kernel;
using System.IO;
using agsXMPP.Xml.Dom;
using Core.Other;

namespace Core.Xml
{
    public class LocalAccess : XMLContainer
    {
        public LocalAccess(string file)
        {
            if (!System.IO.File.Exists(file))
            {
                Document doc = new Document();
                doc.LoadXml("<Access></Access>");
                doc.Save(file);
            }

            Open(file, 10);
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



        public void Clear()
        {
            lock (Document)
            {

                Document.Clear();
                Document.LoadXml("<Access></Access>");
                Save();
            }
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

        public int? GetAccess(string source, ref string cmd_cb)
        {
            lock (Document)
            {
                @out.exe("getting_local_access");
                source = source.ToLower();
                string temp_cmd = "";
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
                        @out.exe("#access: " + el.GetAttribute("access"));
                        int temp = el.GetAttributeInt("access");
                        if (cmd.StartsWith(temp_cmd))
                        {
                            temp_cmd = cmd;
                            temp_acc = temp;
                        }
                    }
                }
                cmd_cb = temp_cmd == "" ? null : temp_cmd;
                return temp_acc;
            }
        }



        public void SetAccess(string cmd, int access)
        {
            lock (Document)
            {
                cmd = cmd.ToLower();
                while (cmd.Contains("  ")) cmd = cmd.Replace("  ", " ");
                access = access > 100 ? 100 : access;
                access = access < 0 ? 0 : access;

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
