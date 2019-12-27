set origin=F:\repo\git\mtgdb\out
set images=D:\distrib\games\mtg\Mtgdb.Pictures
set sets=c19,cmb1,eld,gn2,ha1,hho,peld,ptg,puma
%origin%\bin\debug\Mtgdb.Util.exe -export %images% -small lq -zoomed mq -set %sets% -force-remove-corner -silent

