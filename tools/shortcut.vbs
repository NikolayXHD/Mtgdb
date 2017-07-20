Set objShell = WScript.CreateObject("WScript.Shell")
Set lnk = objShell.CreateShortcut(WScript.Arguments(0))

lnk.TargetPath = "%SYSTEMROOT%\explorer.exe"
lnk.Arguments = "bin\v" + WScript.Arguments(1) + "\Mtgdb.Gui.exe"
lnk.WindowStyle = "1"
lnk.IconLocation = WScript.Arguments(2)
lnk.Save
'Clean up 
Set lnk = Nothing