using OrderFlow.Domain.Enums;

namespace OrderFlow.Domain.Entities
{
	public class OrderAssignmentEntity : BaseEntity
	{
		public int OrderId { get; set; }
		public int CourierId { get; set; }
		public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
		public DateTime? PickedUpAt { get; set; }
		public DateTime? DeliveredAt { get; set; }

		public OrderEntity Order { get; set; } = null!;
		public UserEntity Courier { get; set; } = null!;
	}
}


