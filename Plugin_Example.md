# !!! #

1) **Don't forget to to use the prefix , when sending command to bot!**
**See Pako.cfg on "delimiter" attribute**

**So your command must look by default for example:` * `**

```
*admin cmd dir D:\
```

**not**

```
admin cmd dir D:\
```

2) Add reference of Core.dll into your Plug-in solution. **Your classes should be named
right like in the example!**

3) Build your Plug-in and put it into Plugins folder. Launch and enjoy if successfully!

# En example of creating a new plug-in #


Code:



```
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
        SessionHandler _session = null;

        public string File
        {
            get
            {
                return "MyPlugin.dll";
            }
        }

        public string Name
        {
            get
            {
                return "plug";
            }
        }

        public string Comment
        {
            get
            {
                return "Can give answer :-)" ;
            }
        }

        public bool MucOnly
        {
            get
            {
                return false;
            }
        }

        public SessionHandler Session
        {
            get
            {
                return _session;
            }
            set
            {
                _session = value;
            }
        } 

        public bool SubscribePresence 
        { 
            get
            {
                return false;
            }
        }

        public bool SubscribeMessages 
        { 
            get
            {
                return false;
            }
        }
        
        public bool SubscribeIq 
        { 
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Handle a command insede of the plug-in
        /// </summary>
        /// <param name="d"></param>

        public void PerformAction(IPluginData d)
        {
           //So if you type the command  plug the bot will say you: hi, little friend! :-)

           d.r.Reply("hi, little friend! :-)");

        }

        // IPlugin implementation

        // Plugin initialization and shut down
        public void Start(SessionHandler sh)
        {
            
        }

        public void Stop()
        {
        }

        // Handlers
        public void CommandHandler(agsXMPP.protocol.client.Message msg, SessionHandler s, Message emulation, CmdhState signed, int level)
        {
        }

        public void PresenceHandler(Presence m_pres, SessionHandler sh)
        {
        }

        public void IqHandler(IQ iq, XmppClientConnection Con)
        {
        }
    }



}
```



Now if you type:

```
*plug
```

bot will answer you:

```
hi, little friend! :-)
```


For more complicated structure of plug-in see any of included in sources Plug-ins projects.