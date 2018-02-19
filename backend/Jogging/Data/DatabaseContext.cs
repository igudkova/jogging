using System.Data.Entity;
using Jogging.Data.Entities;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Jogging.Data
{
    public class DatabaseContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Run> Runs { get; set; }

        public DatabaseContext() : base("JoggingContext")
        {
            Database.SetInitializer(new DatabaseInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Runs)
                .WithOptional(s => s.User)
                .WillCascadeOnDelete(true);
        }
    }

    public class DatabaseInitializer : CreateDatabaseIfNotExists<DatabaseContext>
    {
        protected override void Seed(DatabaseContext context)
        {
            context.Roles.Add(new IdentityRole() { Name = "Manager" });
            context.Roles.Add(new IdentityRole() { Name = "User" });
            context.SaveChanges();

            base.Seed(context);
        }
    }
}