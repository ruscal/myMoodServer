/// <reference path="jquery.js" />
/// <reference path="jquery.defaults.js" />
/// <reference path="jquery.serialization.js" />
/// <reference path="jquery.linq.js" />
/// <reference path="jquery.qtip.js" />
/// <reference path="jquery.placeholder.js" />

if (typeof console === "undefined") {
    console = { log: function () { } };
}

/* Add indexOf function to Array in IE */
if (!Array.prototype.indexOf) {
    Array.prototype.indexOf = function (elt /*, from*/) {
        var len = this.length >>> 0;

        var from = Number(arguments[1]) || 0;
        from = (from < 0)
         ? Math.ceil(from)
         : Math.floor(from);
        if (from < 0)
            from += len;

        for (; from < len; from++) {
            if (from in this &&
          this[from] === elt)
                return from;
        }
        return -1;
    };
}

/* Add various helper methods to String type */
String.prototype.replaceAll = function (stringToFind, stringToReplace) {
    var temp = this;
    var index = temp.indexOf(stringToFind);
    while (index != -1) {
        temp = temp.replace(stringToFind, stringToReplace);
        index = temp.indexOf(stringToFind);
    }
    return temp;
}

// prevent the backspace key from accidentally navigating back.
$(document).keydown(function (e) {
    var doPrevent;
    if (e.keyCode == 8) {
        var d = e.srcElement || e.target;
        if (d.tagName.toUpperCase() == 'INPUT' || d.tagName.toUpperCase() == 'TEXTAREA') {
            doPrevent = d.readOnly || d.disabled;
        }
        else
            doPrevent = true;
    }
    else
        doPrevent = false;

    if (doPrevent)
        e.preventDefault();
});

// watch for "abortable" ajax requests
var currentAjaxRequests = new Object();

$.ajaxPrefilter(function (options, originalOptions, jqXHR) {
    if (options.abortOnNew) {
        if (currentAjaxRequests[options.url]) {
            currentAjaxRequests[options.url].abort();
        }
        currentAjaxRequests[options.url] = jqXHR;
    }
});

// add support for input placeholder text
$(document).ready(function () {
    if ($.fn.placeholder != null) {
        $("input[placeholder],textarea[placeholder]").placeholder();
    }
});

function populateValidationMessagesFromModelState(formElement, modelState) {
    if (modelState != null) {
        $(formElement).find("input,select,textarea").removeClass("input-validation-error");
        $(formElement).find("span.field-validation-error").text("").removeClass("field-validation-error").addClass("field-validation-valid");
        $.each(modelState, function () {
            var ms = this;
            var inputName = ms.Key.replace(".", "\\.");
            var messages = $.Enumerable.From(ms.Errors).Where(function (e) { return e != ""; });

            $(formElement).find("input[name='" + inputName + "'], select[name='" + inputName + "'], textarea[name='" + inputName + "']").addClass("input-validation-error")
            $(formElement).find("span[data-valmsg-for='" + inputName + "']").text(messages.ToArray().join("\r\n\r\n")).addClass("field-validation-error").removeClass("field-validation-valid");
        });
    }
}

function setupValidationTooltips(formElement) {
    if (formElement.jQuery == null) formElement = $(formElement);
    if ($.fn.qtip != null && formElement.length > 0) {
        // attach handler for client-side validation
        var validator = formElement.get(0).tagName == 'FORM' ? formElement.data("validator") || $.data(formElement, 'validator') : formElement.find("form").data("validator");
        if (validator != null) {
            var oldErrorFunction = validator.settings.errorPlacement;
            validator.settings.errorPlacement = function (error, inputElement) {
                var errorText = error.text();
                var tooltipPosition = error.data("tooltippos") == "below" ? { my: "top center", at: "bottom center", viewport: $(window)} :
                error.data("tooltippos") == "above" ? { my: "bottom center", at: "top center", viewport: $(window)} :
                error.data("tooltippos") == "left" ? { my: "right center", at: "left center", viewport: $(window)} :
                { my: "left center", at: "right center", viewport: $(window) };
                var tooltipTarget = inputElement.parent().hasClass("multi-input-wrapper") ? inputElement.parent() : inputElement;
                if (errorText.length > 0) {
                    error.text("");
                    tooltipTarget.qtip({
                        content: errorText,
                        position: tooltipPosition,
                        style: { classes: 'ui-tooltip-red' },
                        show: { event: false, ready: true, effect: function () { $(this).fadeIn(500); } },
                        hide: { effect: function () { $(this).fadeOut(500); } }
                    })
                    .qtip("show");
                } else {
                    tooltipTarget.qtip("destroy");
                }
            };
        }

        // perform find-and-replace for elements rendered by server-side validation
        formElement.find("input.input-validation-error, select.input-validation-error, textarea.input-validation-error").each(function () {
            var inputElement = $(this);
            var inputName = $(this).attr("name").replace(".", "\\.");
            var error = $("span.field-validation-error[data-valmsg-for=" + inputName + "]");
            var errorText = error.text();
            if (errorText.length > 0) {
                error.text("");
                var tooltipPosition = error.data("tooltippos") == "below" ? { my: "top center", at: "bottom center", viewport: $(window)} :
                error.data("tooltippos") == "above" ? { my: "bottom center", at: "top center", viewport: $(window)} :
                error.data("tooltippos") == "left" ? { my: "right center", at: "left center", viewport: $(window)} :
                { my: "left center", at: "right center", viewport: $(window) };
                var tooltipTarget = inputElement.parent().hasClass("multi-input-wrapper") ? inputElement.parent() : inputElement;
                tooltipTarget.qtip({
                    content: errorText,
                    position: tooltipPosition,
                    style: { classes: 'ui-tooltip-red' },
                    show: { event: false, ready: true, effect: function () { $(this).fadeIn(500); } },
                    hide: { effect: function () { $(this).fadeOut(500); } }
                })
            .qtip("show");
            }
        });
    }
}

