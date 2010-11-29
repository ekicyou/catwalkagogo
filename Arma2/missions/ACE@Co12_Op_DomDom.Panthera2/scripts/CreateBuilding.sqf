if(not isServer) exitWith{};

_obj = _this select 0;
_class = _this select 1;
_height = 0;
if(count _this > 2) then{
	_height = _this select 2;
};
_leader = objNull;
if(count _this > 3) then{
	_leader = _this select 3;
};


_fncSetPitchBank = compile preprocessFile "scripts\SetPitchBank.sqf";

_pos = getPos _obj;
_pos set [2, _height];
_bld = _class createVehicle _pos;
_bld setPos _pos;
_bld setDir (getDir _obj);
[_bld, 0, 0] call _fncSetPitchBank;

if(not isNull _leader) then{
	[_leader, 999999, true, true, _bld] execVM "scripts\PatrolBuilding.sqf";
};