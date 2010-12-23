_heli = _this select 0;

if(isServer) then{
	_heli land "GET OUT";
	waitUntil{((getPos _heli) select 2) < 5};
	{
		unassignVehicle _x;
	} forEach (assignedCargo _heli);
	waitUntil{(count (crew _heli)) <= 4};
	waitUntil{(Reinf_1 distance _heli) > 1000};
	{[_x, "TownS", "noslow"] execVM "scripts\upsmon.sqf";} forEach [Reinf_1, Reinf_2, Reinf_3, Reinf_4, Reinf_5, Reinf_6];
};