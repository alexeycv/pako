Pako.exe: Core.dll src/Pako/Program.cs
	gmcs src/Pako/*.cs -r:lib/Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -out:bin/Pako.exe
Core.dll: src/Core/Manager/ReplyGenerator.cs src/Core/Manager/CalcHandler.cs src/Core/Manager/VipManager.cs src/Core/Client/Actor.cs src/Core/Client/Response.cs src/Core/Client/AutoMuc.cs src/Core/Client/RFCHandler.cs src/Core/Client/CommandHandler.cs src/Core/Client/Session.cs src/Core/Client/DirBuilder.cs src/Core/Client/SessionHandler.cs src/Core/Client/Error.cs src/Core/Client/Stdior.cs src/Core/Client/ErrorLoger.cs src/Core/Client/Tempdb.cs src/Core/Client/IQHandler.cs src/Core/Client/UserData.cs src/Core/Client/msgType.cs src/Core/Client/Utils.cs src/Core/Client/out.cs src/Core/Client/XMLContainer.cs src/Core/Client/PresenceHandler.cs src/Core/Plugins/IPlugin.cs src/Core/Plugins/PluginTransfer.cs src/Core/Plugins/IPluginData.cs src/Core/Plugins/Variables.cs src/Core/Plugins/PluginHandler.cs src/Core/Manager/AccessManager.cs src/Core/Manager/Config.cs src/Core/Manager/AutoMucManager.cs src/Core/Conference/ActionResult.cs src/Core/Conference/MUser.cs src/Core/Conference/MUC.cs src/Core/Special/Calculate.cs src/Core/Special/Dictionary.cs src/Core/Special/Censor.cs src/Core/Special/ExpressionCalculator.cs src/Core/Special/Definitions.cs
	gmcs src/Core/Manager/*.cs src/Core/Client/*.cs src/Core/Plugins/*.cs src/Core/Conference/*.cs src/Core/Special/*.cs -r:lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -t:library -out:bin/Core.dll

plugins: Admin.dll Alias.dll Def.dll Dict.dll Iq.dll Misc.dll Muc.dll Web.dll

Admin.dll: bin/Core.dll src/Plugins/Admin/*.cs
	gmcs src/Plugins/Admin/*.cs -r:lib/Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -t:library -out:bin/Plugins/Admin.dll

Alias.dll: bin/Core.dll src/Plugins/Alias/*.cs
	gmcs src/Plugins/Alias/*.cs -r:lib/Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -t:library -out:bin/Plugins/Alias.dll

Def.dll: bin/Core.dll src/Plugins/Def/*.cs
	gmcs src/Plugins/Def/*.cs -r:lib/Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -t:library -out:bin/Plugins/Def.dll

Dict.dll: bin/Core.dll src/Plugins/Dict/*.cs
	gmcs src/Plugins/Dict/*.cs -r:lib/Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -t:library -out:bin/Plugins/Dict.dll

Iq.dll: bin/Core.dll src/Plugins/Iq/*.cs
	gmcs src/Plugins/Iq/*.cs -r:lib/Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -t:library -out:bin/Plugins/Iq.dll

Misc.dll: bin/Core.dll src/Plugins/Misc/*.cs
	gmcs src/Plugins/Misc/*.cs -r:lib/Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -t:library -out:bin/Plugins/Misc.dll

Muc.dll: bin/Core.dll src/Plugins/Muc/*.cs
	gmcs src/Plugins/Muc/*.cs -r:lib/Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -t:library -out:bin/Plugins/Muc.dll

Web.dll: bin/Core.dll src/Plugins/Web/*.cs
	gmcs src/Plugins/Web/*.cs -r:System.Data,System.Web,lib/Core.dll,lib/agsXMPP.dll,lib/Mono.Data.SqliteClient.dll -t:library -out:bin/Plugins/Web.dll

clean: 
	rm bin/Pako.exe
	rm bin/Core.dll
	rm bin/Plugins/*.dll