﻿using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class DiversifiedAchievement : BaseAchievement
    {
        public DiversifiedAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public override AchievementId Id => AchievementId.Diversified;

        public override AchievementGroup Group => AchievementGroup.Game;

        public override string Name => "Diversified Gamer";

        public override string DescriptionFormat => "This Achievement is earned by playing {0} different Games.";

        public override string IconClass => "fa fa-pie-chart";

        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 5},
            {AchievementLevel.Silver, 25},
            {AchievementLevel.Gold, 100}
        };

        public override AchievementAwarded IsAwardedForThisPlayer(int playerId)
        {

            var result = new AchievementAwarded
            {
                AchievementId = this.Id
            };

            var differentPlayedGames =
                DataContext.GetQueryable<PlayerGameResult>()
                    .Where(pgr => pgr.PlayerId == playerId)
                    .Select(pgr => pgr.PlayedGame.GameDefinition.Id)
                    .Distinct()
                    .ToList();

            if (differentPlayedGames.Any())
            {
                var count = differentPlayedGames.Count;
                result.PlayerProgress = count;
                result.LevelAwarded = GetLevelAwarded(count);
                result.RelatedEntities = differentPlayedGames.ToList();
            }
            return result;
        }
    }
}