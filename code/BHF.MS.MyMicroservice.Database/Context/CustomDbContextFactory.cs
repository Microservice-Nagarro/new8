using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Diagnostics.CodeAnalysis;

namespace BHF.MS.new8.Database.Context
{
    [ExcludeFromCodeCoverage(Justification = "It's EF context design-time factory implementation")]
    public class CustomDbContextFactory : IDesignTimeDbContextFactory<CustomDbContext>
    {
        public CustomDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CustomDbContext>();
            optionsBuilder.UseSqlServer(string.Join(" ", args));
            return new CustomDbContext(optionsBuilder.Options);
        }
    }
}

