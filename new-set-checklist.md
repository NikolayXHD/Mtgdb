4.1.0.3
-------

new sets are plgs,ha3,slu,ss3,2xm

[v] update data structures to match mtgjson v5 scheme 
[v] update card / prices download links
[v] download and scale new images
[v] test new images
[v] publish new images
[v] check legalities
[v] check if new keywords were introduced
    mill

Incorporating new sets checklist
################################

Data
====

Download new mtgjson.com file
Increase index versions for LuceneSearcher, LuceneSpellchecker, KeywordsSearcher

Legalities
==========

verify modern and standard legalities in data file

if necessary fix the legalities using patch.json

New keywords
============

Add new keywords to KeywordDefinitions.cs,
keywords can be found at mtg.gamepedia.com

Generated mana
==============

Seek for unknown patterns for generating mana.
- Ban all generated mana in filter by Generated mana
- search:
```
texten:(mana OR add*)
```

Images
======

download new gatherer images by using ImageDownloadingUtils tests

verify new images are used by adding test case to Set_images_are_from_expected_directory

visually inspect set images by running Mtgdb.Gui

check new XLHQ images for cases when *split* cards are named like *aftermath*
search pattern for everything app
```
D:\Distrib\games\mtg\Mega\XLHQ\ » !.xrop !200DPI !100DPI !GRN
```
only aftermath and flip and *no* split cards should appear in result


use DeploymentUtils to
- download
- scale
- select
- sign
- .7z
the images

upload .7z -ipped set directories to yandex drive
also update these files in out/update/img/
    lq-list/filelist.txt
    lq-token-list/filelist.txt
    mq-list/filelist.txt
    mq-token-list/filelist.txt

update

Additional data
===============

Download new translations from gatherer.wizards.com

Download new prices

Increase index versions for LuceneSearcher, LuceneSpellchecker, KeywordsSearcher

Publish
=======

update release notes at https://github.com/NikolayXHD/Mtgdb/wiki/Release-notes

run package.bat

Miscellaneous
=============

Mass import gdrive links
------------------------

gdrive-windows-x64.exe list --max 0 --name-width 0 --order name --query "'11Fm-i-coWuRscB0QeFwqdDdPknXBs_Lz' in parents"
gdrive-windows-x64.exe list --max 0 --name-width 0 --order name --query "'1AfTea0-lYJUT__vrxyeNbpZJRVouTIfq' in parents"

gdrive-windows-x64.exe list --max 0 --name-width 0 --order name --query "'1HGB0CJXBPc9E_YTiIo1kbSV_RULqERdl' in parents"
gdrive-windows-x64.exe list --max 0 --name-width 0 --order name --query "'1ZonX6PNJBVjOcqQzojwTzWdZ8HoNyAz0' in parents"

lq       11Fm-i-coWuRscB0QeFwqdDdPknXBs_Lz
mq       1AfTea0-lYJUT__vrxyeNbpZJRVouTIfq
lq-token 1HGB0CJXBPc9E_YTiIo1kbSV_RULqERdl
mq-token 1ZonX6PNJBVjOcqQzojwTzWdZ8HoNyAz0

old way
-------

- Open gdrive directory in web browser 
- Scroll down to make sure all files are loaded into the table
- Copy html of the table element & paste into text editor
- Pretty print XML

- make following replacements

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
by `<Dir GdriveId="\1" Subdir="\2" />`
