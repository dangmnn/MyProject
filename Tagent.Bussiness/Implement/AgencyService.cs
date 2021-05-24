using System;
using System.Collections.Generic;
using System.Text;
using Tagent.Bussiness.Define;
using Tagent.Domain.Entities;
using Tagent.Domain.Repository.Define;

namespace Tagent.Bussiness.Implement
{
    public class AgencyService : BaseService<Agency>, IAgencyService
    {
        public AgencyService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
}
}
