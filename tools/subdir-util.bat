set source=D:\Distrib\games\mtg\Mtgdb.Pictures\lq
set target=D:\Distrib\games\mtg\Mtgdb.Pictures\lq-7z
FOR /F "usebackq tokens=* delims=" %%A IN (`DIR %source% /B`) DO 7z\7za.exe a "%target%\%%A.7z" -t7z "%source%\%%A" -r

set source=D:\Distrib\games\mtg\Mtgdb.Pictures\mq
set target=D:\Distrib\games\mtg\Mtgdb.Pictures\mq-7z
FOR /F "usebackq tokens=* delims=" %%A IN (`DIR %source% /B`) DO 7z\7za.exe a "%target%\%%A.7z" -t7z "%source%\%%A" -r

pause