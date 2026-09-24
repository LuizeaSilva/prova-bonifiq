using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using ProvaPub.Repository;
using ProvaPub.Services;
using Xunit;

namespace ProvaPub.Tests
{
    public class CustomerServiceTests
    {
        private const int CustomerId = 1;

        // Quarta-feira, 10/01/2024, 10h UTC: dia útil e dentro do horário comercial
        private static readonly DateTime BusinessHour = new(2024, 1, 10, 10, 0, 0, DateTimeKind.Utc);

        private readonly TestDbContext _ctx;

        public CustomerServiceTests()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _ctx = new TestDbContext(options);
            _ctx.Customers.Add(new Customer { Id = CustomerId, Name = "Cliente Teste" });
            _ctx.SaveChanges();
        }

        private CustomerService CreateService(DateTime? utcNow = null)
        {
            return new CustomerService(_ctx, new FakeDateTimeProvider(utcNow ?? BusinessHour));
        }

        private void AddOrder(DateTime orderDate)
        {
            _ctx.Orders.Add(new Order { CustomerId = CustomerId, Value = 50, OrderDate = orderDate });
            _ctx.SaveChanges();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task CanPurchase_InvalidCustomerId_ThrowsArgumentOutOfRange(int customerId)
        {
            var service = CreateService();

            var ex = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.CanPurchase(customerId, 50));
            Assert.Equal("customerId", ex.ParamName);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task CanPurchase_InvalidPurchaseValue_ThrowsArgumentOutOfRange(decimal purchaseValue)
        {
            var service = CreateService();

            var ex = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.CanPurchase(CustomerId, purchaseValue));
            Assert.Equal("purchaseValue", ex.ParamName);
        }

        [Fact]
        public async Task CanPurchase_CustomerNotRegistered_ThrowsInvalidOperation()
        {
            var service = CreateService();

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.CanPurchase(999, 50));
        }

        [Fact]
        public async Task CanPurchase_OrderInLastMonth_ReturnsFalse()
        {
            AddOrder(BusinessHour.AddDays(-10));
            var service = CreateService();

            Assert.False(await service.CanPurchase(CustomerId, 50));
        }

        [Fact]
        public async Task CanPurchase_OrderExactlyOneMonthAgo_ReturnsFalse()
        {
            AddOrder(BusinessHour.AddMonths(-1));
            var service = CreateService();

            Assert.False(await service.CanPurchase(CustomerId, 50));
        }

        [Fact]
        public async Task CanPurchase_OnlyOrdersOlderThanOneMonth_ReturnsTrue()
        {
            AddOrder(BusinessHour.AddMonths(-1).AddDays(-1));
            var service = CreateService();

            Assert.True(await service.CanPurchase(CustomerId, 50));
        }

        [Fact]
        public async Task CanPurchase_OrderFromAnotherCustomerInLastMonth_ReturnsTrue()
        {
            _ctx.Customers.Add(new Customer { Id = 2, Name = "Outro Cliente" });
            _ctx.Orders.Add(new Order { CustomerId = 2, Value = 50, OrderDate = BusinessHour.AddDays(-1) });
            _ctx.SaveChanges();
            var service = CreateService();

            Assert.True(await service.CanPurchase(CustomerId, 50));
        }

        [Theory]
        [InlineData(100.01)]
        [InlineData(500)]
        public async Task CanPurchase_FirstPurchaseAbove100_ReturnsFalse(decimal purchaseValue)
        {
            var service = CreateService();

            Assert.False(await service.CanPurchase(CustomerId, purchaseValue));
        }

        [Theory]
        [InlineData(0.01)]
        [InlineData(50)]
        [InlineData(100)]
        public async Task CanPurchase_FirstPurchaseUpTo100_ReturnsTrue(decimal purchaseValue)
        {
            var service = CreateService();

            Assert.True(await service.CanPurchase(CustomerId, purchaseValue));
        }

        [Fact]
        public async Task CanPurchase_ReturningCustomerAbove100_ReturnsTrue()
        {
            AddOrder(BusinessHour.AddMonths(-2));
            var service = CreateService();

            Assert.True(await service.CanPurchase(CustomerId, 500));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(7)]
        [InlineData(19)]
        [InlineData(23)]
        public async Task CanPurchase_OutsideBusinessHours_ReturnsFalse(int hour)
        {
            var service = CreateService(BusinessHour.Date.AddHours(hour));

            Assert.False(await service.CanPurchase(CustomerId, 50));
        }

        [Theory]
        [InlineData(8)]
        [InlineData(12)]
        [InlineData(18)]
        public async Task CanPurchase_WithinBusinessHours_ReturnsTrue(int hour)
        {
            var service = CreateService(BusinessHour.Date.AddHours(hour));

            Assert.True(await service.CanPurchase(CustomerId, 50));
        }

        [Theory]
        [InlineData(13)] // Sábado
        [InlineData(14)] // Domingo
        public async Task CanPurchase_OnWeekend_ReturnsFalse(int day)
        {
            var service = CreateService(new DateTime(2024, 1, day, 10, 0, 0, DateTimeKind.Utc));

            Assert.False(await service.CanPurchase(CustomerId, 50));
        }

        [Theory]
        [InlineData(8)]  // Segunda-feira
        [InlineData(12)] // Sexta-feira
        public async Task CanPurchase_OnWeekday_ReturnsTrue(int day)
        {
            var service = CreateService(new DateTime(2024, 1, day, 10, 0, 0, DateTimeKind.Utc));

            Assert.True(await service.CanPurchase(CustomerId, 50));
        }
    }
}
