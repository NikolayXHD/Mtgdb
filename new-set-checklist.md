Incorporating new sets checklist
================================

Data
----

Download new mtgjson.com file
Download new translations from gatherer.wizards.com
Download new prices
Increase index versions for LuceneSearcher, LuceneSpellchecker, KeywordsSearcher

Legalities
----------

verify modern and standard legalities in data file
if necessary fix the legalities using patch.json
add new item XXX_block to legality dropdown if new set opens a new block

Generated mana
--------------

Seek for unknown patterns for generating mana.
- Ban all generated mana in filter by Generated mana
- search: texten:(mana OR add*)

Images
------

download new gatherer images
  by using ImageDownloadingUtils tests
  
verify new images are used
  by adding test case to Set_images_are_from_expected_directory
  
visually inspect set images
  by running Mtgdb.Gui
  
use select images.bat and sign images.bat
create new MEGA directories
add them to config
upload new images to mega storage
create update notification for users