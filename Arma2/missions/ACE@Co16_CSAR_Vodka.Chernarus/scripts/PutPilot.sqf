if(not isServer) exitWith{};

uPilot allowDamage false;

sleep 1;

_center = getMarkerPos "CSARZone";
_lenX = random 750;
_lenY = random 500;
_ang = random 360;
_pos = [(_center select 0) + ((cos _ang) * _lenX), (_center select 1) + ((sin _ang) * _lenY), 0];

vSu25Wreck setPos _pos;
//vSu25Wreck setDamage 1;
"Bo_FAB_250" createVehicle _pos;
[] spawn {
	sleep 1;
	"Bo_FAB_250" createVehicle (getPos vSu25Wreck);
	{
		_wp = _x addWaypoint [getPos vSu25Wreck, 100];
		_wp setWaypointType "GUARD";
	} forEach [AttackTeam1, AttackTeam2, AttackTeam3];
	{
		_x spawn {
			waitUntil{(_this distance vSu25Wreck) < 300};
			{
				_x leaveVehicle _this;
				commandGetOut _x;
			} forEach crew _this;
		};
	} forEach [vPraga1, vPraga2, vPraga3];
	uPilot allowDamage true;
};

trgGuardPoint setPos _pos;

_center = _pos;
_len = 100;
_ang = random 360;
_pos = [(_center select 0) + ((cos _ang) * _len), (_center select 1) + ((sin _ang) * _len), 0];

uPilot setPos _pos;
uPilot setDir (random 360);