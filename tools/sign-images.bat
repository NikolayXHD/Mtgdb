set origin=F:\Repo\Git\mtgDb\out
set images=D:\distrib\games\mtg\Mtgdb.Pictures
set qs=lq mq

rem set sets=war,ss2,htr17
set sets=c19,celd,eld,m20,mh1,pwar,ss2

for %%q in (%qs%) do (
    %origin%\bin\debug\Mtgdb.Util.exe -sign %images%\%%q -output %images%\%%q-list\filelist.txt -set %sets%
)