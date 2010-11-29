// Loadingtext
if((not isNull player) and (time < 10) and isMultiplayer) then
{
    titleCut ["", "BLACK FADED", 999];
    finishMissionInit;
    [] spawn {
		sleep 0.1;
        waitUntil{!(isNil "BIS_fnc_init")};
        disableUserInput true;
        
        // Info text
        [str("CSAR Vodka"),
         str("Rescue The Pilot."),
         str(date select 0) + "." + str(date select 1) + "." + str(date select 2)] spawn BIS_fnc_infoText;
        
        sleep 8;
        titleCut ["", "BLACK IN", 5];
        sleep 5;
        disableUserInput false;
    };
};