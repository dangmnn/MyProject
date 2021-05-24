using System;
using System.Collections.Generic;
using Tagent.Domain.Database;
using Tagent.Domain.Repository.Define;

namespace Tagent.Domain.Repository.Implement
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private TagentDBContext _dbcontext;
        private IDictionary<Type, object> _repository;
        private bool IDisposed = false;

        public UnitOfWork(IEntityContext dbcontext)
        {
            _dbcontext = dbcontext.GetContext as TagentDBContext;
            _repository = new Dictionary<Type, object>();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IRepository<T> GetRepository<T>() where T : class
        {
            IRepository<T> repository;
            if (!this._repository.ContainsKey(typeof(T)))
                this._repository.Add(typeof(T), repository = new Repository<T>(_dbcontext));
            else
                repository = this._repository[typeof(T)] as Repository<T>;
            return repository;
        }

        public void Save() => this._dbcontext.SaveChanges();

        protected virtual void Dispose(bool disposing)
        {
            if (!this.IDisposed && disposing)
            {
                this._dbcontext.Dispose();
            }
            this.IDisposed = true;
        }
        ~UnitOfWork()
        {
            Dispose(false);
        }
    }
}
