exe:   core plugins src/Pako/Program.cs
	gmcs src/Pako/*.cs -r:bin/Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -out:bin/Pako.exe

core:
	gmcs src/Core/Conference/*.cs src/Core/DataBase/*.cs src/Core/Kernel/*.cs src/Core/Other/*.cs src/Core/Plugins/*.cs src/Core/Xml/*.cs -r:lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -target:library -out:bin/Core.dll


plugins: 
	gmcs src/Plugins/Admin/*.cs -r:bin/Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -t:library -out:bin/Plugins/Admin.dll
	gmcs src/Plugins/Alias/*.cs -r:bin/Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -t:library -out:bin/Plugins/Alias.dll
	gmcs src/Plugins/Def/*.cs -r:bin/Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -t:library -out:bin/Plugins/Def.dll
	gmcs src/Plugins/Dict/*.cs -r:bin/Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -t:library -out:bin/Plugins/Dict.dll
	gmcs src/Plugins/Iq/*.cs -r:bin/Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -t:library -out:bin/Plugins/Iq.dll
	gmcs src/Plugins/Misc/*.cs -r:bin/Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -t:library -out:bin/Plugins/Misc.dll
	gmcs src/Plugins/Muc/*.cs -r:bin/Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -t:library -out:bin/Plugins/Muc.dll
	gmcs src/Plugins/Web/*.cs -r:System.Data,System.Web,bin/Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -t:library -out:bin/Plugins/Web.dll

install:
	cp lib/agsXMPP.dll bin/agsXMPP.dll
	cp lib/Mono.Data.SqliteClient.dll bin/Mono.Data.SqliteClient.dll
	nano bin/Pako.cfg

run:
	mono bin/Pako.exe

