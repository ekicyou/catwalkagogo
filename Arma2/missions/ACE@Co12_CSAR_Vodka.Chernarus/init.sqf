fncSquat = compile preprocessFile "scripts\Squat.sqf";
fncGearRes = compile preprocessFile "scripts\GearRes.sqf";

if(isServer) then{
	{
		_x allowFleeing 0;
		if(side _x == resistance) then{
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
		};
	} forEach allUnits;
	allUnits execVM "skill.sqf";
};

//civilian setFriend [east, 1];
//civilian setFriend [resistance, 0];
//resistance setFriend [civilian, 0];

[] execVM "briefing.sqf";

finishMissionInit;

[] execVM "scripts\PutPilot.sqf";