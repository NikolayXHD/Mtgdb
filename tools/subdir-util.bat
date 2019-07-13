rem FOR /F "usebackq tokens=* delims=" %%A IN (`DIR %source% /B`) DO 7z\7za.exe a "%target%\%%A.7z" -t7z "%source%\%%A" -r

set root=D:\Distrib\games\mtg\Mtgdb.Pictures
set qs=mq lq
rem set sets=war ss2 htr17
set sets=m20

for %%q in (%qs%) do (
   for %%s in (%sets%) do (
      echo %%q %%s
      del /q /s "%root%\%%q-7z\%%s.7z"
      7z\7za.exe a "%root%\%%q-7z\%%s.7z" -t7z "%root%\%%q\%%s" -r
   )
)