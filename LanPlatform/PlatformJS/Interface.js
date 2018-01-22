/*
    UI
*/
var LPInterface = {};

LPInterface.PAGE_INACTIVE = "";
LPInterface.PAGE_UPDATE = "update";
LPInterface.PAGE_ALERT = "alert";
LPInterface.PAGE_URGENT = "urgent";
LPInterface.PAGE_ACTIVE = "active";

LPInterface.StatusNames = ["", "update", "alert", "urgent", "active"];

LPInterface.Initialize = function () {

}

LPInterface.NavSelect = function (section) {
    LPInterface.NavButton.NavClick(section);

    return;
}

LPInterface.SetSectionStatus = function (sectionName, status) {
    LPInterface.NavButton.SetNavStatus(sectionName, status);

    return;
}