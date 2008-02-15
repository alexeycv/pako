using System;
using agsXMPP.Xml.Dom;
using Core.Client;

namespace Core.Manager
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


        public int GetAccess(string source)
        {

            lock (Document)
            {
             //   @out.exe("'" + cmd + "'");
                foreach (Element el in Document.RootElement.SelectElements("command"))
                {
                    string cmd = el.GetAttribute("name");
                    string[] ws = source.SplitEx(source.Length);
                    string[] rs = cmd.SplitEx(cmd.Length);

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
                return -1;
        
            }
        }



        public void SetAccess(string cmd, int access)
        {
            lock (Document)
            {

                access = access > 100 ? 100 : access;
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
