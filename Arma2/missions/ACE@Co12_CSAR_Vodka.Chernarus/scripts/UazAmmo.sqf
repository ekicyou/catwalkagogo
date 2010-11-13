_ammo = _this select 0;
clearMagazineCargo _ammo;
clearWeaponCargo _ammo;

_ammo addWeaponCargo ["ACE_RPG27", 2];
_ammo addMagazineCargo ["PipeBomb", 1];
