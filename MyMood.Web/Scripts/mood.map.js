/// <reference path="fabric.js" />
/// <reference path="moment.js" />

(function ($, undefined) {
    $.widget('discover.moodmap', {

        options: {
            data: null,
            dataUrl: null,
            updateInterval: null,
            fromDate: null,
            toDate: null,
            dayStartTime: { hour: 7, minute: 0 },
            dayEndTime: { hour: 22, minute: 0 },
            dayTimeMarkers: [{ hour: 7, minute: 0, lineWidth: 0 }, { hour: 12, minute: 0 }, { hour: 17, minute: 0}],
            theme: {
                backgroundColor: "#727880",
                dayBackgroundColor: "#545B66",
                lineColor: "#121212",
                dividerColor: "#FFFFFF",
                dividerWidth: 6,
                dataPointColor: "#FFFFFF",
                dataPointSize: 4,
                timeMarkerColor: "rgba(255,255,255,0.5)",
                timeMarkerWidth: 4,
                textColor: "#FFFFFF",
                fontFamily: "Impact",
                dayNameFormat: "dddd",
                dayNameFontSize: 26,
                timeMarkerFontSize: 16,
                showDataPoints: false,
                smoothLines: true,
                tension: 0.2,
                logoImageUrl: null,
                noDataLogoUrl: null
                
            },
            mode: 1 //(1=percentage, 2=count)
        },

        widgetEventPrefix: "",

        minDataPointsRequiredForSpline: 4,

        snapshotSpanInMinutes: 10,

        _create: function () {
            var options = this.options;
            var widget = this;

            // create canvas
            widget.canvas = new fabric.StaticCanvas($("<canvas>").attr("id", "canvas-" + $("canvas").length).appendTo(this.element).get(0), {
                renderOnAddition: false,
                backgroundColor: options.theme.backgroundColor
            });
            widget.canvas.setDimensions({ width: $(widget.element).width(), height: $(widget.element).height() });

            // cache remote assets...
            widget._cacheAssets();

            // create visual elements
            if (options.data != null) {
                options.data = widget._transformData(options.data);
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

        _cacheAssets: function () {
            var options = this.options;
            var widget = this;

            widget.assetCache = {};

            var positions = widget._calculateMainElementPositions();

            if (options.theme.logoImageUrl != null) {
                if (options.theme.logoImageUrl.endsWith(".svg")) {
                    fabric.loadSVGFromURL(options.theme.logoImageUrl, function (objects) {
                        var group = new fabric.Group(objects, {
                            top: widget.canvas.height - (positions.marginY * 0.5),
                            left: positions.marginX * 0.7,
                            originX: "left",
                            originY: "top"
                        });
                        group.scaleToHeight(positions.marginY * 0.5);
                        widget.assetCache.logo = group;
                    });
                } else {
                    fabric.Image.fromURL(options.theme.logoImageUrl, function (img) {
                        img.set({
                            top: widget.canvas.height - (positions.marginY * 0.5),
                            left: positions.marginX * 0.7,
                            originX: "left",
                            originY: "top"
                        });
                        img.scaleToHeight(positions.marginY * 0.5);
                        widget.assetCache.logo = img;
                    });
                }
            }

            if (options.theme.noDataLogoUrl != null) {
                if (options.theme.noDataLogoUrl.endsWith(".svg")) {
                    fabric.loadSVGFromURL(options.theme.noDataLogoUrl, function (objects) {
                        var group = new fabric.Group(objects, {
                            top: widget.canvas.height * 0.5,
                            left: widget.canvas.width * 0.5
                        });
                        group.scaleToHeight(widget.canvas.height * 0.5);
                        widget.assetCache.noDataLogo = group;
                    });
                } else {
                    fabric.Image.fromURL(options.theme.noDataLogoUrl, function (img) {
                        img.set({
                            top: widget.canvas.height * 0.5,
                            left: widget.canvas.width * 0.5
                        });
                        img.scaleToHeight(widget.canvas.height * 0.5);
                        widget.assetCache.noDataLogo = img;
                    });
                }
            }
        },

        _refreshData: function () {
            var options = this.options;
            var widget = this;

            var now = new Date();

            if (options.dataUrl != null) {
                var requestParams = {
                    request: {
                        ReportStart: widget.latestSnapshotDate || options.fromDate,
                        ReportEnd: options.toDate
                    }
                };

                $.ajax({
                    url: options.dataUrl,
                    //data: JSON.stringify(requestParams),
                    type: "GET",
                    dataType: "json",
                    success: function (newData) {
                        options.data = widget._transformData(newData);
                        //                        if (options.data == null) {
                        //                            options.data = widget._transformData(newData);
                        //                        } else {
                        //                            // todo: merge newer stuff into overall set of data
                        //                        }

                        //widget.latestSnapshotDate = max snapshot date;

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

        _transformData: function (data) {
            var widget = this;

            data.Moods = $.Enumerable.From(data.Moods).OrderByDescending(function (m) { return m.MoodType; }).ThenByDescending(function (m) { return m.DisplayIndex; }).ToArray();
            data.Snapshots = $.Enumerable.From(data.Snapshots).OrderBy(function (s) { return s.t; }).ToArray();

            widget._expandSnapshots(data.Moods, data.Snapshots);

            return data;
        },

        _mergeData: function (data) {
            var options = this.options;
            var widget = this;

        },

        _calculateMainElementPositions: function () {
            var options = this.options;
            var widget = this;
            var canvas = this.canvas;

            var marginX = canvas.width * 0.05;
            var marginY = canvas.height * 0.025;


            var chartHeight = (canvas.height * 0.7);
            var chartTop = (canvas.height * 0.5) - (chartHeight * 0.59);
            var chartBottom = chartTop + chartHeight;
            var chartLeft = marginX;
            var chartWidth = canvas.width - (marginX * 2);
            var chartRight = chartLeft + chartWidth;
            var chartCentre = new fabric.Point(chartLeft + (chartWidth * 0.5), chartTop + (chartHeight * 0.5));

            var headerTop = marginY;
            var headerHeight = canvas.height - chartHeight - (marginY * 2);

            var footerTop = chartBottom + (marginY * 2);
            var footerLeft = marginX;
            var footerHeight = canvas.height - footerTop - (marginY * 2);
            var footerWidth = canvas.width - (marginX * 2);
            var footerRight = canvas.width - marginX;
            var footerBottom = canvas.height - marginY;

            return {
                marginX: marginX,
                marginY: marginY,
                headerTop: headerTop,
                headerHeight: headerHeight,
                chartHeight: chartHeight,
                chartWidth: chartWidth,
                chartTop: chartTop,
                chartBottom: chartBottom,
                chartLeft: chartLeft,
                chartRight: chartRight,
                chartCentre: chartCentre,
                footerTop: footerTop,
                footerLeft: footerLeft,
                footerHeight: footerHeight,
                footerWidth: footerWidth,
                footerRight: footerRight,
                footerBottom: footerBottom
            };
        },

        _createVisualElements: function () {
            var options = this.options;
            var widget = this;
            var canvas = this.canvas;

            var positions = widget._calculateMainElementPositions();

            widget.visuals = {};

            var moods = $.Enumerable.From(options.data.Moods);
            var snapshots = $.Enumerable.From(options.data.Snapshots);

            var maxSnapshotTotal = snapshots.Select(function (s) { return $.Enumerable.From(s.d).Select(function (mr) { return mr.c; }).Sum(); }).Max();

            // round up to nearest 100
            maxSnapshotTotal = Math.ceil(maxSnapshotTotal * 0.01) * 100.0;

            if (snapshots.Any()) {

                var snapshotDays = $.Enumerable.From(options.data.Snapshots).Select(function (s) { return moment(s.t).clone().startOf("day").valueOf(); }).Distinct().ToArray();

                var minTime = snapshots.Select(function (s) { return s.t; }).Min();
                var maxTime = snapshots.Select(function (s) { return s.t; }).Max();

                // create sections for each day of the event (up to now)...
                widget.visuals.days = new Array();

                var dayMargin = canvas.width * 0.01;
                var dayWidth = (positions.chartWidth - ((snapshotDays.length - 1) * dayMargin)) / snapshotDays.length;

                $.each(snapshotDays, function (n) {
                    var date = this;

                    var day = {
                        date: date,
                        start: moment(date).hours(options.dayStartTime.hour).minute(options.dayStartTime.minute),
                        end: moment(date).hours(options.dayEndTime.hour).minute(options.dayEndTime.minute),
                        top: positions.chartTop,
                        bottom: positions.chartBottom,
                        left: positions.chartLeft + (n * (dayWidth + dayMargin)),
                        right: positions.chartLeft + (n * (dayWidth + dayMargin)) + dayWidth,
                        width: dayWidth,
                        height: positions.chartHeight,
                        label: new fabric.Text(moment(date).format(options.theme.dayNameFormat), {
                            useNative: true,
                            fontFamily: options.theme.fontFamily,
                            fontSize: options.theme.dayNameFontSize,
                            fill: options.theme.textColor,
                            strokeWidth: 0,
                            top: positions.chartTop - (positions.marginY * 0.1),
                            left: positions.chartLeft + (n * (dayWidth + dayMargin)),
                            originX: "left",
                            originY: "bottom",
                            scaleY: 1.5
                        }),
                        moodLayers: new Array(),
                        moodDividers: new Array(),
                        snapshotBars: new Array()
                    };

                    day.bg = new fabric.Rect({ top: day.top + (day.height * 0.5), left: day.left + (day.width * 0.5), width: day.width, height: day.height, fill: options.theme.dayBackgroundColor });


                    if (options.mode == 1) {
                        // create layers for each mood...
                        var previousMoodType = moods.First().MoodType;

                        moods.ForEach(function (mood, i) {

                            var moodLayer = {
                                mood: mood,
                                dataPoints: snapshots.Where(function (s) { return day.start.isAfter(s.t); }).Reverse().Take(1).Select(function (s) { return { t: day.start.toDate(), d: s.d }; })
                                .Concat(snapshots.Where(function (s) { return (day.start.isBefore(s.t)) && (day.end.isAfter(s.t)); }).Select(function (s) { return { t: s.t, d: s.d }; }))
                                .Concat(snapshots.Where(function (s) { return day.end.isBefore(s.t); }).Take(1).Select(function (s) { return { t: day.end.toDate(), d: s.d }; }))
                                .Select(function (s) {
                                    var dp = $.Enumerable.From(s.d).Where(function (mr) { return mr.i == mood.DisplayIndex; }).First();
                                    var offsetX = (s.t - day.start.toDate()) / (day.end.toDate() - day.start.toDate());
                                    return {
                                        time: s.t,
                                        displayIndex: dp.i,
                                        count: dp.c,
                                        percent: dp.p,
                                        offsetY: dp.offsetY,
                                        offsetX: offsetX,
                                        posY: day.top + (dp.offsetY * day.height),
                                        posX: day.left + (offsetX * day.width)
                                    };
                                })
                                .ToArray()
                            };

                            // create outline of mood layer...
                            var pathCommands = new Array();

                            if (options.theme.smoothLines && moodLayer.dataPoints.length >= widget.minDataPointsRequiredForSpline) {
                                // generate smooth spline
                                var flattenedPoints = $.Enumerable.From(moodLayer.dataPoints).SelectMany(function (dp) { return [dp.posX, dp.posY]; }).ToArray();

                                pathCommands = splineFromPoints(flattenedPoints, options.theme.tension);

                            } else {
                                // generate simple line
                                $.each(moodLayer.dataPoints, function (n) {
                                    var dp = this;
                                    pathCommands.push([(n == 0 ? "M" : "L"), dp.posX, dp.posY]);
                                });
                            }

                            if (previousMoodType != mood.MoodType) {
                                // insert a "divider"...
                                day.moodDividers.push({
                                    path: new fabric.Path(pathCommands.slice(0), { strokeWidth: options.theme.dividerWidth, stroke: options.theme.dividerColor, fill: null })
                                });
                            }

                            pathCommands.push(["L", moodLayer.dataPoints[moodLayer.dataPoints.length - 1].posX, day.bottom]);
                            pathCommands.push(["L", day.left, day.bottom]);
                            pathCommands.push(["z"]);

                            moodLayer.path = new fabric.Path(pathCommands, { strokeWidth: 0, fill: mood.DisplayColor });

                            // create visual data points (if required)...
                            if (options.theme.showDataPoints) {
                                moodLayer.points = new Array();
                                $.each(moodLayer.dataPoints, function (n) {
                                    var dp = this;
                                    moodLayer.points.push(new fabric.Circle({ top: dp.posY, left: dp.posX, radius: options.theme.dataPointSize * 0.5, strokeWidth: 0, fill: options.theme.dataPointColor }));
                                });
                            }


                            day.moodLayers.push(moodLayer);

                            previousMoodType = mood.MoodType;
                        });



                    } else if (options.mode == 2) {

                        var daySnapshots = snapshots.Where(function (s) { return day.start.isAfter(s.t); }).Reverse().Take(1).Select(function (s) { return { t: day.start.toDate(), d: s.d }; })
                            .Concat(snapshots.Where(function (s) { return (day.start.isBefore(s.t)) && (day.end.isAfter(s.t)); }).Select(function (s) { return { t: s.t, d: s.d }; }))
                            .Concat(snapshots.Where(function (s) { return day.end.isBefore(s.t); }).Take(1).Select(function (s) { return { t: day.end.toDate(), d: s.d }; }));

                        var snapshotBarWidth = day.width / (day.end.diff(day.start, "minutes") * 0.1); //Math.max(1, snapshots.Count());

                        if (daySnapshots.Any()) {

                            var currentPosX = day.left;

                            daySnapshots.ForEach(function (snapshot) {

                                var snapshotBar = {
                                    moodBars: new Array()
                                };

                                var currentPosY = day.bottom;

                                moods.Reverse().ForEach(function (mood) {
                                    var dp = $.Enumerable.From(snapshot.d).Where(function (mr) { return mr.i == mood.DisplayIndex; }).First();

                                    var moodBar = {
                                        left: currentPosX,
                                        bottom: currentPosY,
                                        height: Math.max(1, (dp.c / maxSnapshotTotal) * day.height),
                                        top: currentPosY - Math.max(1, (dp.c / maxSnapshotTotal) * day.height),
                                        width: snapshotBarWidth
                                    };

                                    moodBar.path = new fabric.Path([
                                        ["M", moodBar.left, moodBar.top],
                                        ["L", moodBar.left + moodBar.width, moodBar.top],
                                        ["L", moodBar.left + moodBar.width, moodBar.bottom],
                                        ["L", moodBar.left, moodBar.bottom],
                                        ["z"]
                                    ],
                                    {
                                        strokeWidth: 0,
                                        stroke: mood.DisplayColor,
                                        fill: mood.DisplayColor
                                    });

                                    snapshotBar.moodBars.push(moodBar);

                                    currentPosY -= moodBar.height;
                                });

                                day.snapshotBars.push(snapshotBar);

                                currentPosX += snapshotBarWidth;
                            });
                        }

                    }

                    // create time markers...
                    day.timeMarkers = new Array();

                    $.each(options.dayTimeMarkers, function () {
                        var markerTime = moment(day.date).hour(this.hour).minute(this.minute);

                        var offsetX = (markerTime.toDate() - day.start.toDate()) / (day.end.toDate() - day.start.toDate());
                        var posX = day.left + (offsetX * day.width);

                        day.timeMarkers.push({
                            marker: new fabric.Rect({
                                top: day.top,
                                left: posX,
                                height: day.height,
                                width: (this.lineWidth != null) ? this.lineWidth : options.theme.timeMarkerWidth,
                                strokeWidth: 0,
                                fill: options.theme.timeMarkerColor,
                                originX: "left",
                                originY: "top"
                            }),
                            label: new fabric.Text(markerTime.format("HH:mm"), {
                                useNative: true,
                                fontFamily: options.theme.fontFamily,
                                fontSize: options.theme.timeMarkerFontSize,
                                fill: options.theme.textColor,
                                strokeWidth: 0,
                                top: positions.chartBottom + (positions.marginY * 0.1),
                                left: posX,
                                originX: "left",
                                originY: "top",
                                scaleY: 1.5
                            })
                        });
                    });

                    // frame to trim any curves/lines that slip outside the chart area...
                    day.frame = new fabric.Path([
                        ["M", day.left - dayMargin, 0],
                        ["L", day.right + dayMargin, 0],
                        ["L", day.right + dayMargin, canvas.height],
                        ["L", day.left - dayMargin, canvas.height],
                        ["L", day.left - dayMargin, day.top],
                        ["L", day.left, day.top],
                        ["L", day.left, day.bottom],
                        ["L", day.right, day.bottom],
                        ["L", day.right, day.top],
                        ["L", day.left - dayMargin, day.top],
                        ["z"]
                    ],
                    {
                        strokeWidth: 0,
                        fill: options.theme.backgroundColor
                    });

                    widget.visuals.days.push(day);
                });

                if (options.mode == 2 && widget.visuals.days.length > 0) {
                    // show scale for y-axis
                    widget.visuals.axisYLabels = new Array();

                    var axisYLabelPos = $.Enumerable.From(widget.visuals.days).Select(function (d) { return { x: d.right, top: d.top, bottom: d.bottom, middle: d.top + (d.height * 0.5) }; }).Last();

                    widget.visuals.axisYLabels.push(new fabric.Text(String.format("{0}", maxSnapshotTotal),
                        {
                            useNative: true,
                            fontFamily: options.theme.fontFamily,
                            fontSize: options.theme.timeMarkerFontSize * 0.8,
                            fill: options.theme.textColor,
                            strokeWidth: 0,
                            top: axisYLabelPos.top,
                            left: axisYLabelPos.x + 10,
                            originX: "left",
                            originY: "center",
                            scaleY: 1.5
                        }));
                    widget.visuals.axisYLabels.push(new fabric.Text(String.format("{0}", maxSnapshotTotal * 0.5),
                        {
                            useNative: true,
                            fontFamily: options.theme.fontFamily,
                            fontSize: options.theme.timeMarkerFontSize * 0.8,
                            fill: options.theme.textColor,
                            strokeWidth: 0,
                            top: axisYLabelPos.middle,
                            left: axisYLabelPos.x + 10,
                            originX: "left",
                            originY: "center",
                            scaleY: 1.5
                        }));
                    widget.visuals.axisYLabels.push(new fabric.Text(String.format("{0}", 0),
                        {
                            useNative: true,
                            fontFamily: options.theme.fontFamily,
                            fontSize: options.theme.timeMarkerFontSize * 0.8,
                            fill: options.theme.textColor,
                            strokeWidth: 0,
                            top: axisYLabelPos.bottom,
                            left: axisYLabelPos.x + 10,
                            originX: "left",
                            originY: "center",
                            scaleY: 1.5
                        }));
                }


                // mood legend items...
                widget.visuals.legendItems = new Array();

                var now = moment().utc();

                var latestSnapshot = snapshots.Last();

                var moodLegendItemWidth = positions.footerWidth / moods.Count();

                var currentLegendX = positions.footerLeft;

                moods.ForEach(function (mood) {
                    var moodPercentage = $.Enumerable.From(latestSnapshot.d).Where(function (mr) { return mr.i == mood.DisplayIndex; }).Select(function (mr) { return mr.p; }).FirstOrDefault();
                    var moodSubTotal = $.Enumerable.From(latestSnapshot.d).Where(function (mr) { return mr.i == mood.DisplayIndex; }).Select(function (mr) { return mr.c; }).FirstOrDefault();

                    var legendItem = {
                        boxTop: positions.footerBottom - (positions.footerHeight * 0.1),
                        boxLeft: currentLegendX + (moodLegendItemWidth * 0.1),
                        width: moodLegendItemWidth * 0.8,
                        height: (positions.footerHeight * 0.1)
                    };

                    legendItem.box = new fabric.Path([
                        ["M", legendItem.boxLeft, legendItem.boxTop],
                        ["L", legendItem.boxLeft + legendItem.width, legendItem.boxTop],
                        ["L", legendItem.boxLeft + legendItem.width, legendItem.boxTop + legendItem.height],
                        ["L", legendItem.boxLeft, legendItem.boxTop + legendItem.height],
                        ["z"]
                    ],
                    {
                        stroke: "#FFFFFF",
                        strokeWidth: 2,
                        fill: mood.DisplayColor
                    });

                    var textLineY = positions.footerTop + (positions.footerHeight * 0.6);
                    var numericText = options.mode == 1 ? String.format("{0} %", Math.round(moodPercentage)) : String.format("{0}", moodSubTotal);

                    legendItem.percent = new fabric.Text(numericText, {
                        useNative: true,
                        fontFamily: options.theme.fontFamily,
                        fontSize: 30,
                        fill: "#FFFFFF",
                        strokeWidth: 0,
                        top: textLineY,
                        left: currentLegendX + (moodLegendItemWidth * 0.5),
                        originX: "center",
                        originY: "bottom",
                        scaleY: 1.0,
                        scaleX: 1.5
                    });

                    legendItem.name = new fabric.Text(String.format("{0}", mood.Name), {
                        useNative: true,
                        fontFamily: options.theme.fontFamily,
                        fontSize: 22,
                        fill: "#F0F0F0",
                        strokeWidth: 0,
                        top: textLineY,
                        left: currentLegendX + (moodLegendItemWidth * 0.5),
                        originX: "center",
                        originY: "top",
                        scaleY: 1.6
                    });

                    widget.visuals.legendItems.push(legendItem);

                    currentLegendX += moodLegendItemWidth;
                });

                // add visual elements to canvas & render...
                canvas.clear();

                $.each(widget.visuals.days, function () {
                    var day = this;

                    if (day.bg != null) canvas.add(day.bg);

                    if (day.moodLayers != null) {
                        $.each(day.moodLayers, function () {
                            if (this.path != null) canvas.add(this.path);
                            if (this.points != null) {
                                $.each(this.points, function () { canvas.add(this); });
                            }
                        });

                        $.each(day.moodDividers, function () {
                            if (this.path != null) canvas.add(this.path);
                        });
                    }
                    if (day.snapshotBars != null) {
                        $.each(day.snapshotBars, function () {
                            var snapshotBar = this;
                            $.each(snapshotBar.moodBars, function () {
                                if (this.path != null) canvas.add(this.path);
                            });
                        });
                    }
                });

                $.each(widget.visuals.days, function () {
                    var day = this;

                    if (day.frame != null) canvas.add(day.frame);

                    if (day.label != null) canvas.add(day.label);

                    if (day.timeMarkers != null) {
                        $.each(day.timeMarkers, function () {
                            if (this.marker != null) canvas.add(this.marker);
                            if (this.label != null) canvas.add(this.label);
                        });
                    }
                });

                $.each(widget.visuals.legendItems, function () {
                    var legendItem = this;

                    if (legendItem.box != null) canvas.add(legendItem.box);
                    if (legendItem.percent != null) canvas.add(legendItem.percent);
                    if (legendItem.name != null) canvas.add(legendItem.name);
                });

                if (widget.visuals.axisYLabels != null) {
                    $.each(widget.visuals.axisYLabels, function () {
                        var lbl = this;
                        canvas.add(lbl);
                    });
                }

            } else {
                canvas.clear();

                if (widget.assetCache.noDataLogo) {
                    widget.assetCache.noDataLogo.set({ opacity: 0.5 });
                    canvas.add(widget.assetCache.noDataLogo);
                }
            }



            canvas.renderAll();
        },

        _updateVisualElements: function () {
            var options = this.options;
            var widget = this;
            var canvas = this.canvas;

            widget._createVisualElements();
        },

        _expandSnapshots: function (moods, snapshots) {
            var options = this.options;
            var widget = this;

            for (var i = 0; i < snapshots.length; i++) {
                var thisSnapshot = snapshots[i];

                // fill in data points for moods which have been omitted due to zero values
                $.each(moods, function (n) {
                    var mood = this;
                    if ($.Enumerable.From(thisSnapshot.d).Where(function (dp) { return dp.i == mood.DisplayIndex; }).Any() == false) {
                        thisSnapshot.d.push({ i: mood.DisplayIndex, p: 0, c: 0 });
                    }
                });

                thisSnapshot.d = $.Enumerable.From(thisSnapshot.d).OrderByDescending(function (dp) { return dp.i; }).ToArray();

                // pre-calculate y-offset for the data points
                var offsetY = 0;
                $.each(thisSnapshot.d, function (n) {
                    var dp = this;
                    dp.offsetY = offsetY;
                    offsetY += (dp.p * 0.01);
                });

                // inject a snapshot where the previous snapshot was more than X minutes before
                if (i > 0) {
                    var previousSnapshot = snapshots[i - 1];

                    if (moment(thisSnapshot.t).diff(previousSnapshot.t, "minutes") > widget.snapshotSpanInMinutes) {
                        var extraSnapshot = $.extend({}, previousSnapshot);
                        extraSnapshot.t = moment(thisSnapshot.t).subtract(widget.snapshotSpanInMinutes, "minutes").toDate();
                        snapshots.splice(i, 0, extraSnapshot);
                    }
                }
            }
        }


    });

    function splineFromPoints(pts, t) {
        var path = new Array();

        var cp = new Array(); // array of control points, as x0,y0,x1,y1,...
        var n = pts.length;

        for (var i = 0; i < n - 4; i += 2) {
            cp = cp.concat(getControlPointsForSpline(pts[i], pts[i + 1], pts[i + 2], pts[i + 3], pts[i + 4], pts[i + 5], t));
        }

        path.push(["M", pts[0], pts[1]]);
        path.push(["Q", cp[0], cp[1], pts[2], pts[3]]);

        for (var i = 2; i < n - 5; i += 2) {
            path.push(["C", cp[2 * i - 2], cp[2 * i - 1], cp[2 * i], cp[2 * i + 1], pts[i + 2], pts[i + 3]]);
        }

        path.push(["Q", cp[2 * n - 10], cp[2 * n - 9], pts[n - 2], pts[n - 1]]);

        return path;
    };

    function getControlPointsForSpline(x0, y0, x1, y1, x2, y2, t) {
        //  x0,y0,x1,y1 are the coordinates of the end (knot) pts of this segment
        //  x2,y2 is the next knot -- not connected here but needed to calculate p2
        //  p1 is the control point calculated here, from x1 back toward x0.
        //  p2 is the next control point, calculated here and returned to become the 
        //  next segment's p1.
        //  t is the 'tension' which controls how far the control points spread.

        //  Scaling factors: distances from this knot to the previous and following knots.
        var d01 = Math.sqrt(Math.pow(x1 - x0, 2) + Math.pow(y1 - y0, 2));
        var d12 = Math.sqrt(Math.pow(x2 - x1, 2) + Math.pow(y2 - y1, 2));

        var fa = t * d01 / (d01 + d12);
        var fb = t - fa;

        var p1x = (x1 + fa * (x0 - x2));
        var p1y = (y1 + fa * (y0 - y2));

        var p2x = (x1 - fb * (x0 - x2));
        var p2y = (y1 - fb * (y0 - y2));

        return [p1x, p1y, p2x, p2y];
    };

} (jQuery));