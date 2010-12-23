_unit = _this select 0;
_type = _this select 1;

removeAllWeapons _unit;
removeBackpack _unit;

_unit addMagazine "ACE_Bandage";
_unit addMagazine "ACE_Bandage";
_unit addMagazine "ACE_Morphine";
_unit addMagazine "ACE_Morphine";
_unit addWeapon "ACE_Earplugs";
_unit addWeapon "NVGoggles";
_unit addWeapon "ACE_GlassesBalaklavaGray";
_unit addWeapon "ACE_GlassesLHD_glasses";
//_unit addWeapon "ACE_GlassesGasMask_RU";
switch(_type) do{
	case "SL":{
		{_unit addMagazine "ACE_30Rnd_762x39_T_SA58"} forEach [1,2,3,4,5,6];
		_unit addWeapon "Sa58V_RCO_EP1";
		{_unit addMagazine "HandGrenade_West"} forEach [1,2]; 
		{_unit addMagazine "SmokeShell"} forEach [1,2]; 
		_unit addMagazine "Laserbatteries";
		_unit addWeapon "Laserdesignator";
		_unit addWeapon "ACE_PRC119";
		_unit addWeapon "ACE_Map_Tools";
		[_unit, "ACE_30Rnd_762x39_T_SA58", 4] call ACE_fnc_PackMagazine;
		[_unit, "HandGrenade_West", 2] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShell", 2] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShellBlue", 2] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShellGreen", 2] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShellOrange", 2] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShellPurple", 2] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShellRed", 2] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShellYellow", 2] call ACE_fnc_PackMagazine;
		[_unit, "ACE_M2SLAM_M", 1] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Epinephrine", 2] call ACE_fnc_PackMagazine;
		{_unit addMagazine "ACE_15Rnd_9x19_P226"} forEach [1,2,3,4]; 
		_unit addWeapon "ACE_P226"
	};
	case "MG":{
		{_unit addMagazine "100Rnd_762x51_M240"} forEach [1,2,3,4];
		_unit addWeapon "M60A4_EP1";
		{_unit addMagazine "HandGrenade_West"} forEach [1,2,3,4];
		{_unit addMagazine "SmokeShell"} forEach [1,2];
		_unit addWeapon "ACE_CharliePack";
		[_unit, "HandGrenade_West", 4] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShell", 2] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Epinephrine", 2] call ACE_fnc_PackMagazine;
		{_unit addMagazine "ACE_15Rnd_9x19_P226"} forEach [1,2,3,4]; 
		_unit addWeapon "ACE_P226"
	};
	case "AMG":{
		{_unit addMagazine "30Rnd_762x39_SA58"} forEach [1,2,3,4,5,6];
		_unit addWeapon "Sa58V_CCO_EP1";
		{_unit addMagazine "HandGrenade_West"} forEach [1,2,3,4];
		{_unit addMagazine "SmokeShell"} forEach [1,2];
		_unit addWeapon "ACE_CharliePack";
		[_unit, "30Rnd_762x39_SA58", 6] call ACE_fnc_PackMagazine;
		[_unit, "HandGrenade_West", 4] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShell", 2] call ACE_fnc_PackMagazine;
		[_unit, "100Rnd_762x51_M240", 4] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Epinephrine", 2] call ACE_fnc_PackMagazine;
		{_unit addMagazine "ACE_15Rnd_9x19_P226"} forEach [1,2,3,4]; 
		_unit addWeapon "ACE_P226"
	};
	case "AT":{
		{_unit addMagazine "30Rnd_762x39_SA58"} forEach [1,2,3,4,5,6];
		_unit addWeapon "Sa58V_CCO_EP1";
		_unit addMagazine "MAAWS_HEDP";
		{_unit addMagazine "HandGrenade_West"} forEach [1]; 
		{_unit addMagazine "SmokeShell"} forEach [1]; 
		_unit addWeapon "ACE_CarlGustav_M3";
		[_unit, "ACE_CharliePack"] call ACE_fnc_PutWeaponOnBack;
		[_unit, "30Rnd_762x39_SA58", 4] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Epinephrine", 2] call ACE_fnc_PackMagazine;
		[_unit, "MAAWS_HEDP", 2] call ACE_fnc_PackMagazine;
		{_unit addMagazine "ACE_15Rnd_9x19_P226"} forEach [1,2,3,4]; 
		_unit addWeapon "ACE_P226"
	};
	case "DM":{
		{_unit addMagazine "10Rnd_762x54_SVD"} forEach [1,2,3,4,5,6,7,8];
		_unit addWeapon "SVD";
		{_unit addMagazine "20Rnd_B_765x17_Ball"} forEach [1,2,3,4];
		_unit addWeapon "Sa61_EP1";
		{_unit addMagazine "HandGrenade_West"} forEach [1];
		{_unit addMagazine "SmokeShell"} forEach [1];
		_unit addWeapon "ACE_CharliePack";
		_unit addMagazine "ACE_Battery_Rangefinder";
		_unit addWeapon "Binocular_Vector";
		_unit addWeapon "ACE_Kestrel4500";
		[_unit, "10Rnd_762x54_SVD", 6] call ACE_fnc_PackMagazine;
		[_unit, "HandGrenade_West", 4] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShell", 2] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Epinephrine", 2] call ACE_fnc_PackMagazine;
	};
	case "SNP":{
		{_unit addMagazine "5Rnd_762x51_M24"} forEach [1,2,3,4,5,6,7,8];
		_unit addWeapon "M24";
		{_unit addMagazine "20Rnd_B_765x17_Ball"} forEach [1,2,3,4];
		_unit addWeapon "Sa61_EP1";
		{_unit addMagazine "HandGrenade_West"} forEach [1];
		{_unit addMagazine "SmokeShell"} forEach [1];
		_unit addWeapon "ACE_CharliePack";
		_unit addMagazine "ACE_Battery_Rangefinder";
		_unit addWeapon "Binocular_Vector";
		_unit addWeapon "ACE_Kestrel4500";
		[_unit, "5Rnd_762x51_M24 ", 6] call ACE_fnc_PackMagazine;
		[_unit, "HandGrenade_West", 4] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShell", 2] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Epinephrine", 2] call ACE_fnc_PackMagazine;
	};
	case "MED":{
		{_unit addMagazine "30Rnd_762x39_SA58"} forEach [1,2,3,4,5,6];
		_unit addWeapon "Sa58V_CCO_EP1";
		{_unit addMagazine "HandGrenade_West"} forEach [1,2,3,4];
		{_unit addMagazine "SmokeShell"} forEach [1,2];
		_unit addWeapon "ACE_CharliePack";
		[_unit, "30Rnd_762x39_SA58", 6] call ACE_fnc_PackMagazine;
		[_unit, "HandGrenade_West", 4] call ACE_fnc_PackMagazine;
		[_unit, "SmokeShell", 2] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Bandage", 20] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Morphine", 20] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Epinephrine", 20] call ACE_fnc_PackMagazine;
		{_unit addMagazine "ACE_15Rnd_9x19_P226"} forEach [1,2,3,4]; 
		_unit addWeapon "ACE_P226"
	};
	case "Sasha":{
		{_unit addMagazine "ACE_5Rnd_762x51_T_M24"} forEach [1,2,3,4,5,6,7,8];
		_unit addWeapon "M24";
		{_unit addMagazine "ACE_20Rnd_9x18_APS"} forEach [1,2,3,4];
		_unit addWeapon "ACE_APS";
		{_unit addMagazine "HandGrenade_West"} forEach [1];
		{_unit addMagazine "SmokeShell"} forEach [1];
	};
	case "Pilot":{
		//{_unit addMagazine "20Rnd_B_765x17_Ball"} forEach [1,2,3,4,5,6];
		//_unit addWeapon "Sa61_EP1";
		removeAllWeapons _unit;
		{_unit addMagazine "ACE_20Rnd_9x18_APS"} forEach [1,2,3,4];
		_unit addWeapon "ACE_APS";
		_unit addWeapon "ItemRadio";
		//_unit addWeapon "ItemMap";
		_unit addWeapon "ItemWatch";
		_unit addWeapon "EvMoney";
		_unit addMagazine "ACE_Morphine";
		_unit addMagazine "ACE_Bandage";
		_unit addMagazine "SmokeShellRed";
		_unit addMagazine "SmokeShellGreen";
	};
	case "Pilot2":{
		{_unit addMagazine "20Rnd_B_765x17_Ball"} forEach [1,2,3,4];
		_unit addWeapon "Sa61_EP1";
		_unit addMagazine "SmokeShellRed";
		_unit addMagazine "SmokeShellGreen";
		_unit addWeapon "ACE_PRC119_ACU";
		[_unit, "ACE_Epinephrine", 2] call ACE_fnc_PackMagazine;
	};
};

_unit selectWeapon (primaryWeapon _unit);