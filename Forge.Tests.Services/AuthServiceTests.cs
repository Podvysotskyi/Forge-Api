using Forge.Contracts.Data.Auth;
using Forge.Contracts.Data.Organization;
using Forge.Contracts.Types.Role;
using Forge.Contracts.Types.Status;
using Forge.Domain.Entities;
using Forge.Domain.Exceptions.Auth;
using Forge.Domain.Exceptions.User;
using Forge.Domain.Repositories;
using Forge.Services;
using Forge.Services.Abstractions;

namespace Forge.Tests.Services;

public class AuthServiceTests {
    [Test]
    public async Task Login_ShouldReturnUser_WhenCredentialsAreValid()
    {
        var user = CreateUser(password: "hashed");
        var userRepository = new FakeUserRepository(user)
        {
            StatusToReturn = new Status { Id = 1, Name = nameof(UserStatus.Active) },
            PermissionsToReturn = [new Permission { Id = 1, Name = "Users.Read" }]
        };
        var service = CreateService(userRepository: userRepository, passwordService: new FakePasswordService { VerifyPasswordResult = true });

        var result = await service.Login(new LoginDto { LoginOrEmail = "jdoe", Password = "plain" });

        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(user.Id));
            Assert.That(result.Login, Is.EqualTo(user.Login));
            Assert.That(result.Email, Is.EqualTo(user.Email));
            Assert.That(userRepository.LoadStatusCallCount, Is.EqualTo(0));
            Assert.That(userRepository.LoadPermissionsCallCount, Is.EqualTo(0));
        });
    }

    [Test]
    public async Task Login_ShouldNotLoadPermissionsOrStatus()
    {
        var userRepository = new FakeUserRepository(CreateUser(password: "hashed"))
        {
            StatusToReturn = new Status { Id = 2, Name = nameof(UserStatus.Blocked) },
            PermissionsToReturn = [new Permission { Id = 1, Name = "Users.Read" }]
        };
        var service = CreateService(userRepository: userRepository, passwordService: new FakePasswordService { VerifyPasswordResult = true });

        var result = await service.Login(new LoginDto { LoginOrEmail = "jdoe", Password = "plain" });

        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(userRepository.LoadStatusCallCount, Is.EqualTo(0));
            Assert.That(userRepository.LoadPermissionsCallCount, Is.EqualTo(0));
        });
    }

    [Test]
    public void Login_ShouldThrowInvalidCredentialsException_WhenPasswordIsInvalid()
    {
        var userRepository = new FakeUserRepository(CreateUser(password: "hashed"));
        var service = CreateService(userRepository: userRepository, passwordService: new FakePasswordService { VerifyPasswordResult = false });

        Assert.ThrowsAsync<InvalidCredentialsException>(() => service.Login(new LoginDto { LoginOrEmail = "jdoe", Password = "wrong" }));
        Assert.That(userRepository.LoadStatusCallCount, Is.EqualTo(0));
    }

    [Test]
    public void Login_ShouldThrowInvalidCredentialsException_WhenUserDoesNotExist()
    {
        var userRepository = new FakeUserRepository(CreateUser()) { ThrowOnGetByLoginOrEmail = true };
        var service = CreateService(userRepository: userRepository, passwordService: new FakePasswordService { VerifyPasswordResult = true });

        Assert.ThrowsAsync<InvalidCredentialsException>(() => service.Login(new LoginDto { LoginOrEmail = "missing", Password = "plain" }));
    }

    [Test]
    public async Task Register_ShouldCreateUserAssignDefaultRoleCreateOrganizationAndReturnUser()
    {
        var createdUser = CreateUser(password: "stored");
        var userRepository = new FakeUserRepository(createdUser)
        {
            LoginExistsResult = false,
            EmailExistsResult = false,
            StatusToReturn = new Status { Id = 1, Name = nameof(UserStatus.Active) },
            PermissionsToReturn = [new Permission { Id = 1, Name = "Users.Read" }]
        };
        var userStatusRepository = new FakeStatusRepository();
        var roleRepository = new FakeRoleRepository();
        var organizationRepository = new FakeOrganizationRepository();
        var repositoryManager = new FakeRepositoryManager(userRepository, userStatusRepository, roleRepository, organizationRepository);
        var service = new AuthService(repositoryManager, new FakePasswordService { HashPasswordResult = "hashed" });

        var result = await service.Register(new RegisterDto { Login = "jdoe", Email = "jdoe@example.com", Password = "plain" });

        Assert.Multiple(() =>
        {
            Assert.That(userRepository.LoginExistsCallCount, Is.EqualTo(1));
            Assert.That(userRepository.EmailExistsCallCount, Is.EqualTo(1));
            Assert.That(userStatusRepository.LastRequestedUserStatus, Is.EqualTo(UserStatus.Active));
            Assert.That(userRepository.LastCreateLogin, Is.EqualTo("jdoe"));
            Assert.That(userRepository.LastCreateEmail, Is.EqualTo("jdoe@example.com"));
            Assert.That(userRepository.LastCreatePassword, Is.EqualTo("hashed"));
            Assert.That(userRepository.LastCreatedStatus!.Name, Is.EqualTo(nameof(UserStatus.Active)));
            Assert.That(roleRepository.LastRequestedUserRole, Is.EqualTo(UserRole.User));
            Assert.That(createdUser.Roles.Select(role => role.Name), Is.EqualTo(new[] { nameof(UserRole.User) }));
            Assert.That(organizationRepository.LastCreateUser, Is.Null);
            Assert.That(repositoryManager.SaveChangesCallCount, Is.EqualTo(0));
            Assert.That(result.Status, Is.Null);
            Assert.That(userRepository.LoadStatusCallCount, Is.EqualTo(0));
            Assert.That(userRepository.LoadPermissionsCallCount, Is.EqualTo(0));
        });
    }

    [Test]
    public void Register_ShouldThrowUserAlreadyExistsException_WhenLoginAlreadyExists()
    {
        var userRepository = new FakeUserRepository(CreateUser())
        {
            LoginExistsResult = true,
            EmailExistsResult = false
        };
        var userStatusRepository = new FakeStatusRepository();
        var roleRepository = new FakeRoleRepository();
        var organizationRepository = new FakeOrganizationRepository();
        var repositoryManager = new FakeRepositoryManager(userRepository, userStatusRepository, roleRepository, organizationRepository);
        var service = new AuthService(repositoryManager, new FakePasswordService { HashPasswordResult = "hashed" });

        var exception = Assert.ThrowsAsync<UserAlreadyExistsException>(() =>
            service.Register(new RegisterDto { Login = "jdoe", Email = "jdoe@example.com", Password = "plain" }));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(userRepository.LoginExistsCallCount, Is.EqualTo(1));
            Assert.That(userRepository.EmailExistsCallCount, Is.EqualTo(1));
            Assert.That(userRepository.CreateCallCount, Is.EqualTo(0));
            Assert.That(repositoryManager.SaveChangesCallCount, Is.EqualTo(0));
        });
    }

    [Test]
    public void Register_ShouldThrowUserAlreadyExistsException_WhenEmailAlreadyExists()
    {
        var userRepository = new FakeUserRepository(CreateUser())
        {
            LoginExistsResult = false,
            EmailExistsResult = true
        };
        var userStatusRepository = new FakeStatusRepository();
        var roleRepository = new FakeRoleRepository();
        var organizationRepository = new FakeOrganizationRepository();
        var repositoryManager = new FakeRepositoryManager(userRepository, userStatusRepository, roleRepository, organizationRepository);
        var service = new AuthService(repositoryManager, new FakePasswordService { HashPasswordResult = "hashed" });

        var exception = Assert.ThrowsAsync<UserAlreadyExistsException>(() =>
            service.Register(new RegisterDto { Login = "jdoe", Email = "jdoe@example.com", Password = "plain" }));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(userRepository.LoginExistsCallCount, Is.EqualTo(1));
            Assert.That(userRepository.EmailExistsCallCount, Is.EqualTo(1));
            Assert.That(userRepository.CreateCallCount, Is.EqualTo(0));
            Assert.That(repositoryManager.SaveChangesCallCount, Is.EqualTo(0));
        });
    }

    [Test]
    public async Task Register_ShouldPassCancellationTokenToDependencies()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var userRepository = new FakeUserRepository(CreateUser());
        var userStatusRepository = new FakeStatusRepository();
        var roleRepository = new FakeRoleRepository();
        var organizationRepository = new FakeOrganizationRepository();
        var repositoryManager = new FakeRepositoryManager(userRepository, userStatusRepository, roleRepository, organizationRepository);
        var service = new AuthService(repositoryManager, new FakePasswordService { HashPasswordResult = "hashed" });

        await service.Register(new RegisterDto { Login = "jdoe", Email = "jdoe@example.com", Password = "plain" }, cancellationTokenSource.Token);

        Assert.Multiple(() =>
        {
            Assert.That(userRepository.LastLoginExistsCancellationToken, Is.EqualTo(cancellationTokenSource.Token));
            Assert.That(userRepository.LastEmailExistsCancellationToken, Is.EqualTo(cancellationTokenSource.Token));
            Assert.That(userStatusRepository.LastFindCancellationToken, Is.EqualTo(cancellationTokenSource.Token));
            Assert.That(userRepository.LastCreateCancellationToken, Is.EqualTo(cancellationTokenSource.Token));
            Assert.That(roleRepository.LastFindCancellationToken, Is.EqualTo(cancellationTokenSource.Token));
            Assert.That(organizationRepository.LastCreateCancellationToken, Is.EqualTo(default(CancellationToken)));
            Assert.That(userRepository.LastLoadStatusCancellationToken, Is.EqualTo(default(CancellationToken)));
            Assert.That(repositoryManager.LastSaveChangesCancellationToken, Is.EqualTo(default(CancellationToken)));
        });
    }

    private static AuthService CreateService(
        FakeUserRepository userRepository,
        FakePasswordService passwordService,
        FakeStatusRepository? userStatusRepository = null,
        FakeRoleRepository? roleRepository = null,
        FakeOrganizationRepository? organizationRepository = null)
    {
        return new AuthService(
            new FakeRepositoryManager(
                userRepository,
                userStatusRepository ?? new FakeStatusRepository(),
                roleRepository ?? new FakeRoleRepository(),
                organizationRepository ?? new FakeOrganizationRepository()),
            passwordService);
    }

    private static User CreateUser(string password = "password")
    {
        return new User
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Login = "jdoe",
            Email = "jdoe@example.com",
            Password = password,
            StatusId = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    private sealed class FakePasswordService : IPasswordService
    {
        public string HashPasswordResult { get; init; } = "hashed";
        public bool VerifyPasswordResult { get; init; }

        public string HashPassword(string password) => HashPasswordResult;
        public bool VerifyPassword(string hashedPassword, string providedPassword) => VerifyPasswordResult;
    }

    private sealed class FakeRepositoryManager(
        IUserRepository userRepository,
        IStatusRepository statusRepository,
        IRoleRepository roleRepository,
        IOrganizationRepository organizationRepository) : IRepositoryManager
    {
        public int SaveChangesCallCount { get; private set; }
        public CancellationToken LastSaveChangesCancellationToken { get; private set; }

        public IOrganizationRepository OrganizationRepository { get; } = organizationRepository;
        public IPermissionRepository PermissionRepository => throw new NotSupportedException();
        public IRoleRepository RoleRepository { get; } = roleRepository;
        public IUserRepository UserRepository { get; } = userRepository;
        public IStatusRepository StatusRepository { get; } = statusRepository;

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCallCount++;
            LastSaveChangesCancellationToken = cancellationToken;
            return Task.FromResult(1);
        }
    }

    private sealed class FakeUserRepository(User createdUser) : IUserRepository {
        public bool ThrowOnGetByLoginOrEmail { get; init; }
        public bool LoginExistsResult { get; init; }
        public bool EmailExistsResult { get; init; }
        public IEnumerable<Permission> PermissionsToReturn { get; init; } = [];
        public Status StatusToReturn { get; init; } = new() { Id = 1, Name = nameof(UserStatus.Active) };

        public int LoginExistsCallCount { get; private set; }
        public int EmailExistsCallCount { get; private set; }
        public int CreateCallCount { get; private set; }
        public int LoadStatusCallCount { get; private set; }
        public int LoadPermissionsCallCount { get; private set; }
        public CancellationToken LastLoginExistsCancellationToken { get; private set; }
        public CancellationToken LastEmailExistsCancellationToken { get; private set; }
        public CancellationToken LastCreateCancellationToken { get; private set; }
        public CancellationToken LastLoadStatusCancellationToken { get; private set; }
        public CancellationToken LastLoadPermissionsCancellationToken { get; private set; }
        public string? LastCreateLogin { get; private set; }
        public string? LastCreateEmail { get; private set; }
        public string? LastCreatePassword { get; private set; }
        public Status? LastCreatedStatus { get; private set; }

        public Task<User> GetByLogin(string login, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<User> GetByEmail(string email, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        
        public Task<bool> LoginExists(string login, CancellationToken cancellationToken = default)
        {
            LoginExistsCallCount++;
            LastLoginExistsCancellationToken = cancellationToken;
            return Task.FromResult(LoginExistsResult);
        }

        public Task<bool> EmailExists(string email, CancellationToken cancellationToken = default)
        {
            EmailExistsCallCount++;
            LastEmailExistsCancellationToken = cancellationToken;
            return Task.FromResult(EmailExistsResult);
        }

        public Task<User> GetByLoginOrEmail(string loginOrEmail, CancellationToken cancellationToken = default)
        {
            return ThrowOnGetByLoginOrEmail
                ? Task.FromException<User>(new UserDoesNotExistsException())
                : Task.FromResult(createdUser);
        }

        public Task<User> Create(string login, string email, string password, Status status, CancellationToken cancellationToken = default)
        {
            CreateCallCount++;
            LastCreateLogin = login;
            LastCreateEmail = email;
            LastCreatePassword = password;
            LastCreatedStatus = status;
            LastCreateCancellationToken = cancellationToken;
            return Task.FromResult(createdUser);
        }

        public Task<IEnumerable<Permission>> LoadPermissions(User user, CancellationToken cancellationToken = default)
        {
            LoadPermissionsCallCount++;
            LastLoadPermissionsCancellationToken = cancellationToken;
            return Task.FromResult(PermissionsToReturn);
        }

        public Task<IEnumerable<Role>> LoadRoles(User user, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<Status> LoadStatus(User user, CancellationToken cancellationToken = default)
        {
            LoadStatusCallCount++;
            LastLoadStatusCancellationToken = cancellationToken;
            user.Status = StatusToReturn;
            return Task.FromResult(StatusToReturn);
        }

    }

    private sealed class FakeRoleRepository : IRoleRepository
    {
        public UserRole? LastRequestedUserRole { get; private set; }
        public CancellationToken LastFindCancellationToken { get; private set; }

        public Task<List<Role>> UserRoles(CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IEnumerable<Role>> OrganizationUserRoles(CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<Role> GetById(int id, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<Role> Find(UserRole type, CancellationToken cancellationToken = default)
        {
            LastRequestedUserRole = type;
            LastFindCancellationToken = cancellationToken;

            return Task.FromResult(new Role { Id = 4, Name = type.ToString() });
        }

        public Task<Role> Find(OrganizationUserRole type, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IEnumerable<User>> LoadUsers(Role role, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IEnumerable<Permission>> LoadPermissions(Role role, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }

    private sealed class FakeStatusRepository : IStatusRepository
    {
        public UserStatus? LastRequestedUserStatus { get; private set; }
        public CancellationToken LastFindCancellationToken { get; private set; }

        public Task<IEnumerable<Status>> GetUserStatuses(CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IEnumerable<Status>> GetOrganizationUserStatuses(CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<Status> GetById(int id, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<Status> Find(UserStatus type, CancellationToken cancellationToken = default)
        {
            LastRequestedUserStatus = type;
            LastFindCancellationToken = cancellationToken;

            return Task.FromResult(new Status { Id = 1, Name = type.ToString() });
        }

        public Task<Status> Find(OrganizationUserStatus type, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }

    private sealed class FakeOrganizationRepository : IOrganizationRepository
    {
        public User? LastCreateUser { get; private set; }
        public CancellationToken LastCreateCancellationToken { get; private set; }

        public Task<List<Organization>> GetAll(CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<Organization> GetById(Guid id, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<bool> NameExists(string name, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<Organization> Create(string name, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<Organization> Create(User user, CancellationToken cancellationToken = default)
        {
            LastCreateUser = user;
            LastCreateCancellationToken = cancellationToken;
            return Task.FromResult(new Organization
            {
                Id = Guid.NewGuid(),
                Name = user.Id.ToString(),
                UserId = user.Id,
                User = user,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        public Task<Organization> Delete(Organization organization, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IEnumerable<OrganizationUser>> LoadUsers(Organization organization, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }
}
