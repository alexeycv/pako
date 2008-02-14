using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Client
{
    static public class @out
    {

        static bool debug;
        static object obj = new object();


        /// <summary>
        /// A provider for writing into a console, but using debug(in/off) mode
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
