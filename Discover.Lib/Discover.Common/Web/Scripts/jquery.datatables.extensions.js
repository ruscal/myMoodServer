/// <reference path="jquery.js" />
/// <reference path="jquery.datatables.js" />

(function ($) {

	// auto-detection of percentage values
	$.fn.dataTableExt.aTypes.unshift(
		function (sData) {
    		if (sData !== null && sData.match(/^-?\d+(\.\d+)?%$/)) {
    			return 'percent';
    		}
    		return null;
		}
	);

	// numeric sorting for percentage values
	$.extend($.fn.dataTableExt.oSort, {
		"percent-pre": function (a) {
			var x = (a == "-") ? 0 : a.replace(/%/, "");
			return parseFloat(x);
		},

		"percent-asc": function (a, b) {
			return ((a < b) ? -1 : ((a > b) ? 1 : 0));
		},

		"percent-desc": function (a, b) {
			return ((a < b) ? 1 : ((a > b) ? -1 : 0));
		}
	});

	// auto-detection of integer "sortvalue" data attribute attached to span wrapped around the actual display value
	$.fn.dataTableExt.aTypes.unshift(
		function (sData) {
			if (sData !== null && sData.match(/^\s*<span[^>]*data-sortvalue=['\"](\d+)['\"][^>]*>(.+)<\/span>\s*$/i)) {
				return 'sortvalue-wrapper-numeric';
			}
			return null;
		}
	);

	// sort based on integer "sortvalue" data attribute attached to span wrapped around the actual display value
	$.extend($.fn.dataTableExt.oSort, {
		"sortvalue-wrapper-numeric-pre": function (a) {
			var m = /^\s*<span[^>]*data-sortvalue=['\"](\d+)['\"][^>]*>(.+)<\/span>\s*$/i.exec(a);
			return parseInt(m[1]);
		},

		"sortvalue-wrapper-numeric-asc": function (a, b) {
			return ((a < b) ? -1 : ((a > b) ? 1 : 0));
		},

		"sortvalue-wrapper-numeric-desc": function (a, b) {
			return ((a < b) ? 1 : ((a > b) ? -1 : 0));
		}
	});

	// auto-detection of string "sortvalue" data attribute attached to span wrapped around the actual display value
	$.fn.dataTableExt.aTypes.unshift(
		function (sData) {
			if (sData !== null && sData.match(/^\s*<span[^>]*data-sortvalue=['\"](\D[^'\"]*)['\"][^>]*>(.+)<\/span>\s*$/i)) {
				return 'sortvalue-wrapper-string';
			}
			return null;
		}
	);

	$.extend($.fn.dataTableExt.oSort, {
		"sortvalue-wrapper-string-pre": function (a) {
			var m = /^\s*<span[^>]*data-sortvalue=['\"](\D[^'\"]*)['\"][^>]*>(.+)<\/span>\s*$/i.exec(a);
			return m[1];
		},

		"sortvalue-wrapper-string-asc": function (a, b) {
			return ((a < b) ? -1 : ((a > b) ? 1 : 0));
		},

		"sortvalue-wrapper-string-desc": function (a, b) {
			return ((a < b) ? 1 : ((a > b) ? -1 : 0));
		}
	});

})(jQuery);