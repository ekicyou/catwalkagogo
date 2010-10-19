// OnLanding.sqf by Strato
// usase: [_unit] execVM "scripts\OnLanding.sqf";
// パラ降下する歩兵に対して使用する。
// 降下後、バックパックの中に入っている武器を取り出す(AIのみ)。

_unit = _this select 0;

waitUntil{(vehicle _unit == _unit) and ((getPos _unit select 2) < 0.1)};

if(local _unit) then{
	_unit playMove "AinvPknlMstpSlayWrflDnon_medic";
	if(not isPlayer _unit) then{
		{
			[_unit, _x, 1] call ACE_fnc_PackMagazine;
			_unit removeMagazine _x;
		} forEach magazines _unit;
		{
			[_unit, _x, 1] call ACE_fnc_PackWeapon;
			_unit removeWeapon _x;
		} forEach weapons _unit;
		sleep 1;
		{
			_mag = _x select 0;
			_count = _x select 1;
			if(_mag in ["HandGrenade_East", "SmokeShell", "30Rnd_545x39_AK", "ACE_45Rnd_545x39_B_AK", "1Rnd_HE_GP25"]) then{
				[_unit, "MAG", _mag, _count] call ACE_fnc_RemoveGear;
				for [{_i = 0}, {_i < _count}, {_i = _i + 1}] do{
					_unit addMagazine _mag;
				};
			};
		} forEach ([_unit] call ACE_fnc_RuckMagazinesList);
		sleep 5;
		{
			_wep = _x select 0;
			_count = _x select 1;
			[_unit, "WEP", _wep, _count] call ACE_fnc_RemoveGear;
			for [{_i = 0}, {_i < _count}, {_i = _i + 1}] do{
				_unit addWeapon _wep;
			};
		} forEach ([_unit] call ACE_fnc_RuckWeaponsList);
	};
};
_unit selectWeapon (primaryWeapon _unit);