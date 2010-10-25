if (isServer) then{
	bTaskADone = false; publicVariable "bTaskADone";
	bTaskBDone = false; publicVariable "bTaskBDone";
	bReported = false;  publicVariable "bReported";
	
	allUnits execVM "skill.sqf";
	{
		if(_x isKindOf "CDF_Soldier_GL") then{
			[_x] execVM "scripts\FlareAI.sqf";
		};
		if(_x isKindOf "CDF_Soldier_Medic") then{
			_unit = _x;
			{_unit addMagazine "ACE_Morphine"} forEach [1,2,3,4];
			{_unit addMagazine "ACE_Epinephrine"} forEach [1,2,3,4];
		};
	} forEach allUnits;
};

[] spawn {
	sleep 1;
	_light = "#lightpoint" createVehicle (getPos oTelek_1); 
	_light setLightBrightness 0.05;
	_light setLightAmbient[1.0, 0, 0]; 
	_light setLightColor[1.0, 0, 0]; 
	_light lightAttachObject [oTelek_1, [0,0,10]];
	{
		_light = "#lightpoint" createVehicle (getPos _x); 
		_light setLightBrightness 0.05;
		_light setLightAmbient[0.8, 0.8, 0.5]; 
		_light setLightColor[1.0, 1.0, 0.5]; 
		_light lightAttachObject [_x, [0,0,0]];
	} forEach [oLamp_1, oLamp_3, oLamp_4, oLamp_6, oLamp_8];
};


[] execVM "briefing.sqf";