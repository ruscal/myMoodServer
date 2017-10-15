/// <reference path="fabric.js" />

(function ($, undefined) {
    $.widget('discover.moodmeter', {

        options: {
            data: null,
            dataUrl: null,
            updateInterval: null,
            theme: {
                backgroundColor: "#32383E",
                lineColor: "#FFFFFF",
                barPositiveFillColor: ["#F5991D", "#FDC30B"],
                barNegativeFillColor: ["#BA4125", "#D25322"],
                barBackgroundColor: "#272B30",
                textColor: "#FFFFFF",
                fontFamily: "Impact",
                showMajorTickText: false,
                showMinorTicks: false,
                tickMarkFontSize: 26,
                tickMarkAlignment: "right", // [left|right]
                headerFontSize: 42,
                footerFontSize: 28,
                logoImageUrl: "/Content/Images/Callout/logo.png"
            }
        },

        widgetEventPrefix: "",

        _create: function () {
            var options = this.options;
            var widget = this;

            // create canvas
            widget.canvas = new fabric.StaticCanvas($("<canvas>").attr("id", "canvas-" + $("canvas").length).appendTo(this.element).get(0), {
                renderOnAddition: false,
                backgroundColor: options.theme.backgroundColor
            });
            widget.canvas.setDimensions({ width: $(widget.element).width(), height: $(widget.element).height() });

            // create visual elements
            if (options.data != null) {
                widget._createVisualElements();
                if (options.updateInterval) {
                    setTimeout(function () { widget._refreshData(); }, options.updateInterval);
                }
            } else if (options.dataUrl) {
                widget._refreshData();
            }
        },

        destroy: function () {

            // if using jQuery UI 1.8.x
            $.Widget.prototype.destroy.call(this);
            // if using jQuery UI 1.9.x
            //this._destroy();
        },

        _refreshData: function () {
            var options = this.options;
            var widget = this;
            var canvas = this.canvas;

            if (options.dataUrl) {
                $.ajax({
                    url: options.dataUrl,
                    type: "GET",
                    dataType: "json",
                    success: function (newData) {
                        //newData.percentagePositive = (options.data == 100) ? 0 : 100;
                        if (widget.visuals != null) {
                            if (Math.round(options.data) != Math.round(newData.percentagePositive)) {
                                options.data = newData.percentagePositive;
                                widget._updateVisualElements();
                            }
                        } else {
                            options.data = newData.percentagePositive;
                            widget._createVisualElements();
                        }
                    },
                    error: function (xhr, status, err) {
                        // swallow it
                    },
                    complete: function () {
                        if (options.updateInterval) {
                            setTimeout(function () { widget._refreshData(); }, options.updateInterval);
                        }
                    }
                });
            }
        },

        _calculateMainElementPositions: function () {
            var options = this.options;
            var widget = this;
            var canvas = this.canvas;

            // calc scale factors (to use for elements designed for use at 1080p "native" resolution)
            var scaleFactorX = canvas.width / 1750;
            var scaleFactorY = canvas.height / 1080;

            var rightMargin = canvas.width * 0.1;
            var leftMargin = canvas.width * 0.1;

            // meter/bar...
            var barWidth = options.showMajorTickText ? canvas.width * 0.25 : canvas.width * 0.5;
            var barFillWidth = barWidth * 0.70;
            var barTop = canvas.height * 0.12;
            var barLeft = options.theme.tickMarkAlignment == "left" ? options.showMajorTickText ? canvas.width * 0.6 : canvas.width * 0.4 : leftMargin;
            var barHeight = canvas.height * 0.76;
            var barFillMargin = (barWidth - barFillWidth) * 0.5;

            // tick marks...
            var tickEdgeMargin = options.theme.tickMarkAlignment == "left" ? leftMargin : rightMargin;
            var tickTextMargin = 8;
            var tickMajorThickness = options.showMinorTicks ? 3 : 6;
            var tickMajorWidth = canvas.width * 0.2;
            var tickMinorWidth = tickMajorWidth * 0.6;
            var tickPosX = options.theme.tickMarkAlignment == "left" ? leftMargin : canvas.width - rightMargin;

            return {
                scaleFactorX: scaleFactorX,
                scaleFactorY: scaleFactorY,
                rightMargin: rightMargin,
                leftMargin: leftMargin,
                barTop: barTop,
                barBottom: barTop + barHeight,
                barLeft: barLeft,
                barRight: barLeft + barWidth,
                barWidth: barWidth,
                barHeight: barHeight,
                barFillWidth: barFillWidth,
                barFillMargin: barFillMargin,
                barTipRadius: barWidth * 0.5,
                barFillTipRadius: barFillWidth * 0.5,

                tickEdgeMargin: tickEdgeMargin,
                tickTextMargin: tickTextMargin,
                tickMajorThickness: tickMajorThickness,
                tickMajorWidth: tickMajorWidth,
                tickMinorWidth: tickMinorWidth,
                tickPosX: tickPosX
            };
        },

        _createVisualElements: function () {
            var options = this.options;
            var widget = this;
            var canvas = this.canvas;

            var positions = widget._calculateMainElementPositions();

            widget.visuals = {};

            var barFillHeightPositive = positions.barHeight * (options.data * 0.01) + 1;
            var barFillHeightNegative = positions.barHeight + 2;

            widget.visuals.barOutline = new fabric.Path([
                ["M", positions.barLeft, positions.barTop],
                ["A", positions.barTipRadius, positions.barTipRadius, 0, 0, 1, positions.barRight, positions.barTop],
                ["L", positions.barRight, positions.barBottom],
                ["A", positions.barTipRadius, positions.barTipRadius, 0, 0, 1, positions.barLeft, positions.barBottom],
                ["z"]
            ],
            {
                stroke: options.theme.lineColor,
                strokeWidth: 5,
                fill: options.theme.barBackgroundColor
            });

            widget.visuals.barTopTip = new fabric.Path([
                ["M", positions.barLeft + positions.barFillMargin, positions.barTop],
                ["A", positions.barFillTipRadius, positions.barFillTipRadius, 0, 0, 1, positions.barRight - positions.barFillMargin, positions.barTop],
                ["z"]
            ],
            {
                strokeWidth: 0
            });

            widget.visuals.barNegative = new fabric.Rect({
                top: positions.barTop + (barFillHeightNegative * 0.5) - 1,
                left: positions.barLeft + (positions.barWidth * 0.5),
                height: barFillHeightNegative,
                width: positions.barFillWidth,
                strokeWidth: 0
            });

            if ($.isArray(options.theme.barNegativeFillColor)) {
                widget.visuals.barNegative.setGradientFill({ x1: 0, y1: 0, x2: positions.barFillWidth, y2: 0, colorStops: arrayToObject(options.theme.barNegativeFillColor) });
            } else {
                widget.visuals.barNegative.setFill(options.theme.barNegativeFillColor);
            }

            if (options.data >= 100) {
                if ($.isArray(options.theme.barPositiveFillColor)) {
                    widget.visuals.barTopTip.setGradientFill({ x1: 0, y1: 0, x2: positions.barFillWidth, y2: 0, colorStops: arrayToObject(options.theme.barPositiveFillColor) });
                } else {
                    widget.visuals.barTopTip.setFill(options.theme.barPositiveFillColor);
                }
            } else {
                if ($.isArray(options.theme.barNegativeFillColor)) {
                    widget.visuals.barTopTip.setGradientFill({ x1: 0, y1: 0, x2: positions.barFillWidth, y2: 0, colorStops: arrayToObject(options.theme.barNegativeFillColor) });
                } else {
                    widget.visuals.barTopTip.setFill(options.theme.barNegativeFillColor);
                }
            }

            widget.visuals.barBottomTip = new fabric.Path([
                ["M", positions.barRight - positions.barFillMargin, positions.barBottom],
                ["A", positions.barFillTipRadius, positions.barFillTipRadius, 0, 0, 1, positions.barLeft + positions.barFillMargin, positions.barBottom],
                ["z"]
            ],
            {
                strokeWidth: 0
            });

            widget.visuals.barPositive = new fabric.Rect({
                top: positions.barBottom - (barFillHeightPositive * 0.5) - 1,
                left: positions.barLeft + (positions.barWidth * 0.5),
                height: barFillHeightPositive + 3,
                width: positions.barFillWidth,
                strokeWidth: 0
            });

            if ($.isArray(options.theme.barPositiveFillColor)) {
                widget.visuals.barBottomTip.setGradientFill({ x1: 0, y1: 0, x2: positions.barFillWidth, y2: 0, colorStops: arrayToObject(options.theme.barPositiveFillColor) });
                widget.visuals.barPositive.setGradientFill({ x1: 0, y1: 0, x2: positions.barFillWidth, y2: 0, colorStops: arrayToObject(options.theme.barPositiveFillColor) });
            } else {
                widget.visuals.barBottomTip.setFill(options.theme.barPositiveFillColor);
                widget.visuals.barPositive.setFill(options.theme.barPositiveFillColor);
            }

            // tick marks...
            widget.visuals.tickMarks = new Array();

            var tickPosY = positions.barBottom;
            var tickIncrementY = positions.barHeight / 100;

            var m = null;
            var t = null;

            $.Enumerable.Range(0, 101).ForEach(function (n) {
                if (n == 0 || n % 10 == 0) {
                    // major tick
                    m = new fabric.Line([positions.tickPosX, tickPosY, options.theme.tickMarkAlignment == "left" ? positions.tickPosX + positions.tickMajorWidth : positions.tickPosX - positions.tickMajorWidth, tickPosY], {
                        fill: options.theme.lineColor,
                        stroke: options.theme.lineColor,
                        strokeWidth: positions.tickMajorThickness
                    });
                    if (options.showMajorTickText) {
                        t = new fabric.Text(n.toString(), {
                            useNative: true,
                            fontFamily: options.theme.fontFamily,
                            fontSize: Math.round(options.theme.tickMarkFontSize * positions.scaleFactorY),
                            fill: options.theme.textColor,
                            top: tickPosY,
                            left: options.theme.tickMarkAlignment == "left" ? positions.tickPosX + positions.tickMajorWidth + positions.tickTextMargin : positions.tickPosX - positions.tickMajorWidth - positions.tickTextMargin,
                            originX: options.theme.tickMarkAlignment,
                            originY: "center"
                        });
                    } else {
                        t = null;
                    }
                } else if (options.showMinorTicks) {
                    // minor tick
                    m = new fabric.Line([tickPosX, tickPosY, options.theme.tickMarkAlignment == "left" ? positions.tickPosX + positions.tickMinorWidth : positions.tickPosX - positions.tickMinorWidth, tickPosY], {
                        fill: options.theme.lineColor,
                        stroke: options.theme.lineColor,
                        strokeWidth: 1
                    });
                    t = null;
                }

                widget.visuals.tickMarks.push({ mark: m, text: t });

                tickPosY -= tickIncrementY;
            });

            // header...
            widget.visuals.headerText = new fabric.Text(String.format("{0} %", Math.round(options.data)), {
                useNative: true,
                fontFamily: options.theme.fontFamily,
                fontSize: Math.round(options.theme.headerFontSize * positions.scaleFactorY),
                fill: options.theme.textColor,
                top: (positions.barTop - (positions.barWidth * 0.5)) * 0.5,
                left: (canvas.width * 0.5),
                originX: "center",
                originY: "center"
            });

            // footer...
            var footerHeight = canvas.height - (positions.barBottom + (positions.barWidth * 0.5));

            widget.visuals.footerText = new fabric.Text("myMood", {
                useNative: true,
                fontFamily: options.theme.fontFamily,
                fontSize: Math.round(options.theme.footerFontSize * positions.scaleFactorY),
                fill: options.theme.textColor,
                top: canvas.height - (footerHeight * 0.5),
                left: canvas.width - positions.rightMargin,
                originX: "right",
                originY: "center"
            });

            widget.visuals.footerLogo = null; // image reference will be set when loading is complete

            if (options.theme.logoImageUrl) {
                fabric.Image.fromURL(options.theme.logoImageUrl, function (img) {
                    widget.visuals.footerLogo = img;

                    var aspect = img.width / img.height;

                    img.set({
                        top: canvas.height - (footerHeight * 0.5),
                        left: (canvas.width - widget.visuals.footerText.width - positions.rightMargin) * 0.5,
                        height: footerHeight,
                        width: footerHeight * aspect,
                        originX: "center",
                        originY: "center"
                    });
                    canvas.add(img);
                    canvas.renderAll();
                });
            }

            // add all visual elements to the canvas & render...
            canvas.add(widget.visuals.barOutline);
            canvas.add(widget.visuals.barTopTip);
            canvas.add(widget.visuals.barNegative);
            canvas.add(widget.visuals.barBottomTip);
            canvas.add(widget.visuals.barPositive);
            canvas.add(widget.visuals.headerText);
            canvas.add(widget.visuals.footerText);

            $.each(widget.visuals.tickMarks, function () {
                canvas.add(this.mark);
                if (this.text != null) canvas.add(this.text);
            });

            canvas.renderAll();
        },

        _updateVisualElements: function () {
            var options = this.options;
            var widget = this;
            var canvas = this.canvas;

            var positions = widget._calculateMainElementPositions();

            var newFillHeight = positions.barHeight * (options.data * 0.01) + 3;
            var fillDiff = newFillHeight - widget.visuals.barPositive.height;

            if (options.data < 100) {
                if ($.isArray(options.theme.barNegativeFillColor)) {
                    widget.visuals.barTopTip.setGradientFill({ x1: 0, y1: 0, x2: positions.barFillWidth, y2: 0, colorStops: arrayToObject(options.theme.barNegativeFillColor) });
                } else {
                    widget.visuals.barTopTip.setFill(options.theme.barNegativeFillColor);
                }
            }
            widget.visuals.barPositive.animate("height", newFillHeight, {
                duration: 2000,
                onChange: function (val) {
                    widget.visuals.barPositive.top = positions.barBottom - (widget.visuals.barPositive.height * 0.5) + 1;
                    canvas.renderAll();
                },
                onComplete: function () {
                    if (options.data >= 100) {
                        if ($.isArray(options.theme.barPositiveFillColor)) {
                            widget.visuals.barTopTip.setGradientFill({ x1: 0, y1: 0, x2: positions.barFillWidth, y2: 0, colorStops: arrayToObject(options.theme.barPositiveFillColor) });
                        } else {
                            widget.visuals.barTopTip.setFill(options.theme.barPositiveFillColor);
                        }
                        canvas.renderAll();
                    }
                }
            });
            widget.visuals.headerText.animate("opacity", 0.0, {
                duration: 1000,
                onChange: canvas.renderAll.bind(canvas),
                onComplete: function () {
                    widget.visuals.headerText.setText(String.format("{0} %", Math.round(options.data)));
                    widget.visuals.headerText.animate("opacity", 1.0, {
                        duration: 1000,
                        onChange: function (val) {
                            canvas.renderAll();
                        }
                    });
                }
            });
        }
    });

    function arrayToObject(arr) {
        var rv = {};
        for (var i = 0; i < arr.length; ++i)
            if (arr[i] !== undefined) rv[i] = arr[i];
        return rv;
    }

} (jQuery));