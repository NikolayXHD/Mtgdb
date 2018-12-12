- Open directory in gdrive
- Scroll down to make sure all files are loaded into the table
- Copy html of the table element & paste into text editor
- Pretty print XML
- replace

```regexp
^.+key="([^"]+)".+>(.+\.7z)<.+$
```
by `\1 \2`

```regexp
^Использовано.+$
```
by ``

```regexp
\r\n\r\n
```
by `\r\n`

```regexp
^(.{33}) (.+)\.7z$
```
by `<Dir Subdir="\2" GdriveId="\1" />`