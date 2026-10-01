using CicdDemo.Api.Data;
using CicdDemo.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace CicdDemo.Tests
{
    public class CustomerServiceTest
    {
        private AppDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task CreateAndRead_Customer_Succeeds()
        {
            // Arrange
            using var context = CreateDbContext();
            var service = new CustomerService(context);
            var newCustomer = new Customer { Name = "John Doe", Email = "john@example.com" };

            // Act
            var created = await service.CreateCustomerAsync(newCustomer);
            var fetched = await service.GetCustomerByIdAsync(created.Id);

            // Assert
            Assert.NotNull(fetched);
            Assert.Equal("John Doe", fetched.Name);
        }
    }
}
