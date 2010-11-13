private ["_unit", "_identity"];

_unit = _this select 0;
_identity = "ACE_GlassesGasMask_RU";

if(isNil {_unit getVariable "ACE_Identity"}) then{
	//_unit addWeapon "ACE_GlassesGasMask_RU";
	["ace_sys_goggles_setident2", [_unit, _identity]] call CBA_fnc_globalEvent;
	_unit setVariable ["ACE_Identity", _identity, false];
};