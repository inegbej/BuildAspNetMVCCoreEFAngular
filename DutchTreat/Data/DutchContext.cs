using DutchTreat.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DutchTreat.Data
{
    public class DutchContext : IdentityDbContext<StoreUser>
    {
        // 
        public DutchContext(DbContextOptions<DutchContext> options) : base(options)
        {
        }

        // 
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }

        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
        }
    }
}
