/// <reference path="fabric.js" />

(function ($, undefined) {
    $.widget('discover.moodbreakdown', {

        options: {
            data: null,
            dataUrl: null,
            updateInterval: null,
            theme: {
                backgroundColor: "#727880",
                pieBackgroundColor: "#FFFFFF",
                lineColor: "#121212",
                textColor: "#FFFFFF",
                fontFamily: "Impact",
                moodNameFontSize: 48,
                moodPercentageFontSize: 130,
                moodImageRootUrl: "/Content/Images/Callout",
                noDataLogoUrl: null
            }
        },

        widgetEventPrefix: "",

        maxLabelsPerColumn: 3,

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
                if (options.updateInterval) {
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

            if (options.dataUrl != null) {
                $.ajax({
                    url: options.dataUrl,
                    type: "GET",
                    dataType: "json",
                    success: function (newData) {
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
                        if (options.updateInterval) {
                            setTimeout(function () { widget._refreshData(); }, options.updateInterval);
                        }
                    }
                });
            }
        },

        _cacheAssets: function () {

        },

        _createVisualElements: function () {
            var options = this.options;
            var widget = this;
            var canvas = this.canvas;

            var currentTimestamp = new Date().getTime();

            widget.visuals = {};

            var moods = $.Enumerable.From(options.data.m);
            var moodResponses = $.Enumerable.From(options.data.d);

            // pre-cache all mood images
            if (widget.moodImages == null) {
                widget.moodImages = {};
                moods.ForEach(function (m) {
                    fabric.Image.fromURL(String.format("{0}/{1}-(Left).png", options.theme.moodImageRootUrl, m.Name), function (img) {
                        img.set({ /*top: -1000, left: -1000,*/opacity: 0 });
                        canvas.add(img);
                        widget.moodImages[m.Name + "-left"] = img;
                    });
                    fabric.Image.fromURL(String.format("{0}/{1}-(Right).png", options.theme.moodImageRootUrl, m.Name), function (img) {
                        img.set({ /*top: -1000, left: -1000,*/opacity: 0 });
                        canvas.add(img);
                        widget.moodImages[m.Name + "-right"] = img;
                    });
                });
            }

            // calc scale factors (to use for elements designed for use at 1080p "native" resolution)
            var scaleFactorX = widget.scaleFactorX = canvas.width / 1750;
            var scaleFactorY = widget.scaleFactorY = canvas.height / 1080;

            // pie background
            var pie = widget.visuals.pie = new fabric.Circle({
                top: canvas.height * 0.5,
                left: canvas.width * 0.5,
                radius: canvas.height * 0.25,
                strokeWidth: 0,
                fill: options.theme.pieBackgroundColor,
                shadow: new fabric.Shadow({ color: "rgba(0,0,0,0.5)", blur: 20 })
            });

            // prepare groups for labelling slices...
            var labelMarginX = (canvas.width * 0.025);
            var labelMarginY = (canvas.height * 0.05);
            var columnWidth = ((canvas.width - widget.visuals.pie.width) * 0.5) - (labelMarginX * 4);
            var columnHeight = canvas.height - (labelMarginY * 2);

            var labelHeight = (columnHeight - (labelMarginY * (widget.maxLabelsPerColumn - 1))) / widget.maxLabelsPerColumn;
            var labelWidth = columnWidth;

            widget.visuals.leftColumn = {
                center: { x: labelMarginX + (columnWidth * 0.5), y: labelMarginY + (columnHeight * 0.5) },
                top: labelMarginY,
                left: labelMarginX,
                width: columnWidth,
                height: columnHeight,
                labels: new Array()
            };
            widget.visuals.rightColumn = {
                center: { x: canvas.width - labelMarginX - (columnWidth * 0.5), y: labelMarginY + (columnHeight * 0.5) },
                top: labelMarginY,
                left: canvas.width - labelMarginX - columnWidth,
                width: columnWidth,
                height: columnHeight,
                labels: new Array()
            };

            //canvas.add(new fabric.Rect({ stroke:"red", fill:"rgba(0,0,0,0.0)", top: leftColumn.center.y, left: leftColumn.center.x, width: leftColumn.width, height: leftColumn.height }));
            //canvas.add(new fabric.Rect({ stroke:"red", fill:"rgba(0,0,0,0.0)", top: rightColumn.center.y, left: rightColumn.center.x, width: rightColumn.width, height: rightColumn.height }));

            // right column labels...
            var rightColumn = widget.visuals.rightColumn;
            $.Enumerable.Range(0, widget.maxLabelsPerColumn).ForEach(function (i) {
                rightColumn.labels.push(widget._createLabelVisuals({
                    textAlignment: "right",
                    center: {
                        x: rightColumn.center.x,
                        y: rightColumn.top + (labelHeight * 0.5) + (rightColumn.labels.length * (labelHeight + labelMarginY))
                    },
                    top: rightColumn.top + (rightColumn.height * 0.5) + ((rightColumn.labels.length) * (labelHeight + labelMarginY)),
                    left: rightColumn.left,
                    width: rightColumn.width,
                    height: labelHeight
                }));
            });

            // left column labels...
            var leftColumn = widget.visuals.leftColumn;
            $.Enumerable.Range(0, widget.maxLabelsPerColumn).ForEach(function (i) {
                leftColumn.labels.push(widget._createLabelVisuals({
                    textAlignment: "left",
                    center: {
                        x: leftColumn.center.x,
                        y: (leftColumn.top + leftColumn.height) - (labelHeight * 0.5 + (leftColumn.labels.length * (labelHeight + labelMarginY)))
                    },
                    top: (leftColumn.top + leftColumn.height) - (labelHeight + ((leftColumn.labels.length) * (labelHeight + labelMarginY))),
                    left: leftColumn.left,
                    width: leftColumn.width,
                    height: labelHeight
                }));
            });

            // create each mood slice...
            widget.visuals.slices = new Array();

            var pieCentre = widget.visuals.pie.getCenterPoint();
            var pieRadius = widget.visuals.pie.width * 0.5;
            var sliceRadius = pieRadius * 0.9;
            var startAngle = 270;

            moods.OrderBy(function (mood) { return mood.DisplayIndex; }).ForEach(function (mood, i) {
                var moodResponse = moodResponses.Where(function (mr) { return mr.i == mood.DisplayIndex; }).First();

                // create pie slice
                var sweptAngle = moodResponse.p * 0.01 * 360.0;

                var slice = new fabric.Path([
                    ["M", pieCentre.x, pieCentre.y],
                    ["L", pieCentre.x + (sliceRadius * Math.cos(startAngle * Math.PI / 180.0)), pieCentre.y + (sliceRadius * Math.sin(startAngle * Math.PI / 180.0))],
                    ["A", sliceRadius, sliceRadius, 0, moodResponse.p > 50.0 ? 1 : 0, 1, pieCentre.x + (sliceRadius * Math.cos((startAngle + sweptAngle) * Math.PI / 180.0)), pieCentre.y + (sliceRadius * Math.sin((startAngle + sweptAngle) * Math.PI / 180.0))],
                    ["z"]
                ],
                {
                    stroke: options.theme.pieBackgroundColor,
                    strokeWidth: 3,
                    fill: mood.DisplayColor
                });

                slice.mood = mood;
                slice.moodResponse = moodResponse;
                slice.sliceRadius = sliceRadius;
                slice.startAngle = startAngle;
                slice.sweptAngle = sweptAngle;

                widget.visuals.slices.push(slice);

                // attach a label (if sufficient response percentage)
                var roundedPercentage = Math.round(moodResponse.p);
                if (roundedPercentage >= (100 / moods.Count())) {
                    var midSectorAngle = startAngle - 270 + (sweptAngle * 0.5);
                    var preferUpperSector = (midSectorAngle < 90 || midSectorAngle > 270);

                    var availableLabel = null;

                    if (midSectorAngle < 180) {
                        availableLabel = preferUpperSector ?
                            $.Enumerable.From(rightColumn.labels).Take(Math.round(widget.maxLabelsPerColumn * 0.5)).Where(function (l) { return l.timestamp < currentTimestamp; }).FirstOrDefault() :
                            $.Enumerable.From(rightColumn.labels).Skip(Math.floor(widget.maxLabelsPerColumn * 0.5)).Where(function (l) { return l.timestamp < currentTimestamp; }).FirstOrDefault();
                    } else {
                        availableLabel = preferUpperSector ?
                            $.Enumerable.From(leftColumn.labels).Skip(Math.floor(widget.maxLabelsPerColumn * 0.5)).Where(function (l) { return l.timestamp < currentTimestamp; }).FirstOrDefault() :
                            $.Enumerable.From(leftColumn.labels).Take(Math.round(widget.maxLabelsPerColumn * 0.5)).Where(function (l) { return l.timestamp < currentTimestamp; }).FirstOrDefault();
                    }

                    if (availableLabel != null) {
                        widget._updateLabel(availableLabel, slice, mood.Name, roundedPercentage, currentTimestamp);
                    }
                }

                startAngle += sweptAngle;
            });

            // add all visual elements to the canvas & render...
            canvas.add(widget.visuals.pie);

            $.each(widget.visuals.slices, function () {
                canvas.add(this);
            });

            $.each(widget.visuals.rightColumn.labels, function () {
                canvas.add(this.moodName);
                canvas.add(this.moodPercent);
                canvas.add(this.line);
            });

            $.each(widget.visuals.leftColumn.labels, function () {
                canvas.add(this.moodName);
                canvas.add(this.moodPercent);
                canvas.add(this.line);
            });

            canvas.renderAll();
        },

        _createLabelVisuals: function (label) {
            var options = this.options;
            var widget = this;

            label.timestamp = 0;

            label.moodName = new fabric.Text("", {
                useNative: true,
                fontFamily: options.theme.fontFamily,
                fontSize: Math.round(options.theme.moodNameFontSize * widget.scaleFactorY),
                fill: options.theme.textColor,
                top: label.center.y - Math.round(options.theme.moodNameFontSize * widget.scaleFactorY),
                left: label.center.x,
                originX: label.textAlignment,
                originY: "center",
                opacity: 0,
                scaleY: 1.5
            });

            label.moodPercent = new fabric.Text("", {
                useNative: true,
                fontFamily: options.theme.fontFamily,
                fontSize: Math.round(options.theme.moodPercentageFontSize * widget.scaleFactorY),
                fill: "#FFFFFF",
                top: label.center.y + Math.round(options.theme.moodPercentageFontSize * 0.5 * widget.scaleFactorY),
                left: label.center.x,
                originX: label.textAlignment,
                originY: "center",
                opacity: 0
            });

            label.line = new fabric.Line([
                label.textAlignment == "left" ? label.left + label.width : label.left,
                label.center.y,
                label.textAlignment == "left" ? label.left + label.width : label.left,
                label.center.y
            ],
            {
                fill: options.theme.lineColor,
                stroke: options.theme.lineColor,
                strokeWidth: 2,
                opacity: 0
            });

            label.moodImage = null;

            //widget.canvas.add(new fabric.Rect({ stroke:"red", fill:"rgba(0,0,0,0.0)", top: label.center.y, left: label.center.x, width: label.width, height: label.height }));

            return label;
        },

        _updateVisualElements: function () {
            var options = this.options;
            var widget = this;
            var canvas = this.canvas;

            var currentTimestamp = new Date().getTime();

            var moods = $.Enumerable.From(options.data.m);
            var moodResponses = $.Enumerable.From(options.data.d);

            var rightColumn = widget.visuals.rightColumn;
            var leftColumn = widget.visuals.leftColumn;

            var pieCentre = widget.visuals.pie.getCenterPoint();
            var pieRadius = widget.visuals.pie.width * 0.5;
            var sliceRadius = pieRadius * 0.9;
            var startAngle = 270;

            // clear all labels
            $.Enumerable.From(rightColumn.labels).Concat($.Enumerable.From(leftColumn.labels)).ForEach(function (l) { widget._updateLabel(l, null, "", 0, 0); });

            $.each(widget.visuals.slices, function (i) {
                var slice = this;
                var mood = slice.mood = moods.Where(function (m) { return m.Name == slice.mood.Name; }).First() || slice.mood;
                var moodResponse = moodResponses.Where(function (mr) { return mr.i == mood.DisplayIndex; }).First();

                var sweptAngle = moodResponse.p * 0.01 * 360.0;

                // update pie position/size
                slice.path[1][1] = pieCentre.x + (sliceRadius * Math.cos(startAngle * Math.PI / 180.0));
                slice.path[1][2] = pieCentre.y + (sliceRadius * Math.sin(startAngle * Math.PI / 180.0));

                slice.path[2][4] = moodResponse.p > 50.0 ? 1 : 0;
                slice.path[2][6] = pieCentre.x + (sliceRadius * Math.cos((startAngle + sweptAngle) * Math.PI / 180.0));
                slice.path[2][7] = pieCentre.y + (sliceRadius * Math.sin((startAngle + sweptAngle) * Math.PI / 180.0));

                slice.sliceRadius = sliceRadius;
                slice.startAngle = startAngle;
                slice.sweptAngle = sweptAngle;

                // locate and update a suitably positioned label...
                var roundedPercentage = Math.round(moodResponse.p);
                if (roundedPercentage >= (100 / moods.Count())) {
                    var midSectorAngle = startAngle - 270 + (sweptAngle * 0.5);
                    var preferUpperSector = (midSectorAngle < 90 || midSectorAngle > 270);

                    var availableLabel = null;

                    if (midSectorAngle < 180) {
                        availableLabel = preferUpperSector ?
                            $.Enumerable.From(rightColumn.labels).Take(Math.round(widget.maxLabelsPerColumn * 0.5)).Where(function (l) { return l.timestamp < currentTimestamp; }).FirstOrDefault() :
                            $.Enumerable.From(rightColumn.labels).Skip(Math.floor(widget.maxLabelsPerColumn * 0.5)).Where(function (l) { return l.timestamp < currentTimestamp; }).FirstOrDefault();
                    } else {
                        availableLabel = preferUpperSector ?
                            $.Enumerable.From(leftColumn.labels).Skip(Math.floor(widget.maxLabelsPerColumn * 0.5)).Where(function (l) { return l.timestamp < currentTimestamp; }).FirstOrDefault() :
                            $.Enumerable.From(leftColumn.labels).Take(Math.round(widget.maxLabelsPerColumn * 0.5)).Where(function (l) { return l.timestamp < currentTimestamp; }).FirstOrDefault();
                    }

                    if (availableLabel != null) {
                        widget._updateLabel(availableLabel, slice, mood.Name, roundedPercentage, currentTimestamp);
                    }
                }

                startAngle += sweptAngle;
            });

            // render the changes
            canvas.renderAll();
        },

        _updateLabel: function (label, targetSlice, moodName, moodPercent, currentTimestamp) {
            var options = this.options;
            var widget = this;
            var canvas = widget.canvas;

            if (targetSlice != null) {
                label.timestamp = currentTimestamp;

                label.moodName.setText(moodName);
                label.moodName.set({ opacity: 1 });

                label.moodPercent.setText(String.format("{0} %", moodPercent));
                label.moodPercent.set({ opacity: 1 });

                var pieCentre = widget.visuals.pie.getCenterPoint();
                var sliceCentre = new fabric.Point(pieCentre.x + (targetSlice.sliceRadius * 0.8 * Math.cos((targetSlice.startAngle + targetSlice.sweptAngle * 0.5) * Math.PI / 180.0)), pieCentre.y + (targetSlice.sliceRadius * 0.8 * Math.sin((targetSlice.startAngle + targetSlice.sweptAngle * 0.5) * Math.PI / 180.0)));
                label.line.set({ x1: sliceCentre.x, y1: sliceCentre.y, opacity: 1 });

                // NB: use pre-cached images to avoid delay/flicker during updates, 
                // but first assignment (during createVisualElements) likely to encounter cache miss, so allow for delay.
                label.moodImage = widget.moodImages[moodName + "-" + label.moodName.originX];
                if (label.moodImage == null) {
                    setTimeout(function () {
                        label.moodImage = widget.moodImages[moodName + "-" + label.moodName.originX];
                        label.moodImage.set({ scaleY: widget.scaleFactorY, top: label.center.y, left: label.center.x, width: label.width, opacity: 1 });
                        canvas.renderAll();
                    }, 100);
                } else {
                    label.moodImage.set({ scaleY: widget.scaleFactorY, top: label.center.y, left: label.center.x, width: label.width, opacity: 1 });
                }

            } else {
                label.timestamp = 0;

                label.moodName.set({ text: "", opacity: 0 });
                label.moodPercent.set({ text: "", opacity: 0 });
                label.line.set({ opacity: 0 });
                if (label.moodImage != null) {
                    label.moodImage.set({ top: -1000, left: -1000, opacity: 0 });
                    label.moodImage = null;
                }
            }
        }

    });


} (jQuery));