set configuration=release

set origin=F:\Repo\Git\mtgDb
set output=%origin%\output

@for /f "delims=" %%x in (%origin%\SolutionInfo.cs) do @set version=%%x
rem [assembly: AssemblyVersion("1.3.5.20")]
set version=%version:~28,-3%

set targetRoot=D:\Distrib\games\mtg\Packaged\%version%
set target=%targetRoot%\Mtgdb.Gui.v%version%
set targetBin=%target%\bin\v%version%

rmdir /s /q %targetRoot%
mkdir %targetRoot%
mkdir %target%

xcopy /r /d /i /y /E %output%\data %target%\data
xcopy /r /d /i /y /E %output%\bin\%configuration% %targetBin%
xcopy /r /d /i /y /E %output%\etc %target%\etc
xcopy /r /d /i /y /E %output%\images %target%\images
xcopy /r /d /i /y /E %output%\update %target%\update

del /q %target%\etc\Mtgdb.Test.xml
del /s /q %target%\update\app\*
del /s /q %target%\update\*.bak
del /s /q %target%\update\*.zip
del /s /q %target%\update\notifications\new\*.txt
del /s /q %target%\update\notifications\read\*.txt
del /q %target%\update\filelist.txt

del /s /q %target%\images\*.jpg
del /s /q %target%\images\*.txt

copy %output%\*.txt %target%
xcopy /r /i /y /E %output%\original %target%
xcopy %output%\..\LICENSE %target%\LICENSE

cscript shortcut.vbs %target%\Mtgdb.Gui.lnk %version% %output%\bin\%configuration%\mtg64.ico

del /s /q %target%\*.vshost.*
%output%\bin\%configuration%\Mtgdb.Util.exe -sign %target%\bin\v%version% -output %targetBin%\filelist.txt

"%output%\update\7z\7za.exe" a %target%.zip -tzip -ir!%target%\* -mmt=on -mm=LZMA -md=64m -mfb=64 -mlc=8
%output%\bin\%configuration%\Mtgdb.Util.exe -sign %target%.zip -output %targetRoot%\filelist.txt