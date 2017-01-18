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
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Web.Mvc;
using UI.Controllers;
using UI.Models.GameDefinitionModels;

namespace UI.Tests.UnitTests.ControllerTests.GameDefinitionControllerTests
{
    [TestFixture]
    public class EditHttpPostTests : GameDefinitionControllerTestBase
    {
        [Test]
        public void ItStaysOnTheEditPageIfValidationFails()
        {
            var viewModel = new GameDefinitionEditViewModel();

            autoMocker.ClassUnderTest.ModelState.AddModelError("key", "message");

            var viewResult = autoMocker.ClassUnderTest.Edit(viewModel, currentUser) as ViewResult;

            Assert.AreEqual(MVC.GameDefinition.Views.Edit, viewResult.ViewName);
        }

        [Test]
        public void ItReloadsTheGameDefinitionIfValidationFails()
        {
            var viewModel = new GameDefinitionEditViewModel();
            autoMocker.ClassUnderTest.ModelState.AddModelError("key", "message");

            var viewResult = autoMocker.ClassUnderTest.Edit(viewModel, currentUser) as ViewResult;

            Assert.AreSame(viewModel, viewResult.Model);
        }

        [Test]
        public void ItSavesTheGameDefinitionIfValidationPasses()
        {
            var viewModel = new GameDefinitionEditViewModel
            {
                Name = "some name"
            };

            autoMocker.ClassUnderTest.Edit(viewModel, currentUser);

            var arguments = autoMocker.Get<IGameDefinitionSaver>().GetArgumentsForCallsMadeOn(mock => mock.UpdateGameDefinition(
                Arg<GameDefinitionUpdateRequest>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));
            var gameDefinitionUpdateRequest = arguments[0][0] as GameDefinitionUpdateRequest;
            Assert.That(gameDefinitionUpdateRequest, Is.Not.Null);
            Assert.That(gameDefinitionUpdateRequest.Active, Is.EqualTo(viewModel.Active));
            Assert.That(gameDefinitionUpdateRequest.BoardGameGeekGameDefinitionId, Is.EqualTo(viewModel.BoardGameGeekGameDefinitionId));
            Assert.That(gameDefinitionUpdateRequest.Description, Is.EqualTo(viewModel.Description));
            Assert.That(gameDefinitionUpdateRequest.GameDefinitionId, Is.EqualTo(viewModel.GameDefinitionId));
            Assert.That(gameDefinitionUpdateRequest.Name, Is.EqualTo(viewModel.Name));
        }

        [Test]
        public void ItRedirectsToTheGamingGroupIndexAndGameDefinitionsSectionAfterSaving()
        {
            var viewModel = new GameDefinitionEditViewModel
            {
                Name = "some name"
            };
            var baseUrl = "base url";
            var expectedUrl = baseUrl + "#" + GamingGroupController.SECTION_ANCHOR_GAMEDEFINITIONS;
            urlHelperMock.BackToRecord(BackToRecordOptions.All);
            urlHelperMock.Replay();
            urlHelperMock.Expect(mock => mock.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name))
                    .Return(baseUrl);

            var redirectResult = autoMocker.ClassUnderTest.Edit(viewModel, currentUser) as RedirectResult;

            Assert.AreEqual(expectedUrl, redirectResult.Url);
        }
    }
}
