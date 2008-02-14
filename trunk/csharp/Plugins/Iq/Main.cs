using System;
using System.Collections.Generic;
using System.Text;
using Core.Plugins;
using Core.Client;
using Core.Conference;
using agsXMPP;
using Plugin;
using agsXMPP.protocol.client;

namespace Plugin
{
 
    public class Main : IPlugin
    {

        public string File
        {
            get
            {
                return "Iq.dll";
            }
        }



        public string Name
        {
            get
            {
                return "Iq";
            }
        }

        public string Comment
        {
            get
            {
                return "Can handle IQ requests" ;
            }
        }

        public void PerformAction(IPluginData d)
        {
            UserHandler ph = new UserHandler(d.r, Name);

        }
    }



}
