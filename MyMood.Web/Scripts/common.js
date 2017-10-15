
$(document).ready(function () {
    $("<div class='flash-message'></div>").css({ display: "none" }).appendTo("body").flashMessage();

    $(".infoIcon").qtip().click(function () { return false; });

    $.ajaxSetup({
        type: "POST",
        cache: false,
        dataType: "json",
        statusCode: {
            404: function () {
                //alert("AJAX 404: Page not found");
            }
        },
        error: function (xhr, status, err) {
            showErrorMessage({ title: "An Error Has Occurred", text: "Sorry, but your request could not be processed due to an unexpected error.\r\n\r\n" + err.toString() });
        }
    });

});

function refreshPage() {
    window.location.reload();
}
