using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using OrderFlow.Application.DTOs;
using OrderFlow.Application.Interfaces.Concrete;
using OrderFlow.Persistence.Context;
using OrderFlow.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OrderFlow.Tests
{
    public class OrderServiceTests
    {
        private static OrderService CreateService(out OrderFlowDbContext db)
        {
            var options = new DbContextOptionsBuilder<OrderFlowDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            db = new OrderFlowDbContext(options);

            var orders = new OrderRepository(db);
            var users = new UserRepository(db);
            var histories = new OrderStatusHistoryRepository(db);
            var assignments = new OrderAssignmentRepository(db);
            var uow = new UnitOfWork(db, orders, users, histories, assignments);

            var userService = new UserService(uow, new NullLogger<UserService>());
            return new OrderService(uow, userService, new NullLogger<OrderService>());
        }

        [Fact]
        public async Task CreateOrderAsync_PersistsOrder()
        {
            var service = CreateService(out var db);
            var request = new CreateOrderRequest
            {
                UserId = 1,
                Description = "Test",
                Items = new Dictionary<string, int> { { "i1", 2 }, { "i2", 3 } }
            };

            await service.CreateOrderAsync(request);

            Assert.Equal(1, db.Orders.Count());
        }
    }
}


