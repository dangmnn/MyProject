using System;
using System.Collections.Generic;
using System.Text;
using Tagent.Bussiness.Define;
using Tagent.Domain.Entities;
using Tagent.Domain.Repository.Define;

namespace Tagent.Bussiness.Implement
{
    public class VerifierService : BaseService<Verifier>, IVerifierService
    {
        public VerifierService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
