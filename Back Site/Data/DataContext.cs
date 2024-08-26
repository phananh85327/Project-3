using Back_Site.Model;
using Microsoft.EntityFrameworkCore;

namespace Back_Site.Data
{
    public class DataContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public DataContext(DbContextOptions<DataContext> options, IConfiguration configuration)
            : base(options)
        {
            // Add DB (Method 2)
            // Can remove configuration param
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            // Add DB (Method 2)
            if (builder.IsConfigured == false)
                builder.UseSqlServer(_configuration.GetConnectionString("Default"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserDataModel>().HasKey(u => new { u.UserID });
            modelBuilder.Entity<CategoryDataModel>().HasKey(c => new { c.CategoryID, c.UserID });
            modelBuilder.Entity<ListDataModel>().HasKey(l => new { l.ListID, l.UserID });
            modelBuilder.Entity<ItemDataModel>().HasKey(i => new { i.ItemID, i.UserID });
        }

        public DbSet<UserDataModel> UserData { get; set; }
        public DbSet<CategoryDataModel> CategoryData { get; set; }
        public DbSet<ListDataModel> ListData { get; set; }
        public DbSet<ItemDataModel> ItemData { get; set; }
    }
}
