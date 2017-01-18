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