Architectures
                        WinXP Win7+ Linux MacOS Mobile
WinForms                v     v
.NET Core local webapp        v     v
nodejs local webapp           v     v     v
client-server webapp    v     v     v     v     v

[v] Search cards

  Sort, Paging
    [v]at webserver
      [ ] nodejs
      [*] .NET core
        + required by Lucene.net anyway
        + easier to migrate from exising c# code

    [ ] by mongodb
      - the need to deploy mongodb on user's pc

  Filter
    [*] Lucene.net 4.8.0 requires C#, .NET core 
      Win7SP1+, winXP not supported
      selected because
        easier to migrate from exising c# code
      
    [ ] or Lucene 7.6.0 requires jvm 8
      Win Vista SP2+, WinXP not supported

  [x] Language depending data
    - too much effort, rarely tested

[v] Display images
  [*] From local fs ->
  [ ] http-server

  Select package images
    [*] By exisitng c# code as packaging step
  Map package images
    [*] By exisitng c# code as packaging step
      store a static id -> image path map

[v] Edit deck
[v] Edit collection

[x] Undo
  only if happens to be easy
  
[x] Search decks
   By collected %

