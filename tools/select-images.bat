set origin=F:\Repo\Git\mtgDb\out
set images=D:\Distrib\games\mtg\Mtgdb.Pictures
%origin%\bin\debug\Mtgdb.Util.exe -silent -export %images% -small lq -set war,pwar,mh1
%origin%\bin\debug\Mtgdb.Util.exe -silent -export %images% -zoomed mq -set war,pwar,mh1
