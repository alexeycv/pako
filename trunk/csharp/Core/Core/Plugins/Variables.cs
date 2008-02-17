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