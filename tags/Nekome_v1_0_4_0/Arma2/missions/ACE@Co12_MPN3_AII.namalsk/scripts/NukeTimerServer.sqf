bNukeTimerOn = true;
publicVariable "bNukeTimerOn";

_start = time;
_max = nNukeTimer;

while{nNukeTimer > 0} do{
	nNukeTimer = _max - (time - _start);
	publicVariable "nNukeTimer";
};

_pos = getPosATL uOfficer;
["ACE_B61_15", _pos] call ACE_fnc_NuclearGroundBurst;