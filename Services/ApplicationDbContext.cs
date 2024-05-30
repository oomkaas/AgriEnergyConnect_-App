using AgriEnergy.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AgriEnergy.Services
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) 
        { 

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Farmer> Farmers { get; set; }

    }

}
