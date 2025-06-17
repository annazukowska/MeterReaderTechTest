using MeterReaderTechTest.Models;
using Microsoft.EntityFrameworkCore;

namespace MeterReaderTechTest.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<MeterReading> MeterReadings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MeterReading>()
                .HasOne(mr => mr.Account)
                .WithMany(a => a.MeterReadings)
                .HasForeignKey(mr => mr.AccountId);
        }
    }
}
