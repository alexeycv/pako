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
using System.Text;
using System.IO;
using Core.Other;
using System.Threading;

namespace Core.Kernel
{
    static public class @out
    {

        static bool debug;
        static object obj = new object();
        static object obj2 = new object();

        /// <summary>
        /// A provider for writing into a console  using debug(in/off) mode
        /// </summary>
        /// <param name="Phrase"></param>
        static public void exe(string Phrase)
        {
        	lock (obj2)
        	{
        		if (debug)
                { 
        			Console.WriteLine(Phrase);
        			if (Utils.OS == Platform.Windows)
            		Phrase = Phrase.Replace("\n","\r\n");
            	FileStream fs = File.Open(Utils.GetPath("log"), FileMode.Append, FileAccess.Write, FileShare.Write);
				StreamWriter sr = new StreamWriter(fs);
				sr.WriteLine(Phrase);
				sr.Dispose();
				sr.Close();
				fs.Dispose();
				fs.Close();
            	
        		}
        	}
        }

        /// <summary>
        /// 
        /// </summary>
        static public void log_get_ready()
        {
        	DirBuilder db = new DirBuilder();
        	if (File.Exists(db.b(Utils.CD,"Bot.log")))
        	{
                FileStream fs = File.Open(db.b(Utils.CD,"Bot.log"), FileMode.Truncate, FileAccess.Write, FileShare.Write);
				fs.Dispose();
				fs.Close();
		
        	}
        }
        
        /// <summary>
        /// A provider for writing into a console  using debug(in/off) mode
        /// </summary>
        /// <param name="Phrase"></param>
        static public void write(string Phrase)
        {
        	lock (obj2)
        	{
            	DirBuilder db = new DirBuilder();
            	Console.WriteLine(Phrase);
            	if (Utils.OS == Platform.Windows)
            		Phrase = Phrase.Replace("\n","\r\n");
            	FileStream fs = File.Open(db.b(Utils.CD,"Bot.log"), FileMode.Append, FileAccess.Write, FileShare.Write);
				StreamWriter sr = new StreamWriter(fs);
				sr.WriteLine(Phrase);
				sr.Dispose();
				sr.Close();
				fs.Dispose();
				fs.Close();
            	
        	}
        }


        /// <summary>
        /// Set if the Console is running in debugging mode or not.
        /// </summary>
        static public bool Debug
        {
            get { lock (obj) { return debug; } }
            set { lock (obj) { debug = value; } }
        }

    }
}
