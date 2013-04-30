exe:   core plugins src/Pako/Program.cs
	echo Compiling Pako.exe...
	gmcs src/Pako/*.cs -r:bin/Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -out:bin/Pako.exe
	echo Compiling Pako.exe... Done

core:
	echo Compiling Core.dll...
	gmcs src/Core/Conference/*.cs src/Core/API/*.cs src/Core/API/Data/*.cs src/Core/DataBase/*.cs src/Core/Kernel/*.cs src/Core/Other/*.cs src/Core/Plugins/*.cs src/Core/Xml/*.cs -r:lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll,System.Data.dll -target:library -out:bin/Core.dll -o+
	echo Compiling Core.dll...


plugins: 
	echo Compiling plug-ins...
	gmcs src/Plugins/Admin/*.cs -r:bin/Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll,System.Data.dll -t:library -out:bin/Plugins/Admin.dll
	gmcs src/Plugins/Alias/*.cs -r:bin/Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -t:library -out:bin/Plugins/Alias.dll
	gmcs src/Plugins/Def/*.cs -r:bin/Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -t:library -out:bin/Plugins/Def.dll
	gmcs src/Plugins/Dict/*.cs -r:bin/Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -t:library -out:bin/Plugins/Dict.dll
	gmcs src/Plugins/Iq/*.cs -r:bin/Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -t:library -out:bin/Plugins/Iq.dll
	gmcs src/Plugins/Misc/*.cs -r:bin/Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -t:library -out:bin/Plugins/Misc.dll
	gmcs src/Plugins/Muc/*.cs -r:bin/Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -t:library -out:bin/Plugins/Muc.dll
	gmcs src/Plugins/Web/*.cs -r:System.Data,System.Web,System.Web.Services,bin/Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -t:library -out:bin/Plugins/Web.dll
	gmcs src/Plugins/Mucfilter/*.cs -r:bin/Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -t:library -out:bin/Plugins/Mucfilter.dll
	gmcs src/Plugins/Aiml/*.cs -r:bin/Core.dll,lib/agsXMPP.dll,lib/AIMLbot.dll,lib/Mono.Data.SqliteClient.dll -t:library -out:bin/Plugins/Aiml.dll
	gmcs src/Plugins/Scheduler/*.cs -r:bin/Core.dll,lib/agsXMPP.dll,System.Data,lib/Mono.Data.SqliteClient.dll -t:library -out:bin/Plugins/Scheduler.dll
	gmcs src/Plugins/Fun/*.cs -r:bin/Core.dll,lib/agsXMPP.dll,System.Data,lib/Mono.Data.SqliteClient.dll -t:library -out:bin/Plugins/Fun.dll
	echo Compiling plug-ins... Done

	echo Copying Mono.Data.SqlClient and agsXMPP to the binary directory...
	cp lib/agsXMPP.dll bin/agsXMPP.dll
	cp lib/Mono.Data.SqliteClient.dll bin/Mono.Data.SqliteClient.dll
	echo Copying done.


install:
	echo Installing Pako...
	cp lib/agsXMPP.dll bin/agsXMPP.dll
	cp lib/Mono.Data.SqliteClient.dll bin/Mono.Data.SqliteClient.dll
	echo Please, configure the bot.
	cp bin/Example-Pako.cfg bin/Pako.cfg
	nano bin/Pako.cfg
	echo Please, configure autmaticly joined rooms. 
	nano bin/Dynamic/Rooms.base
	echo Installing Pako... Done

clean:
	@echo Deleting Pako bot build files...
	@find ./bin -name "Core.dll" -print -delete
	@find ./bin -name "Pako.exe" -print -delete
	@find ./bin/Plugins -name "*.dll" -print -delete
	@echo Success cleaning build files... Done

run:
	mono bin/Pako.exe

