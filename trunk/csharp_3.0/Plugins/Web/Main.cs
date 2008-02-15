using System;
using System.Collections.Generic;
using System.Text;
using Core.Plugins;
using Core.Client;
using Core.Conference;
using agsXMPP;
using www;
using agsXMPP.protocol.client;

namespace Plugin
{
 
    public class Main : IPlugin
    {

        public string File
        {
            get
            {
                return "Web.dll";
            }
        }



        public string Name
        {
            get
            {
                return "Web";
            }
        }

        public string Comment
        {
            get
            {
                return "Can handle internet web-requests" ;
            }
        }

        public void PerformAction(IPluginData d)
        {
            WwwHandler ph = new WwwHandler(d.r, Name);

        }
    }



}
