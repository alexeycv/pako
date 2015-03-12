# Admin (Bot administrators only) #


---

## list ##
```
*admin list
```
Shows to user the list of commands in "admin" volume.

---

## jid ##
```
*admin jid 
*admin jid <new_jid>
```
  * Shows the JID of bot.
  * Sets the JID of bot to 

<new\_jid>

 - any real JID of format: user@server.domain/resource

---

## password ##
```
*admin password
*admin password <new_password>
```
  * Shows to user the password from bot's JID.
  * Sets the password from bot's JID to 

<new\_password>

 -  any string value

---

## usessl ##
```
*admin usessl
*admin usessl true(false)
```
  * Shows "true" if the bot uses SSL-connection and "false" if not.
  * Sets if the bot will use (true) SSl-connection or not (false).

---

## debug ##
```
*admin debug
*admin debug true(false)
```
  * Shows "true" if the visual-debugger is On and "false" if Off.
  * Sets if the bot will turn On (true) visua; debugger or Off (false).

---

## port ##
```
*admin port
*admin port <new_port>
```
  * Shows the port-number which bot uses to connect  to jabber-server
  * Sets the port-number to a 

<new\_port>

 - any real port-number

---

## reconnects ##
```
*admin reconnects
*admin reconnects <number>
```
  * Shows the amout of times, used to reconnect after the connection is lost. After they're ellapsed, bot stops
  * Sets the amout of times, used to reconnect after the connection is lost, to 

&lt;number&gt;

 -  any numeric value.

---

## rectime ##
```
*admin rectime
*admin rectime <number>
```
  * Shows the time (in seconds), which is wasted after the connection is lost before the next reconnect.
  * Sets the time (in seconds), which is wasted after the connection is lost before the next reconnect, to 

&lt;number&gt;

 - any numeric value

---

## compression ##
```
*admin compression
*admin compression true(false)
```
  * Shows "true" if the bot uses Compression of traffic  and "false" if not.
  * Sets if the bot uses (true) Compression of traffic or not (false)

---

## starttls ##
```
*admin starttls
*admin starttls true(false)
```
  * Show "true" if the bot uses Start-Tls when connecting to jabber-server and "false" if not.
  * Sets if the bot uses (true) Start-Tls when connecting to jabber-server or not (false)

---

## nick ##
```
*admin nick
*admin nick <new_nick>
```
  * Shows the default nick of the bot, which is used to join chat-rooms as default.
  * Sets the default nick of the bot, which is used to join chat-rooms as default, to 

<new\_nick>

 -  any string value

---

## status ##
```
*admin status
*admin status <new_status>
```
  * Shows the default status of the bot, which is used to join chat-rooms as default.
  * Sets the default status of the bot, which is used to join chat-rooms as default, to 

<new\_status>

 -  any string value

---

## lang ##
```
*admin lang
*admin lang <new_lang>
```
  * Shows the default language of the bot, with which bot will talk in chat-rooms as default.
  * Sets the default language of the bot ,with which bot will talk in chat-rooms as default, to 

<new\_lang>

 - an existing language packet.

---

## msglimit ##
```
*admin msglimit
*admin msglimit <number>
```
  * Shows the maximum length of a message, which can be passed through groupchat, so if the message is longer than msglimit -  bot sends it as Private Message (also notofies user in groupchat)
  * Sets the  maximum length of a message, which can be passed through groupchat to 

&lt;number&gt;

 - any numeric value

---

## heap ##
```
*admin heap
```
  * Shows the amount of current process's working set in megabytes (Windows only).

> No parameters needed.

---

## pako ##
```
*admin pako
```
  * Shows the information about Pako.exe - file.

> No parameters needed.

---

## prefix ##
```
*admin prefix
*admin prefix <new_prefix>
```
  * Shows the default character set, used to type commands. As default it's `*`
  * Sets the default character set, used to type commands to 

<new\_prefix>

.
**Example:**
```
user: *admin prefix
bot: *

user: *admin prefix ??
bot: Ok
```
After this all the commands have to be looking this:
```
??admin list
??muc show
```

---

## myroot ##
```
*admin myroot
```
  * Shows the directory, where the bot is running on the machine

> No parameters needed.

---

## find ##
```
*admin find [<search_pattern>] <dir>
```
  * Finds all files/directories on the machine by 

<search\_pattern>

 in specified directory: 

&lt;dir&gt;


**Example:**
```
user: *admin find [pako*] /home/user/
bot: 
   <Pako.exe>  /home/user/pako/Pako.exe
   <newpako.tar.gz>   /home/user/gz/newpako.tar.gz
   [pako]     /home/user/pako
All found: 2 files, 1 directory.
```
Notice: directories are in "[.md](.md)" and files in "<>"

