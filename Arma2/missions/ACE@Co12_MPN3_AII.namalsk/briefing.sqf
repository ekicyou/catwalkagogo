waitUntil {!(isNull player)};
waitUntil {player==player};

player createDiaryRecord ["Diary",["Credit","Mission Editor: Strato"]];

_title = "Clear The Area";
tClear=player createSimpleTask [_title];
tClear setSimpleTaskDescription ["Clear the <marker name=""CombatZone"">area</marker>.", _title, _title];
tClear setSimpleTaskDestination (getMarkerPos "CombatZone");
player setCurrentTask tClear;
