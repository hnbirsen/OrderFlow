namespace OrderFlow.Application.DTOs
{
    public class CreateOrderRequest
    {
        public int UserId { get; set; }
        public Dictionary<string, int> Items { get; set; } = new Dictionary<string, int>();
        public string Description { get; set; } = string.Empty;
    }
}
