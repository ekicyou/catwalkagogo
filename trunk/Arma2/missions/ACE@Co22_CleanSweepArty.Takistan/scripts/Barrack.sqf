if(not isServer) exitWith{};

_bld = _this select 0;
_type = _this select 1;
_poses = [4,5,6,8];
if(count _this > 2)then{
	_poses = _this select 2;
};
_types = [];

switch(_type) do{
	case "SL":{
		_types = [["US_Soldier_SL_EP1", "SERGEANT"], ["US_Soldier_Medic_EP1", "PRIVATE"], ["US_Soldier_MG_EP1", "CORPORAL"], ["US_Soldier_AMG_EP1", "PRIVATE"]];
	};
	case "FT1":{
		_types = [["US_Soldier_TL_EP1", "CORPORAL"], ["US_Soldier_AR_EP1", "PRIVATE"], ["US_Soldier_GL_EP1", "PRIVATE"], ["US_Soldier_AT_EP1", "PRIVATE"]];
	};
	case "FT2":{
		_types = [["US_Soldier_TL_EP1", "CORPORAL"], ["US_Soldier_AR_EP1", "PRIVATE"], ["US_Soldier_GL_EP1", "PRIVATE"], ["US_Soldier_Marksman_EP1", "PRIVATE"]];
	};
	case "COM":{
		_types = [["US_Soldier_Officer_EP1", "LIEUTENANT"], ["US_Soldier_Officer_EP1", "SERGEANT"], ["US_Soldier_Light_EP1", "PRIVATE"], ["US_Soldier_Light_EP1", "PRIVATE"]];
	};
};

_group = createGroup West;
{
	_unit = _group createUnit [_x select 0, getPos SpawnPoint, [], 0, "NONE"];
	_unit setRank (_x select 1);
} forEach _types;

waitUntil{ArtyBaseAlert};
if(not alive _bld) exitWith{
	{
		deleteVehicle _x;
	} forEach units _group;
};

_i = 0;
{
	_pos = _bld buildingPos (_poses select _i);
	_x setPos _pos;
	_i = _i + 1;
} forEach units _group;


[leader _group, ["ArtyBaseArea1", "ArtyBaseArea2"] select (floor(random 2)), "noslow", "fortify", "danger"] execVM "scripts\UPSMON.sqf";