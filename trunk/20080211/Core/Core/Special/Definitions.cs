using System;
using System.Collections.Generic;
using System.Text;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;
using agsXMPP.protocol.x.muc;
using Core.Client;
using Core.Conference;


namespace Core.Special
{
    public class Definition
    {
        Document m_doc;
        object[] sobjs = new object[10];
        string m_dir;

        public Definition(string DefsFile)
        {
            for (int i = 0; i < 10; i++)
            {
                sobjs[i] = new object();
            }

            m_doc = new Document();
            m_dir = DefsFile;
            m_doc.LoadFile(m_dir);
        }


        public string FindWord(string word, string pattern)
        {
            lock (sobjs[7])
            {
                string res = null;
                foreach (Element el in Document.RootElement.SelectElements("word"))
                {
                    if (el.GetAttribute("name") == word)
                    {
                      
                        string name = el.GetAttribute("name");
                        string value = el.GetAttribute("value");
                        string author = el.GetAttribute("author");
                         res =  pattern.
                            Replace("{1}", name).
                            Replace("{2}", value).
                            Replace("{3}", author);
                    }
                }
                return res;
            }


        }



        public string FindSimilar(string word, string pattern)
        {
            lock (sobjs[6])
            {
                string res = "";
                int c = 0;

                foreach (Element el in Document.RootElement.SelectElements("word"))
                {
                    if (el.GetAttribute("name").Contains(word))
                    {
                        c++;
                        string name = el.GetAttribute("name");
                        string value = el.GetAttribute("value");
                        string author = el.GetAttribute("author");
                        res += pattern.
                            Replace("{1}", c.ToString()).
                            Replace("{2}", name).
                            Replace("{3}", value).
                            Replace("{4}", author);
                    }
                }

                if (res != "")
                    return res;
                else
                    return null;
            }


        }


        public string FindAllWords(string word, string pattern)
        {
            lock (sobjs[5])
            {
                string res = "";
                int c = 0;

                foreach (Element el in Document.RootElement.SelectElements("word"))
                {
                    if (el.GetAttribute("name") == word)
                    {
                        c++;
                        string name = el.GetAttribute("name");
                        string value = el.GetAttribute("value");
                        string author = el.GetAttribute("author");
                        res += pattern.
                            Replace("{1}", c.ToString()).
                            Replace("{2}", name).
                            Replace("{3}", value).
                            Replace("{4}", author);
                    }
                }

                if (res != "")
                    return res;
                else
                    return null;
            }


        }

        public void ClearDefs()
        {
            lock (sobjs[4])
            {

                Document.Clear();
                Document.LoadXml("<Defs></Defs>");
                Document.Save(m_dir);
            }
        }

  

        public void AddWord(string word, string value, string author)
        {
            lock (sobjs[3])
            {
                    Document.RootElement.AddTag("word");
                    foreach (Element el in Document.RootElement.SelectElements("word"))
                    {
                        if (!el.HasAttribute("name"))
                        {
                            el.SetAttribute("name", word);
                            el.SetAttribute("value", value);
                            el.SetAttribute("author", author);
                            Document.Save(m_dir);
                            return;
                        }
                    }
                  
            }
        }


        public int Count
        {
            get
            {
                lock (sobjs[1])
                {
                    return Document.RootElement.SelectElements("word").Count;
                }
            }
        }




        public Document Document
        {
            get { lock (sobjs[0]) { return m_doc; } }
            set { lock (sobjs[0]) { m_doc = value; } }
        }


    }
}
