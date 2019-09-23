using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using PWApplication.TransactionApi.Infrastructure.Data.DataModels;

namespace PWApplication.TransactionApi.Infrastructure.Data
{
    public class PWTranscationContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<PWTransaction> PWTransactions { get; set; }

        public DbSet<PWOperationPair> PWOperationPairs { get; set; }


        public PWTranscationContext(DbContextOptions<PWTranscationContext> options) : base(options) { }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=PWAppDb;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<PWTransaction>().HasKey(transaction => transaction.Id);
            builder.Entity<PWTransaction>().HasOne(t => t.Agent).WithMany().IsRequired()
                .HasForeignKey(transaction => transaction.AgentId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<PWTransaction>().HasOne(t => t.Сounteragent).WithMany().IsRequired()
                .HasForeignKey(transaction => transaction.СounteragentId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PWOperationPair>().HasOne(t => t.TransactionOne)
                .WithMany().HasForeignKey(t => t.TransactionOneId).IsRequired().OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PWOperationPair>().HasOne(t => t.TransactionTwo)
                .WithMany().HasForeignKey(t => t.TransactionTwoId).IsRequired().OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PWOperationPair>().HasKey(new string[] { "TransactionOneId", "TransactionTwoId" });
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await base.SaveChangesAsync(cancellationToken);

            return true;
        }

    }

    public class PWTranscationContextDesignFactory : IDesignTimeDbContextFactory<PWTranscationContext>
    {
        public PWTranscationContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PWTranscationContext>()
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=PWAppDb;Trusted_Connection=True;");

            return new PWTranscationContext(optionsBuilder.Options);
        }

    }
}
