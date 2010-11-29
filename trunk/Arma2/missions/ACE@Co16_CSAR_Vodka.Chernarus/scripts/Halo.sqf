_unit = _this select 1;
_vehicle = _this select 0;

if(not (_unit in (assignedCargo _vehicle))) exitWith{
	hint "Halo drop can do only cargo crew.";
};
if((getPos _vehicle select 2) < 200) exitWith{
	hint "Not enough altitude.";
};

_unit action ["eject", _vehicle];
unassignVehicle _unit;
[_unit] exec "ca\air2\halo\data\Scripts\HALO_getout.sqs"; 