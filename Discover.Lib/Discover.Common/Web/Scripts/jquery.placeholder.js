﻿/** 
* placeholder - HTML5 input placeholder polyfill
* Copyright (c) 2012 DIY Co
*
* Licensed under the Apache License, Version 2.0 (the "License"); you may not use this 
* file except in compliance with the License. You may obtain a copy of the License at:
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software distributed under 
* the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF 
* ANY KIND, either express or implied. See the License for the specific language 
* governing permissions and limitations under the License.
*
* @author Brian Reavis <brian@diy.org>
*/

; (function ($) {

    $.fn.placeholder = function (opts) {
        var $this = this;

        opts = $.extend({
            force: false
        }, opts);

        window.setTimeout(function () {

            if (!('placeholder' in document.createElement('input')) || opts.force) {
                $this.each(function () {
                    var input = this, $input = $(this);
                    var tagName = this.tagName.toLowerCase();
                    if (tagName === 'input' || tagName === 'textarea') {
                        var text = $(this).attr('placeholder') || '';
                        if (text.length) {
                            var $placeholder = $('<span>').addClass('placeholder').html(text);

                            // clear existing placeholder
                            $input.data('placeholder', text);
                            $input.removeAttr('placeholder');

                            // mimic css styles applied to textbox
                            var cssProperties = [
								'-moz-box-sizing', '-webkit-box-sizing', 'box-sizing', 'padding-top', 'padding-right', 'padding-bottom', 'padding-left',
								'margin-top', 'margin-right', 'margin-bottom', 'margin-left', 'border-top-width', 'border-right-width', 'border-bottom-width', 'border-left-width',
								'line-height', 'font-size', 'font-family', 'width', 'height', 'top', 'left', 'right', 'bottom'
							];
                            for (var i = 0; i < cssProperties.length; i++) {
                                $placeholder.css(cssProperties[i], $input.css(cssProperties[i]));
                            }
                            var zIndex = parseInt($placeholder.css('z-index'));
                            if (isNaN(zIndex) || !zIndex) zIndex = 1;

                            // css overrides
                            $placeholder.css({
                                'cursor': $input.css('cursor') || 'text',
                                'display': 'block',
                                'position': 'absolute',
                                'overflow': 'hidden',
                                'z-index': zIndex + 1,
                                'background': 'none',
                                'border-top-style': 'solid',
                                'border-right-style': 'solid',
                                'border-bottom-style': 'solid',
                                'border-left-style': 'solid',
                                'border-top-color': 'transparent',
                                'border-right-color': 'transparent',
                                'border-bottom-color': 'transparent',
                                'border-left-color': 'transparent'
                            });

                            $placeholder.insertBefore($input);

                            // compensate for y difference caused by absolute / relative difference (line-height factor)
                            var dy = $input.offset().top - $placeholder.offset().top;
                            var marginTop = parseInt($placeholder.css('margin-top'));
                            if (isNaN(marginTop)) marginTop = 0;
                            $placeholder.css('margin-top', marginTop + dy);

                            // show / hide
                            $placeholder.on('mousedown', function () {
                                window.setTimeout(function () {
                                    $input.trigger('focus');
                                }, 0);
                            });
                            $input.on('focus.placeholder', function () {
                                $placeholder.hide();
                            });
                            $input.on('blur.placeholder', function () {
                                if (!$.trim($input.val()).length) {
                                    $placeholder.show();
                                }
                            });

                            $input.trigger('blur.placeholder');
                        }
                    }
                });
            }

        }, 0);

        return this;
    };

})(jQuery);