using Microsoft.EntityFrameworkCore;
using ShopMate._2._0.Domain.Entities;

namespace ShopMate._2._0.Infrastructure.Data
{
    public class LocalDbService : DbContext
    {
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<ShopCart> ShopCarts { get; set; }
        public DbSet<Item> Items { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var databasePath = $"Filename={DbPath.GetPath("shopmate_local_db.db")}";
            //var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = databasePath };
            //var connectionString = connectionStringBuilder.ToString();
            //var connection = new SqliteConnection(connectionString);

            optionsBuilder.UseSqlite(databasePath);
        }
    }
}
