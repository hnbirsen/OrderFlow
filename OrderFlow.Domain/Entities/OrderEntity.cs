using OrderFlow.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

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

        public string ItemsJson { get; set; } = string.Empty;

        [NotMapped]
        public Dictionary<string, int>? Items
        {
            get => string.IsNullOrEmpty(ItemsJson)
                ? new Dictionary<string, int>()
                : JsonSerializer.Deserialize<Dictionary<string, int>>(ItemsJson);
            set => ItemsJson = JsonSerializer.Serialize(value);
        }

        public UserEntity User { get; set; } = null!;
        public ICollection<OrderStatusHistoryEntity> StatusHistory { get; set; } = new List<OrderStatusHistoryEntity>();
    }
}
