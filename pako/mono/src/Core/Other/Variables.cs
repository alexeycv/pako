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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.IO;

namespace Core.Other
{
    /// <summary>
    /// Summary does
    /// cription for Variables.
    /// </summary>



    public class Variables
    {
        List<Variable> m_vars = new List<Variable>();
        object[] sobjs = new object[4];

        public Variables()
        {
            for (int i = 0; i < 4; i++)
            {
                sobjs[i] = new object();
            }
        }

        public List<Variable> Vars
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





}
