set origin=F:\Repo\Git\mtgDb

set msbuildexe="C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe"
set nunitconsoleexe=%origin%\tools\NUnit.Console-3.7.0\nunit3-console.exe

set testdir=%origin%\out\bin\release-test
set testdirisolated=%origin%\out\bin\isolated-test
set testdll=Mtgdb.Util.Test.dll
set testname=Mtgdb.Util.ImageDownloadingUtils.PreProcessImages

%msbuildexe% %origin%\Mtgdb.sln /verbosity:m
del /q /s %testdirisolated%
xcopy /q /r /i /e %testdir% %testdirisolated%
%nunitconsoleexe% %testdirisolated%\%testdll% --wait --test=%testname%