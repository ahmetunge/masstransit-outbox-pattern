using MassTransit;
using MassTransitOutboxTest.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MassTransitOutboxTest.Api.Data
{
    public class OrderDbContext: DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options):base(options) 
        {
            
        }

        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
            modelBuilder.AddInboxStateEntity();
            
        }
    }
}
