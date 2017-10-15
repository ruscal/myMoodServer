/// <reference path="jquery.js" />

(function ($, undefined) {

    $.fn.serializeObject = function () {
        var obj = {};

        // NB: filter out hidden fields that are included solely to ensure values are posted for MVC model-binding of booleans
        var inputs = this.find('input,select,textarea').filter(function () {
            var input = $(this);
            return !(input.attr("type") == "hidden" && input.prev("input[type=checkbox][name=" + input.attr("name") + "]:checked").length > 0);
        });

        $.each(inputs.serializeArray(), function (i, o) {
            var n = o.name, v = o.value;

            obj[n] = (obj[n] === undefined ? v : ($.isArray(obj[n]) ? obj[n].concat(v) : [obj[n], v]));
        });

        return obj;
    };

    $.deserializeArray = function (serializedValues) {
        var result = new Array();

        if (serializedValues != null) {
            if (typeof serializedValues == "string") {
                $.each(serializedValues.split('&'), function (i) {
                    var parts = this.split('=');
                    result.push({
                        name: decodeURIComponent(parts[0]),
                        value: decodeURIComponent(parts[1].replace(/\+/g, "%20"))
                    });
                });
            } else if ($.isPlainObject(serializedValues)) {
                for (var key in serializedValues) {
                    if ($.isArray(serializedValues[key])) {
                        $.each(serializedValues[key], function () { result.push({ name: key, value: this }); });
                    } else {
                        result.push({ name: key, value: serializedValues[key] });
                    }
                }
            }
        }

        return result;
    }

})(jQuery);