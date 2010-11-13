if(not isServer) exitWith{};

_unit = _this select 0;
_pos = _this select 1;
_building = nearestBuilding _unit;

if(isNull _building) exitWith{player groupChat "Building is null."};

_group = createGroup (side _unit);
[_unit] joinSilent _group;
[_unit] spawn fncSquat;
_unit allowDamage false;
_unit setPos (_building buildingPos _pos);
sleep 1;
_unit allowDamage true;