if(not isServer) exitWith{};

_unit = _this select 0;
_pos = getPos _unit;

while{alive _unit} do{
	_unit doMove _pos;
	sleep (random(10) + 6);
	_r = random(1);
	if(_r < 0.5) then{
		_unit setUnitPos "Down";
	}else{
		_unit setUnitPos "Middle";
	};
};