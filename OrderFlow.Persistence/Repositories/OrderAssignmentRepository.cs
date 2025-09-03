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
    }
}


