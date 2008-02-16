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
using System.Diagnostics;

namespace Core.Client
{

/// <summary>
/// Represents current OSes, which can support C# .NET/Mono
/// </summary>
    public enum Platform
    {
        Windows,
        Mac,
        Unix,
        Linux
    }



    static public class Utils
    {
        /// <summary>
        /// Directory builder for any OS
        /// </summary>
        /// <param name="parts"></param>
        /// <returns></returns>
        static public string dir(params string[] parts)
        {
            string data = "";
            int i = 0;
            foreach (string part in parts)
            {
                i++;
                if (i != 1)
                    data += Path.DirectorySeparatorChar + part;
                else
                    data += part;
            }
            return data;
        }

        /// <summary>
        /// Current directory of the process
        /// </summary>
        static public string CurrentDir
        {
            get
            {
                return Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            }
        }

       /// <summary>
       /// The platform-specific sign, used to separate directory/path units
       /// </summary>
        static public char d
        {
            get
            {
                return System.IO.Path.DirectorySeparatorChar;
            }
        }
       /// <summary>
       /// Gets current machine's Operating System's base
       /// </summary>
        static public Platform OS
       {
           get
           {
               @out.exe("before os");
               string os = Environment.OSVersion.ToString().ToLower();
               return
                  (os.IndexOf("unix") > -1) ?
                   Platform.Unix :
                  (os.IndexOf("windows") > -1) ?
                   Platform.Windows :
                  (os.IndexOf("linux") > -1) ?
                   Platform.Linux :
                  (os.IndexOf("mac") > -1) ?
                  Platform.Mac :
                  Platform.Unix;
          
           }
       }

       
       /// <summary>
       ///  Gets the substring in "Source" - the first entering of string, specified in "Pattern"( It's has to contain (.*) which will be used like regular expression to find the needed part)
       /// You can specify as an argument true/false - if you want to get all the pattern or just the matched by "Pattern" word, which was found for (.*)
       /// </summary>
       /// <param name="Source"></param>
       /// <param name="Pattern"></param>
       /// <param name="args"></param>
       /// <returns></returns>
        static public string GetValue(string Source, string Pattern, params object[] args)
        {
            string part1 = Pattern.Substring(0, Pattern.IndexOf("(.*)"));
            string part2 = Pattern.Substring(Pattern.IndexOf("(.*)") + 4);
            bool include;
            if (args.Length == 0)
                include = false;
            else
                try
                {
                    include = (bool)args[0];
                }
                catch { include = false; };
            int posb;
            int pose;
            if (part1 == "")
                posb = 0;
            else
            {
                posb = include ? Source.IndexOf(part1) : Source.IndexOf(part1) + part1.Length;
                if (posb == -1)
                    return "";
            }

            if (part2 == "")
                pose = Source.Length;
            else
            {
                pose = include ? Source.IndexOf(part2, posb + 1) + part2.Length : Source.IndexOf(part2, posb);
                if (pose == -1)
                    return "";
            }
            if (pose >= posb)
                return Source.Substring(posb, pose - posb);
            else
                return "";

        }

       /// <summary>
       /// Removes in string "Source" the first entering of string, specified in "Pattern"( It's has to contain (.*) which will be used like regular expression to find the needed part)
       /// You can specify as an argument true/false - if you want to remove al the pattern or just the matched by "Pattern" word, which was found for (.*)
       /// </summary>
       /// <example>
       /// string data = "Hello, my job is [programmer - ingeneer]
       /// RemoveValue(data, "[(.*)]", false)
       /// or
       /// RemoveValue(data, "[(.*)]") will return a string 
       /// "Hello, my job is []"
       /// RemoveValue(data, "[(.*)]", true) will return a string 
        /// "Hello, my job is "
       /// </example>
       /// <param name="Source"></param>
       /// <param name="Pattern"></param>
       /// <param name="args"></param>
       /// <returns></returns>
        static public string RemoveValue(string Source, string Pattern, params object[] args)
        {

            string part1 = Pattern.Substring(0, Pattern.IndexOf("(.*)"));
            string part2 = Pattern.Substring(Pattern.IndexOf("(.*)") + 4);
            string value = GetValue(Source, Pattern, args);
            if ((value.StartsWith(part1)) && (value.EndsWith(part2)))
                return Source.Remove(Source.IndexOf(value), value.Length);
            return Source.Remove(Source.IndexOf(part1 + value) + 1, value.Length);
        }


