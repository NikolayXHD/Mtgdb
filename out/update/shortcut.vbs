Set objShell = WScript.CreateObject("WScript.Shell")
Set lnk = objShell.CreateShortcut(WScript.Arguments(0))

lnk.TargetPath = WScript.Arguments(1)
lnk.Arguments = ""
lnk.WindowStyle = "1"
lnk.IconLocation = WScript.Arguments(2)
lnk.Save
'Clean up 
Set lnk = Nothing
