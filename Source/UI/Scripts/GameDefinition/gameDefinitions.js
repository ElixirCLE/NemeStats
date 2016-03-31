﻿//Usings
Namespace("Views.GameDefinitions");

//Initialization
Views.GameDefinition.GameDefinitions = function () {
    this.$container = null;
    this.$gameDefinitionsTable = null;
};

//Implementation
Views.GameDefinition.GameDefinitions.prototype = {
    init: function () {
        this.$container = $(".gameDefinitionsList");
        this.$gameDefinitionsTable = this.$container.find("table");

        var valueNames = [
            'name',
            'plays-col',
             { name: 'champion-col', attr: 'data-champion' },
        ];

        var gameList = new List("gameDefinitionsList", { valueNames, page: 10, plugins: [ListPagination({ innerWindow : 10})] });
    },

    onGameCreated: function (gamingGroupId) {
        var container = $("#js-gamedefinitions");
        if (container.length === 1) {
            container
                .fadeOut('fast')
                .load('/GamingGroup/GetCurrentUserGamingGroupGameDefinitions?id=' + gamingGroupId)
                .fadeIn('fast');
        }
    }
}