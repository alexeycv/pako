using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.protocol.iq.roster;
using agsXMPP.Xml.Dom;
using agsXMPP.protocol.x.muc;
using Core.Plugins;
using Core.Manager;
using Core.Conference;
using Core.Special;

namespace Core.Manager
{
    public class CalcHandler
    {
        Hashtable m_calcs;
        object[] sobjs = new object[10];

        public CalcHandler()
        {
            for (int i = 0; i < 10; i++)
            {
                sobjs[i] = new object();
            }
            m_calcs = new Hashtable();
        }

        public Hashtable Calculates
        {
            get { lock (sobjs[0]) { return m_calcs; } }
            set { lock (sobjs[0]) { m_calcs = value; } }
        }


        public bool Exists(Jid Jid)
        {
            return Calculates[Jid.ToString()] != null ? true : false; 
        }

        public bool AddHandle(Jid Jid)
        {
            if (Exists(Jid))
                return false;

                Calculates.Add(Jid.ToString(),new Calculate(Jid));
                return true;
        }


        public Calculate GetHandle(Jid Jid)
        {
            return Exists(Jid) ? (Calculate)Calculates[Jid.ToString()] : new Calculate(Jid);
        }


    }
}
