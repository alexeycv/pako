**Klichuk** and **Xak** are the main developers of the curent project. The founder of this project is **Klichuk**, and if you want to join the project notify him:
[www.klichuk.info](http://www.klichuk.info/contacts/)

**Labels:**

> - **XML** - this means that the associated file has to be written in XML language, according to all feauters of XML-specification

> - **Needed** - this means that associated file is needed for any start of bot: program can not launch without it.

> - **Optional** - this means that associated file can be missed when starting the bot: program will recreate it by it-self.

> - **Open** - this means that associated file can be changed and edited manually: if it's XML file than with special redactor, whcih writes according to XMl specification.

> - **Closed** - this measn that associated file can not be changed/edited manually.



# About #
Today we are introdusing to you a bot, an artificial brain, which works with **XMPP** protocol. It is not a live creature :-), it's just a program, which as all programs, has bugs, holes in the code, but in most works properly. It was called "Pako". It was named after a festival **"Pako Festa"**, which  is an annual festival held in Pakington Street, Geelong West each year around late February. me - the author - Klichuk Bogdan (aka **bbodio**) from Ukraine have been developing the project for already half of year independently. How did i start? Why do i do it? Simple answer: i live by XMPP :) this is the most powerful IM(Instant messaging) protocol, which can let you communicate through it with other IM systems, like ICQ, GaduGadu, HabaHaba, Mail, GMail, IRC etc. Detailed information you can find on the [Jabber.org](http://www.jabber.org), contact the main author of XMPP (aka **Jabber**) protocol - Peter Saint Andre and other members of XSF (XMPP Software Foundation). Somewhere in 01.2007 i made friends with Jabber. First things which was most interesting there where MUC-chatting and bots. Those "creatures", which can talk to you, execute commands, like a real chat-user. Sometimes you don't even need the your computer except your chat-window and an active **Jabber-bot** in the conference or a roster. That time i was a good Delphi-programmer, so i decided to write a bot on Delphi. In 2-3 months i got that Delphi is not a best way to write a bot for a Jabber -  there were no good XMPP-libraries  for Delphi and there problems with running the bot on Linux/Unix (Wine, Cedega). Than i got interested with **C#** and in two weeks my C#-Pako had ability to join a chat-room :). So till autumn 2007 i have been writing the bot which is now many featured Jabber-bot supporting MUC adminstrating, machine control, web requests, for example viewing wikipedia page, **Google** search, torrents search on [Mininova](http://www.mininova.org), currencies viewing, even studying XMPP specification by RFC3921, and a lot of interesting things.


---



# Installing #
> Pako is written on a C# language, which is supported by Windows, Unix/Linux, Mac, Solaris operating systems. That means that bot can work on almost any OS. But it is not a program which can run by "twice click" on a pako executable. For running only it needs a C#-supporting framework for each OS. On Windows it is [.NET](http://microsoft.com) Framework version 2.0 or higher. On other systems you can use [http://www.mono-project.com](Mono.md) and install a Framework dedicated right for your operaing system or build it from sources. For compiling you need
  * Windows: Sharp-Develop 2.2 and higher | Visual C# 2005/2008 | Microsoft Visual Studio 2005/2008. Any of those can compile the code.
  * Other OSes: "gmcs" is includede in standart mono-package. if it is missing use this (Linux/Unix):
