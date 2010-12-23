_ammo = _this select 0;

clearMagazineCargo _ammo;
clearWeaponCargo _ammo;

_ammo addWeaponCargo ["Pecheneg", 4];
_ammo addMagazineCargo ["100Rnd_762x54_PK", 40];