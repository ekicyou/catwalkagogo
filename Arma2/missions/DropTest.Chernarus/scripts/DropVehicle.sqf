// DropVehicle.sqf by Strato
// usage: [_c130, _vehicle(, _originPos)] execVM "scripts\DropVehicle.sqf";
// vehicleをc130から降下させる。
// _originPosにattachToした座標を入れる。省略時はBMD-1に適合。

_c130 = _this select 0;
_vehicle = _this select 1;
_originPos = [0, -0.5, -4.5];
if(count _this > 2) then{
	_originPos = _this select 2;
};

if(local _vehicle) then{
	_startTime = time;
	_dt = 0;
	while{_dt < 5} do{
		_dt = time - _startTime;
		_y = (_originPos select 1) - 14 * (_dt / 5);
		_z = (_originPos select 2);
		if(_y < -7) then{
			_z = _z + ((_y + 7) / 3);
		};
		_vehicle attachTo [_c130, [_originPos select 0, _y, _z]];
		sleep 0.01;
	};
	_vehicle attachTo [_c130, [_originPos select 0, (_originPos select 1) - 14, (_originPos select 2) - 2]];
	sleep 0.01;
	detach _vehicle;
	[_vehicle, "ParachuteBigEast"] execVM "scripts\VehicleParachute.sqf";
};