using CicdDemo.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace CicdDemo.Api.Services
{
    public class CustomerService
    {
        private readonly AppDbContext _context;

        // The DbContext is injected via Dependency Injection
        public CustomerService(AppDbContext context)
        {
            _context = context;
        }

        // CREATE
        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        // READ (All)
        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            return await _context.Customers.ToListAsync();
        }

        // READ (By ID)
        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            return await _context.Customers.FindAsync(id);
        }

        // UPDATE
        public async Task<bool> UpdateCustomerAsync(int id, Customer updatedCustomer)
        {
            var existingCustomer = await _context.Customers.FindAsync(id);
            if (existingCustomer == null)
            {
                return false; // Customer not found
            }

            // Update properties
            existingCustomer.Name = updatedCustomer.Name;
            existingCustomer.Email = updatedCustomer.Email;

            await _context.SaveChangesAsync();
            return true;
        }

        // DELETE
        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return false; // Customer not found
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
