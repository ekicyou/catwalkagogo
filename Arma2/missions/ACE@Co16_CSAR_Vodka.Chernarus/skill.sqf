if(not isServer)exitWith{};

while{true} do{
	{
		if(isNil {_x getVariable "bSetSkillDone"}) then{
			_x allowFleeing 0;
			_x setSkill 1;
			if(side _x == resistance) then{
				if(_x isKindOf "GUE_Soldier_1") then{
					[_x, "RM"] spawn fncGearRes;
				};
				if(_x isKindOf "GUE_Soldier_2") then{
					[_x, "RM"] spawn fncGearRes;
				};
				if(_x isKindOf "GUE_Soldier_3") then{
					[_x, "RM"] spawn fncGearRes;
				};
				if(_x isKindOf "GUE_Soldier_GL") then{
					[_x, "GL"] spawn fncGearRes;
				};
				if(_x isKindOf "ACE_GUE_Soldier_RPG") then{
					[_x, "AT"] spawn fncGearRes;
				};
				if(_x isKindOf "GUE_Soldier_AT") then{
					[_x, "AT"] spawn fncGearRes;
				};
				if(_x isKindOf "GUE_Soldier_CO") then{
					[_x, "SL"] spawn fncGearRes;
				};
				if(_x isKindOf "GUE_Soldier_Sniper") then{
					[_x, "SNP"] spawn fncGearRes;
				};
				if(_x isKindOf "GUE_Soldier_Medic") then{
					[_x, "SNP"] spawn fncGearRes;
				};
				if(_x isKindOf "GUE_Soldier_AR") then{
					[_x, "AR"] spawn fncGearRes;
				};
				if(_x isKindOf "GUE_Soldier_MG") then{
					[_x, "MG"] spawn fncGearRes;
				};
				_x setSkill ["aimingSpeed", 0.5];
				_x setSkill ["aimingAccuracy", 0.1];
				_x setSkill ["aimingShake", 0.1];
				_x setSkill ["spotDistance", 0.3];
			};
			_x setVariable ["bSetSkillDone", true];
			//_x globalChat "SetSkillDone";
		};
	} forEach allUnits;
	player globalChat format ["Units Count %1 /  Friendly Count %2 / Enemy Count %3", count allUnits, west countSide allUnits, (resistance countSide allUnits) - (civilian countSide allUnits)];
	sleep 60;
};