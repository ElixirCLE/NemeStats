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

#endregion LICENSE

using BusinessLogic.Models.Games;
using BusinessLogic.Models.Games.Validation;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace BusinessLogic.Tests.UnitTests.ModelsTests.GamesTests.ValidationTests
{
    [TestFixture]
    public class ValidatePlayerRanksTests
    {
        [Test]
        public void ItRequiresPlayerRanks()
        {
            List<IPlayerRank> playerRanks = null;

            var exception = Assert.Throws<ArgumentException>(() => PlayerRankValidator.ValidatePlayerRanks(playerRanks));

            Assert.That(exception.Message, Is.EqualTo(PlayerRankValidator.EXCEPTION_MESSAGE_MUST_PASS_AT_LEAST_TWO_PLAYERS));
        }

        [Test]
        public void ItCannotHaveMoreThan25Players()
        {
            var playerRanks = new List<IPlayerRank>();
            for (int i = 0; i < 26; i++)
            {
                playerRanks.Add(new PlayerRank
                {
                    GameRank = i + 1,
                    PlayerId = i + 1
                });
            }

            var exception = Assert.Throws<ArgumentException>(() => PlayerRankValidator.ValidatePlayerRanks(playerRanks));

            Assert.That(exception.Message, Is.EqualTo(PlayerRankValidator.EXCEPTION_MESSAGE_CANNOT_HAVE_MORE_THAN_25_PLAYERS));
        }

        [Test]
        public void ItRequiresAtLeastOnePlayer()
        {
            var playerRanks = new List<IPlayerRank>()
            {
                new PlayerRank { PlayerId = 1, GameRank = 1 }
            };

            var exception = Assert.Throws<ArgumentException>(() => PlayerRankValidator.ValidatePlayerRanks(playerRanks));

            Assert.That(exception.Message, Is.EqualTo(PlayerRankValidator.EXCEPTION_MESSAGE_MUST_PASS_AT_LEAST_TWO_PLAYERS));
        }

        [Test]
        public void ItRequiresEachPlayerRankToHaveAGameRank()
        {
            var playerRanks = new List<IPlayerRank>()
                                            {
                                                new PlayerRank() { PlayerId = 1, GameRank = 1 },
                                                new PlayerRank() { PlayerId = 2 }
                                            };

            var exception = Assert.Throws<ArgumentException>(() => PlayerRankValidator.ValidatePlayerRanks(playerRanks));

            Assert.That(exception.Message, Is.EqualTo(PlayerRankValidator.EXCEPTION_MESSAGE_EACH_PLAYER_RANK_MUST_HAVE_A_GAME_RANK));
        }

        [Test]
        public void NoPlayerMayHaveARankGreaterThanTheTotalNumberOfPlayers()
        {
            var playerRanks = new List<IPlayerRank>();
            playerRanks.Add(new PlayerRank() { PlayerId = 1, GameRank = 1 });
            playerRanks.Add(new PlayerRank() { PlayerId = 2, GameRank = 3 });

            var exception = Assert.Throws<ArgumentException>(() => PlayerRankValidator.ValidatePlayerRanks(playerRanks));

            Assert.That(exception.Message, Is.EqualTo(PlayerRankValidator.EXCEPTION_MESSAGE_NO_PLAYER_CAN_HAVE_A_HIGHER_RANK_THAN_THE_NUMBER_OF_PLAYERS));
        }

        [Test]
        public void NoPlayersMayHaveARankLessThanOne()
        {
            var playerRanks = new List<IPlayerRank>();
            playerRanks.Add(new PlayerRank() { PlayerId = 1, GameRank = 1 });
            playerRanks.Add(new PlayerRank() { PlayerId = 2, GameRank = -1 });

            var exception = Assert.Throws<ArgumentException>(() => PlayerRankValidator.ValidatePlayerRanks(playerRanks));

            Assert.That(exception.Message, Is.EqualTo(PlayerRankValidator.EXCEPTION_MESSAGE_NO_PLAYER_CAN_HAVE_A_RANK_LESS_THAN_ONE));
        }

        [Test]
        public void ItAcceptsAGameWithRanksOneTwoAndThreeRanks()
        {
            var playerRanks = new List<IPlayerRank>();
            playerRanks.Add(new PlayerRank() { PlayerId = 1, GameRank = 1 });
            playerRanks.Add(new PlayerRank() { PlayerId = 2, GameRank = 1 });
            playerRanks.Add(new PlayerRank() { PlayerId = 3, GameRank = 2 });
            playerRanks.Add(new PlayerRank() { PlayerId = 4, GameRank = 3 });

            PlayerRankValidator.ValidatePlayerRanks(playerRanks);
        }
    }
}