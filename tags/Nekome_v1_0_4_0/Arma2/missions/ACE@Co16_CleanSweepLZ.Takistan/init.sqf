// JIP fix - why does ArmA execute init.sqf for JIP players, if 'player' is not sync'd yet
if ((!isServer) && (player != player)) then{
	waitUntil {player == player};
};

TransportDone = false;

if (isServer) then
{
	call compile preprocessFileLineNumbers "scripts\Init_UPSMON.sqf";
	{
		if(side _x == west) then{
			_rnd = random 100;
			if(_rnd < 75) then{
				_x addEventHandler ["killed", "(_this select 0) removeWeapon (primaryWeapon (_this select 0))"];
			};
		};
	} forEach allUnits;
	onPlayerConnected "
		publicVariable ""TransportDone"";
	";
};

"Town" setMarkerAlpha 0;
"TownN" setMarkerAlpha 0;
"TownS" setMarkerAlpha 0;
"TownS_1" setMarkerAlpha 0;
"TownS_2" setMarkerAlpha 0;
"TownS_3" setMarkerAlpha 0;
"TownS_4" setMarkerAlpha 0;
"TownS_5" setMarkerAlpha 0;
"TownS_8" setMarkerAlpha 0;
"TownS_7" setMarkerAlpha 0;
9 setRadioMsg "Call Choppers";

[] execVM "briefing.sqf";

//Process statements stored using setVehicleInit
processInitCommands;

//Finish world initialization before mission is launched. 
finishMissionInit;

[] execVM "scripts\ArtyControl.sqf";