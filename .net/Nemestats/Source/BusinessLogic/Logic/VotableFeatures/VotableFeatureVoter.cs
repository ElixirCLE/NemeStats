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
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.VotableFeatures
{
    public class VotableFeatureVoter : IVotableFeatureVoter
    {
        private readonly IDataContext dataContext;

        public VotableFeatureVoter(IDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public VotableFeature CastVote(string votableFeatureId, bool voteUp)
        {
            var votableFeature = dataContext.FindById<VotableFeature>(votableFeatureId);

            if (voteUp)
            {
                votableFeature.NumberOfUpvotes += 1;
            }
            else
            {
                votableFeature.NumberOfDownvotes += 1;
            }

            votableFeature.DateModified = DateTime.UtcNow;

            return dataContext.Save(votableFeature, new ApplicationUser());
        }
    }
}
