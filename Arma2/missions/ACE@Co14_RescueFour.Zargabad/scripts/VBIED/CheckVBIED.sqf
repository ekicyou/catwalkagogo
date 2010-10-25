_unit = _this select 1;
_car = nearestObject [player, "Car"];

if(not isNull _car) then{
	_unit playMove "AmovPercMstpSnonWnonDnon_carCheckWheel";
	sleep 8;

	_bomb = _car getVariable "VBIED_BombActivated";
	_isNil = isNil "_bomb";
	//hint format ["%1 %2 %3", _bomb, _isNil];
	if(not _isNil) then{
		if(_bomb) then{
			hint "IED was found! Deactivating...";
			_unit playMove "ActsPercSnonWnonDnon_carFixing";
			sleep 70;
			_car setVariable ["VBIED_BombActivated", false, true];
			hint "IED has been deactivated!";
		}else{
			hint "IED was not found.";
		};
	}else{
		hint "IED was not found.";
	};
};