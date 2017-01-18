﻿using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class SocialButterflyAchievement : BaseAchievement
    {
        public SocialButterflyAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public override AchievementId Id => AchievementId.SocialButterfly;

        public override AchievementGroup Group => AchievementGroup.Player;

        public override string Name => "Social Butterfly";

        public override string DescriptionFormat => "This Achievement is earned by playing games with {0} different Players.";

        public override string IconClass => "ns-icon-butterfly";

        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 10},
            {AchievementLevel.Silver, 30},
            {AchievementLevel.Gold, 50}
        };

        public override AchievementAwarded IsAwardedForThisPlayer(int playerId)
        {
            var result = new AchievementAwarded
            {
                AchievementId = Id
            };

            var allPlayerIdsPlayedWith =
                DataContext
                    .GetQueryable<PlayerGameResult>()
                    .Where(x => x.PlayedGame.PlayerGameResults.Any(y => y.PlayerId == playerId) && x.PlayerId != playerId)
                    .Select(z => z.PlayerId)
                    .Distinct()
                    .ToList();

            result.PlayerProgress = allPlayerIdsPlayedWith.Count;
            result.RelatedEntities = allPlayerIdsPlayedWith;

            if (result.PlayerProgress < LevelThresholds[AchievementLevel.Bronze])
            {
                return result;
            }

            result.LevelAwarded = GetLevelAwarded(result.PlayerProgress);
               
            return result;
        }
    }
}
