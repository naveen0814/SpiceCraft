using Microsoft.EntityFrameworkCore;
using SpicecraftAPI.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SpicecraftAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<MenuCategory> MenuCategories { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Review> Reviews { get; set; }

        public DbSet<DeliveryPartner> DeliveryPartners { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ Enum to string conversion for UserRole
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();

            // USERS
            modelBuilder.Entity<User>()
                .HasMany(u => u.Carts)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Reviews)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // RESTAURANTS
            modelBuilder.Entity<Restaurant>()
                .HasMany(r => r.MenuItems)
                .WithOne(mi => mi.Restaurant)
                .HasForeignKey(mi => mi.RestaurantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Restaurant>()
                .HasMany(r => r.Orders)
                .WithOne(o => o.Restaurant)
                .HasForeignKey(o => o.RestaurantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Restaurant>()
                .HasMany(r => r.Discounts)
                .WithOne(d => d.Restaurant)
                .HasForeignKey(d => d.RestaurantId)
                .OnDelete(DeleteBehavior.Restrict);

            // MENU ITEMS
            modelBuilder.Entity<MenuItem>()
                .HasOne(mi => mi.Category)
                .WithMany(c => c.MenuItems)
                .HasForeignKey(mi => mi.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MenuItem>()
                .HasMany(mi => mi.CartItems)
                .WithOne(ci => ci.MenuItem)
                .HasForeignKey(ci => ci.MenuId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MenuItem>()
                .HasMany(mi => mi.OrderItems)
                .WithOne(oi => oi.MenuItem)
                .HasForeignKey(oi => oi.MenuId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MenuItem>()
                .HasMany(mi => mi.Reviews)
                .WithOne(r => r.MenuItem)
                .HasForeignKey(r => r.MenuId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MenuItem>()
                .HasMany(mi => mi.Discounts)
                .WithOne(d => d.MenuItem)
                .HasForeignKey(d => d.MenuId)
                .OnDelete(DeleteBehavior.Restrict);

            // CARTS
            modelBuilder.Entity<Cart>()
                .HasMany(c => c.CartItems)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            // ORDERS
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Payment)
                .WithOne(p => p.Order)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.MenuItem)
                .WithMany(m => m.OrderItems)
                .HasForeignKey(oi => oi.MenuId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            // MENU ITEM PRICE
            modelBuilder.Entity<MenuItem>()
                .Property(mi => mi.Price)
                .HasPrecision(10, 2);

            // ORDER TOTAL AMOUNT
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(10, 2);

            // ORDER ITEM PRICE
            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.Price)
                .HasPrecision(10, 2);

            // DISCOUNT PERCENTAGE
            modelBuilder.Entity<Discount>()
                .Property(d => d.DiscountPercentage)
                .HasPrecision(5, 2);


            modelBuilder.Entity<User>()
        .Property(u => u.Role)
        .HasConversion<string>(); // ✅ Save enum as string

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DeliveryPartner>()
       .HasOne(dp => dp.User)
       .WithMany() // Or .WithMany(u => u.DeliveryPartners) if you want a navigation list in User
       .HasForeignKey(dp => dp.UserId)
       .OnDelete(DeleteBehavior.Restrict); // Or your preferred behavior
        }
    }
}
