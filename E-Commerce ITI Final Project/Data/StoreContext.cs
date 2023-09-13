using E_Commerce_ITI_Final_Project.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_ITI_Final_Project.Data
{
    public class StoreContext : IdentityDbContext<Account>
    {
        public StoreContext(DbContextOptions<StoreContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Account>().ToTable("Accounts");
            builder.Entity<Seller>().ToTable("Sellers");
            builder.Entity<AppUser>().ToTable("Users");

            builder.Entity<OrderProduct>().HasKey(op => new { op.OrderId, op.ProductId });

            // configure all decimal properties to [("decimal(18, 2)")]
            //builder.Entity<Order>().Property(o => o.Total).HasColumnType("decimal(18, 2)");
            builder.Entity<Product>().Property(o => o.Price).HasColumnType("decimal(18, 2)");
            builder.Entity<Product>().Property(o => o.AddedOn).HasDefaultValueSql("GETDATE()");

            // configure conversion of OrderStatus Enum
            builder.Entity<OrderProduct>().Property(o => o.Status).HasConversion<string>();

            base.OnModelCreating(builder);
        }


        public DbSet<Account> Accounts { get; set; }
        public DbSet<Seller> Sellers { get; set; }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
