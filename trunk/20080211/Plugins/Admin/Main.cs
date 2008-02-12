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
                return "Admin.dll";
            }
        }


        public bool MucOnly
        {
            get
            {
                return false;
            }
        }


        public string Name
        {
            get
            {
                return "Admin";
            }
        }

        public string Comment
        {
            get
            {
                return "A control panel for an adminisrator" ;
            }
        }

        public void PerformAction(IPluginData d)
        {

            if (d.r.Access >= 100)
            {
                ConfigHandler ph = new ConfigHandler(d.r);
            }else
                d.r.Reply(d.r.FormatPattern("access_not_enough","100"));

        }
    }



}
