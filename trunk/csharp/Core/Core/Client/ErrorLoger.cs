using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace Core.Client
{
    public class ErrorLoger
    {
        string m_dir;
        object[] sobjs = new object[10];
        FileStream fs;
        StreamWriter sr;


        public ErrorLoger(string file)
        {
            for (int i = 0; i < 10; i++)
            {
                sobjs[i] = new object();
            }
            m_dir = file;

  
        }


        public void Write(string data)
        {
            lock (sobjs[0])
            {
                fs = File.Open(m_dir, FileMode.Append);
                sr = new StreamWriter(fs,Encoding.UTF8);
                sr.Write(data);
                sr.Close();
                fs.Close();
            }
        }



        public string Read()
        {
            lock (sobjs[1])
            {
                fs = File.Open(m_dir, FileMode.Open);
                StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                string data = sr.ReadToEnd();
                sr.Close();
                fs.Close();
                return data;
            }
        }

    }
}
