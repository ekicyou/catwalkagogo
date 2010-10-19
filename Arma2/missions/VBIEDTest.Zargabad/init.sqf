// JIP fix - why does ArmA execute init.sqf for JIP players, if 'player' is not sync'd yet
if ((!isServer) && (player != player)) then{
	waitUntil {player == player};
};

if (isServer) then
{
	call compile preprocessFileLineNumbers "scripts\Init_UPSMON.sqf";
	allUnits execVM "skill.sqf";
};

VBIED_Debug = true;
[AmbCivVehicle] execVM "scripts\Init_VBIED.sqf";
[] execVM "briefing.sqf";

//Process statements stored using setVehicleInit
processInitCommands;

//Finish world initialization before mission is launched. 
finishMissionInit;