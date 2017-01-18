﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Models
{
    public class PlayerAchievement :  EntityWithTechnicalKey<int>
    {
        public override int Id { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        [Index("IX_PLAYERID_AND_ACHIEVEMENTID", 1, IsUnique = true)]
        public int PlayerId { get; set; }
        [Index("IX_PLAYERID_AND_ACHIEVEMENTID", 2, IsUnique = true)]
        public AchievementId AchievementId { get; set; }
        public AchievementLevel AchievementLevel { get; set; }

        public virtual Player Player { get; set; }

        [NotMapped]
        public List<int> RelatedEntities
        {
            get
            {
                var result = new List<int>();
                if (!string.IsNullOrEmpty(RelatedEntities_PlainArray))
                {
                    var entities = this.RelatedEntities_PlainArray.Split(',');
                    if (entities.Any())
                    {
                        result = entities.Select(int.Parse).ToList();
                    }
                }

                return result;
            }
            set { this.RelatedEntities_PlainArray = string.Join(",", value.Select(p => p.ToString()).ToArray()); }
        }

        public string RelatedEntities_PlainArray { get; set; }

    }
}
