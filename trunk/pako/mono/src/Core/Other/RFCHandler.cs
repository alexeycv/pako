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
using Core.Kernel;

namespace Core.Other
{
    
    public class RfcManager
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


        public RfcManager(string file)
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
               @out.exe("rfc_starting");
            foreach (string data in _ar_)
              {
                            try
                            {
                               
                                        int num = Convert.ToInt32(data[0].ToString());
				        if (num > 0)                                    
				        {
					@out.exe("rfc_number_converted "+ num.ToString());
                                        if (result == null)
                                        {
					    @out.exe("rfc_number_setting");
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
                            	   @out.exe("rfc_catch_before");
                                if (result != null)
                                    result += data + "\n";
                                      @out.exe("rfc_catch_after");
                            }
                       
                   
                }
              //  @out.exe("ggggggggggggggg");
               return result != null ? result.Trim('\n',' ') : null;
        }


     
    }
}
