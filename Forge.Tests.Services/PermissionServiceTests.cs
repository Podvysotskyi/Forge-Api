using Forge.Domain.Entities;
using Forge.Domain.Repositories;
using Forge.Services;

namespace Forge.Tests.Services;

public class PermissionServiceTests
{
    [Test]
    public async Task GetAll_ShouldReturnMappedPermissionDtos()
    {
        var permissions = new[]
        {
            new Permission { Id = 1, Name = "Users.Read" },
            new Permission { Id = 2, Name = "Users.Write" }
        };
        var permissionRepository = new FakePermissionRepository(permissions);
        var repositoryManager = new FakeRepositoryManager(permissionRepository);
        var service = new PermissionService(repositoryManager);

        var result = (await service.GetAll()).ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(permissionRepository.GetAllCallCount, Is.EqualTo(1));
            Assert.That(result, Has.Length.EqualTo(2));
            Assert.That(result[0].Id, Is.EqualTo(1));
            Assert.That(result[0].Name, Is.EqualTo("Users.Read"));
            Assert.That(result[1].Id, Is.EqualTo(2));
            Assert.That(result[1].Name, Is.EqualTo("Users.Write"));
        });
    }

    [Test]
    public async Task GetAll_ShouldPassCancellationTokenToRepository()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var permissionRepository = new FakePermissionRepository([]);
        var repositoryManager = new FakeRepositoryManager(permissionRepository);
        var service = new PermissionService(repositoryManager);

        await service.GetAll(cancellationTokenSource.Token);

        Assert.That(permissionRepository.LastCancellationToken, Is.EqualTo(cancellationTokenSource.Token));
    }

    private sealed class FakePermissionRepository(IEnumerable<Permission> permissions) : IPermissionRepository
    {
        private readonly List<Permission> _permissions = permissions.ToList();

        public int GetAllCallCount { get; private set; }

        public CancellationToken LastCancellationToken { get; private set; }

        public Task<List<Permission>> GetAll(CancellationToken cancellationToken = default)
        {
            GetAllCallCount++;
            LastCancellationToken = cancellationToken;
            return Task.FromResult(_permissions);
        }

        public Task<Permission> GetById(int id, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<Permission> GetByName(string name, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IEnumerable<Role>> LoadRoles(Permission permission, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }

    private sealed class FakeRepositoryManager(IPermissionRepository permissionRepository) : IRepositoryManager
    {
        public IOrganizationRepository OrganizationRepository => throw new NotSupportedException();
        public IPermissionRepository PermissionRepository { get; } = permissionRepository;
        public IRoleRepository RoleRepository => throw new NotSupportedException();
        public IUserRepository UserRepository => throw new NotSupportedException();
        public IStatusRepository StatusRepository => throw new NotSupportedException();
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }
}
