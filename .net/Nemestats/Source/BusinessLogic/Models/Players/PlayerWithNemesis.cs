﻿#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion

using System.Collections.Generic;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.Points;

namespace BusinessLogic.Models.Players
{
    public class PlayerWithNemesis
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public string ApplicationUserId { get; set; }
        public bool PlayerRegistered { get; set; }
        public int? NemesisPlayerId { get; set; }
        public string NemesisPlayerName { get; set; }
        public int? PreviousNemesisPlayerId { get; set; }
        public string PreviousNemesisPlayerName { get; set; }
        public int GamingGroupId { get; set; }
        public int GamesWon { get; set; }
        public int GamesLost { get; set; }
        public int TotalChampionedGames { get; set; }
        public bool PlayerActive { get; set; }
        public NemePointsSummary NemePointsSummary { get; set; }
        public Dictionary<AchievementLevel,int> AchievementsPerLevel { get; set; } = new Dictionary<AchievementLevel, int>();

    }
}
