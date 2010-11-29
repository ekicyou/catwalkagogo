if(not isServer) exitWith{};

sleep 1;

if(isNil "fncPatrolBuilding") then{
	fncGetBuildingPos = {
		private ["_bld", "_positions", "_nextPoses", "_pos"];
		_bld = _this select 0;
		_positions = _this select 1;
		_nextPoses = _positions - (_bld getVariable "PB_UsedPosisions");
		_pos = _nextPoses select (floor(random (count _nextPoses)));
		_bld setVariable ["PB_UsedPosisions", (_bld getVariable "PB_UsedPosisions") + [_pos]];
		//_unit sideChat format ["Added %1 %2", _pos, _bld getVariable "PB_UsedPosisions"];
		_pos
	};
	fncRemoveUsedBuildingPos = {
		private ["_bld", "_pos"];
		_bld = _this select 0;
		_pos = _this select 1;
		_bld setVariable ["PB_UsedPosisions", (_bld getVariable "PB_UsedPosisions") - [_pos]];
		//_unit sideChat format ["Removed %1 %2", _pos, _bld getVariable "PB_UsedPosisions"];
	};
	fncPatrolBuilding = {
		private ["_unit", "_bld", "_positions", "_currentPos"];
		
		_unit = _this select 0;
		_bld = _this select 1;
		_positions = _this select 2;
		_currentPos = _this select 3;
		
		while{alive _unit} do{
			_t = 30 + (random 30);
			_unit suppressFor _t;
			sleep _t;
			[_bld, _currentPos] call fncRemoveUsedBuildingPos;
			_currentPos = [_bld, _positions] call fncGetBuildingPos;
			_unit doMove (_bld buildingPos _currentPos);
			waitUntil{(not alive _unit) or (unitReady _unit)};
		};
		[_bld, _currentPos] call fncRemoveUsedBuildingPos;
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

{
	_x allowDamage false;
} forEach units _leader;

sleep 1;
_building = nearestBuilding _leader;
if(count _this > 4) then{
	_building = _this select 4;
};
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

_building setVariable ["PB_UsedPosisions", []];
{
	_group = createGroup (side _x);
	[_x] joinSilent _group;
	[_x, _building, _positions, _patrol] spawn {
		_unit = _this select 0;
		_building = _this select 1;
		_positions = _this select 2;
		_patrol = _this select 3;
		
		_pos = [_building, _positions] call fncGetBuildingPos;
		_unit setPos (_building buildingPos _pos);
		sleep 1;
		_unit allowDamage true;
		if(_patrol) then{
			[_unit, _building, _positions, _pos] spawn fncPatrolBuilding;
		};
	};
	if(_removeGre) then{
		_x removeMagazines "HandGrenade_East";
		_x removeMagazines "HandGrenade_West";
	};
} forEach units _leader;