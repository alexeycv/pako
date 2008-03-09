﻿/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
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
using System.Text;
using System.IO;


namespace Core.Other
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
            if (!File.Exists(m_dir))
            		File.Create(m_dir);
        

  
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
