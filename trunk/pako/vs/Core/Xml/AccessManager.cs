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
using agsXMPP.Xml.Dom;
using Core.Kernel;
using Core.Other;

namespace Core.Xml
{

 
    public class AcessManager : XMLContainer
    {

        int m_count;

        public AcessManager(string ACSFile)
        {
           
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


        public int? GetAccess(string source)
        {

            lock (Document)
            {
             //   @out.exe("'" + cmd + "'");
                foreach (Element el in Document.RootElement.SelectElements("command"))
                {
                    string cmd = el.GetAttribute("name");
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
                        return el.GetAttributeInt("access");
                    }



                }
                return null;
        
            }
        }



        public void SetAccess(string cmd, int access)
        {
            lock (Document)
            {
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
