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
                return "Alias.dll";
            }
        }

        public string Name
        {
            get
            {
                return "Alias";
            }
        }

        public string Comment
        {
            get
            {
                return "Alias handler for conference" ;
            }
        }

        public void PerformAction(IPluginData d)
        {

            AliasHandler ph = new AliasHandler(d.r, Name);

        }
    }



}
