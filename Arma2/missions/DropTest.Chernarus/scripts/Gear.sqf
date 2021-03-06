if(not isServer) then{
	waitUntil {!isNull player};
};

_unit = _this select 0;
_type = _this select 1;

if(not local _unit) exitWith{};
waitUntil {!isNull _unit};

removeAllWeapons _unit;
removeBackpack _unit;
removeAllItems _unit;

_unit addWeapon "ItemMap";
_unit addWeapon "ItemRadio";
_unit addWeapon "ItemCompass";
_unit addWeapon "ItemWatch";

switch(_type)do{
	case "SL":{
		{_unit addMagazine "8Rnd_9x18_Makarov"} forEach [1,2,3,4,5,6];
		_unit addWeapon "Makarov";
		_unit addWeapon "ACE_ParachuteRoundPack";
		_unit addWeapon "ACE_Rucksack_RD99";
		_unit addWeapon "ItemGPS";
		[_unit, "ACE_AKS74P_Kobra", 1] call ACE_fnc_PackWeapon;
		[_unit, "Binocular", 1] call ACE_fnc_PackWeapon;
		[_unit, "ACE_GlassesGasMask_RU", 1] call ACE_fnc_PackWeapon;
		[_unit, "ACE_Earplugs", 1] call ACE_fnc_PackWeapon;
		[_unit, "NVGoggles", 1] call ACE_fnc_PackWeapon;
		[_unit, "HandGrenade_East", 2] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShell", 2] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShellRed", 1] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShellGreen", 1] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShellBlue", 1] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShellOrange", 1] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShellPurple", 1] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShellYellow", 1] call ACE_fnc_PackMagazine;
		[_unit, "30Rnd_545x39_AK", 8] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Bandage", 2] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Morphine", 1] call ACE_fnc_PackMagazine;
	};
	case "SL2":{
		_unit addWeapon "ACE_AK74M_Kobra";
		_unit addWeapon "ItemGPS";
		_unit addWeapon "Binocular";
		_unit addWeapon "ACE_GlassesGasMask_RU";
		_unit addWeapon "NVGoggles";
		_unit addWeapon "ACE_Earplugs";
		{_unit addMagazine "HandGrenade_East"} forEach [1,2];
		{_unit addMagazine "SmokeShell"} forEach [1,2];
		{_unit addMagazine "30Rnd_545x39_AK"} forEach [1,2,3,4,5,6,7,8];
		{_unit addMagazine "ACE_Bandage"} forEach [1,2];
		{_unit addMagazine "ACE_Morphine"} forEach [1];
	};
	case "TL":{
		{_unit addMagazine "8Rnd_9x18_Makarov"} forEach [1,2,3,4,5,6];
		_unit addWeapon "Makarov";
		_unit addWeapon "ACE_ParachuteRoundPack";
		_unit addWeapon "ACE_Rucksack_RD99";
		[_unit, "ACE_AKS74P_GL", 1] call ACE_fnc_PackWeapon;
		[_unit, "Binocular", 1] call ACE_fnc_PackWeapon;
		[_unit, "ACE_GlassesGasMask_RU", 1] call ACE_fnc_PackWeapon;
		[_unit, "ACE_Earplugs", 1] call ACE_fnc_PackWeapon;
		[_unit, "NVGoggles", 1] call ACE_fnc_PackWeapon;
		[_unit, "HandGrenade_East", 2] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShell", 2] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShellRed", 1] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShellGreen", 1] call ACE_fnc_PackMagazine;
		[_unit, "30Rnd_545x39_AK", 8] call ACE_fnc_PackMagazine;
		[_unit, "1Rnd_HE_GP25", 8] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Bandage", 2] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Morphine", 1] call ACE_fnc_PackMagazine;
	};
	case "GL":{
		{_unit addMagazine "8Rnd_9x18_Makarov"} forEach [1,2,3,4,5,6];
		_unit addWeapon "Makarov";
		_unit addWeapon "ACE_ParachuteRoundPack";
		_unit addWeapon "ACE_Rucksack_RD99";
		[_unit, "ACE_AKS74P_GL", 1] call ACE_fnc_PackWeapon;
		[_unit, "ACE_GlassesGasMask_RU", 1] call ACE_fnc_PackWeapon;
		[_unit, "ACE_Earplugs", 1] call ACE_fnc_PackWeapon;
		[_unit, "NVGoggles", 1] call ACE_fnc_PackWeapon;
		[_unit, "HandGrenade_East", 2] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShell", 2] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShellRed", 1] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShellGreen", 1] call ACE_fnc_PackMagazine;
		[_unit, "30Rnd_545x39_AK", 8] call ACE_fnc_PackMagazine;
		[_unit, "1Rnd_HE_GP25", 8] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Bandage", 2] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Morphine", 1] call ACE_fnc_PackMagazine;
	};
	case "GL2":{
		_unit addWeapon "ACE_AK74M_GL";
		_unit addWeapon "ACE_GlassesGasMask_RU";
		_unit addWeapon "NVGoggles";
		{_unit addMagazine "HandGrenade_East"} forEach [1,2];
		{_unit addMagazine "SmokeShell"} forEach [1,2];
		{_unit addMagazine "30Rnd_545x39_AK"} forEach [1,2,3,4,5,6,7,8];
		{_unit addMagazine "ACE_Bandage"} forEach [1];
		{_unit addMagazine "ACE_Morphine"} forEach [1];
		[_unit, "1Rnd_HE_GP25", 6] call ACE_fnc_PackWeapon;
	};
	case "DM":{
		{_unit addMagazine "8Rnd_9x18_Makarov"} forEach [1,2,3,4,5,6];
		_unit addWeapon "Makarov";
		_unit addWeapon "ACE_ParachuteRoundPack";
		_unit addWeapon "ACE_Rucksack_RD99";
		[_unit, "ACE_AK74M_PSO", 1] call ACE_fnc_PackWeapon;
		[_unit, "ACE_GlassesGasMask_RU", 1] call ACE_fnc_PackWeapon;
		[_unit, "ACE_Earplugs", 1] call ACE_fnc_PackWeapon;
		[_unit, "NVGoggles", 1] call ACE_fnc_PackWeapon;
		[_unit, "HandGrenade_East", 2] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShell", 2] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShellRed", 1] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShellGreen", 1] call ACE_fnc_PackMagazine;
		[_unit, "30Rnd_545x39_AK", 8] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Bandage", 2] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Morphine", 1] call ACE_fnc_PackMagazine;
	};
	case "DM2":{
		_unit addWeapon "ACE_AK74M_PSO";
		_unit addWeapon "ACE_GlassesGasMask_RU";
		_unit addWeapon "NVGoggles";
		{_unit addMagazine "HandGrenade_East"} forEach [1,2];
		{_unit addMagazine "SmokeShell"} forEach [1,2];
		{_unit addMagazine "30Rnd_545x39_AK"} forEach [1,2,3,4,5,6,7,8];
		{_unit addMagazine "ACE_Bandage"} forEach [1,2];
		{_unit addMagazine "ACE_Morphine"} forEach [1];
	};
	case "AR":{
		{_unit addMagazine "8Rnd_9x18_Makarov"} forEach [1,2,3,4,5,6];
		_unit addWeapon "Makarov";
		_unit addWeapon "ACE_ParachuteRoundPack";
		_unit addWeapon "ACE_Rucksack_RD99";
		[_unit, "ACE_RPK74M", 1] call ACE_fnc_PackWeapon;
		[_unit, "ACE_GlassesGasMask_RU", 1] call ACE_fnc_PackWeapon;
		[_unit, "ACE_Earplugs", 1] call ACE_fnc_PackWeapon;
		[_unit, "NVGoggles", 1] call ACE_fnc_PackWeapon;
		[_unit, "HandGrenade_East", 2] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShell", 2] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShellRed", 1] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShellGreen", 1] call ACE_fnc_PackMagazine;
		[_unit, "ACE_45Rnd_545x39_B_AK", 8] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Bandage", 2] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Morphine", 1] call ACE_fnc_PackMagazine;
	};
	case "AR2":{
		_unit addWeapon "ACE_RPK74M";
		_unit addWeapon "ACE_GlassesGasMask_RU";
		_unit addWeapon "NVGoggles";
		_unit addWeapon "ACE_Earplugs";
		{_unit addMagazine "HandGrenade_East"} forEach [1,2];
		{_unit addMagazine "SmokeShell"} forEach [1,2];
		{_unit addMagazine "ACE_45Rnd_545x39_B_AK"} forEach [1,2,3,4,5,6,7,8];
		{_unit addMagazine "ACE_Bandage"} forEach [1,2];
		{_unit addMagazine "ACE_Morphine"} forEach [1];
	};
	case "MED":{
		{_unit addMagazine "8Rnd_9x18_Makarov"} forEach [1,2,3,4,5,6];
		_unit addWeapon "Makarov";
		_unit addWeapon "ACE_ParachuteRoundPack";
		_unit addWeapon "ACE_Rucksack_RD99";
		[_unit, "ACE_AKS74P", 1] call ACE_fnc_PackWeapon;
		[_unit, "ACE_GlassesGasMask_RU", 1] call ACE_fnc_PackWeapon;
		[_unit, "ACE_Earplugs", 1] call ACE_fnc_PackWeapon;
		[_unit, "NVGoggles", 1] call ACE_fnc_PackWeapon;
		[_unit, "HandGrenade_East", 2] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShell", 2] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShellRed", 1] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShellGreen", 1] call ACE_fnc_PackMagazine;
		[_unit, "30Rnd_545x39_AK", 8] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Bandage", 20] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Morphine", 20] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Epinephrine", 20] call ACE_fnc_PackMagazine;
	};
	case "RM":{
		{_unit addMagazine "8Rnd_9x18_Makarov"} forEach [1,2,3,4,5,6];
		_unit addWeapon "Makarov";
		_unit addWeapon "ACE_ParachuteRoundPack";
		_unit addWeapon "ACE_Rucksack_RD99";
		[_unit, "ACE_AKS74P", 1] call ACE_fnc_PackWeapon;
		[_unit, "ACE_GlassesGasMask_RU", 1] call ACE_fnc_PackWeapon;
		[_unit, "ACE_Earplugs", 1] call ACE_fnc_PackWeapon;
		[_unit, "NVGoggles", 1] call ACE_fnc_PackWeapon;
		[_unit, "HandGrenade_East", 2] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShell", 2] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShellRed", 1] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShellGreen", 1] call ACE_fnc_PackMagazine;
		[_unit, "30Rnd_545x39_AK", 8] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Bandage", 2] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Morphine", 1] call ACE_fnc_PackMagazine;
	};
	case "RM2":{
		_unit addWeapon "ACE_AK74M";
		_unit addWeapon "ACE_GlassesGasMask_RU";
		_unit addWeapon "NVGoggles";
		_unit addWeapon "ACE_Earplugs";
		{_unit addMagazine "HandGrenade_East"} forEach [1,2];
		{_unit addMagazine "SmokeShell"} forEach [1,2];
		{_unit addMagazine "30Rnd_545x39_AK"} forEach [1,2,3,4,5,6,7,8];
		{_unit addMagazine "ACE_Bandage"} forEach [1,2];
		{_unit addMagazine "ACE_Morphine"} forEach [1];
	};
	case "CM":{
		_unit addWeapon "AKS_74_U";
		_unit addWeapon "Binocular";
		_unit addWeapon "ACE_GlassesGasMask_RU";
		_unit addWeapon "NVGoggles";
		_unit addWeapon "ACE_Earplugs";
		_unit addWeapon "ACE_P159_RD99";
		{_unit addMagazine "HandGrenade_East"} forEach [1,2];
		{_unit addMagazine "SmokeShell"} forEach [1,2];
		{_unit addMagazine "30Rnd_545x39_AK"} forEach [1,2,3,4,5,6,7,8];
		{_unit addMagazine "ACE_Bandage"} forEach [1,2];
		{_unit addMagazine "ACE_Morphine"} forEach [1];
	};
};
//_unit selectWeapon (primaryWeapon _unit);