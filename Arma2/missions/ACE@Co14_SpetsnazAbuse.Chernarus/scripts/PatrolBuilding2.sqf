// [0,1,3,4,5,6,9,10,11,18,19,23,27,28,29,32,33,34,35,36]
if(not isServer) exitWith{};

_fncPatrolBuilding = {
	private ["_unit", "_bld", "_positions"];
	
	_unit = _this select 0;
	_bld = _this select 1;
	_positions = _this select 2;
	
	_unit allowDamage false;
	_unit setPos (_bld buildingPos (_positions select (floor(random (count _positions)))));
	sleep 1;
	_unit allowDamage true;
	while{alive _unit} do{
		sleep (60 + (random 60));
		_unit doMove (_bld buildingPos (_positions select (floor(random (count _positions)))));
		waitUntil{(not alive _unit) or (unitReady _unit)};
	};
};

_leader = _this select 0;
_positions = _this select 1;
_building = nearestBuilding _leader;

{
	_group = createGroup (side _x);
	[_x] joinSilent _group;
	[_x, _building, _positions] spawn _fncPatrolBuilding;
} forEach units _leader;