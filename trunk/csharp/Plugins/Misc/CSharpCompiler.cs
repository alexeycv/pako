using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Threading;
using Core.Client;


namespace Plugin
{



    /// <summary>
    /// Delegate for Evaluator - C# code compilation
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void CompilerHandler(object sender, CompilerEventArgs e);



    /// <summary>
    /// Events arguments, which cotain information about compilation-complite event
    /// </summary>
    public class CompilerEventArgs : EventArgs
    {

        private CompilerState m_state;
        private string m_output;
        private CompilerErrorCollection m_errors;

        /// <summary>
        /// The enumaration with a list of possible results of C# code compilation
        /// </summary>
        public enum CompilerState
        {
            TimeOut,
            Done,
            Error
        }

        /// <summary>
        /// Get the result state of compilation
        /// </summary>
        public CompilerState State
        {
            get
            {
                return m_state;
            }

            set
            {
                m_state = value;
            }


        }


        /// <summary>
        /// Get's the Console-output of compiled assembly
        /// </summary>
        public string Output
        {
            get
            {
                return m_output;
            }

            set
            {
                m_output = value;
            }

        }


        public CompilerErrorCollection Errors
        {
            get
            {
                return m_errors;
            }

            set
            {
                m_errors = value;
            }

        }
    }

    /// <summary>
    /// A class, which can compile C# code and runs it as a debugger,
    /// redirects Std-output and gets all the Console-messages.
    /// Controls the lifetime of lunched process.
    /// </summary>
    public class CSharpCompiler
    {
        public CSharpCompiler()
        {

        }


        string m_dir;
        System.Threading.Timer timer;
        Process p = new Process();
        StreamReader sr;
        int time_out;
        bool m_killed = false;
        object obj = new object();
        ProcessStartInfo psI;
        string data;
        public event CompilerHandler Compiled;





        private bool killed
        {
            get { lock (obj) { return m_killed; } }
            set { lock (obj) { m_killed = value; } }
        }


        private void TimerCB(object obj)
        {

            Thread thr = new Thread(new ThreadStart(_timer));
            thr.Start();

        }

        private void _timer()
        {
            if (!killed)
            {
                p.Kill();
                timer.Dispose();
                killed = true;
                CompilerEventArgs e = new CompilerEventArgs();
                e.Output = null;
                e.Errors = null;
                e.State = CompilerEventArgs.CompilerState.TimeOut;
                Compiled(this, e);
            }
        }



        private void Execute()
        {


            Thread thr = new Thread(new ThreadStart(Launch));
            timer = new System.Threading.Timer(new TimerCallback(TimerCB), null, time_out, 0);
            thr.Start();



        }

        private void Launch()
        {

            psI = new ProcessStartInfo(m_dir);
            psI.UseShellExecute = false;
            psI.ErrorDialog = false;
            psI.RedirectStandardOutput = true;
            psI.CreateNoWindow = true;
            p.StartInfo = psI;
            p.Start();
            sr = p.StandardOutput;
            data = sr.ReadToEnd();
            timer.Dispose();
            p.Close();

            if (!killed)
            {
                CompilerEventArgs e = new CompilerEventArgs();
                e.Output = data;
                e.Errors = null;
                e.State = CompilerEventArgs.CompilerState.Done;
                Compiled(this, e);
            }

        }


        /// <summary>
        /// Compile C# code by calling this. Fill in  "expression" any C#-expression 
        /// to be compiled as in an entry-point void Main(string[] args);
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>Returns ggg</returns>
        public void Compile(string expression, int timeout)
        {
            time_out = timeout;
            Microsoft.CSharp.CSharpCodeProvider cp = new Microsoft.CSharp.CSharpCodeProvider();
            System.CodeDom.Compiler.ICodeCompiler ic = cp.CreateCompiler();
            System.CodeDom.Compiler.CompilerParameters cpar = new System.CodeDom.Compiler.CompilerParameters();
            cpar.GenerateInMemory = false;
            cpar.GenerateExecutable = true;

            if (Utils.OS == Platform.Windows)
            {
                cpar.ReferencedAssemblies.Add("System.dll");
                cpar.ReferencedAssemblies.Add("System.Data.dll");
            }
            else
            {
                cpar.ReferencedAssemblies.Add("System");
                cpar.ReferencedAssemblies.Add("System.Data");
            }

            @out.exe("COMPILING");
            string source = @"
                    using System;
                    using System.Collections;
                    using System.Collections.Generic;
                    using System.Text;
                    using System.Threading;
                    using System.Diagnostics;
                    using System.IO;
                    using System.Text.RegularExpressions;

                    static class Program
                    {           
                             " + expression + @"
                    }   

";



            System.CodeDom.Compiler.CompilerResults cr = ic.CompileAssemblyFromSource(cpar, source);

            if (cr.Errors.Count > 0)
            {




                CompilerEventArgs e = new CompilerEventArgs();
                e.Errors = cr.Errors;
                e.Output = data;
                e.State = CompilerEventArgs.CompilerState.Error;
                Compiled(this, e);
                return;
            }
            if (cr.CompiledAssembly != null)
            {
                m_dir = cr.PathToAssembly;
                Execute();

            }
        }


    }
}
