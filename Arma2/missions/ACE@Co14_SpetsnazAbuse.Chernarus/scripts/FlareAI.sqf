_unit = _this select 0;

_unit addMagazine "FlareRed_GP25";

bFlareOn = false;
sleep (random 7);

while{alive _unit} do{
	waitUntil{(not alive _unit) or (not isNull (_unit findNearestEnemy _unit))};
	if(alive _unit) then{
		_enemy = _unit findNearestEnemy _unit;
		if((_unit distance _enemy < 300) and (not bFlareOn)) then{
			bFlareOn = true;
			_unit addMagazine "FlareWhite_GP25";
			_unit fire ["GP25Muzzle", "GP25Muzzle", "FlareWhite_GP25"];
			for[{_i=0},{(_i<19) and (alive _unit)},{_i=_i+1}] do{
				sleep 1;
			};
			bFlareOn = false;
		};
	};
	sleep 7;
};