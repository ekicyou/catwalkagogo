waitUntil {!(isNull player)};
waitUntil {player==player};

player createDiaryRecord ["Diary",["Target Building","<img image=""tgtbld.paa"" />"]];
player createDiaryRecord ["Diary",["VBIED","Civilian car may be planted Instant Explosive Device and the detonator will be near there.<br /><br />The IED will be detonated when you get closer to the car than 10 meters and noticed it by the detonator. If the car is destroyed, the IED will also explode.<br /><br />You can check vehicle via action menu and deactivate the IED."]];

MAG_tskObj0=player createSimpleTask ["Rescue 4 Reporters"];
MAG_tskObj0 setSimpleTaskDescription ["Rescue and take 4 reporters to <marker name=""US Base"">US Base</marker>.","Rescue 4 Reporters","Rescue 4 Reporters"];
MAG_tskObj0 setSimpleTaskDestination (getMarkerPos "Rescue");