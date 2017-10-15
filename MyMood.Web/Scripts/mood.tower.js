/// <reference path="fabric.js" />

(function ($, undefined) {
    $.widget('discover.moodtower', {

        options: {
            data: null,
            dataUrl: null,
            updateInterval: null,
            moodPercentageLabelThreshold: 5,
            theme: {
                backgroundColor: "#545B66",
                lineColor: "#656157",
                dividerColor: "#FFFFFF",
                fontFamily: "Impact",
                headerFontSize: 42,
                headerTextColor: "#FFFFFF",
                moodFontSizeMin: 20,
                moodFontSizeMax: 40,
                positiveMoodTextColor: "rgba(0,0,0,0.3)",
                negativeMoodTextColor: "rgba(0,0,0,0.3)",
                dominantMoodTextColor: "#000000",
                logoImageUrl: "/Content/Images/Callout/MoodTowerIcon.png",
                moodImageRootUrl: "/Content/Images/Callout"
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
                options.data = widget._expandSnapshot(options.data);
                widget._createVisualElements();
                if (options.updateInterval != null) {
                    setTimeout(function () { widget._refreshData(); }, options.updateInterval);
                }
            } else if (options.dataUrl != null) {
                widget._refreshData();
            }
        },

        destroy: function () {

            // if using jQuery UI 1.8.x
            $.Widget.prototype.destroy.call(this);
            // if using jQuery UI 1.9.x
            //this._destroy();
        },

        _expandSnapshot: function (snapshot) {
            var options = this.options;
            var widget = this;

            // fill in data points for moods which have been omitted due to zero values
            $.each(snapshot.m, function (n) {
                var mood = this;
                if ($.Enumerable.From(snapshot.d).Where(function (dp) { return dp.i == mood.DisplayIndex; }).Any() == false) {
                    snapshot.d.push({ i: mood.DisplayIndex, p: 0, c: 0 });
                }
            });

            snapshot.d = $.Enumerable.From(snapshot.d).OrderByDescending(function (dp) { return dp.i; }).ToArray();

            return snapshot;
        },

        _refreshData: function () {
            var options = this.options;
            var widget = this;
            var canvas = this.canvas;

            if (options.dataUrl != null) {
                $.ajax({
                    url: options.dataUrl,
                    type: "GET",
                    dataType: "json",
                    success: function (newData) {
                        //var newData = { "t": Date(1362907200000), "d": [{ "i": 8, "p": 100.000000, "c": 1 }, { "i": 7, "p": 0.000000, "c": 1}], "r": 1, "m": [{ "Id": "22fc9ecb-6479-451b-66e2-08cfdc121625", "Name": "Passionate", "DisplayIndex": 1, "DisplayColor": "#f2932f", "MoodType": 1 }, { "Id": "9aa05fea-bf55-497a-66e3-08cfdc121625", "Name": "Excited", "DisplayIndex": 2, "DisplayColor": "#f9a924", "MoodType": 1 }, { "Id": "f7a20c32-b9f5-491b-66e4-08cfdc121625", "Name": "Proud", "DisplayIndex": 3, "DisplayColor": "#fdbc18", "MoodType": 1 }, { "Id": "bf07052c-ce76-4062-66e5-08cfdc121625", "Name": "Engaged", "DisplayIndex": 4, "DisplayColor": "#ffcc03", "MoodType": 1 }, { "Id": "6aae0cee-28dc-4a60-66e6-08cfdc121625", "Name": "Optimistic", "DisplayIndex": 5, "DisplayColor": "#ffda00", "MoodType": 1 }, { "Id": "cd7eb0f0-4736-4e6e-66e7-08cfdc121625", "Name": "Frustrated", "DisplayIndex": 6, "DisplayColor": "#eb641c", "MoodType": 2 }, { "Id": "d0ba5091-cd21-44c4-66e8-08cfdc121625", "Name": "Worried", "DisplayIndex": 7, "DisplayColor": "#c85424", "MoodType": 2 }, { "Id": "06a663de-bdd6-437b-66e9-08cfdc121625", "Name": "Bored", "DisplayIndex": 8, "DisplayColor": "#a3442a", "MoodType": 2 }, { "Id": "599997a2-d3eb-4d58-66ea-08cfdc121625", "Name": "Deflated", "DisplayIndex": 9, "DisplayColor": "#8e4c31", "MoodType": 2 }, { "Id": "fedcd389-7d75-413e-66eb-08cfdc121625", "Name": "Disengaged", "DisplayIndex": 10, "DisplayColor": "#775536", "MoodType": 2}] };
                        options.data = widget._expandSnapshot(newData);
                        if (widget.visuals != null) {
                            widget._updateVisualElements();
                        } else {
                            widget._createVisualElements();
                        }
                    },
                    error: function (xhr, status, err) {
                        // swallow it
                    },
                    complete: function () {
                        if (options.updateInterval != null) {
                            setTimeout(function () { widget._refreshData(); }, options.updateInterval);
                        }
                    }
                });
            }
        },

        _createVisualElements: function () {
            var options = this.options;
            var widget = this;
            var canvas = this.canvas;

            widget.visuals = {};

            var moods = $.Enumerable.From(options.data.m).OrderByDescending(function (mood) { return mood.DisplayIndex; });
            var moodResponses = $.Enumerable.From(options.data.d).OrderByDescending(function (mr) { return mr.i; });

            var maxMoodPercent = moodResponses.Select(function (mr) { return Math.round(mr.p); }).Max();
            var dominantMoodCount = moodResponses.Where(function (mr) { return Math.round(mr.p) == maxMoodPercent; }).Count();

            var positions = widget._calculateMainElementPositions();

            // create header...
            widget.visuals.header = {
                top: positions.headerTop,
                height: positions.headerHeight
            };

            if (options.theme.logoImageUrl != null) {
                if (options.theme.logoImageUrl.endsWith(".svg")) {
                    fabric.loadSVGFromURL(options.theme.logoImageUrl, function (objects) {
                        var group = new fabric.Group(objects, {
                            top: widget.visuals.header.top,
                            left: positions.barCentre.x,
                            originX: "center",
                            originY: "top"
                        });
                        group.scaleToHeight(widget.visuals.header.height);
                        canvas.add(group);
                        canvas.renderAll();
                        widget.visuals.header.logo = group;
                    });
                } else {
                    fabric.Image.fromURL(options.theme.logoImageUrl, function (img) {
                        img.set({
                            top: widget.visuals.header.top,
                            left: positions.barCentre.x,
                            originX: "center",
                            originY: "top"
                        });
                        img.scaleToHeight(widget.visuals.header.height);
                        canvas.add(img);
                        canvas.renderAll();
                        widget.visuals.header.logo = img;
                    });
                }
            }

            // create mood slices...
            widget.visuals.slices = new Array();

            var startY = positions.barTop;
            var previousMoodType = moods.First().MoodType;

            var moodTypes = moods.Select(function (m) { return m.MoodType; }).Distinct();
            var moodsAndResponses = moods.Join(moodResponses, "m=>m.DisplayIndex", "mr=>mr.i", function (m, mr) { return { mood: m, moodResponse: mr} });
            var numDividersVisible = Math.max(0, moodTypes.Count(function (mt) { return moodsAndResponses.Where(function (x) { return x.mood.MoodType == mt; }).Select(function (x) { return x.moodResponse.p; }).Sum() > 0; }) - 1);
            var moodSlicesTotalHeight = positions.barHeight - (positions.dividerHeight * numDividersVisible);

            moods.ForEach(function (mood, i) {
                var moodResponse = moodResponses.Where(function (mr) { return mr.i == mood.DisplayIndex; }).First();

                if (previousMoodType != mood.MoodType) {
                    // insert a "divider" slice...
                    var divider = {
                        top: startY,
                        moodsBefore: moods.Where(function (m) { return m.MoodType == previousMoodType; }).ToArray(),
                        moodsAfter: moods.Where(function (m) { return m.MoodType == mood.MoodType; }).ToArray()
                    };

                    divider.height = $.Enumerable.From(divider.moodsBefore).Any(function (m) { return moodResponses.Where(function (mr) { return mr.i == m.DisplayIndex; }).First().p > 0; }) &&
                                     $.Enumerable.From(divider.moodsAfter).Any(function (m) { return moodResponses.Where(function (mr) { return mr.i == m.DisplayIndex; }).First().p > 0; }) ? positions.dividerHeight : 0;

                    divider.background = new fabric.Path([
                        ["M", positions.barLeft, divider.top],
                        ["L", positions.barRight, divider.top],
                        ["L", positions.barRight, divider.top + divider.height],
                        ["L", positions.barLeft, divider.top + divider.height],
                        ["z"]
                    ],
                    {
                        stroke: options.theme.lineColor,
                        strokeWidth: 1,
                        fill: options.theme.dividerColor
                    });

                    widget.visuals.slices.push(divider);

                    startY += divider.height;
                }

                var roundedPercentage = Math.round(moodResponse.p);

                var slice = {
                    mood: mood,
                    top: startY,
                    height: moodSlicesTotalHeight * (moodResponse.p * 0.01),
                    width: positions.barWidth,
                    roundedPercentage: roundedPercentage,
                    showIcon: (roundedPercentage == maxMoodPercent) && (dominantMoodCount == 1),
                    showLabel: (roundedPercentage >= options.moodPercentageLabelThreshold),
                    labelTextColor: (roundedPercentage == maxMoodPercent) ? options.theme.dominantMoodTextColor : (mood.MoodType == 2) ? options.theme.negativeMoodTextColor : options.theme.positiveMoodTextColor
                };

                slice.background = new fabric.Path([
                    ["M", positions.barLeft, slice.top],
                    ["L", positions.barRight, slice.top],
                    ["L", positions.barRight, slice.top + slice.height],
                    ["L", positions.barLeft, slice.top + slice.height],
                    ["z"]
                ],
                {
                    stroke: options.theme.lineColor,
                    strokeWidth: 1,
                    fill: mood.DisplayColor
                });

                slice.label = new fabric.Text(String.format("{0} {1}%", mood.Name, roundedPercentage), {
                    useNative: true,
                    fontFamily: options.theme.fontFamily,
                    fontSize: widget._calculateLabelFontSize(moodResponse.p),
                    fill: slice.labelTextColor,
                    strokeWidth: 0,
                    top: slice.showIcon ? slice.top + (slice.height * 0.7) : slice.top + (slice.height * 0.5),
                    left: positions.barCentre.x,
                    originX: "center",
                    originY: "center",
                    scaleY: 1.5,
                    opacity: slice.showLabel ? 1 : 0
                });

                // attach icon
                fabric.loadSVGFromURL(String.format("{0}/{1}.svg", options.theme.moodImageRootUrl, mood.Name), function (objects) {
                    var group = new fabric.Group(objects, {
                        top: slice.top + (slice.height * 0.3),
                        left: positions.barCentre.x
                    });
                    group.set({ opacity: slice.showIcon ? 1 : 0 });
                    group.scaleToHeight(slice.height * 0.5);
                    if (group.getWidth() > slice.width * 0.8) {
                        group.scaleToWidth(slice.width * 0.8);
                    }
                    canvas.add(group);
                    canvas.renderAll();
                    slice.icon = group;
                });

                widget.visuals.slices.push(slice);

                startY += slice.height;
                previousMoodType = mood.MoodType;
            });

            // add shadowing layer over bar
            var edgeMaskWidth = positions.marginX;

            widget.visuals.barMask = {
                top: new fabric.Path([
                    ["M", positions.barLeft, positions.barTop],
                    ["L", positions.barRight, positions.barTop],
                    ["L", positions.barRight - edgeMaskWidth, positions.barTop + edgeMaskWidth],
                    ["L", positions.barLeft + edgeMaskWidth, positions.barTop + edgeMaskWidth],
                    ["z"]
                ],
                {
                    strokeWidth: 0
                }),
                bottom: new fabric.Path([
                    ["M", positions.barLeft, positions.barBottom],
                    ["L", positions.barRight, positions.barBottom],
                    ["L", positions.barRight - edgeMaskWidth, positions.barBottom - edgeMaskWidth],
                    ["L", positions.barLeft + edgeMaskWidth, positions.barBottom - edgeMaskWidth],
                    ["z"]
                ],
                {
                    strokeWidth: 0
                }),
                left: new fabric.Path([
                    ["M", positions.barLeft, positions.barTop],
                    ["L", positions.barLeft + edgeMaskWidth, positions.barTop + edgeMaskWidth],
                    ["L", positions.barLeft + edgeMaskWidth, positions.barBottom - edgeMaskWidth],
                    ["L", positions.barLeft, positions.barBottom],
                    ["z"]
                ],
                {
                    strokeWidth: 0
                }),
                right: new fabric.Path([
                    ["M", positions.barRight, positions.barTop],
                    ["L", positions.barRight - edgeMaskWidth, positions.barTop + edgeMaskWidth],
                    ["L", positions.barRight - edgeMaskWidth, positions.barBottom - edgeMaskWidth],
                    ["L", positions.barRight, positions.barBottom],
                    ["z"]
                ],
                {
                    strokeWidth: 0
                })
            };
            widget.visuals.barMask.top.setGradientFill({
                x1: 0,
                y1: 0,
                x2: 0,
                y2: edgeMaskWidth,
                colorStops: {
                    0: "rgba(0,0,0,0.6)",
                    1: "rgba(0,0,0,0)"
                }
            });

            widget.visuals.barMask.bottom.setGradientFill({
                x1: 0,
                y1: 0,
                x2: 0,
                y2: edgeMaskWidth,
                colorStops: {
                    0: "rgba(0,0,0,0)",
                    1: "rgba(0,0,0,0.6)"
                }
            });

            widget.visuals.barMask.left.setGradientFill({
                x1: 0,
                y1: 0,
                x2: edgeMaskWidth,
                y2: 0,
                colorStops: {
                    0: "rgba(0,0,0,0.6)",
                    1: "rgba(0,0,0,0)"
                }
            });

            widget.visuals.barMask.right.setGradientFill({
                x1: 0,
                y1: 0,
                x2: edgeMaskWidth,
                y2: 0,
                colorStops: {
                    0: "rgba(0,0,0,0)",
                    1: "rgba(0,0,0,0.6)"
                }
            });

            // add all visual elements to the canvas & render...
            $.each(widget.visuals.slices, function () {
                canvas.add(this.background);
                if (this.label != null) canvas.add(this.label);
            });

            canvas.add(widget.visuals.barMask.top);
            canvas.add(widget.visuals.barMask.bottom);
            canvas.add(widget.visuals.barMask.left);
            canvas.add(widget.visuals.barMask.right);

            canvas.renderAll();
        },

        _calculateMainElementPositions: function () {
            var options = this.options;
            var widget = this;
            var canvas = this.canvas;

            var marginX = canvas.width * 0.05;
            var marginY = canvas.height * 0.025;

            var barHeight = (canvas.height * 0.8);
            var barTop = canvas.height - barHeight - marginY;
            var barBottom = barTop + barHeight;
            var barLeft = marginX;
            var barWidth = canvas.width - (marginX * 2);
            var barRight = barLeft + barWidth;
            var barCentre = new fabric.Point(barLeft + (barWidth * 0.5), barTop + (barHeight * 0.5));

            var headerTop = marginY;
            var headerHeight = canvas.height - barHeight - (marginY * 2);

            var dividerHeight = barHeight * 0.005;

            return {
                marginX: marginX,
                marginY: marginY,
                headerTop: headerTop,
                headerHeight: headerHeight,
                barHeight: barHeight,
                barWidth: barWidth,
                barTop: barTop,
                barBottom: barBottom,
                barLeft: barLeft,
                barRight: barRight,
                barCentre: barCentre,
                dividerHeight: dividerHeight
            };
        },

        _calculateLabelFontSize: function (moodPercent) {
            var options = this.options;
            var widget = this;

            var fontSizeRange = options.theme.moodFontSizeMax - options.theme.moodFontSizeMin;
            var scaleFactor = (moodPercent - options.moodPercentageLabelThreshold) * 5 * 0.01; // e.g. 25% == full size

            return Math.round(options.theme.moodFontSizeMin + (Math.min(1, scaleFactor) * fontSizeRange));
        },

        _updateVisualElements: function () {
            var options = this.options;
            var widget = this;
            var canvas = this.canvas;

            var moods = $.Enumerable.From(options.data.m).OrderByDescending(function (mood) { return mood.DisplayIndex; });
            var moodResponses = $.Enumerable.From(options.data.d).OrderByDescending(function (mr) { return mr.i; });

            var maxMoodPercent = moodResponses.Select(function (mr) { return Math.round(mr.p); }).Max();
            var dominantMoodCount = moodResponses.Where(function (mr) { return Math.round(mr.p) == maxMoodPercent; }).Count();

            var positions = widget._calculateMainElementPositions();

            var startY = positions.barTop;

            var triggerRender = function () {
                canvas.renderAll();
            };

            var moodTypes = moods.Select(function (m) { return m.MoodType; }).Distinct();
            var moodsAndResponses = moods.Join(moodResponses, "m=>m.DisplayIndex", "mr=>mr.i", function (m, mr) { return { mood: m, moodResponse: mr} });
            var numDividersVisible = Math.max(0, moodTypes.Count(function (mt) { return moodsAndResponses.Where(function (x) { return x.mood.MoodType == mt; }).Select(function (x) { return x.moodResponse.p; }).Sum() > 0; }) - 1);
            var moodSlicesTotalHeight = positions.barHeight - (positions.dividerHeight * numDividersVisible);

            $.each(widget.visuals.slices, function (i) {
                var slice = this;

                if (slice.mood != null) {
                    // mood slice
                    var moodResponse = moodResponses.Where(function (mr) { return mr.i == slice.mood.DisplayIndex; }).First();

                    slice.top = startY;
                    slice.height = moodSlicesTotalHeight * (moodResponse.p * 0.01);
                    slice.roundedPercentage = Math.round(moodResponse.p);
                    slice.showIcon = (slice.roundedPercentage == maxMoodPercent) && (dominantMoodCount == 1);
                    slice.showLabel = (slice.roundedPercentage >= options.moodPercentageLabelThreshold);
                    slice.labelTextColor = (slice.roundedPercentage == maxMoodPercent) ? options.theme.dominantMoodTextColor : (slice.mood.MoodType == 1) ? options.theme.negativeMoodTextColor : options.theme.positiveMoodTextColor;
                } else {
                    // divider
                    slice.top = startY;
                    slice.height = $.Enumerable.From(slice.moodsBefore).Any(function (m) { return moodResponses.Where(function (mr) { return mr.i == m.DisplayIndex; }).First().p > 0; }) &&
                                   $.Enumerable.From(slice.moodsAfter).Any(function (m) { return moodResponses.Where(function (mr) { return mr.i == m.DisplayIndex; }).First().p > 0; }) ? positions.dividerHeight : 0;
                }


                if (slice.background != null) {
                    slice.background.path[0][2] = slice.top;
                    slice.background.path[1][2] = slice.top;
                    slice.background.path[2][2] = slice.top + slice.height;
                    slice.background.path[3][2] = slice.top + slice.height;

                    /*fabric.util.animate({
                    startValue: slice.background.path[0][2],
                    endValue: slice.top,
                    duration: 1500,
                    onChange: function (value) {
                    slice.background.path[0][2] = value;
                    slice.background.path[1][2] = value;
                    triggerRender();
                    }
                    });
                    fabric.util.animate({
                    startValue: slice.background.path[2][2],
                    endValue: slice.top + slice.height,
                    duration: 1500,
                    onChange: function (value) {
                    if(slice.mood) console.log(slice.mood.Name + " = h " + value);
                    slice.background.path[2][2] = value;
                    slice.background.path[3][2] = value;
                    triggerRender();
                    }
                    });*/
                }

                if (slice.label != null) {
                    slice.label.set({
                        top: slice.showIcon ? slice.top + (slice.height * 0.7) : slice.top + (slice.height * 0.5),
                        fontSize: widget._calculateLabelFontSize(moodResponse.p),
                        fill: slice.labelTextColor,
                        opacity: slice.showLabel ? 1 : 0
                    });
                    slice.label.setText(String.format("{0} {1}%", slice.mood.Name, slice.roundedPercentage));
                    //slice.label.animate("opacity", slice.showLabel ? 1 : 0, { duration: 1500, onChange: triggerRender });
                }

                if (slice.icon != null) {
                    slice.icon.set({
                        top: slice.top + (slice.height * 0.3),
                        opacity: slice.showIcon ? 1 : 0
                    });
                    slice.icon.scaleToHeight(slice.height * 0.5);
                    if (slice.icon.getWidth() > slice.width * 0.8) {
                        slice.icon.scaleToWidth(slice.width * 0.8);
                    }
                    //slice.icon.animate("opacity", slice.showIcon ? 1 : 0, { duration: 1500, onChange: triggerRender });
                }

                startY += slice.height;
            });

            triggerRender();
        }
    });

    function arrayToObject(arr) {
        var rv = {};
        for (var i = 0; i < arr.length; ++i)
            if (arr[i] !== undefined) rv[i] = arr[i];
        return rv;
    }

} (jQuery));

