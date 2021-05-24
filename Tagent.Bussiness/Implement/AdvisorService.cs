using System;
using System.Collections.Generic;
using System.Text;
using Tagent.Bussiness.Define;
using Tagent.Domain.Entities;
using Tagent.Domain.Repository.Define;

namespace Tagent.Bussiness.Implement
{
    public class AdvisorService : BaseService<Advisor>, IAdvisorService
    {
        public AdvisorService(IUnitOfWork unitofwork) : base(unitofwork)
        {
        }
    }
}
