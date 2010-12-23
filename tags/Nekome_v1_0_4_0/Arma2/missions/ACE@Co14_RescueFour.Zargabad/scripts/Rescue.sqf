_captive = _this select 0;
_caller = _this select 1;

if((leader _caller) != _caller) exitWith{
	hint "Only leader can make the captive join own group.";
};
if((group _captive) == (group _caller)) exitWith{
	hint "The captive has already been in your group.";
};
if((leader _captive) != _captive) exitWith{
	hint "The captive has already been in other group.";
};

[_captive] join (group _caller);