/// <reference path="jquery.js" />
/// <reference path="jquery-ui.js" />

// set datepicker widget defaults
$.datepicker.setDefaults({
    dateFormat: "dd M yy"
});

// set dialog defaults
$.extend($.ui.dialog.prototype.options, {
    modal: true,
    resizable: false,
    closeOnEscape: false,
    show: "fade",
    hide: "fade"
});

$(document).ready(function () {

    // reposition dialogs upon window resize
    $(window).resize(function () {
        $(".ui-dialog-content").each(function () { $(this).dialog("option", "position", $(this).dialog("option", "position")); });
    });

    // keep dialogs in view upon page scroll
    $(window).scroll(function () {
        $(".ui-dialog-content").each(function () { $(this).dialog("option", "position", $(this).dialog("option", "position")); });
    });

});