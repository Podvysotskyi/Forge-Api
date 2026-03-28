using Forge.Domain.Entities;
using Forge.Domain.Repositories;
using Forge.Services;
using Forge.Contracts.Types.Role;

namespace Forge.Tests.Services;

public class RoleServiceTests
{
    [Test]
    public async Task GetAll_ShouldReturnMappedRoleDtos()
    {
        var roles = new[]
        {
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "User" }
        };
        var roleRepository = new FakeRoleRepository(roles);
        var repositoryManager = new FakeRepositoryManager(roleRepository);
        var service = new RoleService(repositoryManager);

        var result = (await service.GetAll()).ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(roleRepository.UserRolesCallCount, Is.EqualTo(1));
            Assert.That(result, Has.Length.EqualTo(2));
            Assert.That(result[0].Id, Is.EqualTo(1));
            Assert.That(result[0].Name, Is.EqualTo("Admin"));
            Assert.That(result[1].Id, Is.EqualTo(2));
            Assert.That(result[1].Name, Is.EqualTo("User"));
        });
    }

    [Test]
    public async Task GetAll_ShouldPassCancellationTokenToRepository()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var roleRepository = new FakeRoleRepository([]);
        var repositoryManager = new FakeRepositoryManager(roleRepository);
        var service = new RoleService(repositoryManager);

        await service.GetAll(cancellationTokenSource.Token);

        Assert.That(roleRepository.LastCancellationToken, Is.EqualTo(cancellationTokenSource.Token));
    }

    private sealed class FakeRoleRepository(IEnumerable<Role> roles) : IRoleRepository
    {
        private readonly List<Role> _roles = roles.ToList();

        public int UserRolesCallCount { get; private set; }

        public CancellationToken LastCancellationToken { get; private set; }

        public Task<List<Role>> UserRoles(CancellationToken cancellationToken = default)
        {
            UserRolesCallCount++;
            LastCancellationToken = cancellationToken;
            return Task.FromResult(_roles);
        }

        public Task<IEnumerable<Role>> OrganizationUserRoles(CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<Role> GetById(int id, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<Role> Find(UserRole type, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<Role> Find(OrganizationUserRole type, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IEnumerable<User>> LoadUsers(Role role, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IEnumerable<Permission>> LoadPermissions(Role role, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }

    private sealed class FakeRepositoryManager(IRoleRepository roleRepository) : IRepositoryManager
    {
        public IOrganizationRepository OrganizationRepository => throw new NotSupportedException();
        public IPermissionRepository PermissionRepository => throw new NotSupportedException();
        public IRoleRepository RoleRepository { get; } = roleRepository;
        public IUserRepository UserRepository => throw new NotSupportedException();
        public IStatusRepository StatusRepository => throw new NotSupportedException();
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }
}
