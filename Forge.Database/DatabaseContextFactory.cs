using Forge.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Forge.Database;

public class DatabaseContextFactory : IDesignTimeDbContextFactory<RepositoryDbContext>
{
    public RepositoryDbContext CreateDbContext(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Connection string must be provided.", nameof(connectionString));
        }

        var optionsBuilder = new DbContextOptionsBuilder<RepositoryDbContext>();
        optionsBuilder.UseNpgsql(connectionString, b => b.MigrationsAssembly("Forge.Database"));

        return new RepositoryDbContext(optionsBuilder.Options);
    }

    public RepositoryDbContext CreateDbContext(string[] args)
    {
        var forgeServicePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "Forge.Service"));

        var configuration = new ConfigurationBuilder()
            .SetBasePath(forgeServicePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");
        return CreateDbContext(connectionString);
    }
}
