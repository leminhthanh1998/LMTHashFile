; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "LMT Hash Files"
#define MyAppVersion "1.0"
#define MyAppPublisher "Le Minh Thanh"
#define MyAppURL "http://l�minhth�nh.vn"
#define MyAppExeName "LMT Hash Files.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
ArchitecturesInstallIn64BitMode = x64
AppId={{1A834C15-A4B5-42D8-BEC4-C64F9BBFBF35}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\Le Minh Thanh\{#MyAppName}
DisableProgramGroupPage=yes
OutputDir=F:\laptrinh\C#\LMTHashFiles\Setup
OutputBaseFilename=LMTHashFilesSetup
SetupIconFile=F:\laptrinh\C#\LMTHashFiles\LMTHashFiles\bin\Debug\LMT Hash Files_Secure\Icon.ico
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "F:\laptrinh\C#\LMTHashFiles\LMTHashFiles\bin\Debug\x64\LMT Hash Files_Secure\LMT Hash Files.exe"; DestDir: "{app}"; Check: Is64BitInstallMode;Flags:ignoreversion
Source: "F:\laptrinh\C#\LMTHashFiles\LMTHashFiles\bin\Debug\LMT Hash Files_Secure\LMT Hash Files.exe"; DestDir: "{app}"; Check: not Is64BitInstallMode; Flags:ignoreversion
Source: "F:\laptrinh\C#\LMTHashFiles\LMTHashFiles\bin\Debug\LMT Hash Files_Secure\Icon.ico"; DestDir: "{app}"; Flags: ignoreversion
Source: "F:\laptrinh\C#\LMTHashFiles\LMTHashFiles\bin\Debug\LMT Hash Files_Secure\MahApps.Metro.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "F:\laptrinh\C#\LMTHashFiles\LMTHashFiles\bin\Debug\LMT Hash Files_Secure\System.Windows.Interactivity.dll"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{commonprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent
Filename: "{dotnet40}\RegAsm.exe"; Parameters: /codebase MahApps.Metro.dll; WorkingDir: {app}; StatusMsg: "Registering Controls..."; Flags: runminimized
