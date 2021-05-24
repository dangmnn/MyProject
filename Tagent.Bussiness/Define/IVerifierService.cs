using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Tagent.Domain.Entities;

namespace Tagent.Bussiness.Define
{
    public interface IVerifierService
    {
        void Create(Verifier entity);
        void Update(Verifier entity);
        void Delete(int _id);
        Verifier Get(Expression<Func<Verifier, bool>> expression, string[] include);
        IQueryable<Verifier> GetAll(Expression<Func<Verifier, bool>> expression, int numpage, int perpage, params string[] include);
    }
}
