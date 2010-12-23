// VBIED.sqf by Strato
//
// Usage: [_car, _detonator(, _targetSides, _ammoType)] execVM "scripts\VBIED.sqf"
// _car : Vehicle that is planted IED.
// _detonator : Detonator of IED.
// _targetSides : Target side. (default: VBIED_AmbientIEDTargetSides)
// _ammoType : Bomb to be planted.

if(not isServer) exitWith{};
waitUntil{not isNil "VBIED_Init"};

_car = _this select 0;
_detonator = _this select 1;
_targetSides = VBIED_AmbientIEDTargetSides;
_ammoType = VBIED_AmbientIEDAmmoType;
if(count _this > 2) then{
	_targetSides = _this select 2;
};
if(count _this > 3) then{
	_ammoType = _this select 3;
};

_car setVariable ["VBIED_BombActivated", true, true];
_car setVariable ["VBIED_AmmoType", _ammoType];
_trgs = [];
{
	_trg = createTrigger ["EmptyDetector", getPos _car];
	_trg setTriggerArea [10, 10, 0, false];
	_trg setTriggerActivation [format ["%1", _x], "PRESENT", true];
	_trgs = _trgs + [_trg];
} forEach _targetSides;

{
	[_car, _detonator, _x] spawn VBIED_Detonator;
}forEach _trgs;
[_car, _trgs] spawn VBIED_Car;