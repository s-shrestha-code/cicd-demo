using CicdDemo.Api.Data.Test;

namespace CicdDemo.Tests
{
    public class SeedDatabaseTest
    {
        [Fact]
        public async Task GetTestCustomers_ReturnsCustomer()
        {
            var customers = SeedDatabase.GetTestCustomers();
            Assert.True(customers.Any());
        }
    }
}