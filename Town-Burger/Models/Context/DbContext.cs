// Required namespaces for Identity, EF Core, and project-specific models
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Town_Burger.Models.Identity;
using Town_Burger.Models.Responses;

namespace Town_Burger.Models.Context
{
    // The application's database context, inheriting from IdentityDbContext to integrate ASP.NET Core Identity features
    public class AppDbContext : IdentityDbContext<User>
    {
        // Constructor that passes options to the base class
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        // Configures the database connection string from appsettings.json
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Load configuration from appsettings.json
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            // Use SQL Server with the specified connection string and enable detailed logging (for debugging)
            optionsBuilder
                .UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                .EnableSensitiveDataLogging(); // Shows detailed info like parameter values in logs (use carefully in production)
        }

        // Customize the schema and table mappings for Identity tables
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Call base to ensure Identity is configured

            // Map Identity tables to a custom schema called "Security"
            builder.Entity<User>().ToTable("Users", "Security");
            builder.Entity<IdentityRole>().ToTable("Roles", "Security");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", "Security");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", "Security");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", "Security");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", "Security");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", "Security");
        }

        // Define DbSet properties to expose each entity/table to Entity Framework

        public virtual DbSet<Balance> Balances { get; set; }         // User's wallet or credit
        public virtual DbSet<Deposit> Deposits { get; set; }         // Transactions for adding money
        public virtual DbSet<Spend> Spends { get; set; }             // Transactions for spending money
        public virtual DbSet<Address> Addresses { get; set; }        // Shipping or user addresses
        public virtual DbSet<Cart> Carts { get; set; }               // User shopping carts
        public virtual DbSet<CartItem> CartItems { get; set; }       // Items within each cart
        public virtual DbSet<MenuItem> MenuItems { get; set; }       // Food or menu items offered
        public virtual DbSet<Order> Orders { get; set; }             // Orders placed by users
        public virtual DbSet<Review> Reviews { get; set; }           // Customer reviews for menu items or service
        public virtual DbSet<Customer> Customers { get; set; }       // Customer profile or data
        public virtual DbSet<Employee> Employees { get; set; }       // Employee records (delivery, cooks, etc.)
        public virtual DbSet<Secondary> Secondaries { get; set; }    // Unknown - likely additional or extended data
    }
}
