waitUntil {!(isNull player)};
waitUntil {player==player};

MAG_tskObj1=player createSimpleTask ["Call Choppers"];
MAG_tskObj1 setSimpleTaskDescription ["After AA threats are clear, call choppers by radio command. At least one chopper must complete to transport their crew.", "Call Choppers", "Call Choppers"];
MAG_tskObj1 setSimpleTaskDestination (getMarkerPos "LZ");
MAG_tskObj0=player createSimpleTask ["Clear AA Threats"];
MAG_tskObj0 setSimpleTaskDescription ["Clear AA Threats. There are 1 Avenger System, 1 Linebacker, 1 Stinger Pod and AA Rader.","Clear AA Threats","Clear AA Threats"];
MAG_tskObj0 setSimpleTaskDestination (getMarkerPos "Area");

player createDiaryRecord ["Diary",["Objective","Secure the LZ to transport secondary heliborne squads."]];
player createDiaryRecord ["Diary",["Artillery","Artillery supports are available via action menu. There are<br />WP * 1<br />WP / Illumination * 1<br />Illumination * 3<br />Laser guided * 1<br />HE * 1"]];