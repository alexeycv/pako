using System;
using System.Collections.Generic;
using System.Text;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;
using agsXMPP.protocol.x.muc;
using Core.Plugins;
using Core.Conference;
using Core.Special;

namespace Core.Client
{
    public class UserData
    {

        string m_lang;
        object m_access;
        Jid m_jid;
        object[] sobjs = new object[4];
        public UserData(Jid Jid,string Lang, object Access)
        {
            for (int i = 0; i < 4; i++)
            {
                sobjs[i] = new object();
            }
            m_lang = Lang;
            m_jid = Jid;
            m_access = Access;
        }

        public string Language
        {
            get { lock (sobjs[0]) { return m_lang; } }
            set { lock (sobjs[0]) { m_lang = value; } }
        }

        public Jid Jid
        {
            get { lock (sobjs[3]) { return m_jid; } }
            set { lock (sobjs[3]) { m_jid = value; } }
        }


        public object Access
        {
            get { lock (sobjs[1]) { return m_access; } }
            set { lock (sobjs[1]) { m_access = (int)value; } }
        }

    }
}
