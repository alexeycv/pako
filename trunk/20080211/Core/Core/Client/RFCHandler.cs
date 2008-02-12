using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;

namespace Core.Client
{
    
    public class RFCHandler
    {

        object[] sobjs = new object[10];
        ArrayList ar;


        public ArrayList _ar
        {


            get
            {
                lock (sobjs[0])
                {
                    ArrayList rr = new ArrayList();
                    rr = ar;
                    return rr;
                }
            }
        }


        public RFCHandler(string file)
        {
            for (int i = 0; i < 10; i++)
            {
                sobjs[i] = new object();
            }
            string[] ss = File.ReadAllLines(file);
            ar = new ArrayList();
            foreach (string s in ss)
            {
                ar.Add(s);
            }


        }
  
        public string GetState(string number)
        {
            string ss = number;
            string result = null ;
            ss += ss.EndsWith(".") ? "" : ".";
            ArrayList _ar_ = _ar;
            foreach (string data in _ar_)
              {
                            try
                            {
                               
                                    Convert.ToInt32(data[0].ToString());
                                    if (Convert.ToInt32(data[0]) != 0)
                                    {
                                        if (result == null)
                                        {
                                            string numb = data.Substring(0, data.IndexOf(" "));
                                         if (numb == ss)
                                            {
                                                result = data + "\n";
                                            }
                                        }
                                        else
                                            break;
                                    }else
                                        if (result != null)
                                            result += data + "\n";
                            }
                            catch
                            {
                                if (result != null)
                                    result += data + "\n";
                            }
                       
                   
                }
              //  Console.WriteLine("ggggggggggggggg");
                return result;
        }


     
    }
}
