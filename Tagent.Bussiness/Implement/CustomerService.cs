using System;
using System.Collections.Generic;
using System.Text;
using Tagent.Bussiness.Define;
using Tagent.Domain.Entities;
using Tagent.Domain.Repository.Define;

namespace Tagent.Bussiness.Implement
{
    public class CustomerService : BaseService<Customer>, ICustomerService
    {
        public CustomerService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
