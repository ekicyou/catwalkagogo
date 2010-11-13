if(isNull player) exitWith{};

hint "Nuclear bomb's timer has been activated.";
_title = "Escape Immediately";
tEscape=player createSimpleTask [_title];
tEscape setSimpleTaskDescription ["Nuclear bomb's timer has been activated. Get away from there immediately", _title, _title];
tEscape setSimpleTaskDestination (getMarkerPos "CombatZone");
player setCurrentTask tEscape;
taskHint ["NEW OBJECTIVE: \n Escape Immediately",[1,1,1,1],"taskNew"];

waitUntil{nNukeTimer < 300};
player sideChat "Nuclear bomb detonates ETA 5 minutes.";

waitUntil{nNukeTimer < 240};
player sideChat "Nuclear bomb detonates ETA 4 minutes.";

waitUntil{nNukeTimer < 180};
player sideChat "Nuclear bomb detonates ETA 3 minutes.";

waitUntil{nNukeTimer < 120};
player sideChat "Nuclear bomb detonates ETA 2 minutes.";

waitUntil{nNukeTimer < 60};
player sideChat "Nuclear bomb detonates ETA 1 minute.";

waitUntil{nNukeTimer < 30};
player sideChat "Nuclear bomb detonates ETA 30 seconds.";
