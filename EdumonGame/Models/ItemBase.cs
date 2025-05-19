namespace EdumonGame.Models
{
    public class ItemBase
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public int Price { get; set; }
    }

    public class EvolutionItem : ItemBase
    {
        // Additional properties specific to evolution items can be added here
    }
}