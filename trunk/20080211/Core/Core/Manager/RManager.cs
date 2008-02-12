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
    public class RManager
    {
        Hashtable m_resps;
        string m_dir;
        int m_count;
        XmppClientConnection m_con;
        int m_msglimit;
        object[] sobjs = new object[10];

        public RManager(XmppClientConnection Con, string Folder, int MSGLimit)
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
