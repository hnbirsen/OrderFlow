using Microsoft.EntityFrameworkCore;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;
using OrderFlow.Persistence.Context;

namespace OrderFlow.Persistence.Repositories
{
	public class OrderAssignmentRepository : BaseRepository<OrderAssignmentEntity>, IOrderAssignmentRepository
	{
		public OrderAssignmentRepository(OrderFlowDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<OrderAssignmentEntity>> GetByCourierIdAsync(int courierId)
		{
			return await _dbSet
				.Where(x => x.CourierId == courierId)
				.ToListAsync();
		}

		public async Task<OrderAssignmentEntity?> GetActiveAssignmentByOrderIdAsync(int orderId)
		{
			return await _dbSet
				.Where(x => x.OrderId == orderId && x.DeletedAt == null)
				.OrderByDescending(x => x.CreatedAt)
				.FirstOrDefaultAsync();
		}
	}
}


