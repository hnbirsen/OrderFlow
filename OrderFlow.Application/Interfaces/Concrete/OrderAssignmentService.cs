using Microsoft.Extensions.Logging;
using OrderFlow.Application.DTOs;
using OrderFlow.Application.Interfaces.Abstract;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;

namespace OrderFlow.Application.Interfaces.Concrete
{
    public class OrderAssignmentService : IOrderAssignmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderAssignmentService> _logger;

        public OrderAssignmentService(IUnitOfWork unitOfWork, ILogger<OrderAssignmentService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> AssignOrderAsync(AssignOrderRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (request.OrderId <= 0 || request.CourierId <= 0)
            {
                _logger.LogWarning("Invalid assignment parameters. OrderId: {OrderId}, CourierId: {CourierId}", request.OrderId, request.CourierId);
                return false;
            }

            _logger.LogInformation("Entering AssignOrderAsync method for orderId: {OrderId} to courierId: {CourierId}", request.OrderId, request.CourierId);
            var entity = new OrderAssignmentEntity
            {
                OrderId = request.OrderId,
                CourierId = request.CourierId
            };
            await _unitOfWork.OrderAssignments.AddAsync(entity);
            var result = await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Order assignment result: {Result}", result > 0);
            _logger.LogInformation("Exiting AssignOrderAsync method.");
            return result > 0;
        }

        public async Task<IEnumerable<OrderAssignmentEntity>> GetByCourierIdAsync(int courierId)
        {
            if (courierId <= 0)
            {
                _logger.LogWarning("Invalid courierId: {CourierId}", courierId);
                return Enumerable.Empty<OrderAssignmentEntity>();
            }

            _logger.LogInformation("Entering GetByCourierIdAsync method for courierId: {CourierId}", courierId);
            var assignments = await _unitOfWork.OrderAssignments.FindAsync(x => x.CourierId == courierId);
            _logger.LogInformation("{Count} assignments found for courierId: {CourierId}", assignments?.Count() ?? 0, courierId);
            _logger.LogInformation("Exiting GetByCourierIdAsync method.");
            return assignments;
        }

        public async Task<OrderAssignmentEntity?> GetActiveAssignmentByOrderIdAsync(int orderId)
        {
            if (orderId <= 0)
            {
                _logger.LogWarning("Invalid orderId: {OrderId}", orderId);
                return null;
            }

            _logger.LogInformation("Entering GetActiveAssignmentByOrderIdAsync method for orderId: {OrderId}", orderId);
            var assignment = (await _unitOfWork.OrderAssignments.FindAsync(x => x.OrderId == orderId && x.DeletedAt == null)).FirstOrDefault();
            if (assignment == null)
            {
                _logger.LogWarning("No active assignment found for orderId: {OrderId}", orderId);
            }
            _logger.LogInformation("Exiting GetActiveAssignmentByOrderIdAsync method.");
            return assignment;
        }
    }
}