---

## cmd ##
```
*admin cmd <command_prompt_expression>
```
  * This command is command prompt emulator. It measn that it accepts 

<command\_prompt\_expression>

 as a command into "cmd.exe"(Windows) / "sh"(`*`nix) and return a real answer of the command prompt to the user.
**Example:** (Windows)
```
user: *admin cmd vol C:
bot: Volume C is labeled DATA
     Serial number of volume: 88A8-A2H7
```

---

## cmdaccess ##
```
*admin cmdaccess [<command_name>]
*admin cmdaccess [<command_name>] <access_level>
```
  * Shows a minimum access-level, which is needed to access specified command: 

<command\_name>


  * Sets a minimum access-level, which is needed to access specified command: 

<command\_name>

 to 

<access\_level>


**Example:**
```
user: *admin cmdaccess [muc kick] 50
bot: Ok
```
Now the user2, whose access-level is 30, wants types the command:
```
user2: *muc kick Nick
bot: Your access-level has to be at least "50"
```

---

## quit ##
```
*admin quit
```
  * Commands bot to exit the program ( quit all the conferences and go offline) with a specified status-message. You can make bot quit with another status.
Notice, that when you command to to exit with a specified 

&lt;status&gt;

, bots add the author of the command ( exactly he adds the user-part of JID.
```
*admin quit <status>
```
**Example:**
```
user: *admin quit
***bot leaves the room (Good luck, everyone :-D)

user: *admin quit bye, bye!
***bot leaves the room (user -> bye, bye!)

bbodio: *admin quit good luck
***bot leaves the room (bbodio -> good luck)
```

---

## restart ##
```
*admin restart
```
  * Commands bot to restart the program (after this rejoin all the conferences) with a specified status-message. You can make bot restart with another status.
Notice, that when you command to to exit with a specified 

&lt;status&gt;

, bots add the author of the command ( exactly he adds the user-part of JID.
```
*admin restart <status>
```
**Example:**
```
user: *admin restart
***bot leaves the room (Good luck, everyone :-D)
***bot joins the room

user: *admin restart bye, bye!
***bot leaves the room (user -> bye, bye!)
***bot joins the room

bbodio: *admin restart good luck
***bot leaves the room (bbodio -> good luck)
***bot joins the room
```
_See **rectime, reconnects**_

---

## set\_pattern ##
```
*admin set_pattern <lang_pack> <pattern_name> <pattern_value>
```
  * Sets the pattern's, named 

<pattern\_name>

, value to 

<pattern\_value>

 in a specified language packet 

<lang\_pack>


**Example:**
```
user: *admin set_pattern en muc_leave I'm leaving, good bye! :)
bot: Ok
```
Now when bot he says "I'm leaving, good bye! :)" when leaveing a single MUC, or uses it as status-message when going offline

---

## set\_help ##
```
*admin set_help <lang_pack> [<cmd_name>] <help_value>
```
  * Sets the documentation (help information for a command 

<cmd\_name>

 in a language packet 

<lang\_pack>

 to 

<help\_value>

)
**Example:**
```
user: *admin set_help en [muc kick] Use this syntax: {1}muc kick 'nick' , where 'user' is a nick in a chat-room
bot: Ok

user: *help muc kick
bot: Use this syntax: *muc kick 'nick' , where 'user' is a nick in a chat-room
```

---

## proc\_new ##
```
*admin proc_new <proc_name>
```
  * A Shell-command, which can launch new process, named 

<proc\_name>


**Example:**
```
user: *admin proc_new C:\bbodio\program.exe
bot: Process loaded.
```
**Warning:**
A lot of Anti-Viruses count Shell-programs as viruses!

---

## proc\_show (Windows only) ##
```
*admin proc_show
```
  * This command is Task Manager emulator :) You can see a list of proccesses, launched on current machine, thier working set

---

## proc\_kill (Windows only) ##
```
*admin proc_kill <number>
```
  * Kills the process, which number in a list (**proc\_show**) is 

&lt;number&gt;


**Example:**
```
user: *admin proc_kill 1
bot: Process killed
```
So this process was killed as if it have been finished in Task Manager

_See **proc\_show**_

---

## cmdaccess ##
```
*admin cmdaccess [<cmd>]
*admin cmdaccess [<cmd>] <number>
```
  * Shows the access-level of command 

&lt;cmd&gt;


  * Sets the access-level of command 

&lt;cmd&gt;

 to 

&lt;number&gt;


**Example:**
```
user: *admin cmdaccess [muc kick]
bot: 50

user: *admin cmdaccess [muc kick] 70
bot: Ok

user: *admin cmdaccess [muc kick]
bot: 70
```

---

## vip ##
```
*admin vip <jid> <access>
*admin vip <jid> <lang>
*admin vip <jid> <access> <lang> (<lang> <access>)
```
Vip - this is a single JID, which has some exceptions in language - the way the bot talks to him anywhere, or has a special access-level, whcih he will have anyway in any conferences and roster (!!! if bot is able to see Vip's JID!).

**Example:**
```
#1:
user: *admin vip romeo@jabber.dom 56
bot: Ok

user: *misc access romeo@jabber.dom
bot: 56
#2:
user: *admin vip romeo@jabber.dom en
bot: Ok

#3:
user: *admin vip romeo@jabber.dom 56 en
OR
user: *admin vip romeo@jabber.dom en 56

bot: Ok
```
Now if bot identifies some user as romeo@jabber.dom, then:
  * #1: Romeo@jabber.dom will always have access-level 56 and not more/less
  * #2: Bot will always talk to romeo@jabber.dom in english
  * #3: Bot will do both: always talk to romeo@jabber.dom in english and romeo@jabber.dom will always have access-level 56

_See **vip\_del**_

---

## vip\_del ##
```
*admin vip_del <jid>
```
  * Deletes all the Vip previlegiouses of a JID 

&lt;jid&gt;

 if such exist
**Example:**
```
user: *admin vip_del romeo@jabber.dom
bot: Ok
```
Now romeo@jabber.dom is a single user, bot counts his access-level due to current conditions.

If the 

&lt;jid&gt;

 was not found in Vip base, then:
```
user: *admin vip_del romeo2@jabber.dom
bot: No Vip info for this user
```

_See **vip**

---

## pl\_pload ##
```
*admin pl_load <path>
```
If you forgot to add a plug-in into /Plugins directory don't worry. You can load any compatible dll from_

&lt;path&gt;

 dynamicly and if succeed - use it! Also bot copies the loaded

**Example:**
```
user: *admin pl_load D:\Stuff\Web.dll
bot: Plug-in successfully loaded

user: *web
bot: Volume "Web". Type "*web list" to get a list of commands in this volume.
```

_See **pl\_unload, pl\_info**_

---

## pl\_unload ##
```
*admin pl_unload <plugin_name>
```
Unloads the Plug-in  bu it's name: the name of Plugin is his Original name, specified when creating Plug-in.
**Example:**
```
user: *admin pl_unload Web
bot: Plugin unloaded successfully
```
Now you won't get any anwer when you do this:
```
user: *web list*
```

_See **pl\_info**_

---

## gmsg ##
```
*admin gmsg <phrase>
```
  * This is called a "global message" , because  bots sends a message - 

&lt;phrase&gt;

 to all conferences, where he is spending time.
**Example:**
```
user: *admin gmsg Hi to everyone! :)

MUC #1: 
bot: Hi to everyone! :)

MUC #2:
bot: Hi to everyone! :)
...
MUC #n:
bot: Hi to everyone! :)
```

---

## censor ##
```
*admin censor <regex>
```
This is a expression-filter, which lets you to set all the expressions, which bot counts as censored and takes action (in conference): kicks if he is able(with a specified reason-message), or warns if the target is a moderator.


&lt;regex&gt;

 - any .NET Regular Expression.
**Example:**
```
user: *admin censor \bi+c+q+\b
bot: Ok

user(moderator): ICQ rules! 
bot: Please, do not use this again! :-/

user(participant): iccqq rulez!! :))
***user was kicked with reason(You must not use this in the conference!)
```

_See **uncensor, allcensor**_

---

## uncensor ##
```
*admin uncensor <regex> (<number>)
```
Deletes the reflection on a specified Regular Expression by itself - 

&lt;regex&gt;

 or an index numbre in a list of all expressions (_See **allcensor**_)
**Example:**
```
user(moderator): ICQ rules!
bot: Please, do not use this again! :-/

#1:
user: *admin uncensor \bi+c+q+\b
bot: Ok

#2:
user: *admin uncensor 5
bot: Ok

user: ICQ rules!
```
And bot will not reflect on those "icq"s anymore :)

_See **allcensor**_

---

## allcensor ##
```
*admin allcensor
```
  * Shows all the list of Regular Expressions, on which bot reflect and counts as censored.
  * No parameters needed.
```
user: *admin allcensor
bot: Here is a list:
1) \ba+s+s\b
2) ?{4,}
3) \bd+a+m+n\b
```

_See **uncensor, censor**_

---
