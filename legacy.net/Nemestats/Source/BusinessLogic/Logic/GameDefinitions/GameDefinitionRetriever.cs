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

using System;
using System.Data.Entity;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Models;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.BoardGameGeekGameDefinitions;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.Utility;
using BusinessLogic.Paging;
using PagedList;

namespace BusinessLogic.Logic.GameDefinitions
{
    public class GameDefinitionRetriever : IGameDefinitionRetriever
    {
        private readonly IDataContext _dataContext;
        private readonly IPlayerRepository _playerRepository;
        private readonly IBoardGameGeekGameDefinitionInfoRetriever _boardGameGeekGameDefinitionInfoRetriever;

        public GameDefinitionRetriever(IDataContext dataContext, IPlayerRepository playerRepository, IBoardGameGeekGameDefinitionInfoRetriever boardGameGeekGameDefinitionInfoRetriever)
        {
            _dataContext = dataContext;
            _playerRepository = playerRepository;
            _boardGameGeekGameDefinitionInfoRetriever = boardGameGeekGameDefinitionInfoRetriever;
        }

        public virtual List<GameDefinitionSummary> GetGameDefinitionSummaries(List<int> gameDefinitionIds)
        {
            var dateRangeFilter = new BasicDateRangeFilter();

            var gameDefinitionSummaries = _dataContext.GetQueryable<GameDefinition>()
                .Where(gameDefinition => gameDefinitionIds.Contains(gameDefinition.Id))
                .Select(gameDefinition => new GameDefinitionSummary
                {
                    Active = gameDefinition.Active,
                    BoardGameGeekGameDefinitionId = gameDefinition.BoardGameGeekGameDefinitionId,
                    Name = gameDefinition.Name,
                    Description = gameDefinition.Description,
                    GamingGroupId = gameDefinition.GamingGroupId,
                    Id = gameDefinition.Id,
                    PlayedGames = gameDefinition.PlayedGames.Where(
                        playedGame => playedGame.DatePlayed >= ((IDateRangeFilter)dateRangeFilter).FromDate
                                      && playedGame.DatePlayed <= ((IDateRangeFilter)dateRangeFilter).ToDate)
                        .ToList(),
                    Champion = gameDefinition.Champion,
                    ChampionId = gameDefinition.ChampionId,
                    PreviousChampion = gameDefinition.PreviousChampion,
                    PreviousChampionId = gameDefinition.PreviousChampionId,
                    DateCreated = gameDefinition.DateCreated,
                    BoardGameGeekGameDefinition = gameDefinition.BoardGameGeekGameDefinition
                })
                .ToList();

            gameDefinitionSummaries.ForEach(x => x.BoardGameGeekInfo = x.BoardGameGeekGameDefinitionId.HasValue ?
                        _boardGameGeekGameDefinitionInfoRetriever.GetResults(x.BoardGameGeekGameDefinitionId.Value)
                        : null);

            return gameDefinitionSummaries;
        }

        public IPagedList<GameDefinitionDisplayInfo> GetMostPlayedGames(GetMostPlayedGamesQuery query)
        {
            var mostPlayedGames = GetGameDefinitionDisplayInfoQuery(query.GamingGroupId)
                .OrderByDescending(g => g.PlayedTimes);

            return mostPlayedGames.ToPagedList(query.Page, query.PageSize);

        }

        public IPagedList<GameDefinitionDisplayInfo> GetRecentGames(GetRecentPlayedGamesQuery query)
        {
            var recentGames = GetGameDefinitionDisplayInfoQuery(query.GamingGroupId)
               .OrderByDescending(g => g.LastDatePlayed);

            return recentGames.ToPagedList(query.Page, query.PageSize);
        }

        public GameDefinitionDisplayInfo GetGameDefinitionDisplayInfo(int id)
        {
            return _dataContext.GetQueryable<GameDefinition>()
                .Include(g => g.BoardGameGeekGameDefinition)
                .Where(g => g.Id == id)
                .Select(MapToGameDefinitionDisplayInfo()).FirstOrDefault();
        }

