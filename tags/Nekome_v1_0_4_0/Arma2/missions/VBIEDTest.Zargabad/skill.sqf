if(not isServer)exitWith{};

{
	_side = side _x;
	if(_side == west) then{
		_x setSkill 1;
	};
	if(_side == east) then{
		_x setSkill 1;
		_x setSkill ["aimingAccuracy", 0.3];
		_x setSkill ["aimingShake", 0.3];
	};
	if(_side == resistance) then{
		_x setSkill 1;
		_x setSkill ["aimingAccuracy", 0.3];
		_x setSkill ["aimingShake", 0.3];
	};
	if(_side == civilian) then{
		_x setSkill 1;
	};
} forEach _this;