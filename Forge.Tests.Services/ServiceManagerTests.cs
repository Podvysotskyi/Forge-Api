using Forge.Contracts.Types.Role;
using Forge.Contracts.Types.Status;
using Forge.Domain.Entities;
using Forge.Domain.Repositories;
using Forge.Services;
using Forge.Services.Abstractions;

namespace Forge.Tests.Services;

public class ServiceManagerTests
{
    [Test]
    public void Services_ShouldBeInitializedLazily_AndCachedPerProperty()
    {
        var repositoryManager = new FakeRepositoryManager();
        var passwordService = new FakePasswordService();
        var serviceManager = new ServiceManager(repositoryManager, passwordService);
        var authService = serviceManager.AuthService;
        var organizationService = serviceManager.OrganizationService;
        var permissionService = serviceManager.PermissionService;
        var roleService = serviceManager.RoleService;

        Assert.Multiple(() =>
        {
            Assert.That(authService, Is.InstanceOf<AuthService>());
            Assert.That(organizationService, Is.InstanceOf<OrganizationService>());
            Assert.That(permissionService, Is.InstanceOf<PermissionService>());
            Assert.That(roleService, Is.InstanceOf<RoleService>());

            Assert.That(authService, Is.SameAs(serviceManager.AuthService));
            Assert.That(organizationService, Is.SameAs(serviceManager.OrganizationService));
            Assert.That(permissionService, Is.SameAs(serviceManager.PermissionService));
            Assert.That(roleService, Is.SameAs(serviceManager.RoleService));
        });
    }

    [Test]
    public async Task SaveChangesAsync_ShouldDelegateToRepositoryManager()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var repositoryManager = new FakeRepositoryManager();
        var serviceManager = new ServiceManager(repositoryManager, new FakePasswordService());

        var result = await serviceManager.SaveChangesAsync(cancellationTokenSource.Token);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(123));
            Assert.That(repositoryManager.SaveChangesCallCount, Is.EqualTo(1));
            Assert.That(repositoryManager.LastSaveChangesCancellationToken, Is.EqualTo(cancellationTokenSource.Token));
        });
    }

    private sealed class FakePasswordService : IPasswordService
    {
        public string HashPassword(string password) => password;

        public bool VerifyPassword(string hashedPassword, string providedPassword) => hashedPassword == providedPassword;
    }

    private sealed class FakeRepositoryManager : IRepositoryManager
    {
        public int SaveChangesCallCount { get; private set; }
        public CancellationToken LastSaveChangesCancellationToken { get; private set; }

        public IOrganizationRepository OrganizationRepository { get; } = new FakeOrganizationRepository();
        public IPermissionRepository PermissionRepository { get; } = new FakePermissionRepository();
        public IRoleRepository RoleRepository { get; } = new FakeRoleRepository();
        public IUserRepository UserRepository { get; } = new FakeUserRepository();
        public IStatusRepository StatusRepository { get; } = new FakeStatusRepository();

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCallCount++;
            LastSaveChangesCancellationToken = cancellationToken;
            return Task.FromResult(123);
        }
    }

    private sealed class FakeOrganizationRepository : IOrganizationRepository
    {
        public Task<List<Organization>> GetAll(CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<Organization> GetById(Guid id, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<bool> NameExists(string name, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<Organization> Create(string name, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<Organization> Create(User user, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<Organization> Delete(Organization organization, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IEnumerable<OrganizationUser>> LoadUsers(Organization organization, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }

    private sealed class FakePermissionRepository : IPermissionRepository
    {
        public Task<List<Permission>> GetAll(CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<Permission> GetById(int id, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<Permission> GetByName(string name, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IEnumerable<Role>> LoadRoles(Permission permission, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }

    private sealed class FakeRoleRepository : IRoleRepository
    {
        public Task<List<Role>> UserRoles(CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IEnumerable<Role>> OrganizationUserRoles(CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<Role> GetById(int id, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<Role> Find(UserRole type, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<Role> Find(OrganizationUserRole type, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IEnumerable<User>> LoadUsers(Role role, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IEnumerable<Permission>> LoadPermissions(Role role, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        public Task<bool> LoginExists(string login, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<bool> EmailExists(string email, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<User> GetByLogin(string login, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<User> GetByEmail(string email, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<User> GetByLoginOrEmail(string loginOrEmail, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<User> Create(string login, string email, string password, Status status, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IEnumerable<Permission>> LoadPermissions(User user, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IEnumerable<Role>> LoadRoles(User user, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<Status> LoadStatus(User user, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }

    private sealed class FakeStatusRepository : IStatusRepository
    {
        public Task<IEnumerable<Status>> GetUserStatuses(CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IEnumerable<Status>> GetOrganizationUserStatuses(CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<Status> GetById(int id, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<Status> Find(UserStatus type, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<Status> Find(OrganizationUserStatus type, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }
}