function clearValidationTooltips(formElement) {
    if ($.fn.qtip != null) {
        $(formElement).find("input, select, textarea, .multi-input-wrapper").each(function () {
            $(this).qtip("destroy");
        });
    }
}

function enhanceFormInputs(form) {
    setupValidationTooltips(form);
    form.find('input[data-val-required],textarea[data-val-required],select[data-val-required]').each(function () {
        $('label[for="' + $(this).attr('id') + '"]').addClass("required-field");
    });
    if ($.ui.datepicker != null) {
        form.find("input.date-picker").each(function (i, item) {
            if ($(item).attr("data-minDate") !== undefined) {
                $(item).datepicker({ minDate: $(item).attr("data-minDate") });
            }
            else {
                $(item).datepicker();
            }
        });
    }
    if ($.ui.dropdownchecklist != null) {
        form.find("select[multiple]").each(function () { $(this).dropdownchecklist({ icon: true, maxDropHeight: 200, emptyText: $(this).data("emptytext") || "None" }); });
    }
    if ($.fn.placeholder != null) {
        form.find("input[placeholder],textarea[placeholder]").placeholder();
    }
}

function showDialogForm(options) {
    options = $.extend({
        contentUrl: null,
        contentSelector: null,
        "class": "",
        autoFocusFirstInput: true,
        width: 450,
        height: "auto",
        maxWidth: null,
        maxHeight: null,
        buttons: [
            { text: "OK", click: function () { submitDialogForm(this, { onSuccess: function () { $(this).dialog("close"); } }); } },
            { text: "Cancel", click: function () { $(this).dialog("close"); } }
        ],
        onShow: null
    },
    options);

    var dlg = null;

    if (options.contentUrl) {
        var dialogId = "dynamic-dialog-" + new Date().getTime();
        dlg = $("<div id='" + dialogId + "' class='dynamic-dialog'></div>").appendTo("body");
    } else {
        dlg = $(options.contentSelector);
    }

    dlg.addClass(options["class"]).dialog({
        title: options.title || dlg.find("h1,h2,h3").first().text(),
        width: options.width,
        height: options.height,
        maxWidth: options.maxWidth,
        maxHeight: options.maxHeight,
        buttons: options.buttons,
        open: function () {
            dlg.closest(".ui-dialog").css({ maxHeight: options.maxHeight || "", maxWidth: options.maxWidth || "" });
            if (options.contentUrl) {
                dlg.html("<span class='loading-message'><span class='loading-indicator'></span> Loading...</span>");
                dlg.closest(".ui-dialog").find(".ui-dialog-buttonpane").hide();
                $.ajax({
                    url: options.contentUrl,
                    type: "GET",
                    dataType: "html",
                    cache: false,
                    success: function (data, status, xhr) {
                        dlg.html(data);
                        dlg.closest(".ui-dialog").find(".ui-dialog-title").text(dlg.find("h1,h2,h3").first().text() || dlg.dialog("option", "title"));
                        dlg.closest(".ui-dialog").find(".ui-dialog-buttonpane").show();
                        enhanceFormInputs(dlg.find("form"));
                        if (options.maxHeight) {
                            var dlgPaddingAndMargin = dlg.outerHeight(true) - dlg.height();
                            dlg.css("max-height", (dlg.closest(".ui-dialog").height() - dlg.closest(".ui-dialog").find(".ui-dialog-title").outerHeight(true) - dlg.closest(".ui-dialog").find(".ui-dialog-buttonpane").outerHeight(true) - dlgPaddingAndMargin) + "px");
                        }
                        dlg.dialog("option", "position", dlg.dialog("option", "position"));
                        if (options.autoFocusFirstInput) dlg.find("input[type!=hidden],select,textarea").filter(":visible").first().focus();
                        if ($.isFunction(options.onShow)) options.onShow.call(dlg);
                    },
                    error: function () {
                        dlg.html("<div class='load-error'><span class='load-error-icon'></span> An error occurred while trying to load this dialog</div>");
                    }
                });
            } else {
                enhanceFormInputs(dlg.find("form"));
                if (options.maxHeight) {
                    var dlgPaddingAndMargin = dlg.outerHeight(true) - dlg.height();
                    dlg.css("max-height", (dlg.closest(".ui-dialog").height() - dlg.closest(".ui-dialog").find(".ui-dialog-title").outerHeight(true) - dlg.closest(".ui-dialog").find(".ui-dialog-buttonpane").outerHeight(true) - dlgPaddingAndMargin) + "px");
                }
                if (options.autoFocusFirstInput) dlg.find("input[type!=hidden],select,textarea").filter(":visible").first().focus();
                if ($.isFunction(options.onShow)) options.onShow.call(dlg);
            }
        },
        beforeClose: function () {
            clearValidationTooltips(dlg.find("form"));
        },
        close: function () {
            if (dlg.hasClass("dynamic-dialog")) {
                setTimeout(function () { dlg.remove(); }, 0);
            } else if (dlg.data("empty-form") != null) {
                dlg.html(dlg.data("empty-form"));
            }
        }
    });
}

