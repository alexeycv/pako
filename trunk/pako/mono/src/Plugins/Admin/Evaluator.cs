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
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;
using System.IO;
using Core.Kernel;
using Core.Other;
using System.Diagnostics;

namespace Plugin
{

    /// <summary>
    /// Dynamic C# interpreter for handling the bot's session (based on compiler)
    /// </summary>
    public class Evaluator
    {

        public Evaluator()
        {   
        }


        /// <summary>
        /// A compiler which compiles "expression" and returns a result
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="Sh"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public string Compile(string expression, SessionHandler Sh, Response r)
        {
            Microsoft.CSharp.CSharpCodeProvider cp = new Microsoft.CSharp.CSharpCodeProvider();
            System.CodeDom.Compiler.ICodeCompiler ic = cp.CreateCompiler();
            System.CodeDom.Compiler.CompilerParameters cpar = new System.CodeDom.Compiler.CompilerParameters();
            cpar.GenerateInMemory = true;
            cpar.GenerateExecutable = false;
            cpar.TreatWarningsAsErrors = false;

            if (Utils.OS == Platform.Windows)
            {
                cpar.ReferencedAssemblies.Add("System.dll");
                cpar.ReferencedAssemblies.Add("System.Data.dll");
                cpar.ReferencedAssemblies.Add("System.Web.dll");
                cpar.ReferencedAssemblies.Add("Mono.Data.SqliteClient.dll");
            }
            else
            {
                cpar.ReferencedAssemblies.Add("System");
                cpar.ReferencedAssemblies.Add("System.Data");
                cpar.ReferencedAssemblies.Add("System.Web");
                cpar.ReferencedAssemblies.Add("Mono.Data.SqliteClient");
            }

            DirBuilder db = new DirBuilder();
            cpar.ReferencedAssemblies.Add(db.b(Utils.CD, "agsXMPP.dll"));
            cpar.ReferencedAssemblies.Add(db.b(Utils.CD, "Core.dll"));
           
            string source = @"
                    using System;
                    using System.Collections;
                    using System.Collections.Generic;
                    using System.Text;
                    using agsXMPP;
                    using agsXMPP.protocol.client;
                    using agsXMPP.protocol.iq.roster;
                    using agsXMPP.protocol.iq.version;
                    using agsXMPP.Xml.Dom;
                    using agsXMPP.protocol.x.muc;
                    using Core.Plugins;
                    using Core.Kernel;
                    using Core.Conference;
                    using System.Threading;
                    using Core.Other;
                    using Core.DataBase;
                    using Core.Xml;
                    using System.Diagnostics;
                    using System.IO;
                    using System.Text.RegularExpressions;
                    using agsXMPP.protocol.iq.time;
                    using Mono.Data.SqliteClient;   

                    public class evalclass
                    {
                        public string eval(SessionHandler sh, Response r, MUC muc, MUser user)
                        {
                             " + expression + @"
                        }
                    }   

                    ";

            try
            {
                System.CodeDom.Compiler.CompilerResults cr = ic.CompileAssemblyFromSource(cpar, source);
                if (cr.Errors.Count > 0)
                {
                     string data = "";
                    foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
                    {
                        string s = default(string);
                        data += r.f("cs_error", ((ce.Line - 27) < 0 ? 0 : (ce.Line - 27)).ToString(), ce.ErrorText) + "\n";
                    }
                    data = data.TrimEnd('\n');
                    return data;
                } 
                if (cr.CompiledAssembly != null)
                {
                    Type ObjType = cr.CompiledAssembly.GetType("evalclass");
                    string _result;
                  
                 
                   
                 
                    if (ObjType != null)
                    {
                        MethodInfo _method = ObjType.GetMethod("eval");
                        _result = (string)_method.Invoke(Activator.CreateInstance(ObjType), new object[] { Sh, r, r.MUC, r.MUser });
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
