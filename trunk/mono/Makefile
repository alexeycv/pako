Pako.exe: Core.dll src/Pako/Program.cs
        gmcs src/Pako/*.cs -r:Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -out:Pako.exe
Core.dll:
        gmcs src/Core/Conference/*.cs src/Core/DataBase/*.cs src/Core/Kernel/*.cs src/Core/Other/*.cs src/Core/Plugins/*.cs src/Core/Xml/*.cs -r:lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -target:library -out:Core.dll

plugins: Admin.dll Alias.dll Def.dll Dict.dll Iq.dll Misc.dll Muc.dll Web.dll

Admin.dll: Core.dll src/Plugins/Admin/*.cs
        gmcs src/Plugins/Admin/*.cs -r:Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -t:library -out:bin/Plugins/Admin.dll

Alias.dll: Core.dll src/Plugins/Alias/*.cs
        gmcs src/Plugins/Alias/*.cs -r:Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -t:library -out:bin/Plugins/Alias.dll

Def.dll: Core.dll src/Plugins/Def/*.cs
        gmcs src/Plugins/Def/*.cs -r:Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -t:library -out:bin/Plugins/Def.dll

Dict.dll: Core.dll src/Plugins/Dict/*.cs
        gmcs src/Plugins/Dict/*.cs -r:Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -t:library -out:bin/Plugins/Dict.dll

Iq.dll: Core.dll src/Plugins/Iq/*.cs
        gmcs src/Plugins/Iq/*.cs -r:Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -t:library -out:bin/Plugins/Iq.dll

Misc.dll: Core.dll src/Plugins/Misc/*.cs
        gmcs src/Plugins/Misc/*.cs -r:Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -t:library -out:bin/Plugins/Misc.dll

Muc.dll: Core.dll src/Plugins/Muc/*.cs
        gmcs src/Plugins/Muc/*.cs -r:Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -t:library -out:bin/Plugins/Muc.dll

Web.dll: Core.dll src/Plugins/Web/*.cs
        gmcs src/Plugins/Web/*.cs -r:System.Data,System.Web,Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -t:library -out:bin/Plugins/Web.dll