namespace Budget.Api.Categories
{
    public sealed class CategorySummary
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CategoryType Type { get; set; }
    }
}