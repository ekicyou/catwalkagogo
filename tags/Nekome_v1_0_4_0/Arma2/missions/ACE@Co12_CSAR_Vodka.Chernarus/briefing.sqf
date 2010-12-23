waitUntil {!(isNull player)};
waitUntil {player==player};

player createDiaryRecord ["Diary",["Credit","Mission Editor: Strato"]];
player createDiaryRecord ["Diary",["Pilot's Inventory","-Radio(broken)<br />-Watch<br />-Money<br />-Medical kits<br />-APS (4 mags)<br />-SmokeShells (Red / Green)"]];
player createDiaryRecord ["Diary",["Map","<img image=""map.paa"" />"]];

_title = "Search and Rescue";
tskCSAR=player createSimpleTask [_title];
tskCSAR setSimpleTaskDescription ["Search and rescue the CDF pilot and take him back to friendly territory.", _title, _title];
tskCSAR setSimpleTaskDestination (getMarkerPos "CSARZone");
player setCurrentTask tskCSAR;
