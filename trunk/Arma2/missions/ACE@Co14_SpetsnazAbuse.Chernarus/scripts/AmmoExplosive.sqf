if(not isServer) exitWith{};

_ammo = _this select 0;
_pos = [getPos _ammo select 0, getPos _ammo select 1, 0.5];

waitUntil{not alive _ammo};

_bombs = ["ARTY_Sh_81_HE", "GrenadeHand", "GrenadeHand", "GrenadeHand", "GrenadeHand", "GrenadeHand", "GrenadeHand", "GrenadeHand", "GrenadeHand", "GrenadeHand"];
while{(random 3) > 1} do{
	sleep (random 10);
	(_bombs select (floor (random (count _bombs)))) createVehicle _pos;
};