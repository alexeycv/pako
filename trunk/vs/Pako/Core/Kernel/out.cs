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

namespace Core.Kernel
{
    static public class @out
    {

        static bool debug;
        static object obj = new object();


        /// <summary>
        /// A provider for writing into a console  using debug(in/off) mode
        /// </summary>
        /// <param name="Phrase"></param>
        static public void exe(string Phrase)
        {
            if (debug)
            Console.WriteLine(Phrase);
        }


        /// <summary>
        /// Set if the COnsole is running in debugging mode or not.
        /// </summary>
        static public bool Debug
        {
            get { lock (obj) { return debug; } }
            set { lock (obj) { debug = value; } }
        }

    }
}
