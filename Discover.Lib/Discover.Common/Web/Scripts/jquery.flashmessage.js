/// <reference path="jquery.js" />
/// <reference path="jquery.cookie.js" />

(function ($) {
    $.fn.flashMessage = function (options) {
        var target = this;

        var defaultOptions = {
            messageClass: null, 
            timeout: 3000, 
            onHide: null
        }
        options = $.extend(defaultOptions, options);

        if (!options.message) {
            var parts = $.cookie("FlashMessage");
            if (parts != null) {
                parts = parts.split('|');
                options.messageClass = parts.length == 1 ? null : parts[0];
                options.message = parts.length == 1 ? parts[0] : parts[1];
                $.cookie("FlashMessage", null, { path: '/' });
            }
        }

        if (options.messageClass) {
            $(target).addClass(options.messageClass);
        }

        if (options.message) {
            if (typeof options.message === "string") {
                target.html("<span>" + options.message + "</span>");
            } else {
                target.empty().append(options.message);
            }
        }

        if (target.children().length === 0) return;

        target.fadeIn().one("click", function () {
            $(this).fadeOut(500, function () { if ($.isFunction(options.onHide)) options.onHide.call(this); });
        });

        if (options.timeout > 0) {
            setTimeout(function () { target.fadeOut(500, function () { if ($.isFunction(options.onHide)) options.onHide.call(this); }); }, options.timeout);
        }

        return this;
    };
})(jQuery);