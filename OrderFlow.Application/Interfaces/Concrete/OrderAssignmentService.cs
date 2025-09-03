using OrderFlow.Application.DTOs;
using OrderFlow.Application.Interfaces.Abstract;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace OrderFlow.Application.Interfaces.Concrete
{
    public class OrderAssignmentService : IOrderAssignmentService
    {
        private readonly IOrderAssignmentRepository _orderAssignmentRepository;
        private readonly ILogger<OrderAssignmentService> _logger;

        public OrderAssignmentService(IOrderAssignmentRepository orderAssignmentRepository, ILogger<OrderAssignmentService> logger)
        {
            _orderAssignmentRepository = orderAssignmentRepository;
            _logger = logger;
        }

        public async Task<bool> AssignOrderAsync(AssignOrderRequest request)
        {
            _logger.LogInformation("Entering AssignOrderAsync method for orderId: {OrderId} to courierId: {CourierId}", request.OrderId, request.CourierId);
            var entity = new OrderAssignmentEntity
            {
                OrderId = request.OrderId,
                CourierId = request.CourierId
            };
            await _orderAssignmentRepository.AddAsync(entity);
            var result = await _orderAssignmentRepository.CompleteAsync();
            _logger.LogInformation("Order assignment result: {Result}", result);
            _logger.LogInformation("Exiting AssignOrderAsync method.");
            return result;
        }

        public async Task<IEnumerable<OrderAssignmentEntity>> GetByCourierIdAsync(int courierId)
        {
            _logger.LogInformation("Entering GetByCourierIdAsync method for courierId: {CourierId}", courierId);
            var assignments = await _orderAssignmentRepository.FindAsync(x => x.CourierId == courierId);
            _logger.LogInformation("{Count} assignments found for courierId: {CourierId}", assignments?.Count() ?? 0, courierId);
            _logger.LogInformation("Exiting GetByCourierIdAsync method.");
            return assignments;
        }

        public async Task<OrderAssignmentEntity?> GetActiveAssignmentByOrderIdAsync(int orderId)
        {
            _logger.LogInformation("Entering GetActiveAssignmentByOrderIdAsync method for orderId: {OrderId}", orderId);
            var assignment = (await _orderAssignmentRepository.FindAsync(x => x.OrderId == orderId && x.DeletedAt == null)).FirstOrDefault();
            if (assignment == null)
            {
                _logger.LogWarning("No active assignment found for orderId: {OrderId}", orderId);
            }
            _logger.LogInformation("Exiting GetActiveAssignmentByOrderIdAsync method.");
            return assignment;
        }
    }    
}