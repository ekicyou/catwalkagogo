waitUntil {!(isNull player)};
waitUntil {player==player};

player createDiaryRecord ["Diary",["Credit","Mission Editor: Strato"]];

_title = "Extraction";
tExtraction=player createSimpleTask [_title];
tExtraction setSimpleTaskDescription ["Go to <marker name=""Extraction"">here</marker>.", _title, _title];
tExtraction setSimpleTaskDestination (getMarkerPos "Extraction");
_title = "Clear the area";
tClear=player createSimpleTask [_title];
tClear setSimpleTaskDescription ["Clear the <marker name=""CombatZone"">area</marker>.", _title, _title];
tClear setSimpleTaskDestination (getMarkerPos "CombatZone");
player setCurrentTask tClear;
