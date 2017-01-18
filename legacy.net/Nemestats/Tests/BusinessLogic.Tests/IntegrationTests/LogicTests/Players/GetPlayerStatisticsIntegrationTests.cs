﻿using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic.Players;
using NUnit.Framework;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models.Points;

namespace BusinessLogic.Tests.IntegrationTests.LogicTests.Players
{
    [TestFixture]
    public class GetPlayerStatisticsIntegrationTests : IntegrationTestBase
    {
        [Test]
        public void ItReturnsZeroesWhenThereAreNoPlayedGames()
        {
            using (IDataContext dataContext = new NemeStatsDataContext())
            {
                IPlayedGameRetriever playedGameRetriever = new PlayedGameRetriever(dataContext);
                var playerRetriever = new PlayerRetriever(dataContext, new EntityFrameworkPlayerRepository(dataContext), playedGameRetriever);

                var statistics = playerRetriever.GetPlayerStatistics(testPlayerWithNoPlayedGames.Id);

                Assert.That(statistics.TotalGames, Is.EqualTo(0));
                Assert.That(statistics.TotalGamesLost, Is.EqualTo(0));
                Assert.That(statistics.TotalGamesWon, Is.EqualTo(0));
                Assert.That(statistics.NemePointsSummary, Is.EqualTo(new NemePointsSummary(0, 0, 0)));
                Assert.That(statistics.WinPercentage, Is.EqualTo(0));
                Assert.That(statistics.AveragePlayersPerGame, Is.EqualTo(0));
                Assert.That(statistics.GameDefinitionTotals.SummariesOfGameDefinitionTotals.Count, Is.EqualTo(0));
            } 
        }

        [Test]
        public void ItReturnsTheCorrectStatisticsForAnUndefeatedPlayer()
        {
            using (IDataContext dataContext = new NemeStatsDataContext())
            {
                IPlayedGameRetriever playedGameRetriever = new PlayedGameRetriever(dataContext);
                var playerRetriever = new PlayerRetriever(dataContext, new EntityFrameworkPlayerRepository(dataContext), playedGameRetriever);

                var statistics = playerRetriever.GetPlayerStatistics(testPlayer9UndefeatedWith5Games.Id);

                Assert.That(statistics.TotalGames, Is.EqualTo(5));
                Assert.That(statistics.TotalGamesLost, Is.EqualTo(0));
                Assert.That(statistics.TotalGamesWon, Is.EqualTo(5));
                Assert.That(statistics.WinPercentage, Is.EqualTo(100));

                var summariesOfGameDefinitionTotals = statistics.GameDefinitionTotals.SummariesOfGameDefinitionTotals;
                Assert.That(summariesOfGameDefinitionTotals.Count, Is.EqualTo(1));
                var gameDefinitionTotal = summariesOfGameDefinitionTotals[0];
                Assert.That(gameDefinitionTotal.GameDefinitionId, Is.EqualTo(anotherTestGameDefinitionWithOtherGamingGroupId.Id));

            }  
        }

    }
}
