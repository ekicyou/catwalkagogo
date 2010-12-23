_unit = _this select 0;
_type = _this select 1;

removeAllWeapons _unit;
removeBackpack _unit;
_unit addMagazine "ACE_Bandage";
_unit addMagazine "ACE_Bandage";
switch(_type)do{
	case "SL":{
		{_unit addMagazine "20Rnd_762x51_FNFAL"} forEach [1,2,3,4,5,6,7,8];
		_unit addWeapon "FN_FAL";
		{_unit addMagazine "HandGrenade_East"} forEach [1,2];
		_unit addMagazine "SmokeShell";
		_unit addMagazine "SmokeShell";
		_unit addWeapon "ACE_P159_RD99";
		_unit addWeapon "Binocular";
		{_unit addMagazine "1Rnd_HE_M203";} forEach [1,2,3,4,5,6];
		[_unit, "M79_EP1"] call ACE_fnc_PutWeaponOnBack;
		[_unit, "1Rnd_Smoke_M203", 1] call ACE_fnc_PackMagazine;
		[_unit, "1Rnd_SmokeGreen_M203", 1] call ACE_fnc_PackMagazine;
		[_unit, "1Rnd_SmokeYellow_M203", 1] call ACE_fnc_PackMagazine;
		[_unit, "1Rnd_SmokeRed_M203", 1] call ACE_fnc_PackMagazine;
		[_unit, "1Rnd_HE_M203", 7] call ACE_fnc_PackMagazine;
	};
	case "MG":{
		{_unit addMagazine "100Rnd_762x54_PK"} forEach [1,2,3,4];
		_unit addWeapon "Pecheneg";
		{_unit addMagazine "SmokeShell"} forEach [1,2];
		{_unit addMagazine "HandGrenade_East"} forEach [1,2,3,4];
		_unit addWeapon "ACE_Rucksack_RD90";
	};
	case "AMG":{
		{_unit addMagazine "20Rnd_762x51_FNFAL"} forEach [1,2,3,4,5,6,7,8];
		_unit addWeapon "FN_FAL";
		{_unit addMagazine "SmokeShell"} forEach [1,2];
		{_unit addMagazine "HandGrenade_East"} forEach [1,2];
		_unit addWeapon "ACE_Rucksack_RD90";
		[_unit, "100Rnd_762x54_PK", 4] call ACE_fnc_PackMagazine;
	};
	case "SNP":{
		{_unit addMagazine "10Rnd_762x54_SVD"} forEach [1,2,3,4,5,6,7,8];
		_unit addWeapon "SVD";
		{_unit addMagazine "SmokeShell"} forEach [1,2];
		{_unit addMagazine "HandGrenade_East"} forEach [1,2];
		{_unit addMagazine "8Rnd_9x18_Makarov"} forEach [1,2,3,4];
		_unit addWeapon "Makarov";
		_unit addWeapon "ACE_Rucksack_RD90";
		[_unit, "20Rnd_762x51_FNFAL", 12] call ACE_fnc_PackMagazine;
		[_unit, "10Rnd_762x54_SVD", 4] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShell", 4] call ACE_fnc_PackMagazine;
		[_unit, "HandGrenade_East", 4] call ACE_fnc_PackMagazine;
	};
	case "MED":{
		{_unit addMagazine "20Rnd_762x51_FNFAL"} forEach [1,2,3,4,5,6];
		_unit addWeapon "FN_FAL";
		{_unit addMagazine "HandGrenade_East"} forEach [1,2];
		{_unit addMagazine "SmokeShell"} forEach [1,2];
		_unit addWeapon "ACE_Rucksack_EAST_Medic";
		_unit addMagazine "ACE_Morphine";
		_unit addMagazine "ACE_Epinephrine";
		[_unit, "ACE_Morphine", 20] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Bandage", 20] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Epinephrine", 20] call ACE_fnc_PackMagazine;
		[_unit, "20Rnd_762x51_FNFAL", 12] call ACE_fnc_PackMagazine;
		[_unit, "10Rnd_762x54_SVD", 4] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShell", 4] call ACE_fnc_PackMagazine;
		[_unit, "HandGrenade_East", 4] call ACE_fnc_PackMagazine;
	};
	case "AR":{
		{_unit addMagazine "75Rnd_545x39_RPK"} forEach [1,2,3,4,5,6];
		_unit addWeapon "ACE_RPK74M_1P29";
		{_unit addMagazine "SmokeShell"} forEach [1,2];
		{_unit addMagazine "HandGrenade_East"} forEach [1,2,3,4];
		_unit addWeapon "ACE_Rucksack_RD90";
		_unit addMagazine "ACE_Morphine";
		_unit addMagazine "ACE_Epinephrine";
		[_unit, "75Rnd_545x39_RPK", 6] call ACE_fnc_PackMagazine;
	};
	case "AT":{
		{_unit addMagazine "20Rnd_762x51_FNFAL"} forEach [1,2,3,4,5,6,7,8];
		_unit addWeapon "FN_FAL";
		{_unit addMagazine "ACE_RPG29_PG29"} forEach [1,2];
		_unit addWeapon "ACE_RPG29";
	};
	case "AAT":{
		{_unit addMagazine "20Rnd_762x51_FNFAL"} forEach [1,2,3,4,5,6,7,8];
		_unit addWeapon "FN_FAL";
		{_unit addMagazine "SmokeShell"} forEach [1,2];
		{_unit addMagazine "HandGrenade_East"} forEach [1,2];
		_unit addWeapon "ACE_Rucksack_RD90";
		[_unit, "ACE_RPG29_PG29", 2] call ACE_fnc_PackMagazine;
	};
	case "ENGL":{
		{_unit addMagazine "20Rnd_762x51_FNFAL"} forEach [1,2,3,4,5,6];
		_unit addWeapon "FN_FAL";
		{_unit addMagazine "SmokeShell"} forEach [1,2];
		_unit addWeapon "Binocular";
		_unit addWeapon "ACE_P159_RD99";
		_unit addMagazine "PipeBomb";
		_unit addMagazine "PipeBomb";
		_unit addMagazine "MineE";
		_unit addMagazine "MineE";
	};
	case "ENG":{
		{_unit addMagazine "20Rnd_762x51_FNFAL"} forEach [1,2,3,4,5,6];
		_unit addWeapon "FN_FAL";
		{_unit addMagazine "SmokeShell"} forEach [1,2];
		_unit addWeapon "ACE_Rucksack_RD90";
		[_unit, "PipeBomb", 2] call ACE_fnc_PackMagazine;
		[_unit, "MineE", 2] call ACE_fnc_PackMagazine;
	};
};
_unit selectWeapon (primaryWeapon _unit);