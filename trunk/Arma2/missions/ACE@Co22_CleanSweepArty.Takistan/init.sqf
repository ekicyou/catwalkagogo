ArtyBaseAlert = false;
ArriveReinforcement = false;

// JIP fix - why does ArmA execute init.sqf for JIP players, if 'player' is not sync'd yet
if ((!isServer) && (player != player)) then{
	waitUntil {player == player};
};

if (isServer) then
{
	call compile preprocessFileLineNumbers "scripts\Init_UPSMON.sqf";
	[] spawn {
		sleep 10;
		{
			if(side _x == west) then{
				_rnd = random 100;
				if(_rnd < 75) then{
					_x addEventHandler ["killed", "removeAllWeapons (_this select 0)"];
				};
			};
		} forEach allUnits;
	};
};

"RadarArea" setMarkerAlpha 0;
"ArtyBaseArea1" setMarkerAlpha 0;
"ArtyBaseArea2" setMarkerAlpha 0;

[] execVM "briefing.sqf";

//Process statements stored using setVehicleInit
processInitCommands;

//Finish world initialization before mission is launched. 
finishMissionInit;

[] execVM "scripts\ArtyControl.sqf";