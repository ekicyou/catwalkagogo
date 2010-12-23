_unit = _this select 0;
_type = _this select 1;

removeAllWeapons _unit;
_unit addWeapon "ACE_GlassesGasMask_RU";
_unit addWeapon "ACE_GlassesBalaklava";
_unit addWeapon "ACE_Earplugs";
_unit addWeapon "NVGoggles";
{_unit addMagazine "ACE_Bandage"} forEach [1,2];
{_unit addMagazine "ACE_Morphine"} forEach [1,2];
{_unit addMagazine "ACE_20Rnd_9x18_APSB"} forEach [1,2,3,4];
_unit addWeapon "ACE_APSB";
switch(_type) do{
	case "TL": {
		{_unit addMagazine "64Rnd_9x19_SD_Bizon"} forEach [1,2,3,4,5,6,7,8];
		{_unit addMagazine "SmokeShell"} forEach [1,2];
		{_unit addMagazine "ACE_RG60A"} forEach [1];
		_unit addWeapon "bizon_silenced";
		_unit addMagazine "ACE_Battery_Rangefinder";
		_unit addWeapon "Binocular_Vector";
		_unit addWeapon "ACE_Rucksack_RD99";
		[_unit, "ACE_WireCutter"] call ACE_fnc_PutWeaponOnBack;
		[_unit, "PipeBomb", 1] call ACE_fnc_PackMagazine;
	};
	case "OP1": {
		{_unit addMagazine "20Rnd_9x39_SP5_VSS"} forEach [1,2,3,4,5,6,7,8];
		{_unit addMagazine "SmokeShell"} forEach [1,2];
		{_unit addMagazine "ACE_RG60A"} forEach [1,2];
		_unit addWeapon "ACE_Val_Kobra";
		_unit addWeapon "ACE_Rucksack_RD99";
		[_unit, "ACE_RPG27"] call ACE_fnc_PutWeaponOnBack;
		[_unit, "PipeBomb", 1] call ACE_fnc_PackMagazine;
	};
	case "OP2": {
		{_unit addMagazine "ACE_20Rnd_9x39_S_OC14"} forEach [1,2,3,4,5,6,7,8];
		{_unit addMagazine "SmokeShell"} forEach [1,2];
		{_unit addMagazine "ACE_RG60A"} forEach [1,2];
		_unit addWeapon "ACE_oc14sd";
		_unit addWeapon "ACE_Rucksack_RD99";
		[_unit, "ACE_RPG27"] call ACE_fnc_PutWeaponOnBack;
		[_unit, "PipeBomb", 1] call ACE_fnc_PackMagazine;
	};
	case "OP3": {
		{_unit addMagazine "64Rnd_9x19_SD_Bizon"} forEach [1,2,3,4,5,6,7,8];
		{_unit addMagazine "SmokeShell"} forEach [1,2];
		{_unit addMagazine "ACE_RG60A"} forEach [1,2];
		_unit addWeapon "bizon_silenced";
		_unit addWeapon "ACE_Rucksack_RD99";
		[_unit, "ACE_WireCutter"] call ACE_fnc_PutWeaponOnBack;
		[_unit, "PipeBomb", 1] call ACE_fnc_PackMagazine;
	};
	case "MED": {
		{_unit addMagazine "64Rnd_9x19_SD_Bizon"} forEach [1,2,3,4,5,6,7,8];
		{_unit addMagazine "SmokeShell"} forEach [1,2];
		{_unit addMagazine "ACE_RG60A"} forEach [1,2];
		_unit addWeapon "bizon_silenced";
		_unit addWeapon "ACE_Rucksack_RD99";
		[_unit, "ACE_WireCutter"] call ACE_fnc_PutWeaponOnBack;
		[_unit, "ACE_Bandage", 10] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Morphine", 10] call ACE_fnc_PackMagazine;
		[_unit, "ACE_Epinephrine", 10] call ACE_fnc_PackMagazine;
	};
};

//	プライマリィウェポンを選択させる
_primaryWeapon = primaryWeapon _unit;
_unit selectWeapon _primaryWeapon;

//	火器の使用モードが複数ある場合以下の変なコードが必要らしい
_muzzles = getArray( configFile >> "cfgWeapons" >> _primaryWeapon >> "muzzles" );
_unit selectWeapon ( _muzzles select 0 );
