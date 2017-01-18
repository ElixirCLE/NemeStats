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
using System.Collections.Generic;
using System.Configuration.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using NUnit.Framework;
using Rhino.Mocks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayersTests.PlayerInviterTests
{
    [TestFixture]
    public class InvitePlayerTests
    {
        private PlayerInviter playerInviter;
        private IDataContext dataContextMock;
        private IIdentityMessageService emailServiceMock;
        private IConfigurationManager configurationManagerMock;
        private PlayerInvitation playerInvitation;
        private ApplicationUser currentUser;
        private Player player;
        private GamingGroup gamingGroup;
        private GamingGroupInvitation gamingGroupInvitation;
        private string rootUrl = "http://nemestats.com";
        private string existingUserId = "existing user id";

        [SetUp]
        public void SetUp()
        {
            dataContextMock = MockRepository.GenerateMock<IDataContext>();
            emailServiceMock = MockRepository.GenerateMock<IIdentityMessageService>();
            configurationManagerMock = MockRepository.GenerateMock<IConfigurationManager>();
            playerInvitation = new PlayerInvitation
            {
                CustomEmailMessage = "custom message",
                EmailSubject = "email subject",
                InvitedPlayerEmail = "player email",
                InvitedPlayerId = 1
            };
            currentUser = new ApplicationUser
            {
                CurrentGamingGroupId = 15,
                UserName = "Fergie Ferg"
            };
            player = new Player
            {
                Id = playerInvitation.InvitedPlayerId,
                GamingGroupId = 135
            };
            gamingGroup = new GamingGroup
            {
                Id = currentUser.CurrentGamingGroupId,
                Name = "jake's Gaming Group"
            };
            gamingGroupInvitation = new GamingGroupInvitation
            {
                Id = Guid.NewGuid()
            };

            dataContextMock.Expect(mock => mock.FindById<GamingGroup>(currentUser.CurrentGamingGroupId))
                           .Return(gamingGroup);

            List<ApplicationUser> applicationUsers = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Email = playerInvitation.InvitedPlayerEmail,
                    Id = existingUserId
                }
            };
            dataContextMock.Expect(mock => mock.GetQueryable<ApplicationUser>())
                           .Return(applicationUsers.AsQueryable());

            dataContextMock.Expect(mock => mock.Save<GamingGroupInvitation>(Arg<GamingGroupInvitation>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                           .Return(gamingGroupInvitation);

            configurationManagerMock.Expect(mock => mock.AppSettings[PlayerInviter.APP_SETTING_URL_ROOT])
                                    .Return(rootUrl);

            emailServiceMock.Expect(mock => mock.SendAsync(Arg<IdentityMessage>.Is.Anything))
                            .Return(Task.FromResult<object>(null));

            playerInviter = new PlayerInviter(dataContextMock, emailServiceMock, configurationManagerMock);
        }

        [Test]
        public void ItSavesAGamingGroupInvitation()
        {
            playerInviter.InvitePlayer(playerInvitation, currentUser);

            dataContextMock.AssertWasCalled(mock => mock.Save<GamingGroupInvitation>(Arg<GamingGroupInvitation>.Matches(
                invite => invite.PlayerId == playerInvitation.InvitedPlayerId
                && invite.DateSent.Date == DateTime.UtcNow.Date
                && invite.GamingGroupId == currentUser.CurrentGamingGroupId
                && invite.InviteeEmail == playerInvitation.InvitedPlayerEmail
                && invite.InvitingUserId == currentUser.Id),
                Arg<ApplicationUser>.Is.Same(currentUser)));
        }

        [Test]
        public void ItSetsTheRegisteredUserIdOnTheGamingGroupInvitationIfTheUserAlreadyHasAnExistingAccount()
        {
            playerInviter.InvitePlayer(playerInvitation, currentUser);

            dataContextMock.AssertWasCalled(mock => mock.Save<GamingGroupInvitation>(Arg<GamingGroupInvitation>.Matches(
                invite => invite.RegisteredUserId == existingUserId),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItEmailsTheUser()
        {
            string expectedBody = string.Format(PlayerInviter.EMAIL_MESSAGE_INVITE_PLAYER,
                                                currentUser.UserName,
                                                gamingGroup.Name,
                                                rootUrl,
                                                playerInvitation.CustomEmailMessage,
                                                gamingGroupInvitation.Id,
                                                "<br/><br/>");

            playerInviter.InvitePlayer(playerInvitation, currentUser);

            emailServiceMock.AssertWasCalled(mock => mock.SendAsync(Arg<IdentityMessage>.Matches(
                message => message.Subject == playerInvitation.EmailSubject
                && message.Body == expectedBody)));
        }
    }
}
