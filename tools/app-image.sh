#!/bin/sh

VERSION=4.1.0.2
PORTABLE_PACKAGE=/mnt/d/distrib/games/mtg/Packaged/$VERSION/Mtgdb.Gui.v$VERSION.zip
WORKING_DIR=/home/kolia/tmp
TEST_MACHINE_CONNECTION=kolia@dell
ARCH=x86_64
APP_IMAGE_TOOL_URL=https://github.com/AppImage/AppImageKit/releases/download/12/appimagetool-x86_64.AppImage

mkidr -p $WORKING_DIR
7z x $PORTABLE_PACKAGE -o$WORKING_DIR/Mtgdb.Gui
chmod ug+x $WORKING_DIR/Mtgdb.Gui/AppRun
wget --directory-prefix $WORKING_DIR $APP_IMAGE_TOOL_URL
$WORKING_DIR/appimagetool-x86_64.AppImage $WORKING_DIR/Mtgdb.Gui
scp $WORKING_DIR/Mtgdb.Gui-$ARCH.AppImage $TEST_MACHINE_CONNECTION:$WORKING_DIR
