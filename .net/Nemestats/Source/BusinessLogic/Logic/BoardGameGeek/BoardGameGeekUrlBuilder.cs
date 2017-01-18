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

namespace BusinessLogic.Logic.BoardGameGeek
{
    public static class BoardGameGeekUriBuilder
    {
        public const string BOARD_GAME_GEEK_BOARD_GAME_BASE_URI = "http://boardgamegeek.com/boardgame/{0}";
        public const string BOARD_GAME_GEEK_BOARD_USER_BASE_URI = "http://boardgamegeek.com/user/{0}";

        public static Uri BuildBoardGameGeekGameUri(int? boardGameGeekBoardGameObjectId)
        {
            if (boardGameGeekBoardGameObjectId.HasValue)
            {
                return new Uri(string.Format(BOARD_GAME_GEEK_BOARD_GAME_BASE_URI, boardGameGeekBoardGameObjectId));
            }

            return null;
        }

        public static Uri BuildBoardGameGeekUserUri(string userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                return new Uri(string.Format(BOARD_GAME_GEEK_BOARD_USER_BASE_URI, userName));
            }

            return null;
        }
    }
}