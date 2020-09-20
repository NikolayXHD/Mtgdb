#!/bin/bash

# allow running this script from shell:
# $ chmod ug+x start.sh
# run:
# $ ./start.sh

MONO_PATH=$(command -v mono)
if [ ! "$MONO_PATH" ];then
  echo "Mtgdb.Gui cannot be started because Mono runtime is missing.
Mono is required to run Mtgdb.Gui in GNU/Linux. Shell command to install mono:

  sudo apt install mono-complete

In case of problems with installing or running Mono refer to website:
https://www.mono-project.com/download/stable/#download-lin
"

exit 1
fi

DIR=$( cd "$( dirname "$( readlink -f "${BASH_SOURCE[0]}" )" )" && pwd )
ALL_VERSION_DIRS=("$DIR"/bin/v*)
latest="${ALL_VERSION_DIRS[${#ALL_VERSION_DIRS[@]}-1]}"
mono "${latest}"/Mtgdb.Gui.exe
