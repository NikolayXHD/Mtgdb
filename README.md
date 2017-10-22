# Mtgdb.Gui
a Windows desktop program to search MTG cards and build decks

[![Mtgdb.Gui user interface](https://github.com/NikolayXHD/Mtgdb/blob/master/output/help/img/Ixalan_small.jpg?raw=true)](https://github.com/NikolayXHD/Mtgdb/blob/master/output/help/img/Ixalan_small.jpg?raw=true)

## Mtgdb.Controls project
Most non-standard WinForms controls used in Mtgdb.Gui were separated into a
project Mtgdb.Controls with no external dependencies. Thus you can copy-paste
it and reuse in your projects. What's there:

* Draggable tab headers control
* Tooltips with selectable text
* Album like data viewer
* Boolean filter buttons

[![screenshot](https://github.com/NikolayXHD/Mtgdb/blob/master/output/help/img/Filter_example.jpg?raw=true)](https://github.com/NikolayXHD/Mtgdb/blob/master/output/help/img/Filter_example.jpg?raw=true)

To get an idea of what these controls look like and what they can I suggest seeing [wiki](https://github.com/NikolayXHD/Mtgdb/wiki)
for screenshots.

## Search text input
The search functionality in Mtgdb.Gui is quite advanced.
You type a search query in [Lucene query language](https://lucene.apache.org/core/2_9_4/queryparsersyntax.html).
The query editor **highlights query syntax** and gives you **intellisense** when hitting `Ctrl`+`Space`

[![search intellisense](https://github.com/NikolayXHD/Mtgdb/blob/master/output/help/img/search_intellisense.jpg?raw=true)](https://github.com/NikolayXHD/Mtgdb/blob/master/output/help/img/search_intellisense.jpg?raw=true)