function submitDialogForm(dlg, options) {
    options = $.extend({ onSuccess: null, onFailure: null, onModelStateError: null, onUnhandledError: null, onComplete: null, sendAsync: true, abortOnNew: true, sendAsJson: true, extraData: null, disableInputs: true, waitMessage: "Saving..." }, options);

    var form = $(dlg).find("form");
    clearValidationTooltips(form);

    var files = form.find("input[type=file]");
    var formData = files.length > 0 ? form.serializeArray().concat($.deserializeArray(options.extraData)) : options.sendAsJson ? JSON.stringify($.extend(form.serializeObject(), options.extraData || {})) : form.serialize() + "&" + options.extraData

    $.ajax({
        url: form.attr("action"),
        type: form.attr("method"),
        contentType: files.length == 0 && options.sendAsJson ? "application/json; charset=utf-8" : "application/x-www-form-urlencoded",
        dataType: "json",
        data: formData,
        iframe: files.length > 0,
        processData: files.length == 0,
        files: files,
        async: options.sendAsync,
        abortOnNew: options.abortOnNew,
        cache: false,
        beforeSend: function (xhr, settings) {
            if (options.disableInputs) {
                $(dlg).find("input[type!=file],textarea,select").attr("disabled", true);
                $(dlg).find("input[type=file]").attr("readonly", true);
            }
            if (options.waitMessage) {
                $(dlg).closest(".ui-dialog").find(".ui-dialog-titlebar-close").hide();
                $(dlg).closest(".ui-dialog").find(".ui-dialog-buttonpane").each(function () {
                    $(this).find(".ui-dialog-buttonset").hide();
                    $(this).append("<span class='loading-message'><span class='loading-indicator'></span> " + options.waitMessage + "</span>");
                });
            }
        },
        success: function (data, status, xhr) {
            if (data.success == true || data.Success == true) {
                clearValidationTooltips(form);
                if ($.isFunction(options.onSuccess)) options.onSuccess.call(dlg, data);
            } else {
                if (data.formWithErrorMessages || data.FormWithErrorMessages) {
                    $(dlg).html(data.formWithErrorMessages || data.FormWithErrorMessages);
                    enhanceFormInputs($(dlg).find("form"));
                } else if (data.modelState || data.ModelState) {
                    if ($.isFunction(options.onModelStateError)) {
                        options.onModelStateError.call(dlg, data.modelState || data.ModelState);
                    } else {
                        populateValidationMessagesFromModelState(form, data.modelState || data.ModelState);
                        setupValidationTooltips(form);
                    }
                }
                if ($.isFunction(options.onFailure)) options.onFailure.call(dlg, data);
            }
        },
        error: function (xhr, status, err) {
            if (xhr.status != 0 && status != "abort") {
                if ($.isFunction(options.onUnhandledError)) {
                    options.onUnhandledError.call(dlg, { xhr: xhr, status: status, err: err });
                } else {
                    showErrorMessage({ title: "An Error Has Occurred", text: "Sorry, but your request could not be processed due to an unexpected error.\r\n\r\n" + err.toString() });
                }
            }
        },
        complete: function () {
            if (options.disableInputs) {
                $(dlg).find("input[type!=file],textarea,select").removeAttr("disabled");
                $(dlg).find("input[type=file]").removeAttr("readonly");
            }
            if ($.isFunction(options.onComplete)) {
                options.onComplete.call(dlg);
            }
            if (options.waitMessage) {
                $(dlg).closest(".ui-dialog").find(".ui-dialog-titlebar-close").show();
                $(dlg).closest(".ui-dialog").find(".ui-dialog-buttonpane").each(function () {
                    $(this).find(".loading-message").fadeTo(200, 0.0, function () { $(this).siblings().show(); $(this).remove(); });
                });
            }
        }
    });
}

