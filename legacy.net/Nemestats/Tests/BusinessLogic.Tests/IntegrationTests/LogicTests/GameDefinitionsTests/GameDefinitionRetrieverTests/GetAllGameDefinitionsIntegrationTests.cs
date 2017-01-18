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

using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Caching;
using BusinessLogic.Logic.BoardGameGeekGameDefinitions;
using BusinessLogic.Logic.Utilities;
using Is = NUnit.Framework.Is;

namespace BusinessLogic.Tests.IntegrationTests.LogicTests.GameDefinitionsTests.GameDefinitionRetrieverTests
{
    [TestFixture]
    public class GetAllGameDefinitionsIntegrationTests : IntegrationTestBase
    {
        protected GameDefinitionRetriever retriever;
        protected IList<GameDefinitionSummary> actualGameDefinitionSummaries;

        [SetUp]
        public void SetUp()
        {
            using(NemeStatsDataContext dataContext = new NemeStatsDataContext())
            {
                var playerRepository = new EntityFrameworkPlayerRepository(dataContext);
                var cacheableGameDataRetriever = new BoardGameGeekGameDefinitionInfoRetriever(new DateUtilities(), new CacheService(), dataContext);

                retriever = new GameDefinitionRetriever(dataContext, playerRepository, cacheableGameDataRetriever);
                actualGameDefinitionSummaries = retriever.GetAllGameDefinitions(testUserWithDefaultGamingGroup.CurrentGamingGroupId);
            }
        }

        [Test]
        public void ItOnlyGetsGameDefinitionsForTheCurrentPlayersGamingGroup()
        {
            Assert.True(actualGameDefinitionSummaries.All(game => game.GamingGroupId == testUserWithDefaultGamingGroup.CurrentGamingGroupId));
        }

        [Test]
        public void ItSortsGameDefinitionsByNameAscending()
        {
            string previousName = null;

            foreach (GameDefinition gameDefinition in actualGameDefinitionSummaries)
            {
                if (previousName != null)
                {
                    Assert.LessOrEqual(previousName, gameDefinition.Name);
                }

                previousName = gameDefinition.Name;
            }
        }

        [Test]
        public void ItGetsBackChampionAndPlayerInformation()
        {
            Assert.That(actualGameDefinitionSummaries.All(game => game.Champion != null), Is.True);
            Assert.That(actualGameDefinitionSummaries.All(game => game.Champion.Player != null), Is.True);
        }

        [Test]
        public void ItGetsBackPreviousChampionAndPlayerInformation()
        {
            Assert.That(actualGameDefinitionSummaries.All(game => game.PreviousChampion != null), Is.True);
            Assert.That(actualGameDefinitionSummaries.All(game => game.PreviousChampion.Player != null), Is.True);
        }

        [Test]
        public void ItGetsTheBoardGameGeekInfo()
        {
            Assert.That(actualGameDefinitionSummaries.First(x => x.Id == testGameDefinition.Id).BoardGameGeekInfo, Is.Not.Null);
        }

        [Test, Ignore("Miscellaneous integration test that might be occassionally useful but not worth slowing things down.")]
        public void TryGamingGroup1()
        {
            using (NemeStatsDataContext dataContext = new NemeStatsDataContext())
            {
                var playerRepository = new EntityFrameworkPlayerRepository(dataContext);
                var cacheableGameDataRetriever = new BoardGameGeekGameDefinitionInfoRetriever(new DateUtilities(), new CacheService(), dataContext);

                retriever = new GameDefinitionRetriever(dataContext, playerRepository, cacheableGameDataRetriever);
                IList<GameDefinitionSummary> gameDefinitionSummaries = retriever.GetAllGameDefinitions(1);

                foreach (GameDefinitionSummary summary in gameDefinitionSummaries)
                {
                    if (summary.ChampionId != null)
                    {
                        Assert.That(summary.Champion, Is.Not.Null);
                        Assert.That(summary.Champion.Player, Is.Not.Null);
                    }
                }
            }
        }
    }
}
