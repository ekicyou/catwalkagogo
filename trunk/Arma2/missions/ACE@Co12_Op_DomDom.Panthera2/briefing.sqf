waitUntil {!(isNull player)};
waitUntil {player==player};

player createDiaryRecord ["Diary",["Credit","Mission Editor: Strato"]];

_title = "Clean Sweep KrnLake";
tskKrnLake=player createSimpleTask [_title];
tskKrnLake setSimpleTaskDescription ["Clean Sweep KrnLake.", _title, _title];
tskKrnLake setSimpleTaskDestination (getMarkerPos "KrnLake");
_title = "Seize the pensions";
tskPensions=player createSimpleTask [_title];
tskPensions setSimpleTaskDescription ["Seize the pensions hold by guerilla.", _title, _title];
tskPensions setSimpleTaskDestination (getMarkerPos "Pensions");
_title = "Destroy Ammo Truck";
tskAmmoTruck=player createSimpleTask [_title];
tskAmmoTruck setSimpleTaskDescription ["Destroy Ammo Truck.", _title, _title];
tskAmmoTruck setSimpleTaskDestination (getMarkerPos "AmmoTruck");
