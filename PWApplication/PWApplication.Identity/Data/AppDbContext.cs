using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage;
using PWApplication.Identity.Models.DataModels;

namespace PWApplication.Identity.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<PWTransaction> PWTransactions { get; set; }

        public DbSet<PWOperationPair> PWOperationPairs { get; set; }

        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=PWAppDb;Trusted_Connection=True;");
            }
        }

        private IDbContextTransaction _currentTransaction;

        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;

        public bool HasActiveTransaction => _currentTransaction != null;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            
            builder.Entity<PWTransaction>().HasKey(transaction => transaction.Id);
            builder.Entity<PWTransaction>().HasOne(t => t.Agent).WithMany().IsRequired()
                .HasForeignKey(transaction => transaction.AgentId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<PWTransaction>().HasOne(t => t.Сounteragent).WithMany().IsRequired()
                .HasForeignKey(transaction => transaction.СounteragentId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<PWTransaction>().Property<decimal>(t => t.TransactionAmount).HasColumnType("decimal(18, 2)");
            builder.Entity<PWTransaction>().Property<decimal>(t => t.AgentBalance).HasColumnType("decimal(18, 2)");
            builder.Entity<PWTransaction>().Property<DateTime>(t => t.OperationDateTime);
            builder.Entity<PWTransaction>().Property<string>(t => t.Description).HasMaxLength(500);
            

            builder.Entity<PWOperationPair>().HasOne(t => t.TransactionOne)
                .WithMany().HasForeignKey(t => t.TransactionOneId).IsRequired().OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PWOperationPair>().HasOne(t => t.TransactionTwo)
                .WithMany().HasForeignKey(t => t.TransactionTwoId).IsRequired().OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PWOperationPair>().HasKey(new string[] {"TransactionOneId","TransactionTwoId"});
       
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {   
            var result = await base.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null) return null;

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveChangesAsync();
                transaction.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
    }

    public class AppDbContextDesignFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=PWAppDb;Trusted_Connection=True;");

            return new AppDbContext(optionsBuilder.Options);
        }

    }
}
