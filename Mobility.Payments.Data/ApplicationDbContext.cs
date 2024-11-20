namespace Mobility.Payments.Data
{
    using Microsoft.EntityFrameworkCore;
    using Mobility.Payments.Data.Configuration;
    using Mobility.Payments.Domain.Entities;

    public class ApplicationDbContext : DbContext
    {
        public const string ConnectionName = "MobilityConnection";

        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Payment> Payment { get; set; }

        public DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentConfiguration).Assembly);
        }
    }
}
