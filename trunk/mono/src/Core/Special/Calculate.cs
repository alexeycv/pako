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
using System.Collections;
using System.Diagnostics;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;
using agsXMPP.protocol.x.muc;
using Core.Client;
using Core.Conference;
using Core.Special;


namespace Core.Special
{
    /// <summary>
    /// Summary description for Class1.
    /// </summary>
    public class Calculate
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        //[STAThread]
        Variables Vars = new Variables();
        ExpressionCalculator expr = new ExpressionCalculator();
        Jid m_jid;
        Response r;
        object sobj = new object();

        public Calculate(Jid Jid)
        {
            m_jid = Jid;
        }


        public Jid Jid
        {
            get { lock (sobj) { return m_jid; } }
            set { lock (sobj) { m_jid = value; } }
        }


     


        string DecodeSyntax(string exp)
        {
            string res = exp.ToLower();
    
            res =
                res.Replace("abs", "Math.Abs").
                    Replace("acos", "Math.Acos").
                    Replace("asin", "Math.Asin").
                    Replace("atan", "Math.Atan").
                    Replace("bigmul", "Math.BigMul").
                    Replace("atan2", "Math.Atan2").
                    Replace("cos", "Math.Cos").
                    Replace("ceiling", "Math.Ceiling").
                    Replace("cosh", "Math.Cosh").
                    Replace("divrem", "Math.DivRem").
                    Replace("[e]", "Math.E").
                    Replace("ieeeramainder", "Math.IEEERamainder").
                    Replace("log", "Math.Log").
                    Replace("ln", "Math.Log10").
                    Replace("max", "Math.Max").
                    Replace("min", "Math.Min").
                    Replace("[pi]", "Math.PI").
                    Replace("exp", "Math.Exp").
                    Replace("floor", "Math.Floor").
                    Replace("pow", "Math.Pow").
                    Replace("round", "Math.Round").
                    Replace("sin", "Math.Sin").
                    Replace("sinh", "Math.Sinh").
                    Replace("sqrt", "Math.Sqrt").
                    Replace("tan", "Math.Tan").
                    Replace("tanh", "Math.Tanh").
                    Replace("sign", "Math.Sign").
                    Replace("trunc", "Math.Truncate");
            return res;




        }

        public void Execute(string expression, Response resp)
        {
            r = resp;
            expression = DecodeSyntax(expression);
            string res = "";
            try
            {
           
                string commandStr = expression;

                #region Handle List Command
                if (commandStr == "list")
                {
                    resp.Reply(GetList());
                    return;
                }
                #endregion

                #region Handle Clear Command
                if (commandStr == "clear")
                {
                    resp.Reply(Clear());
                    return;
                }
                #endregion

                #region Handle Calculation



               // @out.exe("<==>"+commandStr);
                string[] commands = commandStr.Split('=');
                string exp;
                string name;
              //  @out.exe("<==>"+commands.Length.ToString());
                if (commands.Length > 1)
                {
                   // @out.exe("<==>2");
                    name = commands[0].Trim();
                    exp = commands[1];
                    if (expr.CheckVariableName(name))
                    {
                        bool iscompiled = expr.IsCompiled(expr.FormCode(exp, Vars));
                        double value = expr.Calculate(exp, Vars);
                        res = name + " = " + value.ToString();
                        Vars.SetValue(name, value);
                    }
                    else
                        res = r.FormatPattern("calc_var_syntax_error");


                }
                else

                    if (commands.Length == 1)
                    {
                       // @out.exe("<==>1");
                        name = commands[0].Trim();
                        if (expr.CheckVariableName(name))
                            Vars.SetValue(name, 0);
                        else
                        {
                            //@out.exe("<==>not value");
                            // @out.exe("The length is [1], no variable specified, the default will be \"ans\"");
                            bool iscompiled = expr.IsCompiled(expr.FormCode(name, Vars));
                           // @out.exe("<=============>" + expr.FormCode(name, Vars));
                           //  @out.exe(iscompiled);
                            if (iscompiled)
                            {
                               // @out.exe("<==>tried susseccfulle");
                                double value = expr.Calculate(name, Vars);
                               // @out.exe("<==>"+value.ToString());
                                res = "ans = " + value.ToString();
                                Vars.SetValue("ans", value);
                            }
                            else
                            {
                                res = r.FormatPattern("calc_syntax_error");
                                //@out.exe("Syntax Error");
                                //return;
                            }
                        }
                    }
                    else
                    {
                        res = r.FormatPattern("calc_syntax_error");

                    }





                #endregion

            }
            catch (Exception ex)
            {

                res = ex.ToString();
            }

            resp.Reply(res);
        }





        private string Clear()
        {
            Vars = new Variables();
            return r.FormatPattern("calc_vars_cleared");
        }

        private string GetList()
        {

            string res = r.FormatPattern("calc_vars_list", Vars.Count.ToString())+"\n";
            foreach (Variable var in Vars.Vars)
            {

                res += var.Name + " = " + var.Value.ToString() + "\n";
            }
            return res;
        }
    }
}
