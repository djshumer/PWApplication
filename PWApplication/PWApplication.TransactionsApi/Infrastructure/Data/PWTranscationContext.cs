using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Transaction.Api.Infrastructure.Data.DataModels;

namespace Transaction.Api.Infrastructure.Data
{
    public class PWTranscationContext : IdentityDbContext<ApplicationUser>, IUnitOfWork
    {
        public DbSet<PWTransaction> PWTransactions { get; set; }

        public DbSet<PWOperationPair> PWOperationPairs { get; set; }

        private IDbContextTransaction _currentTransaction;
        
        public PWTranscationContext() : base() { }

        public PWTranscationContext(DbContextOptions<PWTranscationContext> options) : base(options) { }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=PWAppDb;Trusted_Connection=True;");
            }
        }    
        
        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;

        public bool HasActiveTransaction => _currentTransaction != null;

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
