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
using Core.Client;

namespace Core.Manager
{
   
    public class UDManager: XMLContainer
    {
        UserData[] m_users;
        int m_count;


        public UDManager(string UsersFile)
        {
            Open(UsersFile, 3);
            GetUDList();
         
        }

        public void GetUDList()
        {

            lock (this)
            {
                m_count = 0;
                foreach (Element el in Document.RootElement.SelectSingleElement("usersdata").SelectElements("user"))
                {
                    m_count++;
                }

                m_users = new UserData[m_count];

                int ii = 0;

                for (int i = 0; i < m_count; i++)
                {
                    m_users[i] = new UserData(null, null, null);
                }

                foreach (Element el in Document.RootElement.SelectSingleElement("usersdata").SelectElements("user"))
                {
                    object m_a = null;
                    string m_l = null;

                    if (el.HasAttribute("access"))
                        m_a = (int)el.GetAttributeInt("access");
                    if (el.HasAttribute("lang"))
                        m_l = el.GetAttribute("lang");

                    m_users[ii] = new UserData(new Jid(el.GetAttribute("jid")), m_l, m_a);
                    ii++;
                }
            }
        }


        public UserData[] UsersData
        {
            get { lock (aso[0]) { return m_users; } }
            set { lock (aso[0]) { m_users = value; } }

        }
        /// <summary>
        /// Gets all the available instances of the jid-specific data
        /// </summary>
        /// <param name="Jid"></param>
        /// <returns></returns>
        public UserData GetUserData(Jid Jid)
        {
            lock (UsersData)
            {
                if (Jid != null)
                {

                    foreach (UserData ud in UsersData)
                    {


                        if (ud.Jid.Bare == Jid.Bare)
                            return ud;
                    }


                }
                return null;
            }
        }

        /// <summary>
        /// SetData overload, which lets change jid-specific language;
        /// </summary>
        /// <param name="Jid"></param>
        /// <param name="Lang"></param>
        public void SetData(Jid Jid, string Lang)
        {
            lock (Document)
            {
                bool exists = false;
                foreach (Element el in Document.RootElement.SelectSingleElement("usersdata").SelectElements("user"))
                {
                    if (el.GetAttribute("jid") == Jid.Bare)
                    {
                        el.SetAttribute("lang", Lang);
                        exists = true;
                        Save();
                        GetUDList();
                    }
                }

                if (!exists)
                {
                    Document.RootElement.SelectSingleElement("usersdata").AddTag("user");
                    foreach (Element el in Document.RootElement.SelectSingleElement("usersdata").SelectElements("user"))
                    {
                        if (!el.HasAttribute("jid"))
                        {
                            el.SetAttribute("jid", Jid.Bare);
                            el.SetAttribute("lang", Lang);
                            exists = true;
                            Save();
                            GetUDList();
                        }
                    }
                }
            }
        }

        public bool DelData(Jid Jid)
        {
            lock (Document)
            {

                foreach (Element el in Document.RootElement.SelectSingleElement("usersdata").SelectElements("user"))
                {
                    if (el.GetAttribute("jid") == Jid.Bare.ToLower())
                    {
                        string m_source = Document.ToString().Replace(el.ToString(), "");
                        Document.Clear();
                        Document.LoadXml(m_source);
                        Save();
                        GetUDList();
                        return true;
                    }
                }
                return false;
            }
        }


