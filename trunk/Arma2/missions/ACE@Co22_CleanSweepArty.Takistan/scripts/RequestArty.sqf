_type = _this select 0;

if(isNull player) exitWith{
	[ArtyOfficer, "RequestArty.sqf is executed on server!"] call Arty_BroadcastMessage;
};

if(leader player != player) exitWith{
	hint "Only leader can request artillery.";
};

_count = ArtyOfficer getVariable "Arty_Count";
if(_count <= 0) exitWith{
	hint "Out of ammo.";
};

if(RIPPER getVariable "ARTY_ONMISSION") exitWith{
	hint "Artillery is currently busy.";
};

ArtyOfficer setVariable ["Arty_Type", _type, true];

hint "Set destination point by clicking on map";
onMapSingleClick "
	ArtyOfficer setVariable [""Arty_Pos"", _pos, true];
	ArtyOfficer setVariable [""Arty_Requested"", true, true];
	ArtyOfficer setVariable [""Arty_Caller"", player, true];
	onMapSingleClick """";
	true
";

waitUntil{RIPPER getVariable "ARTY_ONMISSION"};
hint format ["Artillery rounds remains: %1", ArtyOfficer getVariable "Arty_Count"];