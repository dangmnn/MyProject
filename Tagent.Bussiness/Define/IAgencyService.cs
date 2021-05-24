using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Tagent.Domain.Entities;

namespace Tagent.Bussiness.Define
{
    public interface IAgencyService
    {
        void Create(Agency entity);
        void Update(Agency entity);
        void Delete(int _id);
        Agency Get(Expression<Func<Agency, bool>> expression, string[] include);
        IQueryable<Agency> GetAll(Expression<Func<Agency, bool>> expression, int numpage, int perpage, params string[] include);
    }
}
