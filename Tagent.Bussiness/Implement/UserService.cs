using Tagent.Bussiness.Define;
using Tagent.Domain.Entities;
using Tagent.Domain.Repository.Define;

namespace Tagent.Bussiness.Implement
{
    public class UserService : BaseService<User>, IUserService
    {
        public UserService(IUnitOfWork unitofwork) : base(unitofwork)
        {
        }
    }
}