```
sudo apt-get install mono-gmcs
```
**Install the mono, gmcs with sources from [Mono-Project](http://www.mono-project.com). Checked with 1.2.4, 1.9, 1.9.1, 2.0.5.**

**Warning!**
**1. With 1.2.6 there where problems when trying to compile so this version is not advanced!**
**2. On FreeBSD 6.3 can be some troubles when launching on mono 2.x.**

There are two ways to get Pako:
  * Get [binaries](http://code.google.com/p/pako/downloads/list), already built from sources, there only is needed to set the configurator and the bot is ready for a launch.
  * Get [sources](http://pako.googlecode.com/svn/trunk/pako/) from the SVN repository and compile he code to the complite binaries. How do you use SVN? SVN-clients (Subversion) can be installed on Windows ([TortoiseSVN](http://tortoisesvn.net/downloads)), on Linux/Unix.
If you already have a SVN-client lets get it on:
  * Windows (TortoiseSVN):
Install the program, then push anywhere on the free space of the desktop your right mouth-button. there you will find new volumes, added by Tortoise. In submenus look there for an "Export" item. Push it and you will see a new window opened in the center of ther screen. Fill in the first field the address of the SVN-repository: http://pako.googlecode.com/svn/trunk/pako/vs/ and in second insert a root where the sources will be save don your machine. Press "Ok" and wait till the exporting is finished.
After that move to the directory, where source-code was saved, there you can see Pako.sln - the main project file. Open it with the Sharp-Develop 2.2 and higher or Visual C# 2005/2008 or Microsoft Visual Studio 2005/2008. See in the upper panel a menu item "Project". Find there "Build" and in a few seconds a program will be built. If you see in the downer panel "Errors" some errors occured contact me - i will answer your questions - [bbodio](http://code.google.com/p/pako/wiki/bbodio). Done the compilation is done. See below the instructions to configure and launch the bot.
  * Linux/Unix (svn command)
You can use "svn" command to downlaod sources. If svn is missing type next command:
```
sudo apt-get install subversion
```
Now insert into a console this:
```
svn co http://pako.googlecode.com/svn/trunk/pako/mono
```
And the source cod will be in your $HOME/mono directory. Move there, and type the next commands:
```
make
make install
```
While executing "make install" uoy will be offered to fill in the configurator Pako.cfg and /Dynamic/Rooms.base to configure, which chat-rooms will be joined when started.
Also there can be a need in SQLite 3. So you should install
```
sudo apt-get install libmono-sqlite2.0-cil
```
## Configuring ##


**Labels:**

- **Rename Example-Pako.cfg to Pako.cfg and open it**:

Thsi is configurator, where you can specify all the XMPP-connection attributes:
Example of config is already included into the source package.
Open it and follow the tips near each field and you're done.

Part of Pako.cfg:
```
<Config>
   <bot>
    # Main language, which bot will use as default
    #<lang value="en" />
     
     ...

    # Here you have to input the list of administartors of bot, using freespace to separate them
    #<admins value="jid1@server.dom jid2@server.dom" />

    # Specifies the access of bot administrators, which will be used as default
    #<prefix value="*" />

  </bot>
</Config>
```

See: each tag has a comment, which helps to set correctly. Each tag has a "value" attribute, where you should fill-in the values of the parameter: for example:

```
<lang value="en" />
```
See: here you have to fill-in a default language, which bot will use to comunicate with users. So you should fill-in inti 'value" attribute a name of a language. Please check twice if it's correct.

```
<admins value="jid1@server.dom jid2@server.dom" />
```
See: the "admins" tag, where you have to fill-in an unlimited number of administartors, whose access-level is ALWAYS 100 (use freespace to separate jids). If you want to use one jid as administartor's you just use this: value="jid@server.dom"

```
<prefix value="*" />
```
See: here you have specify a sign, which will be used for entering  commands.
As default there is ` * `, that means that all the  commands have to be type like that:
```
*muc setsubject My New subject
*web google 1 pako
```

If prefix is ? than all the commands have to look like this:
```
?muc mynick Pako2
?def add pako = bot
```


---

Let's take a look at /Lang
There can be added unlimited count of language-packets. Each language packet is: a directory, named after international name of country, which represents current language
In this directory there has to be "help.pack" and "lang.pack";


---

**Labels:**

- **Open**

Example of **lang.pack**:

```
<lang>
   <patterns 
   admin_leave="Bye, i'm leaving :)"
   ...
   help_not_found="The information about the command {1} was not found"
   />
```

See: "patterns" has to contain all the patterns, which bot will use to answer to user. See the original /Lang/en/lang.pack to get a full list of needed patterns for the language-packet.
So if the bot has to answer with pattern admin\_leave(which is used to notify administrators, that bot is shutting down), he will answer whatever you fill in thius attribute.
if the user wants to get help for some command , which has no documentation yet (see help.pack syntax), bot will use help\_not\_found pattern to answer.
That means that you can fully change his answers as you which and he can even talk French or German :).
P.S. the patterns are formatted: if you had noticed the {1}, then this is a sign , which will be replaced by something, specified by bot. In help\_not\_found {1} is a exact command, which documentation was not found.




---

**Labels:**

- **Open**

Example of help.pack:

```
<help>
   <command name="muc kick" value="Please follow this syntax:&#xA;{1}muc kick 'nick'&#xA;where 'nick' is any user to kick" />
   <command name="muc setsubject" value="Please follow this syntax:&#xA;{1}muc subject newsubject &#xA;where 'newsubject' is any subject chat-room" />  
</help> 
```
See: it contains all documentation for commands: corerct syntax and short discription. Every tag "command" is responsible for one command. it conatins an attribute "name" where has to be a name of command and attribute "value" where has to be exact  text, which bot will return as documentation for current command.
See the {1} in "value" - this is prefix, which is used to type commands(this sign is dynamic, so bot has always to replace {1} with the current prefix.

### Warning: you MUST name language-directories properly, as by default: ###
### /en or /ru etc. Because you will later use those name as language name ###
### If the language directory is /en then the language is called "en" ###


---

Let's take it to /Dynamic directory.
You can fill all this configurators by your-self, or you can make changes during bot-session.


---


**Labels:**

- **Open**

**Rooms.base**
You have to specify 1 or more rooms, where bot will join each time when starting.
Open /Dynamic/Rooms.base and follow the example. You can add unlimited count of chat-rooms.

Example of Rooms.base:
```
<amucs>
  <rooms>
    <room lang="en" jid="talks@conference.server.dom" nick="Pako" status="Hello :)" />
    <room lang="en" jid="chat@conference.server.dom"  nick="Pako" status="hello :-D" />
  </rooms>
</amucs>
```
So we can see that he will enter two rooms when starting:

  1. chat@conference.server.dom with status "hello :-D" and nick "Pako"
  1. talks@conference.server.dom with status "Hello :)" and nick "Pako"



---


**Labels:**

- **Open**

- **Optional**

**Access.base**:
Here you can specify the minimum access, which user needs to access the command:

Example of Access.base:
```
<Access>
   <command name="muc kick" access="40" />
   <command name="muc setsubject" access="40" />
    ...
   <command name="muc censor" access="60" />
   <command name="muc uncensor" access="60" />
</Access>
```


For example, if you want to access command "muc kick" , you're access-level has to be at least 40, if you want to use command "muc censor" then you're access level has to be at least 60.
You can specify unlimited count of commands and access-level, needed for them.



---

Let's take it to /Static directory.

**Labels:**

- **Open**

Here you can see rfc3921.txt - this is full RFC 3921, written by Peter Saint-Andre : the specification of XMPP protocol.
Bot uses it to execute command "web rfc" from a Web.dll plug-in. This command is a rfc 3921 explorer: you can read any state of this document using state-number(1.1 or 2.2.1)

## Launching ##

  * Windows:
Move to the %pako%\Pako\bin\Release folder. See there Pako.exe. DoubleClick and the progam is launched. :) If all the configurators where filled correctly then you will meet the bot in the setted chat-room.
  * Linux/Unix:
You just have to move to the %pako% directory, where the sources where saved and installed, type
```
make run
```
and done. Or you can type
```
mono /%pako%/bin/Pako.exe
```

---


# Related links #

  * [Mono-Project](http://www.mono-project.com)

> Downlaod Mono binaries, sources for any OS. Take part in mailing lists, forums, discuss, ask, get answers : enjoy :)

  * [Go-Mono](http://www.go-mono.com)

> A lot of interesting stuff, downloads, dedicated  to Mono.

  * [CodeProject](http://www.codeproject.com)

> A best place for .NET-programmer. Tons of examples to see, thousands of new friends to get, Tons of interesting themes to read.

  * [AG-Software](http://ag-software.de)

> Home of **agsXMPP**. Site related to MiniClient and agsXMPP Jabber library. binaries, sources, examples, forum.

  * [C-SharpCorner](http://www.c-sharpcorner.com)

> C#, ASP, WPF, WCF .NET, Windows Vista Community.

  * [SQLite](http://www.sqlite.org/download.html)

> Download the freshest SQLite3

  * [GNU/GPL v3](http://www.gnu.org/licenses/gpl.html)

> See the license agreement.





# Bugs #

If you had found any bugs or mistakes in the code, please save the bug report and send it to me with detailed information about where and when bug occurred. I'll try to fix the problem.
New ideas and comments are always welcomed. Mail me about your idea and i will think about - maybe the next , what will be modified in the code - will be your idea!.


# Support #

See the [author](http://www.klichuk.info) contact information here.

Please leave your feedback [there](http://code.google.com/p/pako/issues/list).


# Special thanks #

**Alex** agsXMPP author and code support,

**vt** code support and testing,

**Pily** hoster, documentator,

**Dominges** documentator, tester,

**Quality** tester, new ideas,

**£lfikK** tester, new ideas,

Thank guys! :)