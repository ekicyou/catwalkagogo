_count = ArtyOfficer getVariable "Arty_Count";
_isAct = [];
_i = 0;
{
	if(_x > 0) then{
		_isAct = _isAct + ["1"];
	}else{
		_isAct = _isAct + ["0"];
	};
} forEach _count;

Arty_MENU_inCommunication = 
[
	// First array: "User menu" This will be displayed under the menu, bool value: has Input Focus or not.
	// Note that as to version Arma2 1.05, if the bool value set to false, Custom Icons will not be displayed.
	["User menu",false],
	// Syntax and semantics for following array elements:
	// ["Title_in_menu", [assigned_key], "Submenu_name", CMD, [["expression",script-string]], "isVisible", "isActive" <, optional icon path> ]
	// Title_in_menu: string that will be displayed for the player
	// Assigned_key: 0 - no key, 1 - escape key, 2 - key-1, 3 - key-2, ... , 10 - key-9, 11 - key-0, 12 and up... the whole keyboard
	// Submenu_name: User menu name string (eg "#USER:MY_SUBMENU_NAME" ), "" for script to execute.
	// CMD: (for main menu:) CMD_SEPARATOR -1; CMD_NOTHING -2; CMD_HIDE_MENU -3; CMD_BACK -4; (for custom menu:) CMD_EXECUTE -5
	// script-string: command to be executed on activation. (no arguments passed)
	// isVisible - Boolean 1 or 0 for yes or no, - or optional argument string, eg: "CursorOnGround"
	// isActive - Boolean 1 or 0 for yes or no - if item is not active, it appears gray.
	// optional icon path: The path to the texture of the cursor, that should be used on this menuitem.
	["Cancel", [0], "", -3, [["expression", ""]], "1", "1"],
	["", [0], "", -1, [["expression", ""]], "1", "1"],
	["WP", [2], "", -5, [["expression", "temp=[0] execVM 'scripts\RequestArty.sqf'"]], "1", _isAct select 0],
	["WP / Illumination", [3], "", -5, [["expression", "temp=[1] execVM 'scripts\RequestArty.sqf'"]], "1", _isAct select 1],
	["Illumination", [4], "", -5, [["expression", "temp=[2] execVM 'scripts\RequestArty.sqf'"]], "1", _isAct select 2],
	["Laser", [5], "", -5, [["expression", "temp=[3] execVM 'scripts\RequestArty.sqf'"]], "1", _isAct select 3],
	["HE", [6], "", -5, [["expression", "temp=[4] execVM 'scripts\RequestArty.sqf'"]], "1", _isAct select 4]
];

showCommandingMenu "#USER:Arty_MENU_inCommunication";