       /// <summary>
       /// Singularily replaces first entering of string "From" in string "Source" with "To" 
       /// </summary>
       /// <param name="Source"></param>
       /// <param name="From"></param>
       /// <param name="To"></param>
       /// <returns></returns>
        static public string ReplaceOnce(string Source, string From, string To)
        {
            string result = Source;
            if (string.IsNullOrEmpty(result))
                return "";
            if ((string.IsNullOrEmpty(To)) || (string.IsNullOrEmpty(To)))
                return result;
            int posb = result.IndexOf(From);
            if (posb == -1)
                return result;
            result = result.Remove(posb, From.Length).Insert(posb, To);
            return result;

        }

       /// <summary>
       /// This is SplitEx -> split words by empty-space, specifiyng maximum parts.
       /// </summary>
       /// <example>
       /// Let's take a look on this phrase:
       /// string data = "Hello, my job is programmer - ingeneer";
       /// A ussual split will return a string[7] massive with each part,delimited with emptyspace.
       /// When we use ParseWords(data,3) it will return string[4] massive, which parts will be:
       /// "Hello," "my" "job" "is programmer - ingeneer"
       /// For ParseWords(data, 2):
       /// "Hello," "my" "job is programmer - ingeneer"
       /// </example>
       /// <param name="Source"></param>  /// <summary>
       /// This is SplitEx -> split words by empty-space, specifiyng maximum parts.
       /// </summary>
       /// <example>
       /// Let's take a look on this phrase:
       /// string data = "Hello, my job is programmer - ingeneer";
       /// A ussual split will return a string[7] massive with each part,delimited with emptyspace.
       /// When we use ParseWords(data,3) it will return string[4] massive, which parts will be:
       /// "Hello," "my" "job" "is programmer - ingeneer"
       /// For ParseWords(data, 2):
       /// "Hello," "my" "job is programmer - ingeneer"
       /// </example>
       /// <param name="Source"></param>
       /// <param name="Fixed"></param>
       /// <returns></returns>
        static public string[] SplitEx(string Source, int Fixed)
        {
            string[] ss = Source.TrimStart(' ').Split(' ');
            int i = 0;
            string[] res;
            string _source = Source;
            foreach (string s in ss)
            {
                if (s != "")
                {
                    if (i + 1 <= Fixed)
                    {
                        i++;
                        _source = _source.Remove(0, _source.IndexOf(s) + s.Length);
                    }

                    else
                    {
                        string rr;
                        i++;
                        rr = _source.Substring(_source.IndexOf(s));
                        break;
                    }
                }

            }
            _source = Source.TrimStart(' ');
            res = new string[i];
            i = 0;
            foreach (string s in ss)
            {
                if (s != "")
                {
                    if (i + 1 <= Fixed)
                    {
                        res[i] = s.Trim();
                        i++;
                        _source = _source.Remove(0, _source.IndexOf(s) + s.Length);
                    }

                    else
                    {
                        string rr;

                        rr = _source.Substring(_source.IndexOf(s));
                        res[i] = rr;
                        i++;
                        return res;

                    }
                }

            }

            return res;
        }
    
  
        /// <summary>
        /// A formatter for a TimeSpan, which converts ticks count into a real data/time.
        /// </summary>
        /// <param name="Ticks"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        static public string FormatTimeSpan(long Ticks, Response r)
        {
            TimeSpan ts = new TimeSpan(Ticks);
            @out.exe(Ticks.ToString());
            return
                                      (ts.Days > 31 ? Convert.ToString(System.Math.Truncate((double)ts.Days / 31)) + r.FormatPattern("month") + " " : "") +
                                      (ts.Days > 0 ? ts.Days.ToString() + " " + r.FormatPattern("day") + " " : "") +
                                      (ts.Hours > 0 ? ts.Hours.ToString() + " " + r.FormatPattern("hour") + " " : "") +
                                      (ts.Minutes > 0 ? ts.Minutes.ToString() + " " + r.FormatPattern("minute") + " " : "") +
                                      (ts.Seconds > 0 ? ts.Seconds.ToString() + " " + r.FormatPattern("second") + " " : "");

        }


   


    }
}
