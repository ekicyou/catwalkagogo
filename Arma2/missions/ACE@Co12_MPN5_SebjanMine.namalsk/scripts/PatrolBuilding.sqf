if(not isServer) exitWith{};

_fncPatrolBuilding = {
	private ["_unit", "_bld", "_posCount"];
	
	_unit = _this select 0;
	_bld = _this select 1;
	_positions = _this select 2;
	
	while{alive _unit} do{
		_t = (60 + (random 60));
		_unit suppressFor _t;
		sleep _t;
		_unit doMove (_bld buildingPos (_positions select (floor(random (count _positions)))));
		waitUntil{(not alive _unit) or (unitReady _unit)};
	};
};

_leader = _this select 0;
_height = 99999;
if(count _this > 1) then{
	_height = _this select 1;
};
_patrol = true;
if(count _this > 2) then{
	_patrol = _this select 2;
};
_removeGre = true;
if(count _this > 3) then{
	_removeGre = _this select 3;
};
_building = nearestBuilding _leader;

if(isNull _building) exitWith{player groupChat "Building is null."};

_positions = [];
_i = 0;
while{((_building buildingPos _i) select 0) != 0} do{
	_pos = (_building buildingPos _i);
	if((_pos select 2) < _height) then{
		_positions = _positions + [_i];
	};
	_i = _i + 1;
};

{
	_group = createGroup (side _x);
	[_x] joinSilent _group;
	[] spawn {
		_unit allowDamage false;
		_unit setPos (_bld buildingPos (_positions select (floor(random (count _positions)))));
		sleep 1;
		_unit allowDamage true;
		if(_patrol) then{
			[_x, _building, _positions] spawn _fncPatrolBuilding;
		};
	};
	if(_removeGre) then{
		_x removeMagazines "HandGrenade_East";
	};
} forEach units _leader;