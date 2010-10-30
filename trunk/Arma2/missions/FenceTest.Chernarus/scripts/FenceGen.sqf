// FenceGen by Strato
// [_points(, _type, _length)] execVM "scripts\FenceGen.sqf"
// _points : フェンスを生成する頂点のオブジェクトの配列。
// _type : フェンスのクラス名(省略可)
// _length : フェンス一つひとつの長さ(省略可)

if(not isServer) exitWith{};

_points = _this select 0;
if(count _points <= 1) exitWith{};
_type = "Fence_Ind";
if(count _this > 1) then{
	_type = _this select 1;
};
_length = 3;
if(count _this > 2) then{
	_length = _this select 2;
};

_fncSetPitchBank = compile preprocessFile "scripts\SetPitchBank.sqf";
_fncGetAngle = {
	private ["_srcPos", "_tgtPos", "_xd", "_yd"];
	_srcPos = _this select 0;
	_tgtPos = _this select 1;
	_xd = (_tgtPos select 0) - (_srcPos select 0);
	_yd = (_tgtPos select 1) - (_srcPos select 1);
	round (_xd atan2 _yd)
};

_lastH = (getPosASL (_points select 0)) select 2;
_count = (count _points) - 1;
for[{_i = 0}, {_i < _count}, {_i = _i + 1}] do{
	_start = getPosATL (_points select _i);
	_end = getPosATL (_points select (_i + 1));
	_angle = ((_end select 0) - (_start select 0)) atan2 ((_end select 1) - (_start select 1));
	_n = _start distance _end;
	_j = _length / 2;
	_hd = (_end select 2) - (_start select 2);
	while{_j < _n} do{
		_pos = [(_start select 0) + _j * (sin _angle), (_start select 1) + _j * (cos _angle), (_start select 2) + (_j / _n) * _hd];
		_fence = _type createVehicle _pos;
		_fence setPosATL _pos;
		_fence setDir (_angle + 90);
		_j = _j + _length;
		
		// 前のフェンスの高さとの差から傾きを設定。
		_h = (getPosASL _fence) select 2;
		_bank = (_length atan2 (_lastH - _h));
		[_fence, 0, _bank - 90] call _fncSetPitchBank;
		_lastH = _h;
	};
	
	// 頂点位置調整
	_j = _j - (_length / 2);
	(_points select (_i + 1)) setPosATL [(_start select 0) + _j * (sin _angle), (_start select 1) + _j * (cos _angle), _end select 2];
	_lastH = (_length / 2) * (tan _bank);
	
	// marker
	_start = getPos (_points select _i);
	_end = getPos (_points select (_i + 1));
	_length = (_points select _i) distance (_points select (_i + 1));
	_mid = [((_start select 0) + (_end select 0)) / 2, ((_start select 1) + (_end select 1)) / 2, 0];
	_ang = [_start, _end] call _fncGetAngle;
	_marker = createMarker [format ["%1-%2", _points select _i, _points select (_i + 1)], _mid];
	_marker setMarkerDir _ang;
	_marker setMarkerColor "ColorBlack";
	_marker setMarkerShape "RECTANGLE";
	_marker setMarkerSize [1, _length / 2];
};