        private static Expression<Func<GameDefinition, GameDefinitionDisplayInfo>> MapToGameDefinitionDisplayInfo()
        {
            return g => new GameDefinitionDisplayInfo
            {
                Id = g.Id,
                Name = g.Name,
                PlayedTimes = g.PlayedGames.Count,
                LastDatePlayed = g.PlayedGames.Select(p => p.DatePlayed).OrderByDescending(d => d).FirstOrDefault(),
                ThumbnailImageUrl = g.BoardGameGeekGameDefinition != null ? g.BoardGameGeekGameDefinition.Thumbnail : "",
                ImageUrl = g.BoardGameGeekGameDefinition != null ? g.BoardGameGeekGameDefinition.Image : "",
                BoardGameGeekGameDefinitionId = g.BoardGameGeekGameDefinitionId
            };
        }

        private IQueryable<GameDefinitionDisplayInfo> GetGameDefinitionDisplayInfoQuery(int gamingGroupÌd)
        {
            return _dataContext.GetQueryable<GameDefinition>()
                .Include(g => g.BoardGameGeekGameDefinition)
                .Where(g => g.GamingGroupId == gamingGroupÌd)
                .Select(MapToGameDefinitionDisplayInfo());
        }

        public virtual IList<GameDefinitionSummary> GetAllGameDefinitions(int gamingGroupId, IDateRangeFilter dateRangeFilter = null)
        {
            if (dateRangeFilter == null)
            {
                dateRangeFilter = new BasicDateRangeFilter();
            }

            var returnValue = _dataContext.GetQueryable<GameDefinition>()
                .Where(gameDefinition => gameDefinition.GamingGroupId == gamingGroupId && gameDefinition.Active)
                .Select(gameDefinition => new GameDefinitionSummary
                {
                    Active = gameDefinition.Active,
                    BoardGameGeekGameDefinitionId = gameDefinition.BoardGameGeekGameDefinitionId,
                    Name = gameDefinition.Name,
                    Description = gameDefinition.Description,
                    GamingGroupId = gameDefinition.GamingGroupId,
                    Id = gameDefinition.Id,
                    PlayedGames = gameDefinition.PlayedGames.Where(
                        playedGame => playedGame.DatePlayed >= dateRangeFilter.FromDate
                                      && playedGame.DatePlayed <= dateRangeFilter.ToDate)
                        .ToList(),
                    Champion = gameDefinition.Champion,
                    ChampionId = gameDefinition.ChampionId,
                    PreviousChampion = gameDefinition.PreviousChampion,
                    PreviousChampionId = gameDefinition.PreviousChampionId,
                    DateCreated = gameDefinition.DateCreated
                })
                  .OrderBy(game => game.Name)
                .ToList();

            AddPlayersToChampionData(returnValue);

            returnValue.ForEach(summary =>
            {
                summary.Champion = summary.Champion ?? new NullChampion();
                summary.PreviousChampion = summary.PreviousChampion ?? new NullChampion();
                summary.TotalNumberOfGamesPlayed = summary.PlayedGames.Count;
                if (summary.BoardGameGeekGameDefinitionId.HasValue)
                {
                    summary.BoardGameGeekInfo = _boardGameGeekGameDefinitionInfoRetriever.GetResults(summary.BoardGameGeekGameDefinitionId.Value);
                }
            });
            return returnValue;
        }

