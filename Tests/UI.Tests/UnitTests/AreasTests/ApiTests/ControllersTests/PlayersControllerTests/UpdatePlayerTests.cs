﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Areas.Api.Controllers;
using UI.Areas.Api.Models;

namespace UI.Tests.UnitTests.AreasTests.ApiTests.ControllersTests.PlayersControllerTests
{
    [TestFixture]
    public class UpdatePlayerTests : ApiControllerTestBase<PlayersController>
    {
        [Test]
        public void ItSavesThePlayer()
        {
            const int PLAYER_ID = 1;

            var updatePlayerMessage = new UpdatePlayerMessage
            {
                PlayerName = "some player name",
                Active = false
            };

            autoMocker.ClassUnderTest.UpdatePlayer(updatePlayerMessage, PLAYER_ID, 100);

            autoMocker.Get<IPlayerSaver>().AssertWasCalled(mock => mock.UpdatePlayer(
                Arg<UpdatePlayerRequest>.Matches(player => player.Active == updatePlayerMessage.Active
                                    && player.Name == updatePlayerMessage.PlayerName
                                    && player.PlayerId == PLAYER_ID),
                Arg<ApplicationUser>.Is.Same(applicationUser)));
        }

        [Test]
        public void ItReturnsABadRequestIfTheMessageIsNull()
        {
            HttpResponseMessage actualResponse = autoMocker.ClassUnderTest.UpdatePlayer(null, 1, 100);

            AssertThatApiAction.HasThisError(actualResponse, HttpStatusCode.BadRequest, "You must pass at least one valid parameter.");
        }

        [Test]
        public void ItReturnsANoContentResponse()
        {
            var actualResults = autoMocker.ClassUnderTest.UpdatePlayer(new UpdatePlayerMessage(), 0, 0);

            AssertThatApiAction.ReturnsANoContentResponseWithNoContent(actualResults);
        }
    }
}