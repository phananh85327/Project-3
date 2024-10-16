using EC.Model;
using Microsoft.EntityFrameworkCore;

namespace EC.Data
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
            modelBuilder.Entity<UserDataModel>().HasKey(user => new { user.UserID });
            modelBuilder.Entity<CategoryDataModel>().HasKey(category => new { category.CategoryID });
            modelBuilder.Entity<ListDataModel>().HasKey(list => new { list.ListID });
            modelBuilder.Entity<CustomerDataModel>().HasKey(customer => new { customer.CustomerID });
            modelBuilder.Entity<ItemDataModel>().HasKey(item => new { item.ItemID, item.ItemNo });
            modelBuilder.Entity<MailDataModel>().HasKey(mail => new { mail.MailID });
        }

        public DbSet<UserDataModel> UserData { get; set; }
        public DbSet<CategoryDataModel> CategoryData { get; set; }
        public DbSet<ListDataModel> ListData { get; set; }
        public DbSet<CustomerDataModel> CustomerData { get; set; } 
        public DbSet<ItemDataModel> ItemData { get; set; }
        public DbSet<MailDataModel> MailData { get; set; }
    }
}
