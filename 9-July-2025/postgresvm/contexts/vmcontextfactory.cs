 using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using PostgresVm.contexts;
using System.IO;

namespace Twitterapi.Contexts
{
    public class TwitterContextFactory : IDesignTimeDbContextFactory<VmContext>
    {
        public VmContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<VmContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            return new VmContext(optionsBuilder.Options);
        }
    }
}