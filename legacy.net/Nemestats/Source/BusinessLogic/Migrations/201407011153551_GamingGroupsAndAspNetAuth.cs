#region LICENSE
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
namespace BusinessLogic.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class GamingGroupsAndAspNetAuth : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");

            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);

            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");

            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.GamingGroup",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String()
                })
                .PrimaryKey(t => t.Id);

            //create a new GamingGroup only if there are existing users
            Sql("INSERT INTO GamingGroup (Name) SELECT 'Initial Gaming Group' FROM AspNetUsers WHERE EXISTS(SELECT TOP 1 1 FROM AspNetUsers)");

            AddColumn("dbo.Player", "GamingGroupId", c => c.Int(nullable: true));
            AddForeignKey("dbo.Player", "GamingGroupId", "dbo.GamingGroup", "Id");
            Sql("UPDATE dbo.Player SET GamingGroupID = (SELECT TOP 1 Id FROM GamingGroup)");
            AlterColumn("dbo.Player", "GamingGroupId", c => c.Int(nullable: false));

            AddColumn("dbo.GameDefinition", "GamingGroupId", c => c.Int(nullable: true));
            AddForeignKey("dbo.GameDefinition", "GamingGroupId", "dbo.GamingGroup", "Id");
            Sql("UPDATE dbo.GameDefinition SET GamingGroupID = (SELECT TOP 1 Id FROM GamingGroup)");
            AlterColumn("dbo.GameDefinition", "GamingGroupId", c => c.Int(nullable: false));

            AddColumn("dbo.PlayedGame", "GamingGroupId", c => c.Int(nullable: true));
            AddForeignKey("dbo.PlayedGame", "GamingGroupId", "dbo.GamingGroup", "Id");
            Sql("UPDATE dbo.PlayedGame SET GamingGroupID = (SELECT TOP 1 Id FROM GamingGroup)");
            AlterColumn("dbo.PlayedGame", "GamingGroupId", c => c.Int(nullable: false));
        }

        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Player", "GamingGroupId", "dbo.GamingGroup");
            DropForeignKey("dbo.GameDefinition", "GamingGroupId", "dbo.GamingGroup");
            DropForeignKey("dbo.PlayedGame", "GamingGroupId", "dbo.GamingGroup");

            DropColumn("dbo.Player", "GamingGroupId");
            DropColumn("dbo.GameDefinition", "GamingGroupId");
            DropColumn("dbo.PlayedGame", "GamingGroupId");

            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.GamingGroup");
        }
    }
}
