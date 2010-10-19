// DropPackages.sqf by Strato
// usage: [_c130, [_package1, _package2,...]] execVM "scripts\DropPackages.sqf";
// _c130に積んだ物資を投下する。
// 物資はあらかじめattachToする。

_c130 = _this select 0;
_packages = _this select 1;

if(not isServer) exitWith{};

_dropedPackages = [];
_count = count _packages;
_y = 0;
_startTime = time;
_dt = 0;
while{_count > (count _dropedPackages)} do{
	_dt = time - _startTime;
	
	_i = 0;
	{
		if(not (_x in _dropedPackages)) then{
			_y = -2 + (2 * _i)  - (1 * _dt);
			_z = -4;
			if(_y < -5) then{
				_z = -4 + ((_y + 5) / 3);
			};
			if(_y > -8) then{
				_x attachTo [_c130, [0, _y, _z]];
			}else{
				_x attachTo [_c130, [0, -9, -6]];
				sleep 0.01;
				_dropedPackages = _dropedPackages + [_x];
				detach _x;
				[_x, "ParachuteBigEast"] execVM "scripts\VehicleParachute.sqf";
			};
		};
		_i = _i + 1;
	} forEach _packages;
	sleep 0.01;
};

sleep 30;

bBeconActivated = true;
publicVariable "bBeconActivated";