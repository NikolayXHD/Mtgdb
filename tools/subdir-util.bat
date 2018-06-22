set source=D:\Distrib\games\mtg\Mtgdb.Pictures.Delta\lq
set target=D:\Distrib\games\mtg\Mtgdb.Pictures.Delta\lq-7z
FOR /F "usebackq tokens=* delims=" %%A IN (`DIR %source% /B`) DO 7z\7za.exe a "%target%\%%A.7z" -t7z "%source%\%%A" -r

set source=D:\Distrib\games\mtg\Mtgdb.Pictures.Delta\mq
set target=D:\Distrib\games\mtg\Mtgdb.Pictures.Delta\mq-7z
FOR /F "usebackq tokens=* delims=" %%A IN (`DIR %source% /B`) DO 7z\7za.exe a "%target%\%%A.7z" -t7z "%source%\%%A" -r

pause