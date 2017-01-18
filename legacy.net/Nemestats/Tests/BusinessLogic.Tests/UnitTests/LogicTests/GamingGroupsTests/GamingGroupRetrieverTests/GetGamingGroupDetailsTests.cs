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

using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Players;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GamingGroupsTests.GamingGroupRetrieverTests
{
    [TestFixture]
    public class GetGamingGroupDetailsTests : GamingGroupRetrieverTestBase
    {
        private GamingGroup expectedGamingGroup;
        private List<GameDefinitionSummary> gameDefinitionSummaries;
        private GamingGroupFilter filter;

        private int gamingGroupId = 13511;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            expectedGamingGroup = new GamingGroup
            {
                Id = gamingGroupId, 
                OwningUserId = CurrentUser.Id
            };

            filter = new GamingGroupFilter
            {
                GamingGroupId = gamingGroupId
            };

            AutoMocker.Get<IDataContext>().Expect(mock => mock.FindById<GamingGroup>(gamingGroupId))
                .Return(expectedGamingGroup);

            gameDefinitionSummaries = new List<GameDefinitionSummary>
            {
                new GameDefinitionSummary()
            };

            AutoMocker.Get<IGameDefinitionRetriever>().Expect(mock => mock.GetAllGameDefinitions(gamingGroupId, filter.DateRangeFilter))
                                       .Return(gameDefinitionSummaries);

            List<ApplicationUser> applicationUsers = new List<ApplicationUser>();
            applicationUsers.Add(CurrentUser);

            AutoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<ApplicationUser>())
                .Return(applicationUsers.AsQueryable());

            AutoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<ApplicationUser>())
                .Return(applicationUsers.AsQueryable());
        }

        [Test]
        public void ItReturnsTheGamingGroupSummary()
        {
            GamingGroupSummary actualGamingGroup = AutoMocker.ClassUnderTest.GetGamingGroupDetails(filter);

            Assert.AreEqual(expectedGamingGroup.Id, actualGamingGroup.Id);
            Assert.AreEqual(expectedGamingGroup.Name, actualGamingGroup.Name);
            Assert.AreEqual(expectedGamingGroup.OwningUserId, actualGamingGroup.OwningUserId);
            Assert.AreEqual(expectedGamingGroup.DateCreated, actualGamingGroup.DateCreated);
        }

        [Test]
        public void ItReturnsTheOwningUserOnTheGameDefinition()
        {
            GamingGroupSummary actualGamingGroup = AutoMocker.ClassUnderTest.GetGamingGroupDetails(filter);

            Assert.NotNull(actualGamingGroup.OwningUser);
        }

        [Test]
        public void ItReturnsAllActivePlayersInTheGamingGroup()
        {
            List<PlayerWithNemesis> expectedPlayers = new List<PlayerWithNemesis>();
            AutoMocker.Get<IPlayerRetriever>().Expect(mock => mock.GetAllPlayersWithNemesisInfo(gamingGroupId, filter.DateRangeFilter))
                .Return(expectedPlayers);

            GamingGroupSummary actualGamingGroup = AutoMocker.ClassUnderTest.GetGamingGroupDetails(filter);

            Assert.AreSame(expectedPlayers, actualGamingGroup.Players);
        }

        [Test]
        public void ItReturnsAllGameDefinitionsForTheGamingGroup()
        {
            GamingGroupSummary actualGamingGroup = AutoMocker.ClassUnderTest.GetGamingGroupDetails(filter);

            Assert.AreSame(gameDefinitionSummaries, actualGamingGroup.GameDefinitionSummaries);
        }

        [Test]
        public void ItReturnsTheSpecifiedNumberOfPlayedGamesForTheGamingGroup()
        {
            List<PlayedGame> playedGames = new List<PlayedGame>();
            AutoMocker.Get<IPlayedGameRetriever>().Expect(mock => mock.GetRecentGames(filter.NumberOfRecentGamesToShow, filter.GamingGroupId, filter.DateRangeFilter))
                .Return(playedGames);

            GamingGroupSummary actualGamingGroup = AutoMocker.ClassUnderTest.GetGamingGroupDetails(filter);

            Assert.AreSame(playedGames, actualGamingGroup.PlayedGames);
        }
    }
}
