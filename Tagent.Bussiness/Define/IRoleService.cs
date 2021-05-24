using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Tagent.Domain.Entities;

namespace Tagent.Bussiness.Define
{
    public interface IRoleService
    {
        void Create(Role entity);
        void Update(Role entity);
        void Delete(int _id);
        Role Get(Expression<Func<Role, bool>> expression, string[] include);
        IQueryable<Role> GetAll(Expression<Func<Role, bool>> expression, int numpage, int perpage, params string[] include);
    }
}
