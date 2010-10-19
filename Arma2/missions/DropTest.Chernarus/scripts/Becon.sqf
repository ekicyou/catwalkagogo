// Becon.sqf by Strato
// usase: [] execVM "scripts\Becon.sqf"
// init.sqfで実行。
// bBeconActivatedがtrueになった後、ドロップした物資の位置を方角で示す。
// 物資が回収されるとその物資のビーコンは消える。

if(not isServer) then{
	waitUntil {!isNull player};
};

_becons = [oPack_1, oPack_2, oPack_3, oPack_4];
_break = true;

waitUntil{bBeconActivated};
hint "Becon activated";

while{_break} do{
	sleep 5;
	
	_srcPos = getPos player;
	_tgtPos = [];
	
	_min = 999999;
	{
		if((not isNull _x) and (player distance _x < _min)) then{
			_min = player distance _x;
			_tgtPos = getPos _x;
		};
	} forEach _becons;
	
	if(_min == 999999) then{
		_break = false;
	}else{
		// 角度計算
		_xd = (_tgtPos select 0) - (_srcPos select 0);
		_yd = (_tgtPos select 1) - (_srcPos select 1);
		_ang = round (_xd atan2 _yd);
		if(_ang < 0) then{
			_ang = _ang + 360;
		};
		hint format ["Becon: %1", _ang];
	};
};