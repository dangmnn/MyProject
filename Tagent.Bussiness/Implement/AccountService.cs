using Tagent.Bussiness.Define;
using Tagent.Domain.Entities;
using Tagent.Domain.Repository.Define;

namespace Tagent.Bussiness.Implement
{
    public class AccountService : BaseService<Account>, IAccountService
    {
        public AccountService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
