using System;
using System.Collections.Generic;
using System.Text;
using agsXMPP;
using agsXMPP.protocol.client;
using Core.Client;


namespace Core.Plugins
{
    public class PluginTransfer:IPluginData
    {

        Response m_resp;


     


        public PluginTransfer(Response response)
            {

                m_resp = response;
    
            }


        public Response r
        {
            get { return m_resp; }
            set { m_resp = value; }
        }


    }

}
