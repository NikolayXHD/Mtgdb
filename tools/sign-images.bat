set origin=F:\Repo\Git\mtgDb\out
set images=D:\Distrib\games\mtg\Mtgdb.Pictures

%origin%\bin\release\Mtgdb.Util.exe -sign %images%\lq -output %images%\lq-list\filelist.txt -set war,pwar,mh1
%origin%\bin\release\Mtgdb.Util.exe -sign %images%\mq -output %images%\mq-list\filelist.txt -set war,pwar,mh1
