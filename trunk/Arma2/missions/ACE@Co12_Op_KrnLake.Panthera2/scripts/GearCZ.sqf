private ["_unit", "_type"];

_unit = _this select 0;
_type = _this select 1;

removeAllWeapons _unit;
removeBackpack _unit;
[_unit, "ALL"] call ACE_fnc_RemoveGear;

_unit addMagazine "ACE_Bandage";
_unit addMagazine "ACE_Bandage";
_unit addMagazine "ACE_Morphine";
_unit addMagazine "ACE_Morphine";
_unit addWeapon "ACE_Earplugs";
_unit addWeapon "NVGoggles";
_unit addWeapon "ACE_GlassesBalaklavaGray";
_unit addWeapon "ACE_GlassesLHD_glasses";
_unit addWeapon "ACE_GlassesGasMask_US";
switch(_type) do{
	case "SL":{
		{_unit addMagazine "ACE_30Rnd_556x45_T_Stanag"} forEach [1,2,3,4,5,6];
		_unit addWeapon "ACE_M4";
		{_unit addMagazine "HandGrenade_West"} forEach [1,2]; 
		{_unit addMagazine "ACE_M34"} forEach [1,2]; 
		_unit addWeapon "ACE_Rangefinder_OD";
		_unit addMagazine "ACE_Battery_Rangefinder";
		_unit addWeapon "ACE_Kestrel4500";
		_unit addWeapon "ACE_PRC119";
		[_unit, "HandGrenade_West", 4] call ACE_fnc_PackMagazine;
		[_unit, "ACE_M7A3", 2] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShellBlue", 1] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShellGreen", 1] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShellOrange", 1] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShellPurple", 1] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShellRed", 1] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShellYellow", 1] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Epinephrine", 2] call ACE_fnc_PackMagazine;
		{_unit addMagazine "17Rnd_9x19_glock17"} forEach [1,2,3,4]; 
		_unit addWeapon "ACE_Glock17"
	};
	case "MG":{
		{_unit addMagazine "100Rnd_762x51_M240"} forEach [1,2,3,4];
		_unit addWeapon "M60A4_EP1";
		{_unit addMagazine "HandGrenade_West"} forEach [1,2,3,4];
		{_unit addMagazine "ACE_M34"} forEach [1,2];
		_unit addWeapon "ACE_Rucksack_MOLLE_Wood";
		[_unit, "HandGrenade_West", 4] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Epinephrine", 2] call ACE_fnc_PackMagazine;
		{_unit addMagazine "17Rnd_9x19_glock17"} forEach [1,2,3,4]; 
		_unit addWeapon "ACE_Glock17";
	};
	case "GL":{
		{_unit addMagazine "30Rnd_556x45_Stanag"} forEach [1,2,3,4,5,6];
		{_unit addMagazine "1Rnd_HE_M203"} forEach [1,2,3,4,5,6,7,8];
		_unit addWeapon "M16A2GL";
		{_unit addMagazine "HandGrenade_West"} forEach [1,2,3,4];
		{_unit addMagazine "ACE_M34"} forEach [1,2];
		_unit addWeapon "ACE_Rucksack_MOLLE_Wood";
		[_unit, "1Rnd_HE_M203", 12] call ACE_fnc_PackMagazine;
		[_unit, "ACE_1Rnd_CS_M203", 4] call ACE_fnc_PackMagazine;
		[_unit, "HandGrenade_West", 4] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Epinephrine", 2] call ACE_fnc_PackMagazine;
	};
	case "RM":{
		{_unit addMagazine "30Rnd_556x45_Stanag"} forEach [1,2,3,4,5,6];
		_unit addWeapon "M16A2";
		{_unit addMagazine "HandGrenade_West"} forEach [1,2,3,4];
		{_unit addMagazine "ACE_M34"} forEach [1,2];
		_unit addWeapon "ACE_Rucksack_MOLLE_Wood";
		[_unit, "HandGrenade_West", 4] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Epinephrine", 2] call ACE_fnc_PackMagazine;
		{_unit addMagazine "17Rnd_9x19_glock17"} forEach [1,2,3,4]; 
		_unit addWeapon "ACE_Glock17";
		_unit addWeapon "ACE_Rangefinder_OD";
		_unit addMagazine "ACE_Battery_Rangefinder";
		_unit addWeapon "ACE_Kestrel4500";
	};
	case "DM":{
		{_unit addMagazine "30Rnd_556x45_Stanag"} forEach [1,2,3,4,5,6];
		_unit addWeapon "ACE_m16a2_scope";
		{_unit addMagazine "HandGrenade_West"} forEach [1,2];
		{_unit addMagazine "ACE_M34"} forEach [1,2];
		_unit addWeapon "ACE_Rucksack_MOLLE_Wood";
		[_unit, "HandGrenade_West", 4] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Epinephrine", 2] call ACE_fnc_PackMagazine;
		{_unit addMagazine "17Rnd_9x19_glock17"} forEach [1,2,3,4]; 
		_unit addWeapon "ACE_Glock17";
	};
	case "MED":{
		{_unit addMagazine "30Rnd_556x45_Stanag"} forEach [1,2,3,4,5,6];
		_unit addWeapon "ACE_M4";
		{_unit addMagazine "HandGrenade_West"} forEach [1,2,3,4];
		{_unit addMagazine "ACE_M34"} forEach [1,2];
		{_unit addMagazine "ACE_Epinephrine"} forEach [1,2];
		_unit addWeapon "ACE_VTAC_RUSH72_FT_MEDIC";
		[_unit, "HandGrenade_West", 4] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Bandage", 20] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Morphine", 20] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Epinephrine", 20] call ACE_fnc_PackMagazine;
		{_unit addMagazine "17Rnd_9x19_glock17"} forEach [1,2,3,4]; 
		_unit addWeapon "ACE_Glock17"
	};
};

_unit selectWeapon (primaryWeapon _unit);