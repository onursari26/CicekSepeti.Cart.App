using CicekSepeti.Domain.Concrete;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CicekSepeti.Core.Context
{
    public class AplicationContext : IdentityDbContext<User, Role, int>
    {
        private readonly ILoggerFactory _loggerFactory;
        public AplicationContext(DbContextOptions<AplicationContext> options, ILoggerFactory loggerFactory) : base(options)
        {
            _loggerFactory = loggerFactory;
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Discount> Discounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseLoggerFactory(_loggerFactory);
        }
    }
}
