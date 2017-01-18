﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BusinessLogic.Models.Games.Validation;
using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Models.Validation;

namespace BusinessLogic.Models.Games
{
    public class SavePlayedGameRequest
    {
        
        public SavePlayedGameRequest()
        {
            DatePlayed = DateTime.UtcNow;
        }

        public int? PlayedGameId { get; set; }

        public int? GameDefinitionId { get; set; }
        public int? BoardGameGeekGameDefinitionId { get; set; }
        public string GameDefinitionName { get; set; }

        public string Notes { get; set; }

        [PlayerRankValidation]
        [Required]
        public List<CreatePlayerRankRequest> PlayerRanks { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [MaxDate]
        public DateTime DatePlayed { get; set; }

        [Required]
        public WinnerTypes WinnerType { get; set; }

        public bool EditMode { get; set; }
    }

    public class CreatePlayerRankRequest  : IPlayerRank
    {
        
        public int? PlayerId { get; set; }
        [Required]
        public int GameRank { get; set; }
        public decimal? PointsScored { get; set; }

        [Required]
        public string PlayerName { get; set; }
    }
}