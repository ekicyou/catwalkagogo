VBIED_Detonate = {
	private ["_car", "_pos"];
	_car = _this select 0;
	
	if(_car getVariable "VBIED_BombActivated") then{
		_car setVariable ["VBIED_BombActivated", false, true];
		_pos = getPos _car;
		_bomb = (_car getVariable "VBIED_AmmoType") createVehicle _pos;
		_bomb setVelocity (velocity _car);
		_pos set [2, (_pos select 2) + 4];
		sleep 0.2;
		{
			_x setDamage 0.9 + random (0.3);
		} forEach crew _car;
		_car setDamage 1;
	};
};

VBIED_Detonator = {
	if(not isServer)exitWith{};
	
	_car = _this select 0;
	_detonator = _this select 1;
	_trg = _this select 2;
	while{(alive _detonator) and (_car getVariable "VBIED_BombActivated")} do{
		waitUntil{(count list _trg > 0) or (not (_car getVariable "VBIED_BombActivated"))};
		if(_car getVariable "VBIED_BombActivated") then{
			_inEnemy = false;
			if(alive _detonator) then{
				{
					//hint (format ["%1", _detonator knowsAbout _x]);
					_inEnemy = _inEnemy or ((_detonator knowsAbout _x) > 1.4);
				} forEach list _trg;
			};
			if(_inEnemy) then{
				[_car] call VBIED_Detonate;
			};
			sleep 1;
		};
	};
	deleteVehicle _trg;
	//hint "VBIED_Detonator Exit";
};

VBIED_Car = {
	if(not isServer)exitWith{};
	
	_car = _this select 0;
	_trgs = _this select 1;
	while{_car getVariable "VBIED_BombActivated"} do{
		sleep 1;
		{
			_x setPos (getPos _car);
		} forEach _trgs;
		if(damage _car > 0.3) then{
			[_car] call VBIED_Detonate;
		};
	};
};

VBIED_CreateDetonator = {
	private ["_car", "_group", "_unit", "_r"];
	_car = _this select 0;
	_group = createGroup VBIED_AmbientDetonatorSide;
	_unit = _group createUnit [
		VBIED_AmbientDetonatorUnitTypes select (floor (random (count VBIED_AmbientDetonatorUnitTypes))),
		 getPos _car, [], 10, "NONE"];
	
	[_unit] spawn VBIED_InitAmbientDetonator;
	_unit
};

VBIED_PatrolBuilding = {
	if(not isServer)exitWith{};

	_unit = _this select 0;
	_bld = _this select 1;
	_bldPos = _this select 2;

	_unit setPos (_bld buildingPos round(random(_bldpos)));
	while{alive _unit} do{
		sleep (60 + random(60));
		_unit doMove (_bld buildingPos round(random(_bldpos)));
		waitUntil{(not alive _unit) or (unitReady _unit)};
	};
};

VBIED_MoveToNearestBuilding = {
	if(not isServer) exitWith{};
	
	waitUntil{not isNil "MON_GetNearestBuildings"};

	_leader = _this select 0;
	_distance = _this select 1;
	_inside = false;
	_patrol = false;
	_random = false;
	if(count _this > 2) then{
		_inside = _this select 2;
	};
	if(count _this > 3) then{
		_patrol = _this select 3;
	};
	if(count _this > 3) then{
		_random = _this select 4;
	};
	_blds = [_leader,_distance] call MON_GetNearestBuildings;
	
	_i = 0;
	_count = count(_blds);
	if(_random) then{
		_i = floor(random(_count));
	};
	{
		_bld = (_blds select _i) select 0;
		_bldPos = (_blds select _i) select 1;
		if(inside) then{
			_mins = [];
			_minH = 10000;
			for [{_j = 0}, {_j <= bldPos}, {_j = _j + 1}] do{
				_pos = _bld buildingPos _j;
				_h = round(_pos select 2);
				if(_h == _minH) then{
					_mins = _mins + [_j];
				}else{
					if(_h < _minH) then{
						_mins = [_j];
						_minH = _h;
					};
				};
			};
			_bldPos = _mins select floor(random(count _mins));
		}else{
			_bldPos = round(random(_bldpos));
		};
		
		_group = createGroup (side _x);
		[_x] joinSilent _group;
		_pos = (_bld buildingPos _bldPos);
		//_x doMove _pos;
		_x setPos _pos;
		if(VBIED_Debug) then{
			_marker = createMarker [format ["%1", _x], _pos];
			_marker setMarkerType "Dot";
		};
		if(_patrol) then{
			[_x, _bld, _bldPos] spawn VBIED_PatrolBuilding;
		};
		if(_random) then{
			_i = floor(random(_count));
		}else{
			_i = _i + 1;
		};
	} forEach units _leader;
};

VBIED_CheckVehicleActionCond = {
	_car = nearestObject [player, "Car"];
	if((not isNull _car) and ((player distance _car) <= 4)) then{
		_tgtPos = getPos _car;
		_srcPos = getPos player;
		_xd = (_tgtPos select 0) - (_srcPos select 0);
		_yd = (_tgtPos select 1) - (_srcPos select 1);
		_dir = (getDir player);
		if(_dir > 180) then{
			_dir = _dir - 360;
		};
		_ang = abs(_dir - (_xd atan2 _yd));
		(_ang < 30)
	}else{
		false
	};
};

VBIED_AddPlayerAction = {
	while{true} do{
		waitUntil {!isNull player};
		_curPlayer = player;
		_action = _curPlayer addAction ["Check Vehicle", "scripts\VBIED\CheckVBIED.sqf", [], 0, false, false, "", "[] call VBIED_CheckVehicleActionCond"];
		waitUntil{player != _curPlayer};
		player removeAction _action;
	};
};