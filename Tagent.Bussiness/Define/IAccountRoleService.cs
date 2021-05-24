using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Tagent.Domain.Entities;

namespace Tagent.Bussiness.Define
{
    public interface IAccountRoleService
    {
        void Create(AccountRole entity);
        void Update(AccountRole entity);
        void Delete(int _id);
        AccountRole Get(Expression<Func<AccountRole, bool>> expression, string[] include);
        IQueryable<AccountRole> GetAll(Expression<Func<AccountRole, bool>> expression, int numpage, int perpage, params string[] include);
    }
}
