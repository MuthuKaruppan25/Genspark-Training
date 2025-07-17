
// using JobPortal.Contexts;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Design;
// using Microsoft.Extensions.Configuration;
// using System.IO;


// namespace DocumentShare.Contexts
// {
//     public class JobContextFactory : IDesignTimeDbContextFactory<JobContext>
//     {
//         public JobContext CreateDbContext(string[] args)
//         {
//             var configuration = new ConfigurationBuilder()
//                 .SetBasePath(Directory.GetCurrentDirectory())
//                 .AddJsonFile("appsettings.json")
//                 .Build();

//             var optionsBuilder = new DbContextOptionsBuilder<JobContext>();
//             optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));

//             return new JobContext(optionsBuilder.Options);
//         }
//     }
// }

using JobPortal.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Azure.Identity;
using System.IO;

namespace DocumentShare.Contexts
{
    public class JobContextFactory : IDesignTimeDbContextFactory<JobContext>
    {
        public JobContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();

            // Load appsettings.json to get KeyVaultName
            var initialConfig = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .Build();

            string? keyVaultName = initialConfig["KeyVault:Name"];
            if (string.IsNullOrEmpty(keyVaultName))
                throw new Exception("KeyVault:Name is missing in appsettings.json");

            string keyVaultUri = $"https://{keyVaultName}.vault.azure.net/";

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .AddAzureKeyVault(new Uri(keyVaultUri), new DefaultAzureCredential())
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
                throw new Exception("Connection string 'DefaultConnection' not found in Azure Key Vault.");

            var optionsBuilder = new DbContextOptionsBuilder<JobContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new JobContext(optionsBuilder.Options);
        }
    }
}
