using OrderFlow.Application.DTOs;
using OrderFlow.Domain.Dependencies;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Application.Interfaces.Abstract
{
    public interface IOrderAssignmentService : IScoped
    {
        Task<bool> AssignOrderAsync(AssignOrderRequest request);
        Task<IEnumerable<OrderAssignmentEntity>> GetByCourierIdAsync(int courierId);
        Task<OrderAssignmentEntity?> GetActiveAssignmentByOrderIdAsync(int orderId);
    }
}
