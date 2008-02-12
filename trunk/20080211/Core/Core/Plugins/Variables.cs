using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.IO;

namespace Core.Special
{
    /// <summary>
    /// Summary does
    /// cription for Variables.
    /// </summary>



    public class Variables
    {
        ArrayList m_vars = new ArrayList();
        object[] sobjs = new object[4];

        public Variables()
        {
            for (int i = 0; i < 4; i++)
            {
                sobjs[i] = new object();
            }
        }

        public ArrayList Vars
        {
            get { lock (sobjs[1]) { return m_vars; } }
            set { lock (sobjs[1]) { m_vars = value; } }
        }

        public bool Exists(string name)
        {
            foreach (Variable var in Vars)
            {
                if (var.Name == name)
                    return true;
            }

            return false;
        }




        public void SetValue(string name, double value)
        {
            if (this.Exists(name))
            {
                this.GetValue(name).Value = value;
            }
            else
                Vars.Add(new Variable(name, value));


        }


        public int Count
        {
            get { lock (sobjs[3]) { return Vars.Count; } }
        }

        public Variable GetValue(string name)
        {
            foreach (Variable var in Vars)
            {
                if (var.Name == name)
                    return var;
            }
            return null;
        }

    }





    public class Variable
    {
        double m_value;
        string m_name;

        object[] sobjs = new object[3];

        public Variable(string name, double value)
        {
            for (int i = 0; i < 3; i++)
            {
                sobjs[i] = new object();
            }
            m_name = name;
            m_value = value;
        }



        public double Value
        {
            get { lock (sobjs[1]) { return m_value; } }
            set { lock (sobjs[1]) { m_value = value; } }
        }




        public string Name
        {
            get { lock (sobjs[2]) { return m_name; } }
            set { lock (sobjs[2]) { m_name = value; } }
        }


    }



}