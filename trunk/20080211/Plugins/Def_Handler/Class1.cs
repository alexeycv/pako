using System;
using System.Collections.Generic;
using System.Text;
using Core.Plugins;
using Core.Client;
using Core.Conference;
using agsXMPP;
using agsXMPP.protocol.client;

namespace Plugin
{
 
    public class Main : IPlugin
    {

        public string File
        {
            get
            {
                return "Def.dll";
            }
        }

        public string Name
        {
            get
            {
                return "Def";
            }
        }

        public string Comment
        {
            get
            {
                return "A MUC-specific dictionary handler" ;
            }
        }

        public void PerformAction(IPluginData d)
        {

            DefsHandler ph = new DefsHandler(d.r, Name);

        }
    }



}
