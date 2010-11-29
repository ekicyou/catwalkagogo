; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{D48C87E1-1500-4D49-AD8A-3E5D04B97D8B}
AppName=Nekome
AppVerName=Nekome 1.0.0.0
AppPublisher=cat-walk
AppPublisherURL=http://nekoaruki.com/software/nekome
AppSupportURL=http://nekoaruki.com/software/nekome
AppUpdatesURL=http://nekoaruki.com/software/nekome
DefaultDirName={pf}\Nekome
DefaultGroupName=Nekome
AllowNoIcons=yes
InfoBeforeFile=V:\text\catwalkagogo\Nekome\bin\Release\readme.txt
OutputBaseFilename=setup
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "japanese"; MessagesFile: "compiler:Languages\Japanese.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "V:\text\catwalkagogo\Nekome\bin\Release\Nekome.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "V:\text\catwalkagogo\Nekome\bin\Release\readme.txt"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\Nekome"; Filename: "{app}\Nekome.exe"
Name: "{group}\{cm:ProgramOnTheWeb,Nekome}"; Filename: "http://nekoaruki.com/software/nekome"
Name: "{group}\{cm:UninstallProgram,Nekome}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\Nekome"; Filename: "{app}\Nekome.exe"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\Nekome"; Filename: "{app}\Nekome.exe"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\Nekome.exe"; Description: "{cm:LaunchProgram,Nekome}"; Flags: nowait postinstall skipifsilent



