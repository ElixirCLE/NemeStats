﻿using BusinessLogic.Models;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace BusinessLogic.DataAccess
{
    public class NemeStatsDbContext : IdentityDbContext<ApplicationUser>
    {
        public NemeStatsDbContext()
            : base("DefaultConnection")
        {
            this.Configuration.ProxyCreationEnabled = true;
            this.Configuration.LazyLoadingEnabled = true;
            //uncomment to turn on SQL statements printing to the console
            //this.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        }

        public virtual DbSet<GamingGroup> GamingGroups { get; set; }
        public virtual DbSet<Player> Players { get; set; }
        public virtual DbSet<GameDefinition> GameDefinitions { get; set; }
        public virtual DbSet<PlayedGame> PlayedGames { get; set; }
        public virtual DbSet<PlayerGameResult> PlayerGameResults { get; set; }
        public virtual DbSet<UserGamingGroup> UserGamingGroups { get; set; }
        public virtual DbSet<GamingGroupInvitation> GamingGroupInvitations { get; set; }
        public virtual DbSet<Nemesis> Nemeses { get; set; }
        public virtual DbSet<Champion> Champions { get; set; }
        public virtual DbSet<VotableFeature> VotableFeatures { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
