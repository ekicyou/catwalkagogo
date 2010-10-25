if(not isNull player) then{
	[] spawn {
		while{true} do{
			waitUntil {!isNull player};
			_curPlayer = player;
			_action = _curPlayer addAction ["Request Artillery", "scripts\RequestArtyMenu.sqf", "", 0, false, false, "", "leader player == player"];
			waitUntil{player != _curPlayer};
			player removeAction _action;
		};
	};
	[] spawn {
		while{true} do{
			waitUntil{ArtyOfficer getVariable "Arty_MessageBroadcasted"};
			ArtyOfficer setVariable ["Arty_MessageBroadcasted", false, false];
			(ArtyOfficer getVariable "Arty_MessageFrom") sideChat (ArtyOfficer getVariable "Arty_Message");
		};
	};
};


if(not isServer) exitWith{};

ArtyOfficer setVariable ["Arty_Count", [1, 1, 3, 1, 1], true];
ArtyOfficer setVariable ["Arty_Requested", false, true];
ArtyOfficer setVariable ["Arty_Type", 0, true];
ArtyOfficer setVariable ["Arty_Pos", [0, 0, 0], true];
ArtyOfficer setVariable ["Arty_Message", "", true];
ArtyOfficer setVariable ["Arty_MessageFrom", objNull, true];
ArtyOfficer setVariable ["Arty_MessageBroadcasted", false, true];
ArtyOfficer setVariable ["Arty_Caller", objNull, true];
Arty_BroadcastMessage = {
	ArtyOfficer setVariable ["Arty_Message", _this select 1, true];
	ArtyOfficer setVariable ["Arty_MessageFrom", _this select 0, true];
	ArtyOfficer setVariable ["Arty_MessageBroadcasted", true, true];
};

while{true} do{
	waitUntil{ArtyOfficer getVariable "Arty_Requested"};
	ArtyOfficer setVariable ["Arty_Requested", false, true];
	_template1 = [];
	_template2 = [];
	_type = ArtyOfficer getVariable "Arty_Type";
	switch(_type) do{
		case 0:{
			_template1 = ["IMMEDIATE", "WP", 0, 4];
			_template2 = ["IMMEDIATE", "WP", 1, 4];
			[RIPPER, 200] call BIS_ARTY_F_SetDispersion;
			[RIPPER_SLAVE, 200] call BIS_ARTY_F_SetDispersion;
		};
		case 1:{
			_template1 = ["TIMED", "WP", 10, 30];
			_template2 = ["TIMED", "ILLUM", 10, 30];
			[RIPPER, 200] call BIS_ARTY_F_SetDispersion;
			[RIPPER_SLAVE, 200] call BIS_ARTY_F_SetDispersion;
		};
		case 2:{
			_template1 = ["TIMED", "ILLUM", 10, 60];
			_template2 = [];
			[RIPPER, 200] call BIS_ARTY_F_SetDispersion;
			[RIPPER_SLAVE, 200] call BIS_ARTY_F_SetDispersion;
		};
		case 3:{
			_template1 = ["IMMEDIATE", "LASER", 0, 1];
			_template2 = ["IMMEDIATE", "LASER", 0, 1];
			[RIPPER, 50] call BIS_ARTY_F_SetDispersion;
			[RIPPER_SLAVE, 50] call BIS_ARTY_F_SetDispersion;
		};
		case 4:{
			_template1 = ["IMMEDIATE", "HE", 0, 4];
			_template2 = ["IMMEDIATE", "HE", 0, 4];
			[RIPPER, 200] call BIS_ARTY_F_SetDispersion;
			[RIPPER_SLAVE, 200] call BIS_ARTY_F_SetDispersion;
		};
	};
	
	Arty_Dest setPos (ArtyOfficer getVariable "Arty_Pos");
	_pos = getPosASL Arty_Dest;
	_count = (ArtyOfficer getVariable "Arty_Count") select _type;
	if(_count > 0) then{
		if([RIPPER, _pos, _template1 select 1] call BIS_ARTY_F_PosInRange) then{
			[ArtyOfficer getVariable "Arty_Caller", "FireBase, requesting artillery supports, over."] call Arty_BroadcastMessage;
			sleep 5;
			[ArtyOfficer, "This is FireBase. Your artillery request is recieved. Splash ETA 1 minute."] call Arty_BroadcastMessage;
			
			_count = ArtyOfficer getVariable "Arty_Count";
			_count set [_type, (_count select _type) - 1];
			ArtyOfficer setVariable ["Arty_Count", _count, true];
			[RIPPER, _pos, _template1] call BIS_ARTY_F_ExecuteTemplateMission;
			if(count _template2 > 0) then{
				[RIPPER_SLAVE, _pos, _template2] call BIS_ARTY_F_ExecuteTemplateMission;
			};
			
			waitUntil{RIPPER getVariable "ARTY_ONMISSION"};
			[] spawn {
				waitUntil{RIPPER getVariable "ARTY_COMPLETE"};
				[ArtyOfficer, "Rounds complete."] call Arty_BroadcastMessage;
			};
			[] spawn {
				waitUntil{RIPPER getVariable "ARTY_SPLASH"};
				[ArtyOfficer, "Splash."] call Arty_BroadcastMessage;
			};
			waitUntil{not (RIPPER getVariable "ARTY_ONMISSION")};
		}else{
			[ArtyOfficer, "This is FireBase. Requested coordinates is out of range, out."] call Arty_BroadcastMessage;
		};
	};
};