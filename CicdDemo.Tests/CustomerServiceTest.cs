using CicdDemo.Api.Data;
using CicdDemo.Api.Data.Test;
using CicdDemo.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CicdDemo.Tests
{
    public class CustomerServiceTest
    {
        private AppDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                //Tells Entity Framework Core to stop throwing exceptions or warnings when your code tries
                //to use database transactions on an in-memory database provider, which does not natively support them
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task CreateAndRead_Customer_Succeeds()
        {
            var newCustomer = GetTestCustomer();

            Assert.NotNull(newCustomer);

            Customer created = null;
            Customer? fetched = null;

            if (newCustomer != null)
            {
                using var context = CreateDbContext();

                // Use a transaction so the test row is automatically cleaned up
                var cancellationToken = new CancellationToken();
                using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

                try
                {
                    var service = new CustomerService(context);

                    // Act
                    created = await service.CreateCustomerAsync(newCustomer);
                    fetched = await service.GetCustomerByIdAsync(created.Id);

                    // 2. Rollback immediately to prevent database bloat
                    await transaction.RollbackAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    // Transaction auto-rolls back on failure/dispose if not committed
                }

                // Assert
                Assert.NotNull(fetched);
                Assert.Equal(newCustomer.Name, fetched.Name);
            }
        }

        [Fact]
        public async Task GetAllCustomersAsync_Succeeds()
        {
            using var context = CreateDbContext();
            var service = new CustomerService(context);
            var newCustomer = new Customer { Name = "John Doe", Email = "john@example.com" };

            // Act
            var created = await service.CreateCustomerAsync(newCustomer);

            var customers = await service.GetAllCustomersAsync();

            Assert.True(customers.Any());
        }

        [Fact]
        public async Task UpdateCustomerAsync_Succeeds()
        {
            var newCustomer = GetTestCustomer();
            var updateCustomerDetails = new Customer()
            {
                Name = (newCustomer?.Name ?? "John Doe") + " The 2nd",
                Email = (newCustomer?.Email ?? "John.doe@example.com").Replace(".com", ".edu")
            };

            Assert.NotNull(newCustomer);

            Customer created = null;
            Customer? fetched = null;

            if (newCustomer != null)
            {
                using var context = CreateDbContext();

                // Use a transaction so the test row is automatically cleaned up
                var cancellationToken = new CancellationToken();
                using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

                try
                {
                    var service = new CustomerService(context);

                    // Act
                    created = await service.CreateCustomerAsync(newCustomer);
                    await service.UpdateCustomerAsync(created.Id, updateCustomerDetails);

                    fetched = await service.GetCustomerByIdAsync(created.Id);

                    // 2. Rollback immediately to prevent database bloat
                    await transaction.RollbackAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    // Transaction auto-rolls back on failure/dispose if not committed
                }

                // Assert
                Assert.NotNull(fetched);
                Assert.Equal(updateCustomerDetails.Name, fetched.Name);
                Assert.Equal(updateCustomerDetails.Email, fetched.Email);
            }
        }

        [Fact]
        public async Task DeleteCustomerAsync_Succeeds()
        {
            var newCustomer = GetTestCustomer();

            Assert.NotNull(newCustomer);

            Customer created = null;
            Customer? deleted = null;

            if (newCustomer != null)
            {
                using var context = CreateDbContext();

                // Use a transaction so the test row is automatically cleaned up
                var cancellationToken = new CancellationToken();
                using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

                try
                {
                    var service = new CustomerService(context);

                    // Act
                    created = await service.CreateCustomerAsync(newCustomer);
                    var HasBeenDeleted = await service.DeleteCustomerAsync(created.Id);

                    Assert.True(HasBeenDeleted);

                    deleted = await service.GetCustomerByIdAsync(created.Id);

                    // 2. Rollback immediately to prevent database bloat
                    await transaction.RollbackAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    // Transaction auto-rolls back on failure/dispose if not committed
                }

                // Assert
                Assert.NotNull(created);
                Assert.Null(deleted);
            }
        }

        private static Customer? GetTestCustomer()
        {
            var customers = SeedDatabase.GetTestCustomers();

            return customers.FirstOrDefault();
        }
    }
}