        /// <summary>
        /// SetData overload, which lets change jid-specific access;
        /// </summary>
        /// <param name="Jid"></param>
        /// <param name="Access"></param>
        public void SetData(Jid Jid, int Access)
        {
            lock (Document)
            {
                bool exists = false;
                foreach (Element el in Document.RootElement.SelectSingleElement("usersdata").SelectElements("user"))
                {
                    if (el.GetAttribute("jid") == Jid.Bare)
                    {
                        el.SetAttribute("access", Access);
                        exists = true;
                        Save();
                        GetUDList();
                    }
                }

                if (!exists)
                {
                    Document.RootElement.SelectSingleElement("usersdata").AddTag("user");
                    foreach (Element el in Document.RootElement.SelectSingleElement("usersdata").SelectElements("user"))
                    {
                        if (!el.HasAttribute("jid"))
                        {
                            el.SetAttribute("jid", Jid.Bare);
                            el.SetAttribute("access", Access);
                            exists = true;
                            Save();
                            GetUDList();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// SetData overload, which lets change both: jid-specific language and jid-specific access;
        /// </summary>
        /// <param name="Jid"></param>
        /// <param name="Lang"></param>
        /// <param name="Access"></param>
        public void SetData(Jid Jid, string Lang, int Access)
        {
            lock (Document)
            {
                Access = Access > 100 ? 100 : Access;
                bool exists = false;
                foreach (Element el in Document.RootElement.SelectSingleElement("usersdata").SelectElements("user"))
                {
                    if (el.GetAttribute("jid") == Jid.Bare)
                    {
                        el.SetAttribute("lang", Lang);
                        el.SetAttribute("access", Access);
                        exists = true;
                        Save();
                        GetUDList();
                    }
                }

                if (!exists)
                {
                    Document.RootElement.SelectSingleElement("usersdata").AddTag("user");
                    foreach (Element el in Document.RootElement.SelectSingleElement("usersdata").SelectElements("user"))
                    {
                        if (!el.HasAttribute("jid"))
                        {
                            el.SetAttribute("jid", Jid.Bare);
                            el.SetAttribute("lang", Lang);
                            el.SetAttribute("access", Access);
                            exists = true;
                            Save();
                            GetUDList();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// A specific void, which lets set jis=specifi data, by indicating null-not-null parameters:
        /// For example:If Lang is not-null and Access is null then it sets only Lang.
        /// </summary>
        /// <example>
        /// SetData(SomeJid, null, 101);
        /// Setdata(SomeJid, "en", null);
        /// Setdata(SomeJid, "en", 101);
        /// Setdata(SomeJid, null, null); - in this case void return on the very beginning and make no changeS.
        /// </example>
        /// <param name="Jid"></param>
        /// <param name="Lang"></param>
        /// <param name="Access"></param>
        public void SetData(Jid Jid,object nAccess, string nLang)
        {

            if ((nLang == null) && (nAccess == null))
                return;

            lock (Document)
            {
              
                bool exists = false;
                foreach (Element el in Document.RootElement.SelectSingleElement("usersdata").SelectElements("user"))
                {
                    if (el.GetAttribute("jid") == Jid.Bare)
                    {
                        if (nLang != null)
                            el.SetAttribute("lang", nLang);
                        if (nAccess != null)
                        {
                            int access = (int)nAccess;
                            access = access > 100 ? 100 : access;
                            if (access > -1)
                                el.SetAttribute("access", access);
                        }
                        exists = true;
                        Save();
                        GetUDList();
                    }
                }

                if (!exists)
                {
                    Document.RootElement.SelectSingleElement("usersdata").AddTag("user");
                    foreach (Element el in Document.RootElement.SelectSingleElement("usersdata").SelectElements("user"))
                    {
                        if (!el.HasAttribute("jid"))
                        {
                            el.SetAttribute("jid", Jid.Bare);
                            if (nLang != null)
                                el.SetAttribute("lang", nLang);
                            if (nAccess != null)
                            {
                                int access = (int)nAccess;
                                access = access > 100 ? 100 : access; 
                                if (access > -1)
                                    el.SetAttribute("access", access);
                            }
                            exists = true;
                            Save();
                            GetUDList();
                        }
                    }
                }
            }
        }

        public int Count
        {
            get { lock (aso[1]) { return m_count; } }
            set { lock (aso[1]) { m_count = value; } }

        }
    }
}
