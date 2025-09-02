using OrderFlow.Domain.Dependencies;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Domain.Interfaces
{
	public interface IOrderAssignmentRepository : IBaseRepository<OrderAssignmentEntity>, IScoped
	{
		Task<IEnumerable<OrderAssignmentEntity>> GetByCourierIdAsync(int courierId);
		Task<OrderAssignmentEntity?> GetActiveAssignmentByOrderIdAsync(int orderId);
	}
}


