﻿namespace BusinessLogic.Models.PlayedGames
{
    public class RecentlyPlayedGamesFilter
    {
        public const int DEFAULT_NUMBER_OF_GAMES_TO_RETRIEVE = 5;
        public RecentlyPlayedGamesFilter()
        {
            NumberOfGamesToRetrieve = DEFAULT_NUMBER_OF_GAMES_TO_RETRIEVE;
        }

        public int NumberOfGamesToRetrieve { get; set; }
        public int? BoardGameGeekGameDefinitionId { get; set; }

        protected bool Equals(RecentlyPlayedGamesFilter other)
        {
            return NumberOfGamesToRetrieve == other.NumberOfGamesToRetrieve && BoardGameGeekGameDefinitionId == other.BoardGameGeekGameDefinitionId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RecentlyPlayedGamesFilter)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (NumberOfGamesToRetrieve.GetHashCode() * 397) ^ BoardGameGeekGameDefinitionId.GetHashCode();
            }
        }
    }
}
