@echo off
set configuration=release
set origin=F:\Repo\Git\mtgDb
set output=%origin%\out
set pub=D:\Distrib\games\mtg\Mega\Mtgdb.Gui

for /f "delims=" %%x in (%origin%\SolutionInfo.cs) do @set version=%%x
rem [assembly: AssemblyVersion("1.3.5.20")]
set version=%version:~28,-3%

set targetRoot=D:\Distrib\games\mtg\Packaged\%version%
set packageName=Mtgdb.Gui.v%version%
set target=%targetRoot%\%packageName%
set targetBin=%target%\bin\v%version%
set utilexe=%output%\bin\%configuration%\Mtgdb.Util.exe

set googledriveexe=%origin%\tools\gdrive-windows-x64.exe
set fileid=0B_zQYOTucmnUOVE1eDU0STJZeE0

rem goto upload
rem goto sign

"C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe" %origin%\Mtgdb.sln /verbosity:m

if errorlevel 1 exit /b

%utilexe% -update_help

rmdir /q /s %targetRoot%
mkdir %targetRoot%
mkdir %target%

xcopy /q /r /i /e %output%\data %target%\data
xcopy /q /r /i /e %output%\bin\%configuration% %targetBin%
xcopy /q /r /i /e %output%\etc %target%\etc
xcopy /q /r /i /e %output%\images %target%\images
xcopy /q /r /i /e %output%\update %target%\update
xcopy /q /r /i /e %output%\help %target%\help
xcopy /q %output%\..\LICENSE %target%

del /q /s %target%\update\app\*
del /q /s %target%\update\*.bak
del /q /s %target%\update\*.zip
del /q /s %target%\update\notifications\*.zip
del /q /s %target%\update\notifications\new\*
del /q /s %target%\update\notifications\read\*
del /q /s %target%\update\notifications\archive\*
del /q %target%\update\filelist.txt

del /q /s %target%\images\*.jpg
del /q /s %target%\images\*.txt

cscript shortcut.vbs %target%\Mtgdb.Gui.lnk %version% %output%\bin\%configuration%\mtg64.ico

del /q /s %target%\*.vshost.*
%utilexe% -sign %target%\bin\v%version% -output %targetBin%\filelist.txt

"%output%\update\7z\7za.exe" a %target%.zip -tzip -ir!%target%\* -mmt=on -mm=LZMA -md=64m -mfb=64 -mlc=8

:sign
%utilexe% -sign %target%.zip -output %targetRoot%\filelist.txt

del /q /s %pub%\Archive\*.zip
del /q /s %pub%\FileList\*.txt
xcopy /q %target%.zip %pub%\Archive
xcopy /q %targetRoot%\filelist.txt %pub%\FileList

start D:\Games\Mtgdb.Gui\Mtgdb.Gui.lnk
%origin%\Test\NUnit.Console-3.7.0\nunit3-console.exe %output%\bin\release-test\Mtgdb.Test.dll

if errorlevel 1 exit /b %errorlevel%

echo Ready to create update Notification
pause

git -C f:/Repo/Git/Mtgdb.wiki pull
%utilexe% -notify
git -C f:/Repo/Git/Mtgdb.Notifications add -A
git -C f:/Repo/Git/Mtgdb.Notifications commit -m auto
git -C f:/Repo/Git/Mtgdb.Notifications push

:upload
%googledriveexe% update %fileid% %pub%\Archive\%packageName%.zip

exit /b