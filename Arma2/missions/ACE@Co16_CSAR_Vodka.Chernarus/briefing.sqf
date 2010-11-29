waitUntil {!(isNull player)};
waitUntil {player==player};

player createDiaryRecord ["Diary",["Credit","Mission Editor: Strato"]];
player createDiaryRecord ["Diary",["Pilot's Inventory","-Radio<br />-Becon<br />-Watch<br />-Money<br />-Medical kits<br />-APS (4 mags)<br />-SmokeShells (Red / Green)<br />-Lighter<br />-Rescue ration<br /><img image=""paper.paa"""]];
player createDiaryRecord ["Diary",["Map","<img image=""map.paa"" />"]];
player createDiaryRecord ["Diary",["Becon","CDF pilot can activate a becon.<br /><br />Becon tells receivers an azimuth of the receiver to the pilot. This works both enemy and friendly, so that enemies will approach the pilot if the becon is on."]];

_title = "Locate Crushed Su-25";
tskCSAR=player createSimpleTask [_title];
tskCSAR setSimpleTaskDescription ["Locate crushed Su-25 (optional).", _title, _title];
tskCSAR setSimpleTaskDestination (getMarkerPos "CSARZone");
_title = "Search and Rescue";
tskCSAR=player createSimpleTask [_title];
tskCSAR setSimpleTaskDescription ["Search and rescue the CDF pilot and take him back to friendly territory.", _title, _title];
tskCSAR setSimpleTaskDestination (getMarkerPos "CSARZone");
player setCurrentTask tskCSAR;
