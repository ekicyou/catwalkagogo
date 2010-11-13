if(not isServer) exitWith{};

_unit = _this select 0;

while{alive _unit} do{
	_unit setUnitPos "UP";
	_t = (random 5) + 5;
	_unit suppressFor _t;
	sleep _t;
	_unit setUnitPos "Middle";
	_t = (random 5) + 5;
	sleep _t;
};