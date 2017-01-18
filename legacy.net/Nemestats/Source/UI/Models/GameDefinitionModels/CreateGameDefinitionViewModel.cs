﻿using System.ComponentModel.DataAnnotations;

namespace UI.Models.GameDefinitionModels
{
    public class CreateGameDefinitionViewModel
    {
        public CreateGameDefinitionViewModel()
        {
            Active = true;
        }

        public bool Active { get; set; }
        public int? BoardGameGeekGameDefinitionId { get; set; }
        public string Description { get; set; }
        [Required]
        public string Name { get; set; }
        public string ReturnUrl { get; set; }
        public string BGGUserName { get; set; }
        public int GamingGroupId { get; set; }
    }
}