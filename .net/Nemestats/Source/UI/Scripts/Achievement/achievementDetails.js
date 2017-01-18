﻿//Usings
Namespace("Views.Achievement");

//Initialization
Views.Achievement.Details = function () {
    
};

//Implementation
Views.Achievement.Details.prototype = {
    init: function () {
        this.initListJs();
    },
    initListJs: function () {

        var gamedefinitionsValues = [{ name: 'name-col', attr: 'data-name' }];
        var gameDefinitionTableId = "gameslist";

        if (ResponsiveBootstrapToolkit.is('>=md')) {
            new List(gameDefinitionTableId, { valueNames: gamedefinitionsValues, page: 10, plugins: [ListPagination({ innerWindow: 10 })] });
        } else {
            new List(gameDefinitionTableId, { valueNames: gamedefinitionsValues });
        }


        var playedGamesValues = [{ name: 'name-col', attr: 'data-name' },{ name: 'date-col', attr: 'data-date' }];
        var playedGamesTableId = "playedgameslist";

        if (ResponsiveBootstrapToolkit.is('>=md')) {
            new List(playedGamesTableId, { valueNames: playedGamesValues, page: 10, plugins: [ListPagination({ innerWindow: 10 })] });
        } else {
            new List(playedGamesTableId, { valueNames: playedGamesValues });
        }

        var winnersValues = [{ name: 'playername-col', attr: 'data-name' }, { name: 'date-col', attr: 'data-date' }, { name: 'gaminggroup-col', attr: 'data-name' }, { name: 'level-col', attr: 'data-level' }];
        var winnerTableId = "winnersList";

        if (ResponsiveBootstrapToolkit.is('>=md')) {
            new List(winnerTableId, { valueNames: winnersValues, page: 10, plugins: [ListPagination({ innerWindow: 10 })] });
        } else {
            new List(winnerTableId, { valueNames: winnersValues });
        }

        var playersValues = [{ name: 'playername-col', attr: 'data-name' }, { name: 'gaminggroup-col', attr: 'data-name' }];
        var playersTablesIds = "playersRelatedList";

        if (ResponsiveBootstrapToolkit.is('>=md')) {
            new List(playersTablesIds, { valueNames: playersValues, page: 10, plugins: [ListPagination({ innerWindow: 10 })] });
        } else {
            new List(playersTablesIds, { valueNames: winnerplayersValuessValues });
        }
    },
};