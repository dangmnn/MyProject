using System;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using Tagent.Domain.Repository.Define;

namespace Tagent.Bussiness.Implement
{
    public abstract class BaseService<T> where T : class 
    {
        private readonly IUnitOfWork _iunitOfWork;
        private IRepository<T> _irepository;

        public BaseService(IUnitOfWork iunitOfWork)
        {
            this._iunitOfWork = iunitOfWork;
            this._irepository = iunitOfWork.GetRepository<T>();
        }


        public virtual void Create(T _entity)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
                {
                    this._irepository.Create(_entity);
                    this._iunitOfWork.Save();

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual void Update(T _entity)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
                {
                    this._irepository.Update(_entity);
                    this._iunitOfWork.Save();

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual void Delete(int _id)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
                {
                    this._irepository.DeleteById(_id);
                    this._iunitOfWork.Save();

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public virtual T Get(Expression<Func<T, bool>> expression, string[] include ) => this._irepository.Get(expression, include);

        public virtual IQueryable<T> GetAll(Expression<Func<T, bool>> expression, int numpage, int perpage, params string[] include)
            => this._irepository.GetEntities(expression, numpage, perpage, include); 
    }
}
