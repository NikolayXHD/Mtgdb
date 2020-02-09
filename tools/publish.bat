@echo off

set out=powershell write-host -fore DarkGreen

set configuration=release
set origin=F:\Repo\Git\mtgDb
set output=%origin%\out

for /f "delims=" %%x in (%origin%\shared\SolutionInfo.cs) do @set version=%%x
set version=%version:~28,-3%
rem [assembly: AssemblyVersion("1.3.5.20")]

set targetRoot=D:\Distrib\games\mtg\Packaged\%version%
set packageName=Mtgdb.Gui.v%version%
set target=%targetRoot%\%packageName%
set targetBin=%targetRoot%\%packageName%\bin\v%version%

set utilexe=%output%\bin\%configuration%\Mtgdb.Util.exe
set msbuildexe="C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
set nunitconsoleexe=%origin%\tools\NUnit.Console-3.7.0\nunit3-console.exe
set gitexe="C:\Program Files\Git\bin\git.exe"
set sevenzexe=%output%\update\7z\7za.exe

set googledriveexe=%origin%\tools\gdrive-windows-x64.exe
set signid=1IUp6u10KW4tv9AumeddhrL9UtQBejYEg
set fileid=0B_zQYOTucmnUOVE1eDU0STJZeE0
set testsignid=13kTrLvgeyIF2ZMOzJMzGfhcx3M0-63_B
set testfileid=18-gJb7NpBxSgjDgqDkuyKfX42pQUtdRt
set deflatefileid=1X5h6C9u9L13T720DLqMmKwZz_0YzxQmm
set robocopy_params=/s

%out% build

%msbuildexe% %origin%\Mtgdb.sln /verbosity:m
if errorlevel 1 exit /b

%out% create publish directory

rmdir /q /s %targetRoot%
mkdir %targetRoot%
mkdir %targetRoot%\%packageName%

%out% copy files
robocopy %output%\data                     ^
         %target%\data                     ^
         %robocopy_params%                 ^
     /xf %output%\data\allSets-x.json      ^
         %output%\data\AllSets.v42.json    ^
     /xd %output%\data\index\deck          ^
         %output%\data\index\keywords-test ^
         %output%\data\index\search-test   ^
         %output%\data\index\suggest-test

robocopy %output%\bin\%configuration% ^
         %targetBin%                  ^
         %robocopy_params%            ^
     /xf *.xml                        ^
         *.pdb

robocopy %output%\etc            ^
         %target%\etc            ^
         %robocopy_params%

robocopy %output%\images         ^
         %target%\images         ^
         %robocopy_params%       ^
     /xf *.jpg                   ^
         *.png                   ^
         *.txt

robocopy %output%\update                        ^
         %target%\update                        ^
         %robocopy_params%                      ^
     /xf *.bak                                  ^
         *.zip                                  ^
         *.7z                                   ^
         %output%\update\filelist.txt           ^
         %output%\update\version.txt            ^
         %output%\update\img\art\filelist.txt   ^
     /xd %output%\update\app                    ^
         %output%\update\notifications          ^
         %output%\update\megatools-1.9.98-win32

robocopy %output%\color-schemes  ^
         %target%\color-schemes  ^
         %robocopy_params%       ^
     /xf current.colors

robocopy %output%\charts         ^
         %target%\charts         ^
         %robocopy_params%

xcopy /q %output%\..\LICENSE ^
         %target%

echo %packageName%.zip %target%\update\version.txt

%out% make shortcut

cscript shortcut.vbs %target%\Mtgdb.Gui.lnk %version% %output%\bin\%configuration%\mtg64.ico

%out% sign binary files

del /q /s %target%\*.vshost.*
%utilexe% -sign %target%\bin\v%version% -output %targetBin%\filelist.txt

:zip_lzma
%out% create LZMA - compressed zip

%sevenzexe% a %targetRoot%\%packageName%.zip       ^
            -tzip -ir!%targetRoot%\%packageName%\* ^
            -mmt=on -mm=LZMA -md=64m -mfb=64 -mlc=8

%out% sign zip
%utilexe% -sign %targetRoot%\%packageName%.zip -output %targetRoot%\filelist.txt

:publish_test
%out% publish zip to test update URL
%googledriveexe% update %testfileid% %targetRoot%\%packageName%.zip
%googledriveexe% update %testsignid% %targetRoot%\filelist.txt

:start_app
%out% run installed app
start d:\games\mtgdb.gui\mtgdb.gui.lnk

:run_tests
goto confirm
%out% run tests
%nunitconsoleexe% %output%\bin\release-test\Mtgdb.Test.dll
errorlevel 1 exit /b %errorlevel%

:confirm
%out% Press Ctrl+C to cancel

pause

:publish_update_notification
%out% publish update notification
%gitexe% -C f:/Repo/Git/Mtgdb.wiki pull
%utilexe% -notify
%gitexe% -C f:/Repo/Git/Mtgdb.Notifications add -A
%gitexe% -C f:/Repo/Git/Mtgdb.Notifications commit -m auto
%gitexe% -C f:/Repo/Git/Mtgdb.Notifications push

:publish_zip
%out% publish zip to actual update URL
%googledriveexe% update %fileid% %targetRoot%\%packageName%.zip
%googledriveexe% update %signid% %targetRoot%\filelist.txt

:zip_deflate
%out% create deflate - compressed zip
mkdir %targetRoot%\deflate

%sevenzexe% a %targetRoot%\deflate\%packageName%.zip ^
            -tzip -ir!%targetRoot%\%packageName%\*   ^
            -x!data\index\*                          ^
            -x!data\AllPrintings.json                ^
            -x!data\AllPrices.json                   ^
            -mm=deflate

:upload_deflate
%out% upload deflate - compressed zip
%googledriveexe% update %deflatefileid% %targetRoot%\deflate\%packageName%.zip

exit /b
