using Forge.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Forge.Tests.Database;

[TestFixture]
[NonParallelizable]
public class DatabaseContextFactoryTests
{
    private string? _originalCurrentDirectory;
    private string? _tempRoot;

    [SetUp]
    public void Setup()
    {
        _originalCurrentDirectory = Directory.GetCurrentDirectory();
        _tempRoot = Path.Combine(Path.GetTempPath(), $"forge-db-tests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempRoot);
    }

    [TearDown]
    public void TearDown()
    {
        if (_originalCurrentDirectory is not null)
        {
            Directory.SetCurrentDirectory(_originalCurrentDirectory);
        }

        if (!string.IsNullOrWhiteSpace(_tempRoot) && Directory.Exists(_tempRoot))
        {
            Directory.Delete(_tempRoot, recursive: true);
        }
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void CreateDbContext_WithInvalidConnectionString_ThrowsArgumentException(string? connectionString)
    {
        var factory = new DatabaseContextFactory();

        TestDelegate act = () => _ = factory.CreateDbContext(connectionString!);

        var ex = Assert.Throws<ArgumentException>(act);
        Assert.That(ex!.ParamName, Is.EqualTo("connectionString"));
    }

    [Test]
    public void CreateDbContext_WithValidConnectionString_ConfiguresNpgsqlProviderAndConnection()
    {
        const string connectionString = "Host=localhost;Port=5432;Database=forge;Username=forge;Password=secret";
        var factory = new DatabaseContextFactory();

        using var context = factory.CreateDbContext(connectionString);

        Assert.Multiple(() =>
        {
            Assert.That(context.Database.ProviderName, Is.EqualTo("Npgsql.EntityFrameworkCore.PostgreSQL"));
            Assert.That(context.Database.GetConnectionString(), Is.EqualTo(connectionString));
            Assert.That(GetMigrationsAssemblyName(context), Is.EqualTo("Forge.Database"));
        });
    }

    [Test]
    public void CreateDbContext_WithArgs_LoadsDefaultConnectionFromForgeServiceSettings()
    {
        const string expectedConnectionString = "Host=127.0.0.1;Port=5432;Database=testdb;Username=test;Password=test";
        CreateForgeServiceConfig(expectedConnectionString);

        var factory = new DatabaseContextFactory();

        using var context = factory.CreateDbContext([]);

        Assert.That(context.Database.GetConnectionString(), Is.EqualTo(expectedConnectionString));
    }

    [Test]
    public void CreateDbContext_WithArgs_ThrowsWhenDefaultConnectionMissing()
    {
        CreateForgeServiceConfig(connectionString: null);
        var factory = new DatabaseContextFactory();

        TestDelegate act = () => _ = factory.CreateDbContext([]);

        var ex = Assert.Throws<InvalidOperationException>(act);
        Assert.That(ex!.Message, Is.EqualTo("Connection string 'DefaultConnection' was not found."));
    }

    private void CreateForgeServiceConfig(string? connectionString)
    {
        var currentDir = Path.Combine(_tempRoot!, "work");
        var forgeServiceDir = Path.Combine(_tempRoot!, "Forge.Service");

        Directory.CreateDirectory(currentDir);
        Directory.CreateDirectory(forgeServiceDir);

        var appsettingsPath = Path.Combine(forgeServiceDir, "appsettings.json");
        var appsettingsJson = connectionString is null
            ? "{}"
            : $$"""
              {
                "ConnectionStrings": {
                  "DefaultConnection": "{{connectionString}}"
                }
              }
              """;

        File.WriteAllText(appsettingsPath, appsettingsJson);

        Directory.SetCurrentDirectory(currentDir);
    }

    private static string? GetMigrationsAssemblyName(DbContext context)
    {
        var options = context.GetService<IDbContextOptions>();
        return options.Extensions
            .OfType<RelationalOptionsExtension>()
            .Single()
            .MigrationsAssembly;
    }
}
