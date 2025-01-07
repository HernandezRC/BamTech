using Microsoft.EntityFrameworkCore;
using System.Data;

namespace StargateAPI.Business.Data
{
    public class StargateContext(DbContextOptions<StargateContext> options) : DbContext(options)
    {
        public IDbConnection Connection => Database.GetDbConnection();
        public DbSet<Person> People { get; set; }
        public DbSet<AstronautDetail> AstronautDetails { get; set; }
        public DbSet<AstronautDuty> AstronautDuties { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(StargateContext).Assembly);
            
            base.OnModelCreating(modelBuilder);
        }

        //Removed the SeedData method, mostly just to clean up code and it was not being used.

    }
}
