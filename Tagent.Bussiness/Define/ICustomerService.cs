using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Tagent.Domain.Entities;

namespace Tagent.Bussiness.Define
{
    public interface ICustomerService
    {
        void Create(Customer entity);
        void Update(Customer entity);
        void Delete(int _id);
        Customer Get(Expression<Func<Customer, bool>> expression, string[] include);
        IQueryable<Customer> GetAll(Expression<Func<Customer, bool>> expression, int numpage, int perpage, params string[] include);
    }
}