        private void AddPlayersToChampionData(List<GameDefinitionSummary> gameDefinitionSummaries)
        {
            var playerIds = gameDefinitionSummaries.Select(x => x.Champion == null ? -1 : x.Champion.PlayerId).ToList()
                                                   .Union(gameDefinitionSummaries.Select(x => x.PreviousChampion?.PlayerId ?? -1).ToList());

            var players = _dataContext.GetQueryable<Player>().Where(player => playerIds.Contains(player.Id)).ToList();

            foreach (var gameDefinitionSummary in gameDefinitionSummaries)
            {
                if (gameDefinitionSummary.Champion != null)
                {
                    gameDefinitionSummary.Champion.Player = players.FirstOrDefault(player => player.Id == gameDefinitionSummary.Champion.PlayerId);
                }

                if (gameDefinitionSummary.PreviousChampion != null)
                {
                    gameDefinitionSummary.PreviousChampion.Player = players.FirstOrDefault(player => player.Id == gameDefinitionSummary.PreviousChampion.PlayerId);
                }
            }
        }

        public virtual GameDefinitionSummary GetGameDefinitionDetails(int id, int numberOfPlayedGamesToRetrieve)
        {
            var gameDefinition = _dataContext.GetQueryable<GameDefinition>()
                .Include(game => game.PlayedGames)
                .Include(game => game.Champion)
                .Include(game => game.Champion.Player)
                .Include(game => game.PreviousChampion)
                .Include(game => game.PreviousChampion.Player)
                .Include(game => game.GamingGroup)
                .SingleOrDefault(game => game.Id == id);

            if (gameDefinition == null)
            {
                throw new EntityDoesNotExistException(typeof(GameDefinition), id);
            }

            BoardGameGeekInfo boardGameGeekInfo = null;
            if (gameDefinition.BoardGameGeekGameDefinitionId.HasValue)
            {
                boardGameGeekInfo = _boardGameGeekGameDefinitionInfoRetriever.GetResults(gameDefinition.BoardGameGeekGameDefinitionId.Value);
            }

            var gameDefinitionSummary = new GameDefinitionSummary
            {
                Active = gameDefinition.Active,
                BoardGameGeekGameDefinitionId = gameDefinition.BoardGameGeekGameDefinitionId,
                Name = gameDefinition.Name,
                Description = gameDefinition.Description,
                GamingGroup = gameDefinition.GamingGroup,
                GamingGroupId = gameDefinition.GamingGroupId,
                GamingGroupName = gameDefinition.GamingGroup.Name,
                Id = gameDefinition.Id,
                TotalNumberOfGamesPlayed = gameDefinition.PlayedGames.Count,
                AveragePlayersPerGame = gameDefinition.PlayedGames.Select(item => (decimal)item.NumberOfPlayers).DefaultIfEmpty(0M).Average(),
                Champion = gameDefinition.Champion ?? new NullChampion(),
                PreviousChampion = gameDefinition.PreviousChampion ?? new NullChampion(),
                BoardGameGeekInfo = boardGameGeekInfo
            };

            var playedGames = AddPlayedGamesToTheGameDefinition(numberOfPlayedGamesToRetrieve, gameDefinitionSummary);
            var distinctPlayerIds = AddPlayerGameResultsToEachPlayedGame(playedGames);
            AddPlayersToPlayerGameResults(playedGames, distinctPlayerIds);
            gameDefinitionSummary.PlayerWinRecords = _playerRepository.GetPlayerWinRecords(id);

            return gameDefinitionSummary;
        }

        private IList<PlayedGame> AddPlayedGamesToTheGameDefinition(
            int numberOfPlayedGamesToRetrieve,
            GameDefinitionSummary gameDefinitionSummary)
        {
            IList<PlayedGame> playedGames = _dataContext.GetQueryable<PlayedGame>().Include(playedGame => playedGame.PlayerGameResults)
                .Where(playedGame => playedGame.GameDefinitionId == gameDefinitionSummary.Id)
                .OrderByDescending(playedGame => playedGame.DatePlayed)
                .Take(numberOfPlayedGamesToRetrieve)
                .ToList();

            foreach (var playedGame in playedGames)
            {
                playedGame.GameDefinition = gameDefinitionSummary;
            }

            gameDefinitionSummary.PlayedGames = playedGames;

            return playedGames;
        }

