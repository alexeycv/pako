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

namespace Core.Xml
{
    public class XMLContainer
    {

        string m_file;
        Document m_doc;
        public object[] aso;
        int aso_count;
  
        public void Open(string File, int AsyncObjects)
        {
            aso_count = AsyncObjects;
            m_file = File;
            m_doc = new Document();
            m_doc.LoadFile(m_file);
            aso = new object[aso_count + 4];

            for (int i = 0; i < aso_count + 4; i++)
            {
                aso[i] = new object();
            }
        }

        public void Open(int AsyncObjects)
        {
            aso_count = AsyncObjects;
            m_doc = new Document();
            aso = new object[aso_count + 4];

            for (int i = 0; i < aso_count + 4; i++)
            {
                aso[i] = new object();
            }
        }


        public void Save()
        {
            lock (aso[aso_count + 3])
            {
                Document.Save(m_file);
            }
        }



        public Document Document
        {
            get { lock (aso[aso_count]) { return m_doc; } }
            set { lock (aso[aso_count]) { m_doc = value; } }
        }


        public string File
        {
            get { lock (aso[aso_count + 1]) { return m_file; } }
            set { lock (aso[aso_count + 1]) { m_file = value; } }
        }

        public int ASyncObjsCount
        {
            get { lock (aso[aso_count + 2]) { return aso_count; } }
            set { lock (aso[aso_count + 2]) { aso_count = value; } }
        }

    }
}
