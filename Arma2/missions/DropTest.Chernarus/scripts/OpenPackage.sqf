// OpenPackage.sqf by Strato
// usage: this addAction ["OpenPackage", "scripts\OpenPackage.sqf", [_var]];
// 物資を開くaction。
// _varには物資を開いた後trueにしてpublicVariableする変数名を指定。
// _varがtrueになったら、トリガーで物資をdeleteVehicle、その位置にammoをsetPosしている。

_args = _this select 3;
_var = _args select 0;
//_ammo = _args select 1;

_unit playMove "AinvPknlMstpSlayWrflDnon_medic";
sleep 5;
[] call (compile (format ["%1=true", _var]));
publicVariable _var;