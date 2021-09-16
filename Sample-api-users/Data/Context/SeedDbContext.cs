using Microsoft.EntityFrameworkCore;
using Sample_api_users.Model;

namespace Sample_api_users
{
    public class SeedDbContext : DbContext
    {

        public DbSet<SampleType> SampleType { get; set; }

        public SeedDbContext(DbContextOptions<SeedDbContext> options) : base(options)
        {
            //var connection = (Microsoft.Data.SqlClient.SqlConnection)Database.GetDbConnection();
            //var tokenProv = new Microsoft.Azure.Services.AppAuthentication.AzureServiceTokenProvider();
            //connection.AccessToken = tokenProv.GetAccessTokenAsync("https://database.windows.net/").Result;
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured) optionsBuilder.UseSqlServer("name=defaultConnStr");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}