﻿/// <reference path="jquery.js" />

; (function ($) {
    /*
    * ui.dropdownchecklist
    *
    * Copyright (c) 2008-2010 Adrian Tosca, Copyright (c) 2010-2011 Ittrium LLC
    * Dual licensed under the MIT (MIT-LICENSE.txt)
    * and GPL (GPL-LICENSE.txt) licenses.
    *
    * MODIFIED by Adam Dixon-Chapman (2011)
    * - added support for arbitrary images as option icons (size controlled by CSS)
    * - added support for suppressing radio buttons when in single-select mode
    * - added support for a "show more" item (with appropriate handler/callback) if more than X items are present in the list
    * - added support for icons for each option/item (with fullsize popup triggered by hover with delay)
    * - added support for callback before showing the dropdown conatiner (with ability to cancel by returning false) 
    * - many many behavioural fixes
    */
    // The dropdown check list jQuery plugin transforms a regular select html element into a dropdown check list.
    $.widget("ui.dropdownchecklist", {
        // Some globlals
        // $.ui.dropdownchecklist.gLastOpened - keeps track of last opened dropdowncheck list so we can close it
        // $.ui.dropdownchecklist.gIDCounter - simple counter to provide a unique ID as needed

        options: {
            width: null
            , maxDropHeight: null
            , firstItemChecksAll: false
            , closeRadioOnClick: true
            , minWidth: 50
            , positionHow: 'absolute'
            , bgiframe: false
            , explicitClose: null
            , showRadioButtonsForSingleSelect: false
            , onErrorImageUrl: null
            , showMoreItemLimit: null
            , alwaysIncludeShowMore: false
            , showSelectedItemImageAsTooltip: true
            , onShowDropdown: null // function(selector) { return true; }
        },
        version: function () {
            alert('DropDownCheckList v1.2qa - MODIFIED');
        },
        // Creates the drop container that keeps the items and appends it to the document
        _appendDropContainer: function (controlItem) {
            var wrapper = $("<div/>");
            // the container is wrapped in a div
            wrapper.addClass("ui-dropdownchecklist ui-dropdownchecklist-dropcontainer-wrapper");
            wrapper.addClass("ui-widget");
            // assign an id
            wrapper.attr("id", controlItem.attr("id") + '-ddw');
            // initially positioned way off screen to prevent it from displaying
            // NOTE absolute position to enable width/height calculation
            wrapper.css({ position: 'absolute', left: "-33000px", top: "-33000px" });

            var container = $("<div/>"); // the actual container
            container.addClass("ui-dropdownchecklist-dropcontainer ui-widget-content");
            container.css("overflow-y", "auto");
            wrapper.append(container);

            // insert the dropdown after the master control to try to keep the tab order intact
            // if you just add it to the end, tabbing out of the drop down takes focus off the page
            // @todo 22Sept2010 - check if size calculation is thrown off if the parent of the
            //		selector is hidden.  We may need to add it to the end of the document here, 
            //		calculate the size, and then move it back into proper position???
            $(document.body).append(wrapper);
            //wrapper.insertAfter(controlItem);

            // flag that tells if the drop container is shown or not
            wrapper.isOpen = false;
            return wrapper;
        },
        // Look for browser standard 'open' on a closed selector
        _isDropDownKeyShortcut: function (e, keycode) {
            return e.altKey && ($.ui.keyCode.DOWN == keycode); // Alt + Down Arrow
        },
        // Look for key that will tell us to close the open dropdown
        _isDropDownCloseKey: function (e, keycode) {
            return ($.ui.keyCode.ESCAPE == keycode) || ($.ui.keyCode.ENTER == keycode);
        },
        // Handler to change the active focus based on a keystroke, moving some count of
        // items from the element that has the current focus
        _keyFocusChange: function (target, delta, limitToItems) {
            // Find item with current focus
            var focusables = $(":focusable");
            var index = focusables.index(target);
            if (index >= 0) {
                index += delta;
                if (limitToItems) {
                    // Bound change to list of input elements
                    var allCheckboxes = this.dropWrapper.find("input:not([disabled])");
                    var firstIndex = focusables.index(allCheckboxes.get(0));
                    var lastIndex = focusables.index(allCheckboxes.get(allCheckboxes.length - 1));
                    if (index < firstIndex) {
                        index = lastIndex;
                    } else if (index > lastIndex) {
                        index = firstIndex;
                    }
                }
                focusables.get(index).focus();
            }
        },
        // Look for navigation, open, close (wired to keyup)
        _handleKeyboard: function (e) {
            var self = this, sourceSelect = this.sourceSelect, options = this.options;
            var keyCode = (e.keyCode || e.which);
            if (!self.dropWrapper.isOpen && self._isDropDownKeyShortcut(e, keyCode)) {
                // Key command to open the dropdown
                e.stopImmediatePropagation();
                if ($.isFunction(options.onShowDropdown) && options.onShowDropdown.call(sourceSelect, sourceSelect) == false) return;
                self._toggleDropContainer(true);
            } else if (self.dropWrapper.isOpen && self._isDropDownCloseKey(e, keyCode)) {
                // Key command to close the dropdown (but we retain focus in the control)
                e.stopImmediatePropagation();
                self._toggleDropContainer(false);
                self.controlSelector.focus();
            } else if (self.dropWrapper.isOpen
					&& (e.target.type == 'checkbox')
					&& ((keyCode == $.ui.keyCode.DOWN) || (keyCode == $.ui.keyCode.UP))) {
                // Up/Down to cycle throught the open items
                e.stopImmediatePropagation();
                self._keyFocusChange(e.target, (keyCode == $.ui.keyCode.DOWN) ? 1 : -1, true);
            } else if (self.dropWrapper.isOpen && (keyCode == $.ui.keyCode.TAB)) {
                // I wanted to adjust normal 'tab' processing here, but research indicates
                // that TAB key processing is NOT a cancelable event. You have to use a timer
                // hack to pull the focus back to where you want it after browser tab
                // processing completes.  Not going to work for us.
                //e.stopImmediatePropagation();
                //self._keyFocusChange(e.target, (e.shiftKey) ? -1 : 1, true);
            }
        },
        // Look for change of focus
        _handleFocus: function (e, focusIn, forDropdown) {
            var self = this;
            if (forDropdown && !self.dropWrapper.isOpen) {
                // if the focus changes when the control is NOT open, mark it to show where the focus is/is not
                e.stopImmediatePropagation();
                if (focusIn) {
                    self.controlSelector.addClass("ui-state-hover");
                    if ($.ui.dropdownchecklist.gLastOpened != null) {
                        $.ui.dropdownchecklist.gLastOpened._toggleDropContainer(false);
                    }
                } else {
                    self.controlSelector.removeClass("ui-state-hover");
                }
            } else if (!forDropdown && !focusIn) {
                // The dropdown is open, and an item (NOT the dropdown) has just lost the focus.
                // we really need a reliable method to see who has the focus as we process the blur,
                // but that mechanism does not seem to exist.  Instead we rely on a delay before
                // posting the blur, with a focus event cancelling it before the delay expires.
                if (e != null) { e.stopImmediatePropagation(); }
                self.controlSelector.removeClass("ui-state-hover");
                self._toggleDropContainer(false);
            }
        },
        // Clear the pending change of focus, which keeps us 'in' the control
        _cancelBlur: function (e) {
            var self = this;
            if (self.blurringItem != null) {
                clearTimeout(self.blurringItem);
                self.blurringItem = null;
            }
        },
        // Creates the control that will replace the source select and appends it to the document
        // The control resembles a regular select with single selection
        _appendControl: function () {
            var self = this, sourceSelect = this.sourceSelect, options = this.options;

            // the control is wrapped in a basic container
            // inline-block at this level seems to give us better size control
            var wrapper = $("<span/>");
            wrapper.addClass("ui-dropdownchecklist ui-dropdownchecklist-selector-wrapper ui-widget");
            wrapper.css({ display: "inline-block", cursor: "default", overflow: "hidden" });

            // assign an ID 
            var baseID = sourceSelect.attr("id");
            if ((baseID == null) || (baseID == "")) {
                baseID = "ddcl-" + $.ui.dropdownchecklist.gIDCounter++;
            } else {
                baseID = "ddcl-" + baseID;
            }
            wrapper.attr("id", baseID);

            // the actual control which you can style
            // inline-block needed to enable 'width' but has interesting problems cross browser
            var control = $("<span/>");
            control.addClass("ui-dropdownchecklist-selector ui-state-default");
            control.css({ display: "inline-block", overflow: "hidden", 'white-space': 'nowrap' });
            // Setting a tab index means we are interested in the tab sequence
            var tabIndex = sourceSelect.attr("tabIndex");
            if (tabIndex == null) {
                tabIndex = 0;
            } else {
                tabIndex = parseInt(tabIndex);
                if (tabIndex < 0) {
                    tabIndex = 0;
                }
            }
            control.attr("tabIndex", tabIndex);
            control.keyup(function (e) { self._handleKeyboard(e); });
            control.focus(function (e) { self._handleFocus(e, true, true); });
            control.blur(function (e) { self._handleFocus(e, false, true); });
            wrapper.append(control);

            // the optional icon
            if (options.icon != null) {
                var anIcon = $("<div/>");
                anIcon.addClass("ui-icon");
                anIcon.addClass((options.icon.toOpen != null) ? options.icon.toOpen : "ui-icon-triangle-1-e");
                if (options.icon.placement != null) anIcon.css({ 'float': options.icon.placement });
                control.append(anIcon);
            }
            // the text container keeps the control text that is built from the selected (checked) items
            // inline-block needed to prevent long text from wrapping to next line when icon is active
            var textContainer = $("<span/>");
            textContainer.addClass("ui-dropdownchecklist-text");
            textContainer.css({ display: "inline-block", 'white-space': "nowrap", overflow: "hidden" });
            control.append(textContainer);

            // add the hover styles to the control
            wrapper.hover(
	            function () {
	                if (!self.disabled) {
	                    control.addClass("ui-state-hover");
	                }
	            }
	        , function () {
	            if (!self.disabled) {
	                control.removeClass("ui-state-hover");
	            }
	        }
	        );
            // clicking on the control toggles the drop container
            wrapper.click(function (event) {
                if (!self.disabled) {
                    event.stopImmediatePropagation();
                    if (!self.dropWrapper.isOpen && $.isFunction(options.onShowDropdown) && options.onShowDropdown.call(sourceSelect, sourceSelect) == false) return;
                    self._toggleDropContainer(!self.dropWrapper.isOpen);
                }
            });
            wrapper.insertAfter(sourceSelect);

            // Watch for a window resize and adjust the control if open
            $(window).resize(function () {
                if (!self.disabled && self.dropWrapper.isOpen) {
                    // Reopen yourself to get the position right
                    self._toggleDropContainer(true);
                }
            });
            return wrapper;
        },
        // Creates a drop item that coresponds to an option element in the source select
        _createDropItem: function (index, tabIndex, value, text, checked, disabled, indent, imageUrl, thumbnailImageUrl) {
            var self = this, options = this.options, sourceSelect = this.sourceSelect, controlWrapper = this.controlWrapper;
            // the item contains a div that contains a checkbox input and a lable for the text
            // the div
            var item = $("<div/>");
            item.addClass("ui-dropdownchecklist-item");
            item.css({ 'white-space': "nowrap" });

            // generated id must be a bit unique to keep from colliding
            var idBase = controlWrapper.attr("id");
            var id = idBase + '-i' + index;
            var checkBox;

            // all items start out disabled to keep them out of the tab order
            if (self.isMultiple) { // the checkbox
                checkBox = $('<input disabled type="checkbox" id="' + id + '" tabindex="' + tabIndex + '" />');
            } else { // the radiobutton
                checkBox = $('<input disabled type="radio" id="' + id + '" name="' + idBase + '" tabindex="' + tabIndex + '" />');
                if (options.showRadioButtonsForSingleSelect != true) {
                    checkBox.css({ position: "relative", left: "-100px", width: "0px", margin: "0px 0px 0px 2px", padding: "0px", overflow: "hidden" });
                }
            }
            checkBox.attr("checked", checked ? true : false);
            checkBox.addClass(checked ? "checked" : "");
            checkBox.addClass(disabled ? "inactive" : "active");
            checkBox = checkBox.attr("index", index).val(value);
            item.append(checkBox);

            if (thumbnailImageUrl != null || imageUrl != null) {
                var img = $("<img />");
                img.attr({ src: (thumbnailImageUrl || imageUrl), title: "" });
                if (options.onErrorImageUrl) {
                    img.one("error", function () { this.src = options.onErrorImageUrl; });
                }
                img.bind("mouseover.ddcl",
                    function (event) {
                        img.data("hover-delay-timeout", setTimeout(function () {
                            img.removeData("hover-delay-timeout");
                            var popup = $("<div class=\"ui-dropdownchecklist-item-imagepopup\"></div>");
                            popup.attr("id", idBase + "-imagepopup");

                            var fullImage = $("<img />").attr({ src: (imageUrl || thumbnailImageUrl), title: "" });
                            if (options.onErrorImageUrl) {
                                fullImage.one("error", function () { this.src = options.onErrorImageUrl; });
                            }

                            popup.append(fullImage);
                            $("body").append(popup);

                            var popupLeft = (event.pageX + 16);
                            var popupTop = (event.pageY - 16);

                            /* Requesting the window scroll values seems to cause IE8 to flip out and scroll to the top of the dropcontainer!!
                            if((popupLeft + popup.outerWidth()) > ($(window).width() + $(window).scrollLeft())) {
                            popupLeft = event.pageX - 16 - popup.outerWidth();
                            }

                            if((popupTop + popup.outerHeight()) > ($(window).height() + $(window).scrollTop())) {
                            popupTop = event.pageY + 16 - popup.outerHeight();
                            }
                            */

                            popup.css({ top: popupTop + "px", left: popupLeft + "px" });

                            popup.fadeIn(400);
                        }, 1000));
                    }
                );
                img.bind("mouseout.ddcl",
                    function (event) {
                        var delayTimeout = img.data("hover-delay-timeout");
                        if (delayTimeout) {
                            clearTimeout(delayTimeout);
                            img.removeData("hover-delay-timeout");
                        }
                        $("#" + idBase + "-imagepopup").remove();
                    }
                );
                img.bind("mousemove.ddcl",
                    function (event) {
                        var popup = $("#" + idBase + "-imagepopup");
                        if (popup.length > 0) {
                            var popupLeft = (event.pageX + 16);
                            var popupTop = (event.pageY - 16);

                            if ((popupLeft + popup.outerWidth()) > ($(window).width() + $(window).scrollLeft())) {
                                popupLeft = event.pageX - 16 - popup.outerWidth();
                            }

                            if ((popupTop + popup.outerHeight()) > ($(window).height() + $(window).scrollTop())) {
                                popupTop = event.pageY + 16 - popup.outerHeight();
                            }

                            popup.css({ top: popupTop + "px", left: popupLeft + "px" });
                        }
                    }
                );
                img.bind("remove.ddcl", function (event) {
                    if (event.target === img.element) {
                        $("#" + idBase + "-imagepopup").remove();
                    }
                });
                item.append(img);
            }

            // the text
            var label = $("<label for=\"" + id + "\"/>");
            label.addClass("ui-dropdownchecklist-text");
            label.css({ cursor: "default" });
            label.text(text);
            if (indent) {
                item.addClass("ui-dropdownchecklist-indent");
            }
            if (disabled) {
                item.addClass("ui-state-disabled");
            }
            label.click(function (e) { e.stopImmediatePropagation(); });
            item.append(label);

            // active items display themselves with hover
            item.hover(
            	function (e) {
            	    var anItem = $(this);
            	    if (!anItem.hasClass("ui-state-disabled")) { anItem.addClass("ui-state-hover"); }
            	}
            , function (e) {
                var anItem = $(this);
                anItem.removeClass("ui-state-hover");
            }
            );
            // clicking on the checkbox synchronizes the source select
            checkBox.click(function (e) {
                var aCheckBox = $(this);
                e.stopImmediatePropagation();
                if (aCheckBox.hasClass("active")) {
                    // Active checkboxes take active action
                    self._syncSelected(aCheckBox);
                    self.sourceSelect.trigger("change", 'ddcl_internal');
                    if (!self.isMultiple && options.closeRadioOnClick) {
                        self._toggleDropContainer(false);
                    }
                }
            });
            // we are interested in the focus leaving the check box
            // but we need to detect the focus leaving one check box but
            // entering another. There is no reliable way to detect who
            // received the focus on a blur, so post the blur in the future,
            // knowing we will cancel it if we capture the focus in a timely manner
            // 23Sept2010 - unfortunately, IE 7+ and Chrome like to post a blur
            // 				event to the current item with focus when the user
            //				clicks in the scroll bar. So if you have a scrollable
            //				dropdown with focus on an item, clicking in the scroll
            //				will close the drop down.
            //				I have no solution for blur processing at this time.
            /*********
            var timerFunction = function(){ 
            // I had a hell of a time getting setTimeout to fire this, do not try to
            // define it within the blur function
            try { self._handleFocus(null,false,false); } catch(ex){ alert('timer failed: '+ex);}
            };
            checkBox.blur(function(e) { 
            self.blurringItem = setTimeout( timerFunction, 200 ); 
            });
            checkBox.focus(function(e) {self._cancelBlur();});
            **********/
            // check/uncheck the item on clicks on the entire item div
            item.click(function (e) {
                var anItem = $(this);
                e.stopImmediatePropagation();
                if (!anItem.hasClass("ui-state-disabled")) {
                    // check/uncheck the underlying control
                    var aCheckBox = anItem.children("input"); //anItem.find("input");
                    var checked = aCheckBox.attr("checked");
                    aCheckBox.attr("checked", !checked);
                    self._syncSelected(aCheckBox);
                    self.sourceSelect.trigger("change", 'ddcl_internal');
                    if (!checked && !self.isMultiple && options.closeRadioOnClick) {
                        self._toggleDropContainer(false);
                    }
                } else {
                    // retain the focus even if disabled
                    anItem.focus();
                    self._cancelBlur();
                }
            });
            // do not let the focus wander around
            item.focus(function (e) {
                var anItem = $(this);
                e.stopImmediatePropagation();
            });
            item.keyup(function (e) { self._handleKeyboard(e); });
            return item;
        },
        _createGroupItem: function (text, disabled) {
            var self = this;
            var group = $("<div />");
            group.addClass("ui-dropdownchecklist-group ui-widget-header");
            if (disabled) {
                group.addClass("ui-state-disabled");
            }
            group.css({ 'white-space': "nowrap" });

            var label = $("<span/>");
            label.addClass("ui-dropdownchecklist-text");
            label.css({ cursor: "default" });
            label.text(text);
            group.append(label);

            // anything interesting when you click the group???
            group.click(function (e) {
                var aGroup = $(this);
                e.stopImmediatePropagation();
                // retain the focus even if no action is taken
                aGroup.focus();
                self._cancelBlur();
            });
            // do not let the focus wander around
            group.focus(function (e) {
                var aGroup = $(this);
                e.stopImmediatePropagation();
            });
            return group;
        },
        _createCloseItem: function (text) {
            var self = this;
            var closeItem = $("<div />");
            closeItem.addClass("ui-dropdownchecklist-close ui-dropdownchecklist-item");
            closeItem.css({ 'white-space': 'nowrap', 'text-align': 'right' });

            var label = $("<span/>");
            label.addClass("ui-dropdownchecklist-text");
            label.css({ cursor: "default" });
            label.text(text);
            closeItem.append(label);

            // close the control on click
            closeItem.click(function (e) {
                var aGroup = $(this);
                e.stopImmediatePropagation();
                // retain the focus even if no action is taken
                aGroup.focus();
                self._toggleDropContainer(false);
            });
            closeItem.hover(
            	function (e) { $(this).addClass("ui-state-hover"); }
            , function (e) { $(this).removeClass("ui-state-hover"); }
            );
            // do not let the focus wander around
            closeItem.focus(function (e) {
                var aGroup = $(this);
                e.stopImmediatePropagation();
            });
            return closeItem;
        },
        _createShowMoreItem: function (text) {
            var self = this;
            var showMoreItem = $("<div />");
            showMoreItem.addClass("ui-dropdownchecklist-showmore ui-dropdownchecklist-item");
            showMoreItem.css({ 'white-space': 'nowrap', 'text-align': 'right' });

            var label = $("<span/>");
            label.addClass("ui-dropdownchecklist-text");
            label.css({ cursor: "default" });
            label.attr("title", "Click here to see more options");
            label.text(text);
            showMoreItem.append(label);

            // close the control on click AND invoke the appropriate callback
            showMoreItem.click(function (e) {
                var aGroup = $(this);
                e.stopImmediatePropagation();
                // retain the focus even if no action is taken
                aGroup.focus();
                self._toggleDropContainer(false);
                // invoke callback
                if ($.isFunction(self.options.onShowMore)) {
                    try {
                        self.options.onShowMore.call(self, self.sourceSelect.get(0));
                    } catch (ex) {
                        alert('"show more" callback failed: ' + ex);
                    }
                }
            });
            showMoreItem.hover(
            	function (e) { $(this).addClass("ui-state-hover"); }
            , function (e) { $(this).removeClass("ui-state-hover"); }
            );
            // do not let the focus wander around
            showMoreItem.focus(function (e) {
                var aGroup = $(this);
                e.stopImmediatePropagation();
            });

            return showMoreItem;
        },
        // Creates the drop items and appends them to the drop container
        // Also calculates the size needed by the drop container and returns it
        _appendItems: function () {
            var self = this, config = this.options, sourceSelect = this.sourceSelect, dropWrapper = this.dropWrapper;
            var dropContainerDiv = dropWrapper.children(".ui-dropdownchecklist-dropcontainer");
            var showMoreItem = null;
            sourceSelect.children().each(function (index) { // when the select has groups
                if (config.showMoreItemLimit != null && index >= config.showMoreItemLimit) {
                    showMoreItem = self._createShowMoreItem(config.showMoreText || "show more...");
                    dropContainerDiv.append(showMoreItem);
                    return false; // break from for-each loop
                }
                var opt = $(this);
                if (opt.is("option")) {
                    self._appendOption(opt, dropContainerDiv, index, false, false);
                } else if (opt.is("optgroup")) {
                    var disabled = opt.attr("disabled");
                    var text = opt.attr("label");
                    if (text != "") {
                        var group = self._createGroupItem(text, disabled);
                        dropContainerDiv.append(group);
                    }
                    self._appendOptions(opt, dropContainerDiv, index, true, disabled);
                }
            });
            if (config.alwaysIncludeShowMore == true && showMoreItem == null) {
                showMoreItem = self._createShowMoreItem(config.showMoreText || "show more...");
                dropContainerDiv.append(showMoreItem);
            }
            if (config.explicitClose != null) {
                var closeItem = self._createCloseItem(config.explicitClose);
                dropContainerDiv.append(closeItem);
            }

            // make all item labels the same length (for visual consistency when highlighting)
            var maxLabelWidth = 0;
            var itemLabels = dropContainerDiv.find("label");
            itemLabels.each(function () {
                var labelWidth = $(this).width();
                if (labelWidth > maxLabelWidth) maxLabelWidth = labelWidth;
            });
            itemLabels.width(maxLabelWidth);

            var divWidth = dropContainerDiv.outerWidth(true);
            var divHeight = dropContainerDiv.outerHeight(true);
            return { width: divWidth, height: divHeight };
        },
        _appendOptions: function (parent, container, parentIndex, indent, forceDisabled) {
            var self = this;
            parent.children("option").each(function (index) {
                var option = $(this);
                var childIndex = (parentIndex + "." + index);
                self._appendOption(option, container, childIndex, indent, forceDisabled);
            });
        },
        _appendOption: function (option, container, index, indent, forceDisabled) {
            var self = this;
            var text = option.text(); // NB: all HTML content rather than just text
            var value = option.val();
            var selected = option.attr("selected");
            var disabled = (forceDisabled || option.attr("disabled"));
            // Use the same tab index as the selector replacement
            var tabIndex = self.controlSelector.attr("tabindex");
            var item = self._createDropItem(index, tabIndex, value, text, selected, disabled, indent, option.data("image-url"), option.data("thumbnail-image-url"));
            container.append(item);
        },
        // Synchronizes the items checked and the source select
        // When firstItemChecksAll option is active also synchronizes the checked items
        // senderCheckbox parameters is the checkbox input that generated the synchronization
        _syncSelected: function (senderCheckbox) {
            var self = this, options = this.options, sourceSelect = this.sourceSelect, dropWrapper = this.dropWrapper;
            var selectOptions = sourceSelect.get(0).options;
            var allCheckboxes = dropWrapper.find("input.active");
            if (options.firstItemChecksAll) {
                if ((senderCheckbox == null) && $(selectOptions[0]).attr("selected")) {
                    // Initialization call with first item active so force all to be active
                    allCheckboxes.attr("checked", true);
                } else if ((senderCheckbox != null) && (senderCheckbox.attr("index") == 0)) {
                    // Check all checkboxes if the first one is checked
                    allCheckboxes.attr("checked", senderCheckbox.attr("checked"));
                } else {
                    // check the first checkbox if all the other checkboxes are checked
                    var allChecked = true;
                    var firstCheckbox = null;
                    allCheckboxes.each(function (index) {
                        if (index > 0) {
                            var checked = $(this).attr("checked") || false;
                            if (!checked) { allChecked = false; }
                        } else {
                            firstCheckbox = $(this);
                        }
                    });
                    if (firstCheckbox != null) {
                        firstCheckbox.attr("checked", allChecked);
                    }
                }
            }
            // do the actual synch with the source select (and also add/remove CSS class for benefit of browsers with no :checked selector)
            allCheckboxes = dropWrapper.find("input");
            allCheckboxes.each(function (index) {
                if ($(this).attr("checked")) {
                    $(this).addClass("checked");
                    $(selectOptions[index]).attr("selected", "selected");
                } else {
                    $(this).removeClass("checked");
                    $(selectOptions[index]).removeAttr("selected");
                }
            });
            // update the text shown in the control
            self._updateControlText();

            // Ensure the focus stays pointing where the user is working
            if (senderCheckbox != null) { senderCheckbox.focus(); }
        },
        _sourceSelectChangeHandler: function (event) {
            var self = this, dropWrapper = this.dropWrapper;
            dropWrapper.find("input").val(self.sourceSelect.val());

            // update the text shown in the control
            self._updateControlText();
        },
        // Updates the text shown in the control depending on the checked (selected) items
        _updateControlText: function () {
            var self = this, sourceSelect = this.sourceSelect, options = this.options, controlWrapper = this.controlWrapper;
            var firstOption = sourceSelect.find("option:first");
            var selectOptions = sourceSelect.find("option");
            var text = self._formatText(selectOptions, options.firstItemChecksAll, firstOption);
            var controlLabel = controlWrapper.find(".ui-dropdownchecklist-text");
            if (text.jquery) {
                controlLabel.children().remove();
                controlLabel.append(text);
                controlLabel.attr("title", text.text());
            } else {
                controlLabel.html(text);
                controlLabel.attr("title", text);
            }
            controlLabel.unbind(".ddcl");
            if (options.showSelectedItemImageAsTooltip == true) {
                var firstSelectedOption = selectOptions.filter(":selected").first();
                var tooltipImgUrl = (firstSelectedOption.data("image-url") || firstSelectedOption.data("thumbnail-image-url"));
                if (tooltipImgUrl != null) {
                    var idBase = controlWrapper.attr("id") + "-selected";
                    controlLabel.bind("mouseenter.ddcl",
                        function (event) {
                            event.stopPropagation();

                            controlLabel.data("hover-delay-timeout", setTimeout(function () {
                                controlLabel.removeData("hover-delay-timeout");
                                var popup = $("<div class=\"ui-dropdownchecklist-item-imagepopup\"></div>");
                                popup.attr("id", idBase + "-imagepopup");

                                var fullImage = $("<img />").attr({ src: tooltipImgUrl, title: "" });
                                if (options.onErrorImageUrl) {
                                    fullImage.one("error", function () { this.src = options.onErrorImageUrl; });
                                }

                                popup.append(fullImage);
                                $("body").append(popup);

                                var popupLeft = (event.pageX + 16);
                                var popupTop = (event.pageY - 16);

                                if ((popupLeft + popup.outerWidth()) > ($(window).width() + $(window).scrollLeft())) {
                                    popupLeft = event.pageX - 16 - popup.outerWidth();
                                }

                                if ((popupTop + popup.outerHeight()) > ($(window).height() + $(window).scrollTop())) {
                                    popupTop = event.pageY + 16 - popup.outerHeight();
                                }

                                popup.css({ top: popupTop + "px", left: popupLeft + "px" });

                                popup.fadeIn(400);
                            },
                             1000));
                        }
                    );
                    controlLabel.bind("mouseleave.ddcl",
                        function (event) {
                            event.stopPropagation();
                            var delayTimeout = controlLabel.data("hover-delay-timeout");
                            if (delayTimeout) {
                                clearTimeout(delayTimeout);
                                controlLabel.removeData("hover-delay-timeout");
                            }
                            $("#" + idBase + "-imagepopup").remove();
                        }
                    );
                    controlLabel.bind("mousemove.ddcl", function (event) {
                        var popup = $("#" + idBase + "-imagepopup");
                        if (popup.length > 0) {
                            var popupLeft = (event.pageX + 16);
                            var popupTop = (event.pageY - 16);

                            if ((popupLeft + popup.outerWidth()) > ($(window).width() + $(window).scrollLeft())) {
                                popupLeft = event.pageX - 16 - popup.outerWidth();
                            }

                            if ((popupTop + popup.outerHeight()) > ($(window).height() + $(window).scrollTop())) {
                                popupTop = event.pageY + 16 - popup.outerHeight();
                            }

                            popup.css({ top: popupTop + "px", left: popupLeft + "px" });
                        }
                    });
                    controlLabel.bind("remove.ddcl", function (event) {
                        if (event.target === controlLabel.element) {
                            $("#" + idBase + "-imagepopup").remove();
                        }
                    });
                }
            }
        },
        // Formats the text that is shown in the control
        _formatText: function (selectOptions, firstItemChecksAll, firstOption) {
            var text;
            if ($.isFunction(this.options.textFormatFunction)) {
                // let the callback do the formatting, but do not allow it to fail
                try {
                    text = this.options.textFormatFunction(selectOptions);
                } catch (ex) {
                    alert('textFormatFunction failed: ' + ex);
                }
            } else if (firstItemChecksAll && (firstOption != null) && firstOption.attr("selected")) {
                // just set the text from the first item
                text = firstOption.text();
            } else {
                // concatenate the text from the checked items
                text = "";
                selectOptions.each(function () {
                    if ($(this).attr("selected")) {
                        if (text != "") { text += ", "; }
                        text += $(this).html(); 	/* NOTE not .text(), which screws up ampersands for IE */
                    }
                });
                if (text == "") {
                    text = this.options.emptyText || "&nbsp;";
                }
            }
            return text;
        },
        // Shows and hides the drop container
        _toggleDropContainer: function (makeOpen) {
            var self = this;
            // hides the last shown drop container
            var hide = function (instance) {
                if ((instance != null) && instance.dropWrapper.isOpen) {
                    instance.dropWrapper.isOpen = false;
                    $.ui.dropdownchecklist.gLastOpened = null;

                    var config = instance.options;
                    instance.dropWrapper.css({
                        top: "-33000px",
                        left: "-33000px"
                    });
                    var aControl = instance.controlSelector;
                    aControl.removeClass("ui-state-active");
                    aControl.removeClass("ui-state-hover");

                    var anIcon = instance.controlWrapper.find(".ui-icon");
                    if (anIcon.length > 0) {
                        anIcon.removeClass((config.icon.toClose != null) ? config.icon.toClose : "ui-icon-triangle-1-s");
                        anIcon.addClass((config.icon.toOpen != null) ? config.icon.toOpen : "ui-icon-triangle-1-e");
                    }
                    $(document).unbind("click", hide);

                    // keep the items out of the tab order by disabling them
                    instance.dropWrapper.find("input.active").attr("disabled", "disabled");

                    // the following blur just does not fire???  because it is hidden???  because it does not have focus???
                    //instance.sourceSelect.trigger("blur");
                    //instance.sourceSelect.triggerHandler("blur");
                    if ($.isFunction(config.onComplete)) {
                        try {
                            config.onComplete.call(instance, instance.sourceSelect.get(0));
                        } catch (ex) {
                            alert('callback failed: ' + ex);
                        }
                    }
                }
            };
            // shows the given drop container instance
            var show = function (instance) {
                if (!instance.dropWrapper.isOpen) {
                    instance.dropWrapper.isOpen = true;
                    $.ui.dropdownchecklist.gLastOpened = instance;

                    var config = instance.options;
                    /**** Issue127 (and the like) to correct positioning when parent element is relative
                    ****	This positioning only worked with simple, non-relative parent position
                    instance.dropWrapper.css({
                    top: instance.controlWrapper.offset().top + instance.controlWrapper.outerHeight() + "px",
                    left: instance.controlWrapper.offset().left + "px"
                    });
                    ****/
                    if ((config.positionHow == null) || (config.positionHow == 'absolute')) {
                        // flip edge alignement if dropdown falls outside viewport...
                        var ctlSize = { width: instance.controlWrapper.outerWidth(), height: instance.controlWrapper.outerHeight() };
                        var dropSize = { width: instance.dropWrapper.outerWidth(), height: instance.dropWrapper.outerHeight() };
                        var pos = instance.controlWrapper.offset();

                        var scrollbarWidth = 0;
                        try {
                            var itemsHeight = 0;
                            instance.dropWrapper.find(".ui-dropdownchecklist-item").each(function () { itemsHeight += $(this).outerHeight(); });
                            if (itemsHeight > dropSize.height) scrollbarWidth = $.getScrollbarWidth();
                        } catch (ex) { }

                        pos.top += ctlSize.height; // align to bottom-left of control by default

                        if ((pos.left + dropSize.width) > ($(window).width() + $(window).scrollLeft())) {
                            var adjustmentX = Math.max(0, dropSize.width - ctlSize.width) + scrollbarWidth;
                            if ((pos.left - adjustmentX) > 0) {
                                pos.left -= adjustmentX;
                            }
                        }

                        if ((pos.top + dropSize.height) > ($(window).height() + $(window).scrollTop())) {
                            var adjustmentY = (dropSize.height + ctlSize.height);
                            if ((pos.top - adjustmentY) > 0) {
                                pos.top -= adjustmentY;
                            }
                        }

                        /** Floats above subsequent content, but does NOT scroll */
                        instance.dropWrapper.css({
                            position: 'absolute'
		                , top: pos.top + "px"
		                , left: pos.left + "px"
                        });
                    } else if (config.positionHow == 'relative') {
                        /** Scrolls with the parent but does NOT float above subsequent content */
                        instance.dropWrapper.css({
                            position: 'relative'
		                , top: "0px"
		                , left: "0px"
                        });
                    }
                    var zIndex = 0;
                    if (config.zIndex == null) {
                        var ancestorsZIndexes = instance.controlWrapper.parents().map(
							function () {
							    var zIndex = $(this).css("z-index");
							    return isNaN(zIndex) ? 0 : zIndex;
							}
							).get();
                        var parentZIndex = Math.max.apply(Math, ancestorsZIndexes);
                        if (parentZIndex >= 0) zIndex = parentZIndex + 1;
                    } else {
                        /* Explicit set from the optins */
                        zIndex = parseInt(config.zIndex);
                    }
                    if (zIndex > 0) {
                        instance.dropWrapper.css({ 'z-index': zIndex });
                    }

                    var aControl = instance.controlSelector;
                    aControl.addClass("ui-state-active");
                    aControl.removeClass("ui-state-hover");

                    var anIcon = instance.controlWrapper.find(".ui-icon");
                    if (anIcon.length > 0) {
                        anIcon.removeClass((config.icon.toOpen != null) ? config.icon.toOpen : "ui-icon-triangle-1-e");
                        anIcon.addClass((config.icon.toClose != null) ? config.icon.toClose : "ui-icon-triangle-1-s");
                    }
                    $(document).bind("click", function (e) { hide(instance); });

                    // insert the items back into the tab order by enabling all active ones
                    var activeItems = instance.dropWrapper.find("input.active");
                    activeItems.removeAttr("disabled");

                    // we want the focus on the first active input item
                    var firstActiveItem = activeItems.get(0);
                    if (firstActiveItem != null) {
                        firstActiveItem.focus();
                    }

                    // NB: need to do this to slap IE around the face so it will actually apply the appropriate styling every time
                    $(instance.dropWrapper).find("input.checked").removeClass("checked").addClass("checked");
                }
            };
            if (makeOpen) {
                hide($.ui.dropdownchecklist.gLastOpened);
                show(self);
            } else {
                hide(self);
            }
        },
        // Set the size of the control and of the drop container
        _setSize: function (dropCalculatedSize) {
            var options = this.options, dropWrapper = this.dropWrapper, controlWrapper = this.controlWrapper;

            var control = this.controlSelector;
            control.css({ "box-sizing": "border-box" });

            // use the width from config options if set, otherwise set the same width as the source 'select' element
            var controlWidth = dropCalculatedSize.width;
            if (options.width != null) {
                control.width(options.width);
            } else {
                //control.width(this.initialWidth); // not desirable - use CSS instead
            }

            if (options.minWidth != null) {
                control.css({ "min-width": options.minWidth });
            }

            controlWidth = control.width(); // read back the calculated width in pixels

            // if we size the text, then Firefox places icons to the right properly
            // and we do not wrap on long lines
            var controlText = control.children(".ui-dropdownchecklist-text");
            var controlIcon = control.children(".ui-icon");
            if (controlIcon != null) {
                // Must be an inner/outer/border problem, but IE6 needs an extra bit of space,
                // otherwise you can get text pushed down into a second line when icons are active
                controlWidth -= (controlIcon.outerWidth() + 4);
                controlText.css({ width: controlWidth + "px" });
            }
            // Account for padding, borders, etc
            controlWidth = controlWrapper.outerWidth();

            // the drop container height can be set from options
            var maxDropHeight = (options.maxDropHeight != null)
            					? parseInt(options.maxDropHeight)
            					: -1;
            var dropHeight = ((maxDropHeight > 0) && (dropCalculatedSize.height > maxDropHeight))
            					? maxDropHeight
            					: dropCalculatedSize.height;
            // ensure the drop container is not less than the control width (would be ugly)
            var dropWidth = dropCalculatedSize.width < controlWidth ? controlWidth : dropCalculatedSize.width;

            if (dropCalculatedSize.height > maxDropHeight) {
                try {
                    dropWidth += $.getScrollbarWidth();
                } catch (err) { }
            }

            $(dropWrapper).css({
                height: dropHeight + "px",
                width: dropWidth + "px"
            });
            dropWrapper.children(".ui-dropdownchecklist-dropcontainer").css({
                height: dropHeight + "px"
            });
        },
        // Initializes the plugin
        _init: function () {
            var self = this, options = this.options;
            if ($.ui.dropdownchecklist.gIDCounter == null) {
                $.ui.dropdownchecklist.gIDCounter = 1;
            }
            // item blurring relies on a cancelable timer
            self.blurringItem = null;

            // sourceSelect is the select on which the plugin is applied
            var sourceSelect = self.element;
            self.initialWidth = sourceSelect.width() + parseInt(sourceSelect.css("border-left-width")) + parseInt(sourceSelect.css("border-right-width"));
            self.initialDisplay = sourceSelect.css("display");
            sourceSelect.css("display", "none");
            self.initialMultiple = sourceSelect.attr("multiple");
            self.isMultiple = self.initialMultiple;
            if (options.forceMultiple != null) { self.isMultiple = options.forceMultiple; }
            sourceSelect.attr("multiple", true);
            self.sourceSelect = sourceSelect;

            // append the control that resembles a single selection select
            var controlWrapper = self._appendControl();
            self.controlWrapper = controlWrapper;
            self.controlSelector = controlWrapper.children(".ui-dropdownchecklist-selector");

            // create the drop container where the items are shown
            var dropWrapper = self._appendDropContainer(controlWrapper);
            self.dropWrapper = dropWrapper;

            // append the items from the source select element
            var dropCalculatedSize = self._appendItems();

            // updates the text shown in the control
            self._updateControlText(controlWrapper, dropWrapper, sourceSelect);

            // set the sizes of control and drop container
            self._setSize(dropCalculatedSize);

            // look for possible auto-check needed on first item
            if (options.firstItemChecksAll) {
                self._syncSelected(null);
            }
            // BGIFrame for IE6
            if (options.bgiframe && typeof self.dropWrapper.bgiframe == "function") {
                self.dropWrapper.bgiframe();
            }
            // listen for change events on the source select element
            // ensure we avoid processing internally triggered changes
            self.sourceSelect.change(function (event, eventName) {
                if (eventName != 'ddcl_internal') {
                    self._sourceSelectChangeHandler(event);
                }
            });
        },
        // Refresh the disable and check state from the underlying control
        _refreshOption: function (item, disabled, selected) {
            var aParent = item.parent();
            // account for enabled/disabled
            if (disabled) {
                item.attr("disabled", "disabled");
                item.removeClass("active");
                item.addClass("inactive");
                aParent.addClass("ui-state-disabled");
            } else {
                item.removeAttr("disabled");
                item.removeClass("inactive");
                item.addClass("active");
                aParent.removeClass("ui-state-disabled");
            }
            // adjust the checkbox state
            item.attr("checked", selected);
            if (selected) {
                item.addClass("checked");
            } else {
                item.removeClass("checked");
            }
        },
        _refreshGroup: function (group, disabled) {
            if (disabled) {
                group.addClass("ui-state-disabled");
            } else {
                group.removeClass("ui-state-disabled");
            }
        },
        // External command to explicitly close the dropdown
        close: function () {
            this._toggleDropContainer(false);
        },
        // External command to refresh the ddcl from the underlying selector
        refresh: function () {
            var self = this, sourceSelect = this.sourceSelect, dropWrapper = this.dropWrapper;

            // check if options have been added or removed
            if (sourceSelect.children().length != dropWrapper.find("input").length) {
                dropWrapper.find(".ui-dropdownchecklist-item, .ui-dropdownchecklist-group").remove();

                // append the items from the source select element
                var dropCalculatedSize = self._appendItems();

                // set the sizes of control and drop container
                self._setSize(dropCalculatedSize);
            }

            var allCheckBoxes = dropWrapper.find("input");
            var allGroups = dropWrapper.find(".ui-dropdownchecklist-group");

            var groupCount = 0;
            var optionCount = 0;
            sourceSelect.children().each(function (index) {
                var opt = $(this);
                var disabled = opt.attr("disabled");
                if (opt.is("option")) {
                    var selected = opt.attr("selected");
                    var anItem = $(allCheckBoxes[optionCount]);
                    self._refreshOption(anItem, disabled, selected);
                    optionCount += 1;
                } else if (opt.is("optgroup")) {
                    var text = opt.attr("label");
                    if (text != "") {
                        var aGroup = $(allGroups[groupCount]);
                        self._refreshGroup(aGroup, disabled);
                        groupCount += 1;
                    }
                    opt.children("option").each(function () {
                        var subopt = $(this);
                        var subdisabled = (disabled || subopt.attr("disabled"));
                        var selected = subopt.attr("selected");
                        var subItem = $(allCheckBoxes[optionCount]);
                        self._refreshOption(subItem, subdisabled, selected);
                        optionCount += 1;
                    });
                }
            });
            // update the text shown in the control
            self._updateControlText();
        },
        // External command to enable the ddcl control
        enable: function () {
            this.controlSelector.removeClass("ui-state-disabled");
            this.disabled = false;
        },
        // External command to disable the ddcl control
        disable: function () {
            this.controlSelector.addClass("ui-state-disabled");
            this.disabled = true;
        },
        // External command to destroy all traces of the ddcl control
        destroy: function () {
            $.Widget.prototype.destroy.apply(this, arguments);
            this.sourceSelect.css("display", this.initialDisplay);
            this.sourceSelect.attr("multiple", this.initialMultiple);
            this.controlWrapper.unbind().remove();
            this.dropWrapper.remove();
        }
    });

})(jQuery);

