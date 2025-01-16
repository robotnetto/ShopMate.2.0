using Microsoft.EntityFrameworkCore;
using ShopMate._2._0.Domain.Entities;

namespace ShopMate._2._0.Infrastructure.Data
{
    public class LocalDbService : DbContext
    {
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Cart> ShopCarts { get; set; }
        public DbSet<CartItem> Items { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var databasePath = $"Filename={DbPath.GetPath("shopmate_local_db.db")}";
            

            optionsBuilder.UseSqlite(databasePath);
        }
    }
}
