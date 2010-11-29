waitUntil {!(isNull player)};
waitUntil {player==player};

player createDiaryRecord ["Diary",["Credit","Mission Editor: Strato"]];

_title = "Seize Trenta";
tskTrenta=player createSimpleTask [_title];
tskTrenta setSimpleTaskDescription [_title, _title, _title];
tskTrenta setSimpleTaskDestination (getMarkerPos "Trenta");
_title = "Seize Dom Soca";
tskDomSoca=player createSimpleTask [_title];
tskDomSoca setSimpleTaskDescription [_title, _title, _title];
tskDomSoca setSimpleTaskDestination (getMarkerPos "DomSoca");
_title = "Seize Dom Krnica";
tskDomKrnica=player createSimpleTask [_title];
tskDomKrnica setSimpleTaskDescription [_title, _title, _title];
tskDomKrnica setSimpleTaskDestination (getMarkerPos "DomKrnica");