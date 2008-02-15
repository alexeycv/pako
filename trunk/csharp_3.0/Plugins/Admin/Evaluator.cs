using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;
using System.IO;
using Core.Client;
using System.Diagnostics;

namespace Plugin
{
    public class Evaluator
    {
        public Evaluator()
        {
        }

        public string Compile(string expression, SessionHandler Sh)
        {
            Microsoft.CSharp.CSharpCodeProvider cp = new Microsoft.CSharp.CSharpCodeProvider();
            System.CodeDom.Compiler.ICodeCompiler ic = cp.CreateCompiler();
            System.CodeDom.Compiler.CompilerParameters cpar = new System.CodeDom.Compiler.CompilerParameters();
            cpar.GenerateInMemory = true;
            cpar.GenerateExecutable = false;
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

            DirBuilder db = new DirBuilder();
            string m_dir = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            cpar.ReferencedAssemblies.Add(db.b(m_dir, "agsXMPP.dll"));
            cpar.ReferencedAssemblies.Add(db.b(m_dir, "Core.dll"));
            cpar.ReferencedAssemblies.Add(db.b(m_dir, "Mono.Data.SqliteClient.dll"));
           
            string source = @"
                    using System;
                    using System.Collections;
                    using Core.Client;
                    using System.Collections.Generic;
                    using System.Text;
                    using agsXMPP;
                    using agsXMPP.protocol.client;
                    using agsXMPP.protocol.iq.roster;
                    using agsXMPP.protocol.iq.version;
                    using agsXMPP.Xml.Dom;
                    using agsXMPP.protocol.x.muc;
                    using Core.Plugins;
                    using Core.Manager;
                    using Core.Conference;
                    using System.Threading;
                    using Core.Special;
                    using System.Diagnostics;
                    using System.IO;
                    using System.Text.RegularExpressions;
                    using agsXMPP.protocol.iq.time;
                    using Mono.Data.SqliteClient;   

                    public class evalclass
                    {
                        public string eval(SessionHandler sh)
                        {
                             " + expression + @"
                        }
                    }   

";

            try
            {
                System.CodeDom.Compiler.CompilerResults cr = ic.CompileAssemblyFromSource(cpar, source);
                foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
                {
                    @out.exe("=====" + ce.ToString());
                }
                if (cr.CompiledAssembly != null)
                {
                    Type ObjType = cr.CompiledAssembly.GetType("evalclass");
                    string _result;
                   
                    if (ObjType != null)
                    {
                        MethodInfo _method = ObjType.GetMethod("eval");
                        _result = (string)_method.Invoke(Activator.CreateInstance(ObjType), new object[] { Sh });
                    }
                    else
                        _result = null;


                    return _result;

                }
            }
            catch
            {
                return null;
            }
            return null;

        }
    }
}
