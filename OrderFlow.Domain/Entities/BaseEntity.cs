using OrderFlow.Domain.Enums;

namespace OrderFlow.Domain.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }

        public StatusEnum Status { get; set; } = StatusEnum.Active;

        public string CreatedBy { get; set; } = "System";
        public string? UpdatedBy { get; set; } = null;
        public string? DeletedBy { get; set; } = null;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = null;
        public DateTime? DeletedAt { get; set; } = null;
    }
}
