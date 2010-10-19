// VehicleParachute.sqf by Strato
// usage: [_vehicle(, _paraClass)] execVM "scripts\VehicleParachute.sqf";
// パラシュートを生成し、vehicleをattachToする。降下後切り離し。
// _paraClassでパラシュートのクラス名を指定する。省略可。

_vehicle = _this select 0;
_paraClass = "ParachuteBigWest";
if(count _this > 1) then{
	_paraClass = _this select 1;
};

if(local _vehicle)then{
	_pos = getPos _vehicle;
	_para = _paraClass createVehicle _pos;
	_para setPos _pos;
	_para setDir (getDir _vehicle);
	_para setVelocity (velocity _vehicle);
	_para allowDamage false;
	
	_vehicle attachTo [_para, [0,0,0]];
	
	waitUntil{(getPos _vehicle select 2) > 0.5};
	waitUntil{(getPos _vehicle select 2) <= 0.5};
	_para allowDamage true;
	detach _vehicle;
};