        private IList<int> AddPlayerGameResultsToEachPlayedGame(IList<PlayedGame> playedGames)
        {
            var playedGameIds = (from playedGame in playedGames
                                 select playedGame.Id).ToList();

            IList<PlayerGameResult> playerGameResults = _dataContext.GetQueryable<PlayerGameResult>()
                .Where(playerGameResult => playedGameIds.Contains(playerGameResult.PlayedGameId))
                .OrderBy(playerGameResult => playerGameResult.GameRank)
                .ToList();

            var distinctPlayerIds = new HashSet<int>();

            foreach (var playedGame in playedGames)
            {
                playedGame.PlayerGameResults = (from playerGameResult in playerGameResults
                                                where playerGameResult.PlayedGameId == playedGame.Id
                                                select playerGameResult).ToList();

                ExtractDistinctListOfPlayerIds(distinctPlayerIds, playedGame);
            }

            return distinctPlayerIds.ToList();
        }

        private static void ExtractDistinctListOfPlayerIds(HashSet<int> distinctPlayerIds, PlayedGame playedGame)
        {
            foreach (var playerGameResult in playedGame.PlayerGameResults)
            {
                if (!distinctPlayerIds.Contains(playerGameResult.PlayerId))
                {
                    distinctPlayerIds.Add(playerGameResult.PlayerId);
                }
            }
        }

        private void AddPlayersToPlayerGameResults(IList<PlayedGame> playedGames, IList<int> distinctPlayerIds)
        {
            IList<Player> players = _dataContext.GetQueryable<Player>()
                .Where(player => distinctPlayerIds.Contains(player.Id))
                .ToList();

            foreach (var playedGame in playedGames)
            {
                foreach (var playerGameResult in playedGame.PlayerGameResults)
                {
                    playerGameResult.Player = players.First(player => player.Id == playerGameResult.PlayerId);
                }
            }
        }


        public IList<GameDefinitionName> GetAllGameDefinitionNames(int gamingGroupId, string nameQuery = null)
        {
            var gameDefinitionsQuery = _dataContext.GetQueryable<GameDefinition>()
                .Where(gameDefinition => gameDefinition.Active
                                         && gameDefinition.GamingGroupId == gamingGroupId);

            if (!string.IsNullOrEmpty(nameQuery))
            {
                gameDefinitionsQuery = gameDefinitionsQuery.Where(g => g.Name.Contains(nameQuery));
            }
            return gameDefinitionsQuery
                              .Select(gameDefiniton => new GameDefinitionName
                              {
                                  BoardGameGeekGameDefinitionId = gameDefiniton.BoardGameGeekGameDefinitionId,
                                  Id = gameDefiniton.Id,
                                  Name = gameDefiniton.Name
                              }).ToList();
        }

        public List<TrendingGame> GetTrendingGames(int maxNumberOfGames, int numberOfDaysOfTrendingGames)
        {
            var startDate = DateTime.Now.Date.AddDays(-1 * numberOfDaysOfTrendingGames);
            return (from result in _dataContext.GetQueryable<BoardGameGeekGameDefinition>()
                    select new TrendingGame
                    {
                        BoardGameGeekGameDefinitionId = result.Id,
                        Name = result.Name,
                        GamesPlayed = result.GameDefinitions.SelectMany(x => x.PlayedGames.Where(playedGame => playedGame.DatePlayed >= startDate)).Count(),
                        ThumbnailImageUrl = result.Thumbnail,
                        GamingGroupsPlayingThisGame = result.GameDefinitions.Count(
                            gameDefinition => gameDefinition.PlayedGames.Any(playedGame => playedGame.DatePlayed >= startDate))
                    })
                    .OrderByDescending(x => x.GamingGroupsPlayingThisGame)
                    .ThenByDescending(x => x.GamesPlayed)
                    .Take(maxNumberOfGames)
                    .ToList();
        }

    }
}
