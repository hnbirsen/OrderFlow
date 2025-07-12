using OrderFlow.Domain.Enums;

namespace OrderFlow.Application.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public required string Description { get; set; }
        public string TrackingCode { get; set; } = string.Empty;
        public required Dictionary<string, int> Items { get; set; }
        public OrderStatusEnum Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
