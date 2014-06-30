﻿using BusinessLogic.Models.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models
{
    public class GamingGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string OwningUserId { get; set; }

        [ForeignKey("OwningUserId")]
        public virtual ApplicationUser OwningUser { get; set; }
        public virtual IList<Player> Players { get; set; }
        public virtual IList<GameDefinition> GameDefinitions { get; set; }
        public virtual IList<PlayedGame> PlayedGames { get; set; }
    }
}