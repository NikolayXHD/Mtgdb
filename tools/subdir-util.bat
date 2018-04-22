rem set target=D:\Distrib\games\mtg\Mtgdb.Pictures\mq
rem FOR /F "usebackq tokens=* delims=" %%A IN (`DIR %target% /B`) DO 7z\7za.exe a "%target%\%%A.7z" -t7z "%target%\%%A" -r

set target=D:\Distrib\games\mtg\Mega\Mtgdb.Pictures\lq
set source=D:\Distrib\games\mtg\Mtgdb.Pictures\lq-7z
FOR /F "usebackq tokens=* delims=" %%A IN (`DIR %target% /B`) DO copy "%source%\%%A.7z" "%target%\%%A\%%A.7z"