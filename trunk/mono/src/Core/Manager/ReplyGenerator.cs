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
using System.Text;
using System.IO;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;
using agsXMPP.protocol.x.muc;


namespace Core.Client
{
    public class ReplyGenerator
    {
        Hashtable m_resps;
        string m_dir;
        int m_count;
        XmppClientConnection m_con;
        int m_msglimit;
        object[] sobjs = new object[10];

        public ReplyGenerator(XmppClientConnection Con, string Folder, int MSGLimit)
        {

            for (int i = 0; i < 10; i++)
            {
                sobjs[i] = new object();
            }

            m_con = Con;
            m_dir = Folder;          
            m_msglimit = MSGLimit;

            m_count = (Directory.GetDirectories(m_dir)).Length;

            m_resps = new Hashtable();

          

            foreach (string dir in Directory.GetDirectories(m_dir))
            {
                m_resps.Add(Path.GetFileName(dir), new Response(m_con, dir, Path.GetFileName(dir), m_msglimit));      
            }



        }


    


        public Hashtable Responses
        {
            get { lock (sobjs[0]) { return m_resps; } }
            set { lock (sobjs[0]) { m_resps = value; } }
        }



        public Response GetResponse(string Language)
        {
            lock (Responses)
            {
                return (Response)Responses[Language];
               
            }
        }



    }
}