function confirmYesNo(options) {
    options = $.extend({ title: "Please Confirm", text: "", yesButtonText: "Yes", noButtonText: "No", onYes: null, onNo: null }, options);

    if ($.fn.qtip != null) {
        $("<div></div>").qtip({
            id: "confirmYesNo",
            content: {
                text: options.text + "<div class='buttons'><button class='yes'>" + options.yesButtonText + "</button><button class='no'>" + options.noButtonText + "</button></div>",
                title: { text: options.title, button: false }
            },
            position: { my: "center", at: "center", target: $(window), viewport: $(window) },
            style: { classes: "ui-tooltip-discover" },
            show: { solo: true, modal: { on: true, blur: false, escape: false, effect: function () { $(this).fadeIn(300); } }, effect: function () { $(this).fadeIn(500); } },
            hide: { effect: function () { $(this).fadeOut(500); } }
        })
        .qtip("show");

        $("#ui-tooltip-confirmYesNo button.yes").click(function () { $("#ui-tooltip-confirmYesNo").qtip("hide"); $("#ui-tooltip-confirmYesNo").delay(600).qtip("destroy"); if ($.isFunction(options.onYes)) options.onYes(); });
        $("#ui-tooltip-confirmYesNo button.no").click(function () { $("#ui-tooltip-confirmYesNo").qtip("hide"); $("#ui-tooltip-confirmYesNo").delay(600).qtip("destroy"); if ($.isFunction(options.onNo)) options.onNo(); });
    } else {
        if (confirm(options.text)) {
            if ($.isFunction(options.onYes)) options.onYes();
        } else {
            if ($.isFunction(options.onNo)) options.onNo();
        }
    }
}

function showErrorMessage(options) {
    if (typeof options == "string") options = { text: options };
    options = $.extend({ title: "!", text: "", okButtonText: "OK", onOk: null }, options);

    if ($.fn.qtip != null) {
        $("<div></div>").qtip({
            id: "showErrorMessage",
            content: {
                text: options.text + "<div class='buttons'><button class='ok'>" + options.okButtonText + "</button></div>",
                title: { text: options.title, button: false }
            },
            position: { my: "center", at: "center", target: $(window), viewport: $(window) },
            style: { classes: "ui-tooltip-discover" },
            show: { solo: true, modal: { on: true, blur: false, escape: false, effect: function () { $(this).fadeIn(300); } }, effect: function () { $(this).fadeIn(500); } },
            hide: { effect: function () { $(this).fadeOut(500); } }
        })
        .qtip("show");

        $("#ui-tooltip-showErrorMessage button.ok").click(function () { $("#ui-tooltip-showErrorMessage").qtip("hide"); $("#ui-tooltip-showErrorMessage").delay(600).qtip("destroy"); if ($.isFunction(options.onOk)) options.onOk(); });
    } else {
        alert(options.text);
    }
}

function showModelStateErrors(modelState) {
    var messages = $.Enumerable.From(modelState).SelectMany(function (ms) { return $.Enumerable.From(ms.Errors).Where(function (e) { return e != ""; }); });
    if (messages.Any()) {
        showErrorMessage(messages.ToArray().join("\r\n\r\n"));
    }
}

function fillParentHeight(element) {
    $(element).each(function () {
        var siblingHeight = 0;
        $(this).siblings().each(function () { siblingHeight += $(this).outerHeight(true); });
        $(this).css({ /*boxSizing:"border-box",*/height: ($(this).parent().innerHeight() - siblingHeight) });
    });
}

function refreshHttps() {
    window.location.href = window.location.href.replace("http:", "https:");
}

function Goto(url) {
    window.location.href = url;
}