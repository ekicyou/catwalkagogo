_heli = _this select 0;

if(isServer) then{
	_heli land "GET OUT";
	waitUntil{((getPos _heli) select 2) < 5};
	{
		unassignVehicle _x;
	} forEach (assignedCargo _heli);
	waitUntil{(count (crew _heli)) <= 4};
	TransportDone = true;
	publicVariable "TransportDone";
};