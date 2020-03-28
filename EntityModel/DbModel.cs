using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace EntityModel
{
    public class DbModel : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Feedback> Feedback { get; set; }

        public DbModel(DbContextOptions<DbModel> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Phone number should be unique for each user.
            modelBuilder.Entity<User>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique();
        }
    }

    public class DbModelFactory : IDesignTimeDbContextFactory<DbModel>
    {
        /// <summary>
        /// Provides a way for EF Core Tools to create a DbContext instance at design time (i.e. running migrations).
        /// See https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dbcontext-creation
        /// </summary>
        public DbModel CreateDbContext(string[] args)
        {
            // Only used by EF Core Tools, so okay to hardcode to local DB.
            return Create("Server=(LocalDb)\\MSSQLLocalDB;initial catalog=CORA;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework");
            //return Create("Server=tcp:project-tira-staging.database.windows.net,1433;Initial Catalog=project-tira-staging;Persist Security Info=False;User ID=project-tira;Password=LamePassword1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
        }

        public static DbModel Create(string connectionString)
        {
            return new DbModel(new DbContextOptionsBuilder<DbModel>()
               .UseSqlServer(connectionString)
               .Options);
        }

        public static DbModel CreateInMemory()
        {
            return new DbModel(new DbContextOptionsBuilder<DbModel>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options);
        }
    }
}
