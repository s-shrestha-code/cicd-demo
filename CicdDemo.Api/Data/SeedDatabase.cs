namespace CicdDemo.Api.Data.Test
{
    public class SeedDatabase
    {
        //public static void Seed()
        //{
        //    using var scope = host.Services.CreateScope();
        //    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        //    // Ensure the database is created in memory
        //    db.Database.EnsureCreated();

        //    // In development environment, check if data already exists to avoid duplication
        //    //if (IsDevelopmentEnvironment && !db.Customers.Any())
        //    if (!db.Customers.Any())
        //    {
        //        db.Customers.AddRange(GetTestCustomers());

        //        db.SaveChanges();
        //    }
        //}

        public static IList<Customer> GetTestCustomers()
        {
            var customers = new List<Customer>();

            customers.AddRange([
                new Customer { Name = "Alice Smith", Email = "alice@example.com" },
                new Customer { Name = "Bob Jones", Email = "bob@example.com" },
                new Customer { Name = "Charlie Brown", Email = "charlie@example.com" }
            ]);

            return customers;
        }
    }
}
