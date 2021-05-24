using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Tagent.Domain.Repository.Define
{
    public interface IRepository<T> where T : class
    {
        void Create(T _entity);
        void Update(T _entity);
        void DeleteById(int _id);
        T Get(Expression<Func<T, bool>> expression, string[] include);
        IQueryable<T> GetEntities(Expression<Func<T, bool>> expression = null, int numpage = 0, int perpage = 0, params string[] include);
    }
}
