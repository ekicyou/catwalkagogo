waitUntil {!(isNull player)};
waitUntil {player==player};


hTaskZ=player createSimpleTask ["Report"];
hTaskZ setSimpleTaskDescription ["After Task A or B is completed, report the mission result via satellite phone.", "Report", "Report"];
hTaskZ setSimpleTaskDestination (getMarkerPos "Report");
hTaskB=player createSimpleTask ["Task B"];
hTaskB setSimpleTaskDescription ["Destroy Ammo Dumps.", "Task B", "Task B"];
hTaskB setSimpleTaskDestination (getMarkerPos "Intel");
hTaskA=player createSimpleTask ["Task A"];
hTaskA setSimpleTaskDescription ["Seize CDF Information Agency and get a intel. (An officer has a intel file. Get it.)", "Task A", "Task A"];
hTaskA setSimpleTaskDestination (getMarkerPos "Intel");