using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Domain.Entities;
using OrderFlow.Persistence.Extensions;

namespace OrderFlow.Persistence.Configurations;

public class OrderStatusHistoryConfiguration : IEntityTypeConfiguration<OrderStatusHistoryEntity>
{
    public void Configure(EntityTypeBuilder<OrderStatusHistoryEntity> builder)
    {
        builder.ConfigureBaseEntity();

        builder.Property(x => x.OldStatus)
            .IsRequired();

        builder.Property(x => x.NewStatus)
            .IsRequired();

        builder.HasOne(x => x.Order);
    }
}
