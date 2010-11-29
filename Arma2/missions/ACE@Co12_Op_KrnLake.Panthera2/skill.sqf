if(not isServer)exitWith{};

{
	_x setSkill 1;
	if(side _x == resistance) then{
		_x setSkill ["aimingSpeed", 0.5];
		_x setSkill ["aimingAccuracy", 0.1];
		_x setSkill ["aimingShake", 0.1];
		_x setSkill ["spotDistance", 0.2];
	};
} forEach _this;