/*! Copyright (c) 2008 Brandon Aaron (brandon.aaron@gmail.com || http://brandonaaron.net)
* Dual licensed under the MIT (http://www.opensource.org/licenses/mit-license.php) 
* and GPL (http://www.opensource.org/licenses/gpl-license.php) licenses.
*/

/**
* Gets the width of the OS scrollbar
*/
(function ($) {
    var scrollbarWidth = 0;
    $.getScrollbarWidth = function () {
        if (!scrollbarWidth) {
            if ($.browser.msie) {
                var $textarea1 = $('<textarea cols="10" rows="2"></textarea>')
						.css({ position: 'absolute', top: -1000, left: -1000 }).appendTo('body'),
					$textarea2 = $('<textarea cols="10" rows="2" style="overflow: hidden;"></textarea>')
						.css({ position: 'absolute', top: -1000, left: -1000 }).appendTo('body');
                scrollbarWidth = $textarea1.width() - $textarea2.width();
                $textarea1.add($textarea2).remove();
            } else {
                var $div = $('<div />')
					.css({ width: 100, height: 100, overflow: 'auto', position: 'absolute', top: -1000, left: -1000 })
					.prependTo('body').append('<div />').find('div')
						.css({ width: '100%', height: 200 });
                scrollbarWidth = 100 - $div.width();
                $div.parent().remove();
            }
        }
        return scrollbarWidth;
    };
})(jQuery);