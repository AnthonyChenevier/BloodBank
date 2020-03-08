REM ################ Mod build and install script (c) Andreas Pardeike 2020 ################
REM
REM Call this script from Visual Studio's Build Events post-build event command line box:
REM "$(ProjectDir)Install.bat" $(ConfigurationName) "$(ProjectDir)" "$(ProjectName)" "About Common v1.1" "LoadFolders.xml"
REM
REM The project structure should look like this:
REM
REM Modname
REM +- .git
REM +- .vs
REM +- About
REM |  +- About.xml
REM |  +- Preview.png
REM |  +- PublishedFileId.txt
REM +- Assemblies                      <----- this is for RW1.0 + Harmony 1
REM |  +- 0Harmony.dll
REM |  +- 0Harmony.dll.mbd
REM |  +- 0Harmony.pdb
REM |  +- Modname.dll
REM |  +- Modname.dll.mbd
REM |  +- Modname.pdb
REM +- Languages
REM +- packages
REM |  +- Lib.Harmony.2.x.x
REM +- Source
REM |  +- .vs
REM |  +- obj
REM |     +- Debug
REM |     +- Release
REM |  +- Properties
REM |  +- Modname.csproj
REM |  +- Modname.csproj.user
REM |  +- packages.config
REM |  +- Install.bat                  <----- this script
REM +- Textures
REM +- v1.1
REM |  +- Assemblies                   <----- this is for RW1.1 + Harmony 2
REM |     +- 0Harmony.dll
REM |     +- 0Harmony.dll.mbd
REM |     +- 0Harmony.pdb
REM |     +- Modname.dll
REM |     +- Modname.dll.mbd
REM |     +- Modname.pdb
REM +- .gitattributes
REM +- .gitignore
REM +- LICENSE
REM +- LoadFolders.xml
REM +- README.md
REM +- Modname.sln
REM
REM Also needed are the following environment variables in the system settings (example values):
REM
REM MONO_EXE = C:\Program Files\Mono\bin\mono.exe
REM PDB2MDB_PATH = C:\Program Files\Mono\lib\mono\4.5\pdb2mdb.exe
REM RIMWORLD_DIR_STEAM = C:\Program Files (x86)\Steam\steamapps\common\RimWorld
REM RIMWORLD_DIR_STANDALONE = %USERPROFILE%\RimWorld1-0-2408Win64
REM RIMWORLD_MOD_DEBUG = --debugger-agent=transport=dt_socket,address=127.0.0.1:56000,server=y
REM
REM Finally, configure Visual Studio's Debug configuration with the rimworld exe as an external
REM program and set the working directory to the directory containing the exe.
REM
REM To debug, build the project (this script will install the mod), then run "Debug" (F5) which
REM will start RimWorld in paused state. Finally, choose "Debug -> Attach Unity Debugger" and
REM press "Input IP" and accept the default 127.0.0.1 : 56000
@ECHO ON
SETLOCAL ENABLEDELAYEDEXPANSION

SET SOLUTION_DIR=%~2
SET MOD_DIR=%SOLUTION_DIR%Data\
SET TARGET_DIR=%RIMWORLD_DIR_STEAM%\Mods\%~3
SET ZIP_EXE="C:\Program Files\7-Zip\7z.exe"

SET MOD_DLL_PATH=%MOD_DIR%v1.1\Assemblies\%~3.dll

IF %1==Debug (
	IF EXIST "%MOD_DLL_PATH:~0,-4%.pdb" (
		ECHO "Creating mdb at %MOD_DLL_PATH%"
		"%PDB2MDB_PATH%" "%MOD_DLL_PATH%" 1>NUL
	)
)

IF %1==Release (
	IF EXIST "%MOD_DLL_PATH%.mdb" (
		ECHO "Deleting %MOD_DLL_PATH%.mdb"
		DEL "%MOD_DLL_PATH%.mdb" 1>NUL
	)
)

IF EXIST "%RIMWORLD_DIR_STEAM%" (
	ECHO "Copying to %TARGET_DIR%"
	IF NOT EXIST "%TARGET_DIR%" MKDIR "%TARGET_DIR%"
	FOR %%D IN (%~4) DO (
		XCOPY /I /Y /E "%MOD_DIR%%%D" "%TARGET_DIR%\%%D" 1>NUL
	)
	FOR %%D IN (%~5) DO (
		XCOPY /Y "%MOD_DIR%%%D" "%TARGET_DIR%\*" 1>NUL
	)
)

	ECHO "Creating archive %TARGET_DIR%.zip"
	FOR %%D IN (%~4) DO (
		ECHO "Adding %TARGET_DIR%\%%D to %TARGET_DIR%.zip"
		%ZIP_EXE% a "%TARGET_DIR%.zip" "%TARGET_DIR%\%%D" 1>NUL
	)
	FOR %%D IN (%~5) DO (
		ECHO "Adding %TARGET_DIR%\%%D to %TARGET_DIR%.zip"
		%ZIP_EXE% a "%TARGET_DIR%.zip" "%TARGET_DIR%\%%D" 1>NUL
	)