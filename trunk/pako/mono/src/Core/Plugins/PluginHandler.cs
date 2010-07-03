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
using System.Collections;
using System.Text;
using System.Reflection;
using System.IO;
using agsXMPP;
using agsXMPP.protocol.client;
using Core.Kernel;
using Core.Other;

namespace Core.Plugins
{
    public class PHandler
    {
        string m_dir;
        object[] sobjs = new object[11];
        ArrayList m_names;
        Hashtable m_plugs;
        int m_count;
        SessionHandler _session;

       

        public PHandler(string PluginsFolder, SessionHandler sh)
        {
            _session = sh;

            for (int i = 0; i < 11; i++)
            {
                sobjs[i] = new object();
            }
           
            m_dir = PluginsFolder;
            m_count = 0;


            m_plugs = new Hashtable();

            foreach (string dir in Directory.GetFiles(m_dir, "*.dll"))
            {
                string name = Path.GetFileNameWithoutExtension(dir);                  
                    try
                    {

                     
                        Assembly assembly = Assembly.LoadFile(dir);
                        object plugObject = Activator.CreateInstance(
                        assembly.GetType("Plugin.Main"));
                        IPlugin plugin = (IPlugin)plugObject;
                        m_plugs.Add(plugin.Name.ToLower(), plugin);
                        m_count++;        
                        // Execute plugin initialization method
                        plugin.Start(_session);

                    }
                    catch
                    {
                        @out.write("Warning! Error occured when loading plug-in \"" + dir + "\"\n"); 
                    }
            }

            string plugins_res = "Plugins available ("+m_count+"):\n";

            m_names = new ArrayList();
            foreach (IPlugin p in m_plugs.Values)
            {
                plugins_res += "<"+p.File+"> Name: \""+p.Name+"\" ["+p.Comment+"]\n";
                m_names.Add(p.Name.ToLower());
            }
            @out.write(plugins_res + "\n");

        }


        public string LoadPlugin(string dir)
        {
            lock (Plugins)
            {
                    try
                    {

                        Assembly assembly = Assembly.LoadFile(dir);
                        object plugObject = Activator.CreateInstance(
                        assembly.GetType("Plugin.Main"));
                        IPlugin plugin = (IPlugin)plugObject;
                        Plugins.Add(plugin.Name, plugin);
                        m_names.Add(plugin.Name); 
                        // Execute plugin initialization method
                        plugin.Start(_session);
                   
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                   
                   try 
			{
 			 DirBuilder db = new DirBuilder();
                         File.Copy(dir, Utils.dir(m_dir, Path.GetFileName(dir)));

			}
		catch {}
                   return "plugin_loaded";
            }

        }

        public bool UnloadPlugin(string name)
        {
            lock (Plugins)
            {
                if (m_plugs.ContainsKey(name))
                {
                    // Execute a stop action
                    //m_plugs[name].Stop();

                    m_plugs.Remove(name);
       
                    m_names.Remove(name);
           
                    return true;
                }
                return false;
            }
        }

        public bool Handles(string cmd)
        {
            lock (m_names)
            {              
                foreach (string _name in m_names)
                {
                    if (_name.ToLower() == cmd.ToLower())
                        return true;
                }
                return false;
            }
        }


        public string GetPluginsList()
        {
            lock (m_names)
            {
                string data = "";
                foreach (string name in m_names)
                {
                    data += ", " + name.ToLower();
                }
                return data.Trim(',',' ');
            }
        }


     

        public object Execute(string cmd)
        {
            lock (m_names)
            {
                foreach (string _name in m_names)
                {
                    if (_name.ToLower() == cmd.ToLower())
                        cmd = _name;
                }
                return Plugins[cmd];
            }
        }


        public Hashtable Plugins
        {
            get { lock (sobjs[10]) { return m_plugs; } }
            set { lock (sobjs[10]) { m_plugs = value; } }
        }




    }
}
