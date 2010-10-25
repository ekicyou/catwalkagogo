if(not isServer)exitWith{};

{
	_x setSkill 1;
	if(side _x == west) then{
		_x setSkill ["aimingSpeed", 0.3];
		_x setSkill ["aimingAccuracy", 0.3];
		_x setSkill ["aimingShake", 0.3];
		_x setSkill ["spotDistance", 0.3];
	};
} forEach _this;