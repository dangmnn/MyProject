using Tagent.Bussiness.Define;
using Tagent.Domain.Entities;
using Tagent.Domain.Repository.Define;

namespace Tagent.Bussiness.Implement
{
    public class RoleService : BaseService<Role>, IRoleService
    {
        public RoleService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
