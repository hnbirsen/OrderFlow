using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using OrderFlow.Application.Configuration;
using OrderFlow.Application.DTOs;
using OrderFlow.Application.Interfaces.Abstract;
using OrderFlow.Application.Interfaces.Concrete;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Enums;
using OrderFlow.Persistence.Context;
using OrderFlow.Persistence.Repositories;
using System;
using System.Threading.Tasks;
using Xunit;

namespace OrderFlow.Tests
{
    public class AuthServiceTests
    {
        private static AuthService CreateService(out OrderFlowDbContext db)
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

            // Provide JwtSettings for JwtService
            var jwtSettings = new JwtSettings
            {
                SecretKey = "6>~~Y$dWq.`s,S1{B+8>iOAx^6R+{U<Z",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                TokenExpirationMinutes = 60
            };

            IJwtService jwtService = new JwtService(Options.Create(jwtSettings));

            return new AuthService(uow, jwtService, new NullLogger<AuthService>());
        }

        [Fact]
        public async Task LoginAsync_ReturnsNull_WhenInvalidCredentials()
        {
            var service = CreateService(out _);
            var res = await service.LoginAsync(new LoginRequest("a@b.com", "wrong"));
            Assert.Null(res);
        }

        [Fact]
        public async Task LoginAsync_ReturnsToken_WhenValidCredentials()
        {
            var service = CreateService(out var db);
            db.Users.Add(new UserEntity
            {
                Email = "a@b.com",
                Password = AesEncryptionHelper.Encrypt("pass"),
                Role = UserRoleEnum.Admin,
                CreatedAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync();

            var res = await service.LoginAsync(new LoginRequest("a@b.com", "pass"));
            Assert.NotNull(res);
            Assert.False(string.IsNullOrWhiteSpace(res!.AccessToken));
        }
    }
}


