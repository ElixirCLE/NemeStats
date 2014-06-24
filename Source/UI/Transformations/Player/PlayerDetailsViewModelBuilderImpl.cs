﻿using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UI.Models.PlayedGame;
using UI.Models.Players;

namespace UI.Transformations.Player
{
    public class PlayerDetailsViewModelBuilderImpl : PlayerDetailsViewModelBuilder
    {
        internal const string EXCEPTION_PLAYER_GAME_RESULTS_CANNOT_BE_NULL = "PlayerDetails.PlayerGameResults cannot be null.";
        internal const string EXCEPTION_PLAYER_STATISTICS_CANNOT_BE_NULL = "PlayerDetails.PlayerStatistics cannot be null.";
        internal GameResultViewModelBuilder gameResultViewModelBuilder;

        //TODO is this correct? MVC complained that I didn't have a parameterless constructor. Maybe called Mapper.Get or something like that?
        public PlayerDetailsViewModelBuilderImpl()
        {
            gameResultViewModelBuilder = new GameResultViewModelBuilderImpl();
        }

        public PlayerDetailsViewModelBuilderImpl(GameResultViewModelBuilder builder)
        {
            gameResultViewModelBuilder = builder;
        }

        public PlayerDetailsViewModel Build(PlayerDetails playerDetails)
        {
            Validate(playerDetails);

            PlayerDetailsViewModel playerDetailsViewModel = new PlayerDetailsViewModel();
            playerDetailsViewModel.PlayerId = playerDetails.Id;
            playerDetailsViewModel.PlayerName = playerDetails.Name;
            playerDetailsViewModel.Active = playerDetails.Active;
            playerDetailsViewModel.TotalGamesPlayed = playerDetails.PlayerStats.TotalGames;

            PopulatePlayerGameSummaries(playerDetails, playerDetailsViewModel);

            return playerDetailsViewModel;
        }

        private static void Validate(PlayerDetails playerDetails)
        {
            ValidatePlayerDetailsIsNotNull(playerDetails);
            ValidatePlayerGameResultsIsNotNull(playerDetails);
            ValidatePlayerStatisticsIsNotNull(playerDetails);
        }

        private static void ValidatePlayerDetailsIsNotNull(PlayerDetails playerDetails)
        {
            if (playerDetails == null)
            {
                throw new ArgumentNullException("playerDetails");
            }
        }

        private static void ValidatePlayerGameResultsIsNotNull(PlayerDetails playerDetails)
        {
            if (playerDetails.PlayerGameResults == null)
            {
                throw new ArgumentException(EXCEPTION_PLAYER_GAME_RESULTS_CANNOT_BE_NULL);
            }
        }

        private static void ValidatePlayerStatisticsIsNotNull(PlayerDetails playerDetails)
        {
            if (playerDetails.PlayerStats == null)
            {
                throw new ArgumentException(EXCEPTION_PLAYER_STATISTICS_CANNOT_BE_NULL);
            }
        }

        private void PopulatePlayerGameSummaries(PlayerDetails playerDetails, PlayerDetailsViewModel playerDetailsViewModel)
        {
            playerDetailsViewModel.PlayerGameResultDetails = new List<GameResultViewModel>();
            GameResultViewModel gameResultViewModel;
            foreach (PlayerGameResult playerGameResult in playerDetails.PlayerGameResults)
            {
                gameResultViewModel = gameResultViewModelBuilder.Build(playerGameResult);
                playerDetailsViewModel.PlayerGameResultDetails.Add(gameResultViewModel);
            }
        }
    }
}