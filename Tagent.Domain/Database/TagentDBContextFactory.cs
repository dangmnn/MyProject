using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tagent.Domain.Database
{
    public class TagentDBContextFactory : IDesignTimeDbContextFactory<TagentDBContext>
    {
        public TagentDBContext CreateDbContext(string[] args)
        {
            var optionbuilder = new DbContextOptionsBuilder<TagentDBContext>();
            optionbuilder.UseSqlServer("Server=tcp:tagentdb.database.windows.net,1433;Initial Catalog=tagent;Persist Security Info=False;User ID=toor;Password=Tagent12345678;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            return new TagentDBContext(optionbuilder.Options);
        }
    }
}
