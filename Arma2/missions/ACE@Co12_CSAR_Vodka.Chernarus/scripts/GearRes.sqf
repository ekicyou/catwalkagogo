_unit = _this select 0;
_type = _this select 1;

removeAllWeapons _unit;

_unit addMagazine "ACE_Bandage";
_unit addMagazine "ACE_Bandage";
switch(_type) do{
	case "SL":{
		_wep = floor (random 2);
		{_unit addMagazine "30Rnd_545x39_AK"} forEach [1,2,3,4,5,6,7,8];
		if(_wep == 0) then{
			_unit addWeapon "ACE_AKS74P";
		};
		if(_wep == 1) then{
			_unit addWeapon "ACE_AK74M";
		};
		if(_wep == 2) then{
			_unit addWeapon "AKS_74_U";
		};
		{_unit addMagazine "HandGrenade_East"} forEach [1,2];
		{_unit addMagazine "SmokeShell"} forEach [1,2];
		_unit addWeapon "Binocular";
		{_unit addMagazine "8Rnd_9x18_Makarov"} forEach [1,2,3,4];
		_unit addWeapon "Makarov";
	};
	case "RM":{
		_wep = floor (random 3);
		if(_wep == 0) then{
			{_unit addMagazine "30Rnd_545x39_AK"} forEach [1,2,3,4,5,6,7,8];
			_unit addWeapon "ACE_AKS74P";
		};
		if(_wep == 1) then{
			{_unit addMagazine "30Rnd_545x39_AK"} forEach [1,2,3,4,5,6,7,8];
			_unit addWeapon "AKS_74";
		};
		if(_wep == 2) then{
			{_unit addMagazine "30Rnd_762x39_AK47"} forEach [1,2,3,4,5,6,7,8];
			_unit addWeapon "AK_47_S";
		};
		if((random 4) < 1) then{
			_unit addMagazine "PG7V";
			_unit addMagazine "PG7V";
			_unit addWeapon "RPG7V";
		}else{
			{_unit addMagazine "HandGrenade_East"} forEach [1,2];
			{_unit addMagazine "SmokeShell"} forEach [1,2];
		};
	};
	case "GL":{
		_wep = floor (random 4);
		if(_wep == 0) then{
			{_unit addMagazine "30Rnd_545x39_AK"} forEach [1,2,3,4,5,6,7,8];
			_unit addWeapon "ACE_AK74M_GL";
		};
		if(_wep == 1) then{
			{_unit addMagazine "30Rnd_545x39_AK"} forEach [1,2,3,4,5,6,7,8];
			_unit addWeapon "ACE_AKS74P_GL";
		};
		if(_wep == 2) then{
			{_unit addMagazine "30Rnd_545x39_AK"} forEach [1,2,3,4,5,6,7,8];
			_unit addWeapon "AK_74_GL";
		};
		if(_wep == 3) then{
			{_unit addMagazine "30Rnd_762x39_AK47"} forEach [1,2,3,4,5,6,7,8];
			_unit addWeapon "ACE_AKM_GL";
		};
		{_unit addMagazine "1Rnd_HE_GP25"} forEach [1,2,3,4,5,6];
		{_unit addMagazine "HandGrenade_East"} forEach [1,2];
		{_unit addMagazine "SmokeShell"} forEach [1,2];
	};
	case "AR":{
		_wep = floor (random 3);
		if(_wep == 0) then{
			{_unit addMagazine "ACE_45Rnd_545x39_B_AK"} forEach [1,2,3,4,5,6,7,8];
			_unit addWeapon "ACE_RPK74M";
		};
		if(_wep == 1) then{
			{_unit addMagazine "ACE_75Rnd_762x39_B_AK47"} forEach [1,2,3,4,5,6];
			_unit addWeapon "ACE_RPK";
		};
		if(_wep == 2) then{
			{_unit addMagazine "75Rnd_545x39_RPK"} forEach [1,2,3,4,5,6];
			_unit addWeapon "RPK_74";
		};
		{_unit addMagazine "HandGrenade_East"} forEach [1,2];
		{_unit addMagazine "SmokeShell"} forEach [1,2];
	};
	case "SNP":{
		if((random 2) < 1) then{
			{_unit addMagazine "10Rnd_762x54_SVD"} forEach [1,2,3,4,5,6,7,8];
			_unit addWeapon "SVD";
		}else{
			{_unit addMagazine "20Rnd_9x39_SP5_VSS"} forEach [1,2,3,4,5,6,7,8];
			_unit addWeapon "VSS_Vintorez";
		};
		{_unit addMagazine "SmokeShell"} forEach [1,2];
		{_unit addMagazine "HandGrenade_East"} forEach [1,2];
		{_unit addMagazine "ACE_20Rnd_9x18_APS"} forEach [1,2,3,4];
		_unit addWeapon "ACE_APS";
	};
	case "AT":{
		_wep = floor (random 5);
		if(_wep == 0) then{
			{_unit addMagazine "30Rnd_545x39_AK"} forEach [1,2,3,4,5,6];
			_unit addWeapon "ACE_AKS74P";
		};
		if(_wep == 1) then{
			{_unit addMagazine "30Rnd_545x39_AK"} forEach [1,2,3,4,5,6];
			_unit addWeapon "AKS_74";
		};
		if(_wep == 2) then{
			{_unit addMagazine "30Rnd_762x39_AK47"} forEach [1,2,3,4,5,6];
			_unit addWeapon "AK_47_M";
		};
		if(_wep == 3) then{
			{_unit addMagazine "30Rnd_762x39_AK47"} forEach [1,2,3,4,5,6];
			_unit addWeapon "AK_47_S";
		};
		if(_wep == 4) then{
			{_unit addMagazine "30Rnd_545x39_AK"} forEach [1,2,3,4,5,6];
			_unit addWeapon "AKS_74_U";
		};
		_unit addMagazine "ACE_PG7V_PGO7";
		_unit addMagazine "ACE_PG7V_PGO7";
		_unit addMagazine "ACE_PG7V_PGO7";
		_unit addWeapon "ACE_RPG7V_PGO7";
	};
	case "MG":{
		{_unit addMagazine "100Rnd_762x54_PK"} forEach [1,2,3,4,5,6,7,8];
		_unit addWeapon "PK";
		{_unit addMagazine "HandGrenade_East"} forEach [1,2,3,4];
		{_unit addMagazine "SmokeShell"} forEach [1,2];
	};
};

_unit selectWeapon (primaryWeapon _unit);