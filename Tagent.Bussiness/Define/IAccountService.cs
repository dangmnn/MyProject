using System;
using System.Linq;
using System.Linq.Expressions;
using Tagent.Domain.Entities;

namespace Tagent.Bussiness.Define
{
    public interface IAccountService
    {
        void Create(Account entity);
        void Update(Account entity);
        void Delete(int _id);
        Account Get(Expression<Func<Account, bool>> expression, string[] include);
        IQueryable<Account> GetAll(Expression<Func<Account, bool>> expression, int numpage, int perpage, params string[] include);
    }
}
