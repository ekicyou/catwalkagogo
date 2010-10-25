_unit = _this select 0;
_type = _this select 1;

removeAllWeapons _unit;
removeBackpack _unit;
_unit addWeapon "NVGoggles";
_unit addMagazine "ACE_Bandage";
_unit addMagazine "ACE_Bandage";
_unit addMagazine "ACE_Morphine";
_unit addMagazine "ACE_Epinephrine";
switch(_type) do{
	case "SL":{
		{_unit addMagazine "20Rnd_762x51_B_SCAR";} forEach [1,2,3,4,5,6];
		{_unit addMagazine "HandGrenade_West";} forEach [1,2];
		{_unit addMagazine "SmokeShell";} forEach [1,2];
		_unit addMagazine "ACE_Battery_Rangefinder";
		{_unit addMagazine "1Rnd_HE_M203";} forEach [1,2];
		_unit addMagazine "1Rnd_SmokeGreen_M203";
		_unit addMagazine "1Rnd_SmokeRed_M203";
		_unit addWeapon "SCAR_H_STD_EGLM_Spect";
		{_unit addMagazine "17Rnd_9x19_glock17"} forEach [1,2,3,4];
		_unit addWeapon "glock17_EP1";
		_unit addWeapon "Binocular_Vector";
		
		_unit addWeapon "ACE_FAST_PackEDC";
		[_unit, "20Rnd_762x51_B_SCAR", 6] call ACE_fnc_PackMagazine;
		[_unit, "HandGrenade_West", 2] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShell", 2] call ACE_fnc_PackMagazine;
		[_unit, "1Rnd_HE_M203", 8] call ACE_fnc_PackMagazine;
		[_unit, "1Rnd_SmokeGreen_M203", 1] call ACE_fnc_PackMagazine;
		[_unit, "1Rnd_SmokeRed_M203", 1] call ACE_fnc_PackMagazine;
	};
	case "TL":{
		{_unit addMagazine "20Rnd_762x51_B_SCAR";} forEach [1,2,3,4,5,6];
		{_unit addMagazine "HandGrenade_West";} forEach [1,2];
		{_unit addMagazine "SmokeShell";} forEach [1,2];
		_unit addMagazine "ACE_Battery_Rangefinder";
		{_unit addMagazine "1Rnd_HE_M203";} forEach [1,2];
		_unit addMagazine "1Rnd_SmokeGreen_M203";
		_unit addMagazine "1Rnd_SmokeRed_M203";
		_unit addWeapon "SCAR_H_STD_EGLM_Spect";
		{_unit addMagazine "17Rnd_9x19_glock17"} forEach [1,2,3,4];
		_unit addWeapon "glock17_EP1";
		_unit addWeapon "Binocular_Vector";
		
		_unit addWeapon "ACE_FAST_PackEDC";
		[_unit, "20Rnd_762x51_B_SCAR", 6] call ACE_fnc_PackMagazine;
		[_unit, "HandGrenade_West", 2] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShell", 2] call ACE_fnc_PackMagazine;
		[_unit, "1Rnd_HE_M203", 8] call ACE_fnc_PackMagazine;
		[_unit, "1Rnd_SmokeGreen_M203", 1] call ACE_fnc_PackMagazine;
		[_unit, "1Rnd_SmokeRed_M203", 1] call ACE_fnc_PackMagazine;
	};
	case "MG":{
		{_unit addMagazine "100Rnd_762x51_M240";} forEach [1,2,3,4];
		{_unit addMagazine "HandGrenade_West";} forEach [1,2];
		{_unit addMagazine "SmokeShell";} forEach [1,2];
		_unit addWeapon "Mk_48_DES_EP1";
		_unit addWeapon "Sa61_EP1";
	};
	case "Med":{
		{_unit addMagazine "20Rnd_762x51_B_SCAR";} forEach [1,2,3,4,5,6];
		{_unit addMagazine "SmokeShell";} forEach [1,2];
		_unit addWeapon "SCAR_H_CQC_CCO";
		{_unit addMagazine "17Rnd_9x19_glock17"} forEach [1,2,3,4];
		_unit addWeapon "glock17_EP1";
		_unit addWeapon "ACE_Rucksack_MOLLE_ACU_Medic";
		[_unit, "20Rnd_762x51_B_SCAR", 6] call ACE_fnc_PackMagazine;
		[_unit, "HandGrenade_West", 2] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShell", 2] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Bandage", 20] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Morphine", 20] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Epinephrine", 20] call ACE_fnc_PackMagazine;
	};
	case "AT":{
		{_unit addMagazine "20Rnd_762x51_B_SCAR";} forEach [1,2,3,4,5,6];
		_unit addWeapon "SCAR_H_CQC_CCO";
		_unit addMagazine "MAAWS_HEDP";
		_unit addMagazine "MAAWS_HEDP";
		_unit addWeapon "MAAWS";
		{_unit addMagazine "30Rnd_9x19_UZI"} forEach [1,2,3,4];
		_unit addWeapon "UZI_EP1";
	};
	case "AMG":{
		{_unit addMagazine "20Rnd_762x51_B_SCAR";} forEach [1,2,3,4,5,6];
		_unit addWeapon "SCAR_H_CQC_CCO";
		{_unit addMagazine "HandGrenade_West";} forEach [1,2];
		{_unit addMagazine "SmokeShell";} forEach [1,2];
		{_unit addMagazine "30Rnd_9x19_UZI"} forEach [1,2,3,4];
		_unit addWeapon "UZI_EP1";
		
		_unit addWeapon "ACE_FAST_PackEDC";
		[_unit, "20Rnd_762x51_B_SCAR", 6] call ACE_fnc_PackMagazine;
		[_unit, "100Rnd_762x51_M240", 3] call ACE_fnc_PackMagazine;
		[_unit, "HandGrenade_West", 2] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShell", 2] call ACE_fnc_PackMagazine;
	};
	case "SNP":{
		{_unit addMagazine "20Rnd_762x51_B_SCAR";} forEach [1,2,3,4,5,6];
		_unit addWeapon "SCAR_H_LNG_Sniper";
		_unit addMagazine "ACE_Battery_Rangefinder";
		{_unit addMagazine "HandGrenade_West";} forEach [1];
		{_unit addMagazine "SmokeShell";} forEach [1];
		{_unit addMagazine "20Rnd_762x51_B_SCAR"} forEach [1,2,3,4,5,6];
		_unit addWeapon "Sa61_EP1";
		_unit addWeapon "Binocular_Vector";

		_unit addWeapon "ACE_FAST_PackEDC";
		[_unit, "20Rnd_762x51_B_SCAR", 6] call ACE_fnc_PackMagazine;
		[_unit, "HandGrenade_West", 2] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShell", 2] call ACE_fnc_PackMagazine;
	};
};

_unit selectWeapon (primaryWeapon _unit);