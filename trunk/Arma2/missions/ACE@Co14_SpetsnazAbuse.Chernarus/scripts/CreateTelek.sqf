if(not isServer) exitWith{};

_obj = _this select 0;

_telek="Land_telek1" createVehicle (getPos _obj);
_telek setPosATL (getPosATL _obj);
_telek setDir (getDir _obj);