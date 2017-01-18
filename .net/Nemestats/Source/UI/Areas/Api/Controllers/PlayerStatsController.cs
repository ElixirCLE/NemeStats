﻿using System.Net;
using System.Net.Http;
using System.Web.Http;
using BusinessLogic.Logic;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models.Players;
using UI.Areas.Api.Models;
using UI.Transformations;
using VersionedRestApi;

namespace UI.Areas.Api.Controllers
{
    public class PlayerStatsController : ApiControllerBase
    {
        private readonly IPlayerRetriever playerRetriever;
        private readonly ITransformer transformer;

        public PlayerStatsController(IPlayerRetriever playerRetriever, ITransformer transformer)
        {
            this.playerRetriever = playerRetriever;
            this.transformer = transformer;
        }

        [ApiRoute("GamingGroups/{gamingGroupId}/PlayerStats/{playerId}/")]
        [HttpGet]
        public virtual HttpResponseMessage GetPlayerStats([FromUri] int gamingGroupId, [FromUri] int playerId)
        {
            var playerStatistics = playerRetriever.GetPlayerStatistics(playerId);
            var playerStatisticsMessage = transformer.Transform<PlayerStatisticsMessage>(playerStatistics);

            return Request.CreateResponse(HttpStatusCode.OK, playerStatisticsMessage);
        }
    }
}