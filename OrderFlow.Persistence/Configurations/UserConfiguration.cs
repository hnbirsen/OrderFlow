using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Domain.Entities;
using OrderFlow.Persistence.Extensions;

namespace OrderFlow.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ConfigureBaseEntity();

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.FirstName)
           .IsRequired()
           .HasMaxLength(20);
        
        builder.Property(x => x.LastName)
           .IsRequired()
           .HasMaxLength(20);

        builder.Property(x => x.Username)
           .IsRequired()
           .HasMaxLength(20);

        builder.Property(x => x.Password)
            .IsRequired();

        builder.Property(x => x.Role)
            .IsRequired();

        builder.HasMany(x => x.Orders)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId);
    }
}
