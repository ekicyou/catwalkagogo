if(not isServer) exitWith{};

_group = _this select 0;
_building = _this select 1;
_pos = _this select 2;

waitUntil{not isNil "VBIED_Init"};

{
	_newGrp = createGroup Resistance;
	[_x] joinSilent _newGrp;
	[_x, _building, _pos] spawn VBIED_PatrolBuilding;
} forEach units _group;