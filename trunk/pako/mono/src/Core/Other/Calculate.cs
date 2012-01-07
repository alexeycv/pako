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
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Diagnostics;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;
using agsXMPP.protocol.x.muc;
using Core.Kernel;
using Core.Conference;
using Core.Other;


namespace Core.Other
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

        public string Transform(string source)
        {
            string b = "[0-9]+";
            Regex reg = new Regex(b);
            int l = 0;
            foreach (Match m in reg.Matches(source))
            {
                int i = m.Index;
                bool insert = true;
                try { insert = (source[i + m.ToString().Length + l] != '.') && (source.ToLower()[i + m.ToString().Length + l] != 'd'); }
                catch { }
                if (insert)
                {
                    l++;
                    source = source.Insert(i + m.ToString().Length + l - 1, "d");
                }
            }
            return source;
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
                    resp.Reply(GetList().Trim('\n'));
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



                string[] commands = commandStr.Split('=');
                string exp;
                string name;
                if (commands.Length > 1)
                {
                    name = commands[0].Trim();
                    exp = Transform(commands[1].Trim());
                    if (expr.CheckVariableName(name))
                    {
                        Regex _rg = new Regex("[0-9]");
                        if (_rg.IsMatch(name))
                        {
                            r.Reply(r.f("calc_syntax_error"));
                            return;
                        }
                        bool iscompiled = expr.IsCompiled(expr.FormCode(exp, Vars));
                        double value = expr.Calculate(exp, Vars);
                        res = name + " = " + value.ToString();
                        Vars.SetValue(name, value);
                    }
                    else
                        res = r.f("calc_var_syntax_error");


                }
                else

                    if (commands.Length == 1)
                    {
                        name = commands[0].Trim();
                        if (expr.CheckVariableName(name))
                        {
                            if (Vars.GetValue(name) != null)
                                res = Vars.GetValue(name).Value.ToString();
                            else
                                res = r.f("calc_var_not_existing");
                        }
                        else
                        {
                            name = Transform(name);
                            bool iscompiled = expr.IsCompiled(expr.FormCode(name, Vars));
                            if (iscompiled)
                            {
                                double value = expr.Calculate(name, Vars);
                                res = "a = " + value.ToString();
                                Vars.SetValue("a", value);
                            }
                            else
                            {
                                res = r.f("calc_syntax_error");
                            }
                        }
                    }
                    else
                    {
                        res = r.f("calc_syntax_error");

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
            return r.f("calc_vars_cleared");
        }

        private string GetList()
        {

            string res = r.f("calc_vars_list", Vars.Count.ToString()) + "\n";
            foreach (Variable var in Vars.Vars)
            {

                res += var.Name + " = " + var.Value.ToString() + "\n";
            }
            return res;
        }
    }
}
