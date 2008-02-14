using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace Core.Client
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
         public string Execute(string cmd)
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
                sw.Write(cmd);
                sw.Close();
                string data = sr.ReadToEnd();
                string error = err.ReadToEnd();
                if (error.Trim() != "")
                    data += "\n" + error;
                p.Close();
                return data;
            }
        }



    }
}
