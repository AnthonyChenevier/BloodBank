REM ################ Mod build and install script (c) Andreas Pardeike 2020 ################
REM MODIFIED BY Anthony Chenevier 2020: modified for my personal preferences for solution directory format and install location.
REM Also updated for recent changes to 1.1 & harmony usage, and took out the extra xcopys to a standalone install of rimworld 
REM (most likely for the debugging mod, might add that back if debugging hooks become viable again)
REM All credit goes to Andreas Pardeike. Original script at https://gist.github.com/pardeike/08ff826bf40ee60452f02d85e59f32ff
REM
REM Call this script from Visual Studio's Build Events post-build event command line box:
REM "$(ProjectDir)Install.bat" $(ConfigurationName) "$(ProjectDir)" "$(ProjectName)" "About Common v1.1" "LoadFolders.xml"
REM < 0 this script        >< 1 Release/Debug ><2 location of solution><3 Mod name ><4 folders to copy > <5 files to copy >
REM 
REM The project structure should look like this: NOTE: NO IT SHOULDN'T. I JUST CAN'T BE STUFFED TAKING THIS OUT/UPDATING IT
REM TO REFLECT MY STRUCTURE YET - need more research into 1.0 mod folder location stuff
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
REM NOTE: THE THREE VARS BELOW ARE FOR DEBUGGING. SEE NOTE BELOW. DEBUGGING REQUIRES REPLACEMENT
REM OF A VANILLA DLL. NOT SURE IF THIS WILL BE UPDATED FOR 1.1.THE SCRIPT WORKS WITHOUT IT, JUST
REM WITH NO DEBUGGING HOOKS. REGARDLESS I HAVE A .NET COMPATIBLE VERSION OF PDB2MDB SO THIS SCRIPT
REM DOES PRODUCE .MDBS BUT DOESN'T USE %MONO_EXE%. KEPT FOR COMPATIBILITY
REM MONO_EXE = C:\Program Files\Mono\bin\mono.exe
REM PDB2MDB_PATH = C:\Program Files\Mono\lib\mono\4.5\pdb2mdb.exe
REM RIMWORLD_MOD_DEBUG = --debugger-agent=transport=dt_socket,address=127.0.0.1:56000,server=y
REM 
REM RIMWORLD_DIR_STEAM = C:\Program Files (x86)\Steam\steamapps\common\RimWorld
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
SET MOD_DIR=%SOLUTION_DIR:~0,-7%
SET ZIP_FILE=%SOLUTION_DIR:~0,-8%\%~3
SET TARGET_DIR=%RIMWORLD_DIR_STEAM%\Mods\%~3
SET ZIP_EXE="C:\Program Files\7-Zip\7z.exe"

SET MOD_DLL_PATH=%MOD_DIR%v1.1\Assemblies\%~3.dll

ECHO Running post-build script:
ECHO ==========================

IF %1==Debug (
	IF EXIST %PDB2MDB_PATH% (
		IF EXIST "%MOD_DLL_PATH:~0,-4%.pdb" (
			ECHO Creating mdb for %MOD_DLL_PATH%
			"%PDB2MDB_PATH%" "%MOD_DLL_PATH%" 1>NUL
		) ELSE ECHO No pdb found
	) ELSE ECHO %PDB2MDB_PATH% does not exist. Skipping mdb creation
)

IF %1==Release (
	IF EXIST "%MOD_DLL_PATH%.mdb" (
		ECHO Deleting %MOD_DLL_PATH%.mdb
		DEL "%MOD_DLL_PATH%.mdb" 1>NUL
	)
)

IF EXIST "%RIMWORLD_DIR_STEAM%" (
	IF "%RIMWORLD_DIR_STEAM%" == "%SOLUTION_DIR%" (
		ECHO Solution and mod target directory match. Skipping copy operation.
	) ELSE (
		ECHO Copying to %TARGET_DIR%
		IF NOT EXIST "%TARGET_DIR%" (
			MKDIR "%TARGET_DIR%"
		) ELSE (
			ECHO WARNING-'%TARGET_DIR%' already exists. Old files will not be automatically deleted. Make sure no unused files are still there.
		)
		FOR %%D IN (%~4) DO (
		ECHO Copying folder '%MOD_DIR%%%D' to '%TARGET_DIR%\%%D'
			XCOPY /I /Y /E "%MOD_DIR%%%D" "%TARGET_DIR%\%%D" 1>NUL
		)
		FOR %%D IN (%~5) DO (
		ECHO Copying file '%MOD_DIR%%%D' to '%TARGET_DIR%\%%D'
			XCOPY /Y "%MOD_DIR%%%D" "%TARGET_DIR%\*" 1>NUL
		)
	)
	IF EXIST "%ZIP_FILE%.zip" (
		ECHO Deleting old '%ZIP_FILE%.zip'
		DEL "%ZIP_FILE%.zip"
	)
	ECHO Adding mod files to '%ZIP_FILE%.zip'
	FOR %%D IN (%~4) DO (
		ECHO Adding folder '%TARGET_DIR%\%%D' to '%ZIP_FILE%.zip'
		%ZIP_EXE% a "%ZIP_FILE%.zip" "%TARGET_DIR%\%%D" 1>NUL
	)
	FOR %%D IN (%~5) DO (
		ECHO Adding file '%TARGET_DIR%\%%D' to '%ZIP_FILE%.zip'
		%ZIP_EXE% a "%ZIP_FILE%.zip" "%TARGET_DIR%\%%D" 1>NUL
	)
ECHO ==========================
ECHO post-build script complete
)