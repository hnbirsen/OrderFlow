using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Domain.Entities;
using OrderFlow.Persistence.Extensions;

namespace OrderFlow.Persistence.Configurations;

public class OrderAssignmentConfiguration : IEntityTypeConfiguration<OrderAssignmentEntity>
{
    public void Configure(EntityTypeBuilder<OrderAssignmentEntity> builder)
    {
        builder.ConfigureBaseEntity();

        builder.Property(x => x.AssignedAt)
            .IsRequired();

        builder.HasOne(x => x.Order)
            .WithMany()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Courier)
            .WithMany()
            .HasForeignKey(x => x.CourierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.OrderId, x.CourierId })
            .IsUnique(false);
    }
}


