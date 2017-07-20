set origin=F:\Repo\Git\mtgDb\output
rem set images=D:\Distrib\games\mtg\Mega\Mtgdb.Pictures
set images=D:\temp\img

%origin%\bin\release\Mtgdb.Util.exe -sign %images%\mq -output %images%\mq-list\filelist.txt
%origin%\bin\release\Mtgdb.Util.exe -sign %images%\lq -output %images%\lq-list\filelist.txt
