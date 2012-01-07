/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Pako Jabber-bot.                                                              *
 * Copyright. All rights reserved © 2007-2008 by Klichuk Bogdan (Bbodio's Lab)   *
 * Copyright. All rights reserved © 2009-2012 by Alexey Bryohov                  *
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
using System.Diagnostics;
using System.IO;
using Core.Kernel;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;

namespace Core.Other
{
    /// <summary>
    /// Summary description for ExpressionEvaluator.
    /// </summary>


    public class ExpressionCalculator
    {

        public ExpressionCalculator()
        {
            //
            // TODO: Add constructor logic here
            //
        }


        public string FormCode(string expression, Variables vars)
        {

            string src = @"using System; 
                           public class calculator {
								 %MT_VARIABLES%
                            
 
                                  public double eval()
                                 {
							         return " + expression + @";
                                 }
                           }";
            string variables = " private double ans=0;\n";
            if (vars.Count > 0)
                variables = "";
            foreach (Variable v in vars.Vars)
            {

                variables += " private double " + v.Name + "=" + v.Value.ToString().Replace(",", ".") + ";\n";

            }
            // @out.exe(variables);
            src = src.Replace(@"%MT_VARIABLES%", variables);
            return src;
        }



        public double Calculate(string expression, Variables vars)
        {


            //   @out.exe("hhhh"+Directory.GetCurrentDirectory() + "\\Calculator.dll");
            Microsoft.CSharp.CSharpCodeProvider cp = new Microsoft.CSharp.CSharpCodeProvider();
            System.CodeDom.Compiler.ICodeCompiler ic = cp.CreateCompiler();
            System.CodeDom.Compiler.CompilerParameters cpar = new System.CodeDom.Compiler.CompilerParameters();
            cpar.GenerateInMemory = true;
            cpar.GenerateExecutable = false;
            cpar.TreatWarningsAsErrors = false;

            if (Utils.OS == Platform.Windows)
                cpar.ReferencedAssemblies.Add("System.dll");
            else
                cpar.ReferencedAssemblies.Add("System");
            //  cpar.ReferencedAssemblies.Add("calculator.dll");
            //Calculator calc = null;
            //object obj = null;

            string src = FormCode(expression, vars);

            System.CodeDom.Compiler.CompilerResults cr = ic.CompileAssemblyFromSource(cpar, src);
            /*  foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
              {
                  @out.write("=====" + ce.ToString());
              }*/
            if (Utils.OS == Platform.Windows)
                if (cr.Errors.Count > 0)
                    return 0;
            if (cr.CompiledAssembly != null)
            {
                Type ObjType = cr.CompiledAssembly.GetType("calculator");
                double _result;
                try
                {
                    if (ObjType != null)
                    {
                        MethodInfo _method = ObjType.GetMethod("eval");
                        _result = (double)_method.Invoke(Activator.CreateInstance(ObjType), null);
                    }
                    else
                        _result = 0;
                }
                catch
                {
                    _result = 0;
                }

                return _result;

            }
            else
            {

                return 0;
            }


        }


        public bool IsCompiled(string source)
        {
            Microsoft.CSharp.CSharpCodeProvider cp = new Microsoft.CSharp.CSharpCodeProvider();
            System.CodeDom.Compiler.ICodeCompiler ic = cp.CreateCompiler();
            System.CodeDom.Compiler.CompilerParameters cpar = new System.CodeDom.Compiler.CompilerParameters();
            cpar.GenerateInMemory = false;
            cpar.GenerateExecutable = false;
            cpar.TreatWarningsAsErrors = false;

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

            System.CodeDom.Compiler.CompilerResults cr = ic.CompileAssemblyFromSource(cpar, source);
              foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
              {
                  @out.write("==="+ce.ToString());
              }
            if (Utils.OS == Platform.Windows)
            {
                if (cr.Errors.Count > 0)
                    return false;
            }
            bool it = cr.CompiledAssembly != null;
            return it;
        }

        public bool CheckVariableName(string variableName)
        {


            string src = @"using System;
                           public class myclass{
								 public double %MT_VARIABLES%;
                                 
                                  public void call()
                                 {
									%MT_VARIABLES% = 0;
                                 }
                           }";

            src = src.Replace("%MT_VARIABLES%", variableName);
            @out.exe("<<" + variableName);
            @out.exe("<<" + src);
            bool iss = this.IsCompiled(src);
            @out.exe(iss.ToString());
            return iss;


        }




    }
}
