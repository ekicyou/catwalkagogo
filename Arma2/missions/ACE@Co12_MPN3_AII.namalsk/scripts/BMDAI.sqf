if(not isServer) exitWith{};

_bmd = _this select 0;
waitUntil{not isNull (gunner _bmd)};
_gunner = gunner _bmd;

_gunner suppressFor 99999;

sleep 1;

{
	_x setBehaviour "CARELESS";
	_x spawn {
		waitUntil{(not alive _this) or (vehicle _this == _this)};
		_this setBehaviour "COMBAT";
	};
} forEach assignedCargo _bmd;

waitUntil{(not alive _gunner) or (not canMove _bmd) or (((_gunner findNearestEnemy _gunner) distance _bmd) < 100)};

waitUntil{speed _bmd < 8};
_group = createGroup (side _gunner);
{
	unassignVehicle _x;
	_x action ["EJECT", _bmd];
	sleep 0.1;
	[_x] joinSilent _group;
} forEach assignedCargo _bmd;