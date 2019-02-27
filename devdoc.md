build
-----
Dependency tree, build from bottom to top

Mtgdb.sln
	Subrepo\DrawingEx\DrawingEx.sln
	Subrepo\Lucene.Net.Contrib\Lucene.Net.Contrib.sln

run
---
Download out\data\AllSets.json from mtgjson.com

tools/drive.exe
---------------
https://github.com/odeke-em/drive/wiki/Building-on-Windows

Download the Go distribution for Windows [here](https://golang.org/dl)
Follow the instructions here for setting up your GOPATH.
Open a command prompt and run set CGO_ENABLED=0
In the same command prompt run go get -u github.com/odeke-em/drive/cmd/drive
Profit!

cmd
  set GOOGLE_API_CLIENT_ID=...
  set GOOGLE_API_CLIENT_SECRET=...
  drive init --service-account-file D:\Distrib\games\mtg\gdrive