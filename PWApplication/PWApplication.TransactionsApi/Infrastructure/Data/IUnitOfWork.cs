using System;
using System.Threading.Tasks;
using PWApplication.TransactionApi.Infrastructure.Data.DataModels;
using PWApplication.TransactionApi.Infrastructure.Data.Repository;

namespace PWApplication.TransactionApi.Infrastructure.Data
{
    public interface IUnitOfWork : IDisposable
    {
        IPWTransactionRepository TransactionRepository { get; }

        IGenericRepository<PWOperationPair> OperationPairsRepository { get; }

        IUserInfoRepository UserInfoRepository { get; }

        Task<bool> SaveChangesAsync();
    }
}
