# Mtgdb.Gui
a Windows desktop program to search MTG cards and build decks

[![Mtgdb.Gui user interface](https://github.com/NikolayXHD/Mtgdb/raw/master/out/help/l/Ixalan_small.jpg)](https://github.com/NikolayXHD/Mtgdb/raw/master/out/help/l/Ixalan_small.jpg)

## Mtgdb.Controls project
Most non-standard WinForms controls used in Mtgdb.Gui were separated into a
project Mtgdb.Controls with no external dependencies. Thus you can copy-paste
it and reuse in your projects. What's there:

* Draggable tab headers control
* Tooltips with selectable text
* Album like data viewer
* Boolean filter buttons

[![screenshot](https://github.com/NikolayXHD/Mtgdb/raw/master/out/help/l/Filter_example.jpg?raw=true)](https://github.com/NikolayXHD/Mtgdb/raw/master/out/help/l/Filter_example.jpg)

To get an idea of what these controls look like and what they can I suggest seeing [wiki](https://github.com/NikolayXHD/Mtgdb/wiki)
for screenshots.

## Search text input
The search functionality in Mtgdb.Gui is quite advanced.
You type a search query in [Lucene query language](https://lucene.apache.org/core/2_9_4/queryparsersyntax.html).
The query editor **highlights query syntax** and gives you **intellisense** when hitting `Ctrl`+`Space`

[![search intellisense](https://github.com/NikolayXHD/Mtgdb/blob/master/out/help/l/search_intellisense.jpg)](https://github.com/NikolayXHD/Mtgdb/raw/master/output/help/l/search_intellisense.jpg?raw=true)
