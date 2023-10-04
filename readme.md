This mod was created with the intention to allow for offline play that will not result in any kind of online change.
The hope is that this will allow adding in new content (like Vestiges).

The config ADDB.ForceOffline.cfg (auto created after the first start) contains the following 3 options:

## Force the game to start in offline mode
doForceOfflineMode = false

## Set to true to always copy the latest online data even if there is an in-progress offline run (which would be deleted)
CopyOnlineEvenIfExistingInProgressOffline = false

## When this is enabled, offline runs are kept separately in a way that they won't be reported after going back online. 
## This is to prevent any kind of data/synchronization/verification problems. If you're playing with cheats/game changing mods this should be enabled!
dontReportOnline = true

It forces the game offline by changing the return value of ClientConfig.IsForceOffline to true

It prevents online reporting by changing the paths for the save files