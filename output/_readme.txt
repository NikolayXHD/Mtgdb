=== Prerequisites ===

The program requires .NET Framework v4.0 Most people have it already installed.
In case you don't download it from https://www.microsoft.com/en-gb/download/details.aspx?id=17851

=== Running Mtgdb.Gui ===

Run Mtgdb.Gui shortcut in the root of extracted .zip

=== Downloading Images ===

Press the red cloud button in top right corner of Mtgdb.Gui window.
An update window will open where you will see buttons to download images.

To download images manually 
* Open etc\Mtgdb.Gui.xml
* Find the section <ImageLocations>
* Read the comments above commented <Directory /> tags. There you will find a download link
  and a brief description of images to be downloaded.
* Uncomment the <Directory /> tags you want
* Download images to the location specified in <Directory Path="..." />

You can also setup <ImageLocations> <Directory /> tags to use your custom images.

=== How to use images from Mtgdb.Gui in Forge ===

In cmd type and execute:
  Mtgdb.Util.exe -forge
or
  Mtgdb.Util.exe -forge -set AKH
to replace images only specific set

Forge is a very feature-rich application to play Magic The Gaghering
see more at http://www.slightlymagic.net/forum/viewforum.php?f=26

=== I already have card images somewhere on my PC. How do I make Mtgdb.Gui use them? ===

Edit the configuration file \etc\Mtgdb.Gui.xml

=== More help ===

Use help menu in window header to get help on specific topics.