using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Town_Burger.Models.Identity;
using Town_Burger.Models.Responses;

namespace Town_Burger.Models.Context
{
    public class AppDbContext: IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions options):base (options)
        {
            
        }
        //must be configured
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection")).EnableSensitiveDataLogging();
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().ToTable("Users", "Security");
            builder.Entity<IdentityRole>().ToTable("Roles", "Security");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", "Security");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", "Security");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", "Security");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", "Security");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", "Security");



        }
        public DbSet<Balance> Balances { get; set; }
        public DbSet<Deposit> Deposits { get; set; }
        public DbSet<Spend> Spends { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Order> Orders{ get; set; }
        public DbSet<Review> Reviews{ get; set; }
        public DbSet<Customer> Customers{ get; set; }
        public DbSet<Employee> Employees{ get; set; }

        
    }
}