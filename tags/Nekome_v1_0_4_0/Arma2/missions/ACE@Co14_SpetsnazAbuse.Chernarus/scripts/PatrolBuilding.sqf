if(not isServer) exitWith{};

_fncPatrolBuilding = {
	private ["_unit", "_bld", "_posCount"];
	
	_unit = _this select 0;
	_bld = _this select 1;
	_posCount = _this select 2;
	
	_unit allowDamage false;
	_unit setPos (_bld buildingPos (floor(random _posCount)));
	sleep 1;
	_unit allowDamage true;
	while{alive _unit} do{
		sleep (60 + (random 60));
		_unit doMove (_bld buildingPos (floor(random _posCount)));
		waitUntil{(not alive _unit) or (unitReady _unit)};
	};
};

_leader = _this select 0;
_building = nearestBuilding _leader;

if(isNull _building) exitWith{player groupChat "Building is null."};

_posCount = 0;
while{((_building buildingPos _posCount) select 0) != 0} do{
	_posCount = _posCount + 1;
};

{
	_group = createGroup (side _x);
	[_x] joinSilent _group;
	[_x, _building, _posCount] spawn _fncPatrolBuilding;
} forEach units _leader;