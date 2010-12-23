// Init_VBIED.sqf by Strato
//
// Usage: [(_ambCivVehicle)] execVM "scripts\Init_VBIED.sqf";
// _ambCivVehicle : Ambient Civilian Vehicle Module. (optional)
//
// This script should be called in init.sqf on both client and server.
//
// Configulation Values:
// VBIED_AmbientIEDProbably: Probably about civilian car has IED. (0...1) default: 0.1
// VBIED_InitAmbientDetonator: Custom initialization function of ambient IED detonator.
//                             The unit passed in _this select 0
// VBIED_AmbientDetonatorUnitTypes: Unit types of ambient IED detonators.
// VBIED_AmbientDetonatorSide: Side of ambient IED detonators.
// VBIED_AmbientIEDTargetSides: Side of ambient IED targets.
// VBIED_AmbientIEDAmmoType: Ammo type of IED. (default: ARTY_Sh_82_HE)
//                           If you want more exprosion, use ARTY_Sh_122_HE or Bo_FAB_250
// VBIED_Debug

_ambCivVehicle = objNull;
if(count _this > 0) then{
	_ambCivVehicle = _this select 0;
	if(isNil "VBIED_AmbientIEDProbably") then{
		VBIED_AmbientIEDProbably = 0.1;
	};
	if(isNil "VBIED_InitAmbientDetonator") then{
		VBIED_InitAmbientDetonator = {
			_unit = _this select 0;
			removeAllWeapons _unit;
			_unit addWeapon "Binocular";
			_r = floor(random 4);
			if(_r == 0) then{
				{_unit addMagazine "30Rnd_9x19_UZI"} forEach [1,2,3,4];
				_unit addWeapon "UZI_EP1";
			};
			if(_r == 1) then{
				{_unit addMagazine "20Rnd_B_765x17_Ball"} forEach [1,2,3,4,5,6];
				_unit addWeapon "Sa61_EP1";
			};
			if(_r == 2) then{
				{_unit addMagazine "6Rnd_45ACP"} forEach [1,2,3,4];
				_unit addWeapon "revolver_EP1";
			};
			_unit setBehaviour "SAFE";
			_unit setCombatMode "GREEN";
			[_unit] execVM "skill.sqf";
		};
	};
	if(isNil "VBIED_AmbientDetonatorUnitTypes") then{
		VBIED_AmbientDetonatorUnitTypes = ["TK_GUE_Soldier_EP1", "TK_GUE_Soldier_2_EP1", "TK_GUE_Soldier_3_EP1", "TK_GUE_Soldier_4_EP1"];
	};
	if(isNil "VBIED_AmbientDetonatorSide") then{
		VBIED_AmbientDetonatorSide = Resistance;
	};
	if(isNil "VBIED_AmbientIEDTargetSides") then{
		VBIED_AmbientIEDTargetSides = [West];
	};
	if(isNil "VBIED_AmbientIEDAmmoType") then{
		VBIED_AmbientIEDAmmoType = "ARTY_Sh_82_HE";
	};
	if(isNil "VBIED_Debug") then{
		VBIED_Debug = false;
	};
};

[] call compile preprocessFile "scripts\VBIED\Functions.sqf";

if(not isNull _ambCivVehicle) then{
	[] spawn VBIED_AddPlayerAction;
	if(isServer) then{
		_ambCivVehicle setVariable ["vehicleInit", {
			_r = random 1;
			if(_r < VBIED_AmbientIEDProbably) then{
				sleep 5;
				_this setVariable ["VBIED_BombActivated", true, true];
				_unit = [_this] call VBIED_CreateDetonator;
				[_unit, 100, false, true, false] spawn VBIED_MoveToNearestBuilding;
				[_this, _unit] execVM "scripts\VBIED.sqf";
			};
		}]; 
	};
};

VBIED_Init = true;