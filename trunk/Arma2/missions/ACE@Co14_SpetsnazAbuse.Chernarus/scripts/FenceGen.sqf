// FenceGen by Strato
// [_points(, _type, _length)] execVM "scripts\FenceGen.sqf"
// _points : フェンスを生成する頂点の座標の配列。
// _type : フェンスのクラス名(省略可)
// _length : フェンス一つひとつの長さ(省略可)

if(not isServer) exitWith{};

_points = _this select 0;
_type = "Fence_Ind";
if(count _this > 1) then{
	_type = _this select 1;
};
_length = 3;
if(count _this > 2) then{
	_length = _this select 2;
};

_count = (count _points) - 1;
for[{_i = 0}, {_i < _count}, {_i = _i + 1}] do{
	_start = _points select _i;
	_end = _points select (_i + 1);
	_angle = ((_end select 0) - (_start select 0)) atan2 ((_end select 1) - (_start select 1));
	_n = _start distance _end;
	_j = _length / 2;
	while{_j < _n} do{
		_pos = [(_start select 0) + _j * (sin _angle), (_start select 1) + _j * (cos _angle), _start select 2];
		_fence = _type createVehicle _pos;
		_fence setPosATL _pos;
		_fence setDir (_angle + 90);
		_j = _j + _length;
	};
	_j = _j - (_length / 2);
	_end = [(_start select 0) + _j * (sin _angle), (_start select 1) + _j * (cos _angle), _start select 2];
	_points set [_i + 1, _end];
};