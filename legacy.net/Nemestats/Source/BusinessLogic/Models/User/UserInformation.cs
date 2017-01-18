﻿using System.Collections.Generic;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Models.Players;

namespace BusinessLogic.Models.User
{
    public class UserInformation
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public IList<GamingGroupInfoForUser> GamingGroups { get; set; }
        public IList<PlayerInfoForUser> Players { get; set; }
        public BoardGameGeekUserInformation BoardGameGeekUser { get; set; }
    }
}
