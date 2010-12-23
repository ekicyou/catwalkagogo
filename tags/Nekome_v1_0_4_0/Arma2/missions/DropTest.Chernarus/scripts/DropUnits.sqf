// DropUnits.sqf by Strato
// usage: [_units] execVM "scripts\DropUnits.sqf"
// 空挺降下スクリプト@ACE
// マルチでの動作は要検証。パラシュートをACE_ParachuteRoundPackを装備させておくこと。

_units = _this select 0;

{
	if(isPlayer _x) then{
		unassignVehicle _x;
		[vehicle _x, _x] execVM "x\ace\addons\sys_eject\jumpout.sqf";
	}else{
		if(isServer) then{
			unassignVehicle _x;
			[vehicle _x, _x] execVM "x\ace\addons\sys_eject\jumpout.sqf";
		};
	};
	sleep 1;
} forEach _units;