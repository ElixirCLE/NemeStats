using UI.Models.UniversalGameModels;

namespace UI.Models.GameDefinitionModels
{
    public class GameDefinitionSummaryListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int GamingGroupId { get; set; }
        public BoardGameGeekInfoViewModel BoardGameGeekInfo { get; set; }
    }
}