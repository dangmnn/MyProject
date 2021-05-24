using Microsoft.EntityFrameworkCore;
using Tagent.Domain.Entities;

namespace Tagent.Domain.Database
{
    public class TagentDBContext : DbContext, IEntityContext
    {
        public TagentDBContext(DbContextOptions<TagentDBContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<AccountRole> AccountRoles { get; set; }
        public DbSet<Advisor> Advisors { get; set; }
        public DbSet<Agency> Agencies { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Verifier> Verifiers { get; set; }
        public DbSet<User> Users { get; set; }

        public object GetContext => this;
    }
}
