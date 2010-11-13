fncSquat = compile preprocessFile "scripts\Squat.sqf";
fncGearRes = compile preprocessFile "scripts\GearRes.sqf";
fncWearGasMask = compile preprocessFile "scripts\WearGasMask.sqf";

if(isServer) then{
	nNukeTimer = 300;
	bNukeTimerOn = false;
	publicVariable "nNukeTimer";
	publicVariable "bNukeTimerOn";
	{
		_x allowFleeing 0;
		if(side _x == resistance) then{
			if(_x isKindOf "GUE_Soldier_3") then{
				if((random 4) < 1) then{
					[_x, "GL"] spawn fncGearRes;
				}else{
					[_x, "RM"] spawn fncGearRes;
				};
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

[] execVM "briefing.sqf";

finishMissionInit;