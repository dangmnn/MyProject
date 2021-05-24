using System;
using System.Collections.Generic;
using System.Text;

namespace Tagent.Domain.Repository.Define
{
    public interface IUnitOfWork
    {
        IRepository<T> GetRepository<T>() where T : class;
        void Save();
    }
}
