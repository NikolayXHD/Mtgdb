set origin=F:\repo\git\mtgdb\out
set images=D:\distrib\games\mtg\Mtgdb.Pictures
set sets=c19,celd,eld,m20,mh1,pwar,ss2
%origin%\bin\debug\Mtgdb.Util.exe -export %images% -small lq -zoomed mq -set %sets% -force-remove-corner -silent
