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
                return "Misc.dll";
            }
        }

        public string Name
        {
            get
            {
                return "Misc";
            }
        }

        public string Comment
        {
            get
            {
                return "Different stuff for bot and users" ;
            }
        }

        public void PerformAction(IPluginData d)
        {

            MiscHandler ph = new MiscHandler(d.r, Name);

        }
    }



}
