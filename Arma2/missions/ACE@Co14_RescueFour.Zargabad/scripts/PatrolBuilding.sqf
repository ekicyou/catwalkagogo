if(not isServer) exitWith{};

if(isNil "PatrolBuilding2") then{
	PatrolBuilding2 = {
		_unit = _this select 0;
		_bld = _this select 1;
		_positions = _this select 2;
		
		_unit setPos (_bld buildingPos (_positions select (floor(random(count _positions)))));
		while{alive _unit} do{
			sleep (60 + (random 60));
			_unit doMove (_bld buildingPos (_positions select (floor(random(count _positions)))));
			waitUntil{(not alive _unit) or (unitReady _unit)};
		};
	};
};

_leader = _this select 0;
_positions = _this select 1;
_building = nearestBuilding _leader;
if(count _this > 2) then{
	_building = _this select 2;
};

{
	_group = createGroup (side _x);
	[_x] joinSilent _group;
	[_x, _building, _positions] spawn PatrolBuilding2;
} forEach units _leader;