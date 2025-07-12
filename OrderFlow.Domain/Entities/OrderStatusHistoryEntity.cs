using OrderFlow.Domain.Enums;

namespace OrderFlow.Domain.Entities
{
    public class OrderStatusHistoryEntity : BaseEntity
    {
        public int OrderId { get; set; }
        public OrderStatusEnum OldStatus { get; set; }
        public OrderStatusEnum NewStatus { get; set; }


        public OrderEntity Order { get; set; } = null!;
    }
}
