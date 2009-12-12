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
using System.Diagnostics;
using System.Threading;
using System.IO;
using Core.Kernel;

namespace Core.Other
{
     public class Stdior
    {
        /// <summary>
        /// A Command prompt standart Input/Output cross-platform redirecter 
        /// (handles cmd.exe on Windowsm and sh on Unix/Linux)
        /// Returns response of Command prompt 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
         public string Execute(string cmd, Session S)
        {
            Process p = new Process();
            StreamWriter sw;
            StreamReader sr;
            StreamReader err;
            if (Utils.OS == Platform.Windows)
            {
                ProcessStartInfo psI = new ProcessStartInfo("cmd");
                psI.UseShellExecute = false;
                psI.ErrorDialog = false;
                psI.RedirectStandardInput = true;
                psI.RedirectStandardOutput = true;
                psI.RedirectStandardError = true;
                psI.CreateNoWindow = true;
                p.StartInfo = psI;
                p.Start();
                sw = p.StandardInput;
                sr = p.StandardOutput;
              //  sr = new StreamReader(p.StandardOutput.BaseStream, Encoding.ASCII);
                err = p.StandardError;
                err = p.StandardError;
                sw.AutoFlush = true;
                while (sr.ReadLine() != "") ;
                // sr.ReadLine();
                string readed = cmd;
                sw.WriteLine(readed);
                sw.Close();
                string data = sr.ReadToEnd();
                data = data.Replace(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + ">", "");

                //data = data.Replace("\n\r", "");

                // if (data.StartsWith("\n")) 
                //     @out.exe("'"+data+"'");
                string error = err.ReadToEnd();
                string result = "";
                if (error != "")
                    result = error;
                else
                {
                    string s = readed + "\n";
                    data = data.Remove(0, data.IndexOf(s) + s.Length);
                    result = data;
                }
                p.Close();
                result = result.Trim('\n', ' ', '\r');  
                return result;
            }
            else
            {
                ProcessStartInfo psI = new ProcessStartInfo("sh");
                psI.UseShellExecute = false;
                psI.ErrorDialog = false;
                psI.RedirectStandardInput = true;
                psI.RedirectStandardOutput = true;
                psI.RedirectStandardError = true;
                psI.CreateNoWindow = true;
                p.StartInfo = psI;
                p.Start();
                sw = p.StandardInput;
                sr = p.StandardOutput;
                err = p.StandardError;

                sw.AutoFlush = true;
                sw.Write(S.Config.GetTag("sh_locale")+" "+cmd);
                sw.Close();
                string data = sr.ReadToEnd();
                string error = err.ReadToEnd();
                if (error.Trim() != "")
                    data += "\n" + error;
                p.Close();
                data = data.Trim('\n', ' ', '\r');  
                return data;
            }
        }



    }
}
