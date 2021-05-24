using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Tagent.Domain.Entities;

namespace Tagent.Bussiness.Define
{
    public interface IUserService
    {
        void Create(User entity);
        void Update(User entity);
        void Delete(int _id);
        User Get(Expression<Func<User, bool>> expression, string[] include);
        IQueryable<User> GetAll(Expression<Func<User, bool>> expression, int numpage, int perpage, params string[] include);
    }
}
