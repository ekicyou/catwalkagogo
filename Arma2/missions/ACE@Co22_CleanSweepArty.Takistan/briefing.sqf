waitUntil {!(isNull player)};
waitUntil {player==player};

MAG_tskObj3=player createSimpleTask ["Extraction"];
MAG_tskObj3 setSimpleTaskDescription ["Go to safty zone after objective complete.", "Extraction", "Extraction"];
MAG_tskObj3 setSimpleTaskDestination (getMarkerPos "Evac");
MAG_tskObj2=player createSimpleTask ["Ambush reinforcements (optional)"];
MAG_tskObj2 setSimpleTaskDescription ["If you detected by enemies near the artillery base, they will send reinforcements. Ambush them if it's possible. (optional)", "Ambush reinforcements", "Ambush reinforcements"];
MAG_tskObj2 setSimpleTaskDestination (getMarkerPos "Ambush");
MAG_tskObj1=player createSimpleTask ["Destroy MLRSs"];
MAG_tskObj1 setSimpleTaskDescription ["Destroy all MLRSs in the artillery base.", "Destroy MLRSs", "Destroy MLRSs"];
MAG_tskObj1 setSimpleTaskDestination (getMarkerPos "MLRS");
MAG_tskObj0=player createSimpleTask ["Destroy Artillery Radar"];
MAG_tskObj0 setSimpleTaskDescription ["Destroy artillery radar. Then artillery supports will be available.","Destroy Artillery Radar","Destroy Artillery Radar"];
MAG_tskObj0 setSimpleTaskDestination (getMarkerPos "ArtyRader");

player createDiaryRecord ["Diary",["Artillery","Artillery supports are available via action menu. We have total 40 rounds of GRAD rockets."]];