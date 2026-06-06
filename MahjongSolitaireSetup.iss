[Setup]
AppName=Mahjong Solitaire
AppVersion=1.0
DefaultDirName={autopf}\Radish\MahjongSolitaire
DefaultGroupName=Radish
SetupIconFile=images\mahjong.ico
UninstallDisplayIcon={app}\MahjongSolitaire.exe
LicenseFile=LICENSE.txt
OutputBaseFilename=MahjongSolitaireSetup
ArchitecturesInstallIn64BitMode=x64compatible
ArchitecturesAllowed=x64compatible
AppPublisher=Radish
AppPublisherURL=https://radish-vert.vercel.app

[Files]
Source: "bin\Release\net10.0-windows\publish\win-x64\*"; DestDir: "{app}"; Flags: recursesubdirs

[Icons]
Name: "{group}\MahjongSolitaire"; Filename: "{app}\MahjongSolitaire.exe"
Name: "{commondesktop}\MahjongSolitaire"; Filename: "{app}\MahjongSolitaire.exe"; Tasks: desktopicon

[Tasks]
Name: desktopicon; Description: "Create a &desktop shortcut"; GroupDescription: "Additional icons:"

[Run]
Filename: "{app}\MahjongSolitaire.exe"; Description: "Launch Mahjong Solitaire"; Flags: nowait postinstall skipifsilent
