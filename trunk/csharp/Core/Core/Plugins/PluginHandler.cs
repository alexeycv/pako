using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Reflection;
using System.IO;
using agsXMPP;
using agsXMPP.protocol.client;
using Core.Client;

namespace Core.Plugins
{
    public class PHandler
    {
        string m_dir;
        object[] sobjs = new object[11];
        ArrayList m_names;
        Hashtable m_plugs;
        int m_count;

       

        public PHandler(string PluginsFolder)
        {

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

                        m_count++;        
             
                        Assembly assembly = Assembly.LoadFile(dir);
                        object plugObject = Activator.CreateInstance(
                        assembly.GetType("Plugin.Main"));
                        IPlugin plugin = (IPlugin)plugObject;
                        m_plugs.Add(plugin.Name, plugin);
                    }
                    catch
                    {
                        Console.WriteLine("Warning! Error occured when loading plug-in \"" + name + ".dll\"\n"); 
                    }
            }

            string plugins_res = "Plugins available ("+m_count+"):\n";

            m_names = new ArrayList();
            foreach (IPlugin p in m_plugs.Values)
            {
                plugins_res += "<"+p.File+"> Name: \""+p.Name+"\" ["+p.Comment+"]\n";
                m_names.Add(p.Name);
            }
            Console.WriteLine(plugins_res + "\n");

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
                 
                        return "plugin_loaded";
                    }
                    catch (Exception ex)
                    {
                        return ex.ToString();
                    }
            }

        }

        public bool UnloadPlugin(string name)
        {
            lock (Plugins)
            {
                if (m_plugs.ContainsKey(name))
                {
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
                return data;
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
