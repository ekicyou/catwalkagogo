_car = _this select 0;
_detonator = _this select 1;
_dest = _this select 2;
_dist = _this select 3;

waitUntil{not isNil "VBIED_Init"};

waitUntil{((_car distance _dest) < _dist) or (not alive _detonator) or (not (_car getVariable "VBIED_BombActivated"))};

if((alive _detonator) and (_car getVariable "VBIED_BombActivated")) then{
	[_car] call VBIED_Detonate;
};