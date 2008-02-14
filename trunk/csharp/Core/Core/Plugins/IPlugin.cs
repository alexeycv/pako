using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Plugins
{
    public interface IPlugin
    {
        string File { get;}
        string Name { get;}
        string Comment { get;}
        void PerformAction(IPluginData d);
    }
}
       