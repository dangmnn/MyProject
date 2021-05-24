using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Tagent.Domain.Entities;

namespace Tagent.Bussiness.Define
{
    public interface ICompanyService
    {
        void Create(Company entity);
        void Update(Company entity);
        void Delete(int _id);
        Company Get(Expression<Func<Company, bool>> expression, string[] include);
        IQueryable<Company> GetAll(Expression<Func<Company, bool>> expression, int numpage, int perpage, params string[] include);
    }
}
