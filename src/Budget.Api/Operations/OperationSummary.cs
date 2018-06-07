namespace Budget.Api.Operations
{
    public sealed class OperationSummary
    {
        public string Id { get; set; }
        public string CategoryId { get; set; }
        public int Month { get; set; }
        public decimal Plan { get; set; }
        public decimal Fact { get; set; }
    }
}