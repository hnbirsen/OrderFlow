using OrderFlow.Domain.Enums;

namespace OrderFlow.Domain.Entities
{
    public class OrderEntity : BaseEntity
    {
        public int UserId { get; set; }

        public decimal TotalAmount { get; set; } = 0.0m;
        public new OrderStatusEnum Status { get; set; } = OrderStatusEnum.New;
        public string Description { get; set; } = string.Empty;
        public string? TrackingCode { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public Dictionary<string, int> Items { get; set; } = new Dictionary<string, int>();


        public UserEntity User { get; set; } = null!;
        public ICollection<OrderStatusHistoryEntity> StatusHistory { get; set; } = new List<OrderStatusHistoryEntity>();
    }
}
