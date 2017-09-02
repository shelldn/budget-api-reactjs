namespace Budget.Api.Models
{
    public sealed class OperationSummary
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int Month { get; set; }
        public decimal Plan { get; set; }
        public decimal Fact { get; set; }
    }
}