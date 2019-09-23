using System;
using System.Threading.Tasks;
using PWApplication.TransactionApi.Infrastructure.Data.DataModels;
using PWApplication.TransactionApi.Infrastructure.Data.Repository;
using PWApplication.TransactionApi.Infrastructure.Exceptions;

namespace PWApplication.TransactionApi.Infrastructure.Data
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly PWTranscationContext _context;
        private IPWTransactionRepository _transactionRepository;
        private IGenericRepository<PWOperationPair> _operationPairsRepository;
        private IUserInfoRepository _userInfoRepository;

        public UnitOfWork(PWTranscationContext transcationContext)
        {
            _context = transcationContext;
        }

        public IPWTransactionRepository TransactionRepository
        {
            get { return _transactionRepository ?? (_transactionRepository = new PWTransactionRepository(_context)); }
        }

        public IUserInfoRepository UserInfoRepository
        {
            get { return _userInfoRepository ?? (_userInfoRepository = new UserInfoRepository(_context)); }
        }

        public IGenericRepository<PWOperationPair> OperationPairsRepository
        {
            get
            {
                return _operationPairsRepository ??
                       (_operationPairsRepository = new GenericRepository<PWOperationPair>(_context));
            }
        }

        /// <summary>
        /// Save Changes Async to DB
        /// </summary>
        /// DBException
        /// <returns></returns>
        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception exc)
            {
                throw new DbException(exc.Message, exc);
            }
            return true;
        }

        /// <summary>
        /// Save Changes Async to DB
        /// DBException
        /// </summary>
        public void SaveChanges()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (Exception exc)
            {
                throw new DbException(exc.Message, exc);
            }
        }

        #region IDisposable
        private bool disposed = false;

        public void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
