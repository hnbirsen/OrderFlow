using Microsoft.EntityFrameworkCore;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Persistence.Context
{
    public class OrderFlowDbContext : DbContext
    {
        public OrderFlowDbContext(DbContextOptions<OrderFlowDbContext> options): base(options)
        {
        }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<OrderEntity> Orders { get; set; }
        public DbSet<OrderStatusHistoryEntity> OrderStatusHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderFlowDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }

}
