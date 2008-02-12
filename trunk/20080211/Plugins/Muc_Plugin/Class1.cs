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
                return "Muc.dll";
            }
        }


        public string Name
        {
            get
            {
                return "Muc";
            }
        }

        public string Comment
        {
            get
            {
                return "Can serve muc users" ;
            }
        }

        public void PerformAction(IPluginData d)
        {
           
            MucHandler ph = new MucHandler(d.r, Name);

        }
    }



}
