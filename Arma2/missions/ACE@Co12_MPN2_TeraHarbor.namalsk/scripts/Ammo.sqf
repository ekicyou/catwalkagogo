_ammo = _this select 0;
_scopes = false;
if(count _this > 1) then{
	_scopes = _this select 1;
};
clearMagazineCargo _ammo;
clearWeaponCargo _ammo;

if(_scopes) then{
	_ammo addWeaponCargo ["ACE_AK103_1p29", 4];
	_ammo addWeaponCargo ["ACE_AK103_GL_1p29", 2];
	_ammo addWeaponCargo ["ACE_AK104_1p29", 4];
	_ammo addWeaponCargo ["ACE_AK105_1p29", 4];
	//_ammo addWeaponCargo ["AK_107_GL_pso", 4];
	//_ammo addWeaponCargo ["AK_107_pso", 2];
	_ammo addWeaponCargo ["ACE_AK74M_1p29", 4];
	_ammo addWeaponCargo ["ACE_AK74M_GL_1p29", 2];
	_ammo addWeaponCargo ["ACE_AKS74P_1p29", 4];
	_ammo addWeaponCargo ["ACE_AKS74P_GL_1p29", 2];
	//_ammo addWeaponCargo ["ACE_AK103_pso", 4];
	//_ammo addWeaponCargo ["ACE_AK103_GL_pso", 2];
	//_ammo addWeaponCargo ["ACE_AK104_pso", 2];
	//_ammo addWeaponCargo ["ACE_AK105_pso", 4];
	//_ammo addWeaponCargo ["ACE_AK74M_pso", 4];
	//_ammo addWeaponCargo ["ACE_AK74M_GL_pso", 2];
	//_ammo addWeaponCargo ["ACE_AKS74P_pso", 4];
	//_ammo addWeaponCargo ["ACE_AKS74P_GL_pso", 2];
};
//_ammo addWeaponCargo ["AKS_GOLD", 2];
_ammo addMagazineCargo ["30Rnd_762x39_AK47", 100];
_ammo addMagazineCargo ["ACE_30Rnd_762x39_T_AK47", 100];
_ammo addMagazineCargo ["ACE_30Rnd_762x39_SD_AK47", 100];
_ammo addMagazineCargo ["ACE_20Rnd_9x18_APS", 100];
_ammo addWeaponCargo ["ACE_AK103", 4];
_ammo addWeaponCargo ["ACE_AK103_GL", 2];
_ammo addWeaponCargo ["ACE_AK103_GL_Kobra", 2];
_ammo addWeaponCargo ["ACE_AK103_Kobra", 2];
_ammo addWeaponCargo ["ACE_AK104", 4];
_ammo addWeaponCargo ["ACE_AK104_Kobra", 4];
_ammo addWeaponCargo ["ACE_AK105", 4];
_ammo addWeaponCargo ["ACE_AK105_Kobra", 4];
_ammo addWeaponCargo ["ACE_AK74M", 4];
_ammo addWeaponCargo ["ACE_AK74M_GL", 2];
_ammo addWeaponCargo ["ACE_AK74M_GL_Kobra", 2];
_ammo addWeaponCargo ["ACE_AK74M_Kobra", 4];
_ammo addWeaponCargo ["ACE_AKM_GL", 2];
_ammo addWeaponCargo ["ACE_AKMS", 4];
_ammo addWeaponCargo ["ACE_AKMS_SD", 12];
_ammo addWeaponCargo ["ACE_AKS74P", 4];
_ammo addWeaponCargo ["ACE_AKS74P_GL", 2];
_ammo addWeaponCargo ["ACE_APS", 8];
_ammo addWeaponCargo ["ACE_RPK74M_1P29", 2];
_ammo addWeaponCargo ["ACE_gr1", 8];
_ammo addWeaponCargo ["AK_107_GL_kobra", 2];
_ammo addWeaponCargo ["AK_107_kobra", 4];
_ammo addWeaponCargo ["ACE_AKM", 4];
_ammo addWeaponCargo ["ACE_AKMS", 4];
_ammo addWeaponCargo ["PK", 2];
