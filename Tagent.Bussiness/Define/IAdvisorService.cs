using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Tagent.Domain.Entities;

namespace Tagent.Bussiness.Define
{
    public interface IAdvisorService
    {
        void Create(Advisor entity);
        void Update(Advisor entity);
        void Delete(int _id);
        Advisor Get(Expression<Func<Advisor, bool>> expression, string[] include);
        IQueryable<Advisor> GetAll(Expression<Func<Advisor, bool>> expression, int numpage, int perpage, params string[] include);
    }
}
