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

using System.Linq;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Models.Players;
using BusinessLogic.Logic.Players;
using System.Web.Mvc;

namespace UI.Tests.UnitTests.ControllerTests.PlayerControllerTests
{
    [TestFixture]
    public class InvitePlayerHttpPostTests : PlayerControllerTestBase
    {
        [Test]
        public void ItSendsThePlayerInvitation()
        {
            PlayerInvitationViewModel playerInvitationViewModel = new PlayerInvitationViewModel
            {
                EmailAddress = "email address",
                EmailBody = "email body",
                EmailSubject = "email subject",
                PlayerId = 1
            };
            ApplicationUser applicationUser = new ApplicationUser();

            autoMocker.ClassUnderTest.Url.Expect(mock => mock.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name))
                .Return("some url");

            autoMocker.ClassUnderTest.InvitePlayer(playerInvitationViewModel, applicationUser);

            autoMocker.Get<IPlayerInviter>().AssertWasCalled(mock => mock.InvitePlayer(Arg<PlayerInvitation>.Matches(
                invite => invite.CustomEmailMessage == playerInvitationViewModel.EmailBody
                && invite.EmailSubject == playerInvitationViewModel.EmailSubject
                && invite.InvitedPlayerEmail == playerInvitationViewModel.EmailAddress
                && invite.InvitedPlayerId == playerInvitationViewModel.PlayerId),
                Arg<ApplicationUser>.Is.Same(applicationUser)));
        }
    }
}
