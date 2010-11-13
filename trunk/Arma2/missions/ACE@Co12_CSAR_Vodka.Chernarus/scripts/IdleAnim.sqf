_unit = _this select 0;
_anim = _this select 1;

if(not local _unit) exitWith{};

sleep (random 5);

while{alive _unit} do{
	_unit playMove _anim;
	sleep 10;
};