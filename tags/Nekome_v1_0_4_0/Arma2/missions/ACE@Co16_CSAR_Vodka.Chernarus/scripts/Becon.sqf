if(isServer) then{
	[] spawn {
		_groups = [gEnemy1, gEnemy2, gEnemy3, gEnemy4, gEnemy5, gEnemy6, gEnemy7, gEnemy8, gEnemy9, gEnemy10, gEnemy11, gEnemy12, gEnemy13, gEnemy14];
		while{(count _groups) > 0} do{
			waitUntil{bBeconOn};
			// 全滅判定
			_newGroups = [];
			{
				if((count (units _x)) > 0) then{
					_newGroups = _newGroups + [_x];
				};
			} forEach _groups;
			_groups = _newGroups;
			
			// 近いグループを移動
			_min = 999999;
			_nearGroups = [];
			{
				_d = (leader _x) distance uPilot;
				if(_d < 800) then{
					_nearGroups = _nearGroups + [_x];
				};
			} forEach _groups;
			
			{
				_wp = [_x, 1];
				if(waypointType _wp == "DISMISS") then{
					(units _x) joinSilent _x;
					deleteWaypoint _wp;
					_wp = _x addWaypoint [position uPilot, 50];
					_wp setWaypointType "HOLD";
					//player globalChat format ["%1: DISMISS END", _x];
				};
				_wp setWaypointPosition [position uPilot, 100];
				//player globalChat format ["%1: SetPoint %2", _wp, position uPilot];
			} forEach _nearGroups;
			sleep 30;
		};
	};
};

if(player == uPilot) exitWith{};
while{true} do{
	waitUntil{bBeconOn};
	
	_srcPos = getPos player;
	_tgtPos = getPos uPilot;
		
	// 角度計算
	_xd = (_tgtPos select 0) - (_srcPos select 0);
	_yd = (_tgtPos select 1) - (_srcPos select 1);
	_ang = round (_xd atan2 _yd);
	if(_ang < 0) then{
		_ang = _ang + 360;
	};
	hint format ["Becon: %1", _ang];
	
	sleep 5;
};