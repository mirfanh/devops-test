using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
namespace FileProcessor
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }
        public AppDBContext()
        {
            
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TransmissionSummary>().ToTable("TransmissionSummary");
            modelBuilder.Entity<Product>().ToTable("Product");
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<TransmissionSummary> TransmissionSummaries { get; set; }
        public bool ImportPayLoad(PayLoad payLoad)
        {
            Database.EnsureCreated();
            var transaction = Database.BeginTransaction();
            if (TransmissionSummaries.Any(x=>x.Id == payLoad.TransmissionSummary.Id))
            {
                transaction.Rollback();
                return false;
            }
            try
            {
                TransmissionSummaries.Add(payLoad.TransmissionSummary);
                Products.AddRange(payLoad.Products);
                SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw e;
            }
        }
    }
}
