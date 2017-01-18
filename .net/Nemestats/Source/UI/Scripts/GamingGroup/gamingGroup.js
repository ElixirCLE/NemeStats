﻿//Usings
Namespace("Views.GamingGroup");

//Initialization
Views.GamingGroup.GamingGroupView = function () {
    this.$container = null;
    this.$title = null;
    this.$fromDatePicker = null;
    this.$toDatePicker = null;
    this.$settings = {
        fromDate: null,
        toDate: new Date()
    };
    this._serviceAddress = "/GamingGroup/UpdateGamingGroupName";
    this._googleAnalytics = null;
};

//Implementation
Views.GamingGroup.GamingGroupView.prototype = {
    init: function (gaObject, options) {
        var parent = this;
        this.$title = $("#gamingGroupTitle");
        this.$title.toEditBox({ onFocusOut: $.proxy(parent.renameGamingGroup, this), cssClass: "gaming-group-name" });
        this._googleAnalytics = gaObject;

        if (options.fromDate != null) {
            this.$settings.fromDate = options.fromDate;
        }

        if (options.toDate != null) {
            this.$settings.toDate = options.toDate;
        }
        this.$fromDatePicker = $("#from-date-picker");
        this.$toDatePicker = $("#to-date-picker");
        var minDate = new Date(2000, 0, 1);
        var currentMoment = moment();
        var currentLocalIso8601Date = currentMoment.format("YYYY-MM-DD");

        if (Modernizr.inputtypes.date) {
            //if supports HTML5 then use native date picker
            var toDateIso8601 = new moment(this.$settings.toDate).format("YYYY-MM-DD");
            this.$toDatePicker.attr("value", toDateIso8601);
            var minDateIso8601 = minDate.toISOString().split("T")[0];
            this.$fromDatePicker.attr("max", currentMoment.add("days", 1).format("YYYY-MM-DD"));
            this.$fromDatePicker.attr("min", minDateIso8601);
            this.$toDatePicker.attr("max", currentMoment.add("days", 1).format("YYYY-MM-DD"));
            this.$toDatePicker.attr("min", minDateIso8601);
        } else {
            // If not native HTML5 support, fallback to jQuery datePicker
            this.$fromDatePicker.datepicker({
                showOn: "button",
                buttonText: "<i class='fa fa-calendar'></i>",
                showButtonPanel: true,
                maxDate: new Date(),
                minDate: minDate,
                onClose: function (selectedDate) {
                    $("#to-date-picker").datepicker("option", "minDate", selectedDate);
                }
            }).datepicker("setDate", this.$settings.fromDate)
                .datepicker("option", "dateFormat", "yy-mm-dd");

            this.$toDatePicker.datepicker({
                showOn: "button",
                buttonText: "<i class='fa fa-calendar'></i>",
                showButtonPanel: true,
                maxDate: new Date(),
                minDate: minDate,
                onClose: function (selectedDate) {
                    $("#from-date-picker").datepicker("option", "minDate", selectedDate);
                }
            }).datepicker("setDate", this.$settings.toDate)
              .datepicker("option", "dateFormat", "yy-mm-dd");
        }

        this.initListJs();

    },
    initListJs: function () {

        var gamedefinitionsValues = ['name', 'plays-col', { name: 'champion-col', attr: 'data-champion' }];
        var gameDefinitionTableId = "gameDefinitionsList";

        var playersValues = [{ name: 'player-name-col', attr: 'data-name' }, { name: 'total-nemepoints-col', attr: 'data-nemepoints' }, 'played-games-col', 'avg-nemepoints-col', 'overall-win-col', 'championed-games-col', { name: 'nemesis-col', attr: 'data-nemesis' }, { name: 'achievements-col', attr: 'data-achievements' }];
        var playersTableId = "playersList";


        if (ResponsiveBootstrapToolkit.is('>=md')) {
            new List(gameDefinitionTableId, { valueNames: gamedefinitionsValues, page: 10, plugins: [ListPagination({ innerWindow: 10 })] });
            new List(playersTableId, { valueNames: playersValues, page: 10, plugins: [ListPagination({ innerWindow: 10 })] });
        } else {
            new List(gameDefinitionTableId, { valueNames: gamedefinitionsValues });
            new List(playersTableId, { valueNames: playersValues });
        }

    },
    renameGamingGroup: function (element) {
        var parent = this;
        $.ajax({
            type: "POST",
            url: parent._serviceAddress,
            data: { "gamingGroupName": element.value },
            success: function (data) {

            },
            error: function (err) {
                alert("Error " + err.status + ":\r\n" + err.statusText);
            },
            dataType: "json"
        });

        this.trackGAEvent("GamingGroups", "GamingGroupRenamed", "GamingGroupRenamed");
    },
    renderNemeStatsPointsLineGraph: function (url) {
        $.ajax({
            type: "GET",
            url: url,
            success: function (data) {
                var playerDataMap = {};
                var playerData = [];

                for (var i = data.playedGames.length - 1; i >= 0; i--) {
                    for (var j = 0; j < data.playedGames[i].playerGameResults.length; j++) {
                        var gameInfo = data.playedGames[i].playerGameResults[j];
                        if (playerDataMap[gameInfo.playerId] == null) {
                            playerDataMap[gameInfo.playerId] = {
                                values: [{ x: new Date(data.playedGames[i].datePlayed), y: 0 }]
                            };
                            playerData.push({ values: playerDataMap[gameInfo.playerId].values, key: gameInfo.playerName, disabled: !gameInfo.playerActive });
                        }
                        var lastIndex = playerDataMap[gameInfo.playerId].values.length - 1;
                        var nextValue = playerDataMap[gameInfo.playerId].values[lastIndex].y + gameInfo.totalNemeStatsPointsAwarded;
                        playerDataMap[gameInfo.playerId].values.push({ x: new Date(data.playedGames[i].datePlayed), y: nextValue });
                    }
                }

                nv.addGraph(function () {
                    var chart = nv.models.lineChart()
                        .useInteractiveGuideline(true)
                        .showLegend(true)
                        .showYAxis(true)
                        .showXAxis(true);

                    chart.xAxis
                        .axisLabel("Date")
                        .tickFormat(function (d) {
                            return d3.time.format("%x")(new Date(d))
                        });

                    chart.yAxis
                        .axisLabel("NemePoints")
                        .tickFormat(d3.format("d"));

                    d3.select("#NemeStatsPointsLineGraph svg")
                        .datum(playerData)
                        .call(chart);

                    nv.utils.windowResize(function () { chart.update() });
                });
            }
        });
    }
}

var clickElement = function (elementId) {
    $("#" + elementId).click();
}