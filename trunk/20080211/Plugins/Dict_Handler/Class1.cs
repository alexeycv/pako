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
                return "Dict.dll";
            }
        }

        public string Name
        {
            get
            {
                return "Dict";
            }
        }

        public string Comment
        {
            get
            {
                return "A Jid-specific global dictionary" ;
            }
        }

        public void PerformAction(IPluginData d)
        {
           
            DictHandler ph = new DictHandler(d.r, Name);

        }
    }



}
