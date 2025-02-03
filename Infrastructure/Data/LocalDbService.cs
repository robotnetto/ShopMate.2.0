using Microsoft.EntityFrameworkCore;
using ShopMate._2._0.Domain.Entities;

namespace ShopMate._2._0.Infrastructure.Data
{
    public class LocalDbService : DbContext
    {
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Cart> ShopCarts { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<FoodData> FoodItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var databasePath = $"Filename={DbPath.GetPath("shopmate_local_db.db")}";


            optionsBuilder.UseSqlite(databasePath);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cart>()
             .HasMany(c => c.Items)
             .WithOne()
             .HasForeignKey(i => i.CartId);
        }

    }
}
