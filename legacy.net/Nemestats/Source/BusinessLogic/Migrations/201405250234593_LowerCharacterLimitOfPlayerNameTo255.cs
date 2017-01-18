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
    
    public partial class LowerCharacterLimitOfPlayerNameTo255 : DbMigration
    {
        public override void Up()
        {
            DropIndex(table: "Player",
                name: "UniqueNameIndex");

            AlterColumn("dbo.Player", "Name", c => c.String(maxLength: 255));

            CreateIndex(table: "Player",
                column: "Name",
                unique: true,
                name: "UniqueNameIndex");
        }
        
        public override void Down()
        {
            DropIndex(table: "Player",
                name: "UniqueNameIndex");

            AlterColumn("dbo.Player", "Name", c => c.String(maxLength: 500));

            CreateIndex(table: "Player",
                column: "Name",
                unique: true,
                name: "UniqueNameIndex");
        }
    }
}
