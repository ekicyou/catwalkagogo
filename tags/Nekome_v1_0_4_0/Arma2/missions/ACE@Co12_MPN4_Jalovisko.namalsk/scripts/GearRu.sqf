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
_unit addWeapon "ACE_GlassesGasMask_RU";
switch(_type) do{
	case "SL":{
		{_unit addMagazine "ACE_30Rnd_545x39_T_AK"} forEach [1,2,3,4,5,6];
		_unit addWeapon "ACE_AKS74P_GL_Kobra";
		{_unit addMagazine "HandGrenade_East"} forEach [1,2,3,4]; 
		{_unit addMagazine "ACE_RDGM"} forEach [1,2]; 
		_unit addWeapon "Binocular";
		_unit addWeapon "ACE_PRC119";
		[_unit, "ACE_WireCutter"] call ACE_fnc_PutWeaponOnBack;
		[_unit, "ACE_30Rnd_545x39_T_AK", 6] call ACE_fnc_PackMagazine;
		[_unit, "HandGrenade_East", 4] call ACE_fnc_PackMagazine;
		[_unit, "ACE_RDGM", 2] call ACE_fnc_PackMagazine;
		[_unit, "1Rnd_HE_GP25", 6] call ACE_fnc_PackMagazine;
		[_unit, "1Rnd_SMOKE_GP25", 2] call ACE_fnc_PackMagazine;
		[_unit, "1Rnd_SmokeGreen_GP25", 2] call ACE_fnc_PackMagazine;
		[_unit, "1Rnd_SmokeRed_GP25", 2] call ACE_fnc_PackMagazine;
		[_unit, "1Rnd_SmokeYellow_GP25", 2] call ACE_fnc_PackMagazine;
		[_unit, "1Rnd_SmokeYellow_GP25", 2] call ACE_fnc_PackMagazine;
		[_unit, "ACE_1Rnd_CS_GP25", 2] call ACE_fnc_PackMagazine;
		[_unit, "ACE_C4_M", 2] call ACE_fnc_PackMagazine;
	};
	case "MG":{
		{_unit addMagazine "100Rnd_762x54_PK"} forEach [1,2,3,4];
		_unit addWeapon "Pecheneg";
		{_unit addMagazine "HandGrenade_East"} forEach [1,2,3,4];
		{_unit addMagazine "ACE_RDGM"} forEach [1,2];
		_unit addWeapon "ACE_VTAC_RUSH72";
		[_unit, "HandGrenade_East", 4] call ACE_fnc_PackMagazine;
		[_unit, "ACE_RDGM", 2] call ACE_fnc_PackMagazine;
		[_unit, "ACE_RG60A", 2] call ACE_fnc_PackMagazine;
		{_unit addMagazine "ACE_20Rnd_9x18_APS"} forEach [1,2,3,4];
		_unit addWeapon "ACE_APS";
	};
	case "AR":{
		{_unit addMagazine "ACE_45Rnd_545x39_B_AK"} forEach [1,2,3,4,5,6];
		_unit addWeapon "ACE_RPK74M";
		{_unit addMagazine "HandGrenade_East"} forEach [1,2,3,4];
		{_unit addMagazine "ACE_RDGM"} forEach [1,2];
		_unit addWeapon "ACE_VTAC_RUSH72";
		[_unit, "ACE_45Rnd_545x39_B_AK", 6] call ACE_fnc_PackMagazine;
		[_unit, "HandGrenade_East", 4] call ACE_fnc_PackMagazine;
		[_unit, "ACE_RDGM", 2] call ACE_fnc_PackMagazine;
		[_unit, "ACE_RG60A", 2] call ACE_fnc_PackMagazine;
	};
	case "GL":{
		_unit addMagazine "ACE_45Rnd_545x39_B_AK";
		{_unit addMagazine "1Rnd_HE_GP25"} forEach [1,2,3,4];
		_unit addWeapon "ACE_AKS74P_GL_Kobra";
		{_unit addMagazine "30Rnd_545x39_AK"} forEach [1,2,3,4,5];
		{_unit addMagazine "HandGrenade_East"} forEach [1,2,3,4];
		{_unit addMagazine "ACE_RDGM"} forEach [1,2];
		_unit addWeapon "ACE_VTAC_RUSH72";
		[_unit, "30Rnd_545x39_AK", 6] call ACE_fnc_PackMagazine;
		[_unit, "HandGrenade_East", 4] call ACE_fnc_PackMagazine;
		[_unit, "ACE_RDGM", 2] call ACE_fnc_PackMagazine;
		[_unit, "1Rnd_HE_GP25", 12] call ACE_fnc_PackMagazine;
		[_unit, "ACE_1Rnd_CS_GP25", 2] call ACE_fnc_PackMagazine;
	};
	case "AT":{
		_unit addMagazine "ACE_45Rnd_545x39_B_AK";
		_unit addWeapon "ACE_AKS74P_Kobra";
		{_unit addMagazine "30Rnd_545x39_AK"} forEach [1,2,3,4,5];
		{_unit addMagazine "ACE_PG7VL_PGO7"} forEach [1,2,3];
		_unit addWeapon "ACE_RPG7V_PGO7";
		[_unit, "ACE_VTAC_RUSH72"] call ACE_fnc_PutWeaponOnBack;
		[_unit, "30Rnd_545x39_AK", 6] call ACE_fnc_PackMagazine;
		[_unit, "HandGrenade_East", 4] call ACE_fnc_PackMagazine;
		[_unit, "ACE_RDGM", 2] call ACE_fnc_PackMagazine;
		[_unit, "ACE_RG60A", 2] call ACE_fnc_PackMagazine;
	};
	case "MAT":{
		_unit addMagazine "ACE_45Rnd_545x39_B_AK";
		_unit addWeapon "ACE_AKS74P_Kobra";
		{_unit addMagazine "30Rnd_545x39_AK"} forEach [1,2,3,4,5];
		{_unit addMagazine "HandGrenade_East"} forEach [1,2];
		{_unit addMagazine "ACE_RDGM"} forEach [1,2];
		{_unit addMagazine "ACE_RPG29_PG29"} forEach [1];
		_unit addWeapon "ACE_RPG29";
		[_unit, "ACE_VTAC_RUSH72"] call ACE_fnc_PutWeaponOnBack;
		[_unit, "30Rnd_545x39_AK", 6] call ACE_fnc_PackMagazine;
		[_unit, "ACE_RG60A", 2] call ACE_fnc_PackMagazine;
	};
	case "SNP":{
		{_unit addMagazine "10Rnd_762x54_SVD"} forEach [1,2,3,4,5,6,7,8];
		_unit addWeapon "SVD";
		{_unit addMagazine "ACE_20Rnd_9x18_APS"} forEach [1,2,3,4];
		_unit addWeapon "ACE_APS";
		{_unit addMagazine "HandGrenade_East"} forEach [1,2,3,4];
		{_unit addMagazine "ACE_RDGM"} forEach [1,2];
		_unit addWeapon "ACE_VTAC_RUSH72";
		[_unit, "30Rnd_545x39_AK", 6] call ACE_fnc_PackMagazine;
		[_unit, "HandGrenade_East", 4] call ACE_fnc_PackMagazine;
		[_unit, "ACE_RDGM", 2] call ACE_fnc_PackMagazine;
		[_unit, "ACE_RG60A", 2] call ACE_fnc_PackMagazine;
	};
	case "DM":{
		{_unit addMagazine "30Rnd_762x39_AK47"} forEach [1,2,3,4,5,6,7,8];
		_unit addWeapon "ACE_AK103_PSO";
		{_unit addMagazine "ACE_20Rnd_9x18_APS"} forEach [1,2,3,4];
		_unit addWeapon "ACE_APS";
		{_unit addMagazine "HandGrenade_East"} forEach [1,2,3,4];
		{_unit addMagazine "ACE_RDGM"} forEach [1,2];
		_unit addWeapon "ACE_VTAC_RUSH72";
		[_unit, "30Rnd_762x39_AK47", 6] call ACE_fnc_PackMagazine;
		[_unit, "HandGrenade_East", 4] call ACE_fnc_PackMagazine;
		[_unit, "ACE_RDGM", 2] call ACE_fnc_PackMagazine;
		[_unit, "ACE_RG60A", 2] call ACE_fnc_PackMagazine;
	};
	case "CM":{
		{_unit addMagazine "30Rnd_545x39_AK"} forEach [1,2,3,4];
		_unit addWeapon "AKS_74_U";
		{_unit addMagazine "ACE_20Rnd_9x18_APS"} forEach [1,2,3,4];
		_unit addWeapon "ACE_APS";
		_unit addWeapon "ACE_PRC119";
	};
	case "MED":{
		_unit addMagazine "ACE_45Rnd_545x39_B_AK";
		_unit addWeapon "ACE_AKS74P_Kobra";
		{_unit addMagazine "30Rnd_545x39_AK"} forEach [1,2,3,4,5];
		{_unit addMagazine "HandGrenade_East"} forEach [1,2,3,4];
		{_unit addMagazine "ACE_RDGM"} forEach [1,2];
		{_unit addMagazine "ACE_Epinephrine"} forEach [1,2];
		_unit addWeapon "ACE_VTAC_RUSH72_FT_MEDIC";
		[_unit, "30Rnd_545x39_AK", 6] call ACE_fnc_PackMagazine;
		[_unit, "HandGrenade_East", 4] call ACE_fnc_PackMagazine;
		[_unit, "ACE_RDGM", 2] call ACE_fnc_PackMagazine;
		[_unit, "ACE_RG60A", 2] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Bandage", 10] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Morphine", 10] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Epinephrine", 10] call ACE_fnc_PackMagazine;
	};
	case "MVD1":{
		{_unit addMagazine "20Rnd_9x39_SP5_VSS"} forEach [1,2,3,4,5,6,7,8];
		{_unit addMagazine "SmokeShell"} forEach [1,2];
		_unit addWeapon "ACE_Val_Kobra";
	};
	case "MVD2":{
		{_unit addMagazine "20Rnd_9x39_SP5_VSS"} forEach [1,2,3,4,5,6,7,8];
		{_unit addMagazine "SmokeShell"} forEach [1,2];
		_unit addWeapon "VSS_Vintorez";
	};
	case "MVD3":{
		{_unit addMagazine "ACE_20Rnd_9x39_S_OC14"} forEach [1,2,3,4,5,6,7,8];
		{_unit addMagazine "SmokeShell"} forEach [1,2];
		_unit addWeapon "ACE_oc14sd";
	};
};

_unit selectWeapon (primaryWeapon _unit);