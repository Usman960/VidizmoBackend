using Microsoft.EntityFrameworkCore;
using VidizmoBackend.Models; // <-- IMPORTANT: Ensure this matches your model's namespace

namespace VidizmoBackend.Data // <-- IMPORTANT: Ensure this matches your project's data namespace
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            // Constructor to pass options (like connection string) to the base DbContext
        }

        // Define DbSet properties for each model you want to map to a database table
        public DbSet<Product> Products { get; set; } // This will map your Product model to a 'Products' table

        // If you have other models, add them here:
        // public DbSet<Category> Categories { get; set; }
    }
}