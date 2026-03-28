using Forge.Contracts.Data.Organization;
using Forge.Domain.Entities;
using Forge.Domain.Exceptions.Organization;
using Forge.Domain.Repositories;
using Forge.Services;

namespace Forge.Tests.Services;

public class OrganizationServiceTests
{
    [Test]
    public async Task GetAll_ShouldReturnMappedOrganizationDtos()
    {
        var organizations = new[]
        {
            new Organization
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Acme",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Organization
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Globex",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
        var organizationRepository = new FakeOrganizationRepository(organizations);
        var repositoryManager = new FakeRepositoryManager(organizationRepository);
        var service = new OrganizationService(repositoryManager);

        var result = (await service.GetAll()).ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(organizationRepository.GetAllCallCount, Is.EqualTo(1));
            Assert.That(result, Has.Length.EqualTo(2));
            Assert.That(result[0].Id, Is.EqualTo(Guid.Parse("11111111-1111-1111-1111-111111111111")));
            Assert.That(result[0].Name, Is.EqualTo("Acme"));
            Assert.That(result[1].Id, Is.EqualTo(Guid.Parse("22222222-2222-2222-2222-222222222222")));
            Assert.That(result[1].Name, Is.EqualTo("Globex"));
        });
    }

    [Test]
    public async Task GetAll_ShouldPassCancellationTokenToRepository()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var organizationRepository = new FakeOrganizationRepository([]);
        var repositoryManager = new FakeRepositoryManager(organizationRepository);
        var service = new OrganizationService(repositoryManager);

        await service.GetAll(cancellationTokenSource.Token);

        Assert.That(organizationRepository.LastCancellationToken, Is.EqualTo(cancellationTokenSource.Token));
    }

    [Test]
    public async Task Create_WithDtoAndUser_ShouldCreateOrganizationWithoutUserAssignment()
    {
        var user = CreateUser();
        var dto = new CreateOrganizationDto { Name = "Acme" };
        var organizationRepository = new FakeOrganizationRepository([]);
        var repositoryManager = new FakeRepositoryManager(organizationRepository);
        var service = new OrganizationService(repositoryManager);

        var result = await service.Create(dto, user);

        Assert.Multiple(() =>
        {
            Assert.That(organizationRepository.NameExistsCallCount, Is.EqualTo(1));
            Assert.That(organizationRepository.CreateWithDtoCallCount, Is.EqualTo(1));
            Assert.That(repositoryManager.SaveChangesCallCount, Is.EqualTo(0));
            Assert.That(result.Name, Is.EqualTo("Acme"));
            Assert.That(result.UserId, Is.Null);
            Assert.That(result.User, Is.Null);
        });
    }

    [Test]
    public void Create_WithDtoAndUser_ShouldThrowOrganizationAlreadyExistsException_WhenNameExists()
    {
        var organizationRepository = new FakeOrganizationRepository([])
        {
            NameExistsResult = true
        };
        var repositoryManager = new FakeRepositoryManager(organizationRepository);
        var service = new OrganizationService(repositoryManager);

        Assert.ThrowsAsync<OrganizationAlreadyExistsException>(() => service.Create(new CreateOrganizationDto { Name = "Acme" }, CreateUser()));

        Assert.Multiple(() =>
        {
            Assert.That(organizationRepository.NameExistsCallCount, Is.EqualTo(1));
            Assert.That(organizationRepository.CreateWithDtoCallCount, Is.EqualTo(0));
            Assert.That(repositoryManager.SaveChangesCallCount, Is.EqualTo(0));
        });
    }

    [Test]
    public async Task Create_WithUser_ShouldDelegateToRepository()
    {
        var user = CreateUser();
        var organizationRepository = new FakeOrganizationRepository([]);
        var repositoryManager = new FakeRepositoryManager(organizationRepository);
        var service = new OrganizationService(repositoryManager);

        var result = await service.Create(user);

        Assert.Multiple(() =>
        {
            Assert.That(organizationRepository.CreateWithUserCallCount, Is.EqualTo(1));
            Assert.That(organizationRepository.LastCreatedUser, Is.SameAs(user));
            Assert.That(repositoryManager.SaveChangesCallCount, Is.EqualTo(0));
            Assert.That(result.UserId, Is.EqualTo(user.Id));
        });
    }

    [Test]
    public async Task Create_ShouldPassCancellationTokenToDependencies()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var user = CreateUser();
        var organizationRepository = new FakeOrganizationRepository([]);
        var repositoryManager = new FakeRepositoryManager(organizationRepository);
        var service = new OrganizationService(repositoryManager);

        await service.Create(new CreateOrganizationDto { Name = "Acme" }, user, cancellationTokenSource.Token);
        await service.Create(user, cancellationTokenSource.Token);

        Assert.Multiple(() =>
        {
            Assert.That(organizationRepository.LastNameExistsCancellationToken, Is.EqualTo(cancellationTokenSource.Token));
            Assert.That(organizationRepository.LastCreateWithDtoCancellationToken, Is.EqualTo(cancellationTokenSource.Token));
            Assert.That(organizationRepository.LastCreateWithUserCancellationToken, Is.EqualTo(cancellationTokenSource.Token));
            Assert.That(repositoryManager.LastSaveChangesCancellationToken, Is.EqualTo(default(CancellationToken)));
        });
    }

    private static User CreateUser()
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Login = "jdoe",
            Email = "jdoe@example.com",
            Password = "hashed-password",
            StatusId = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    private sealed class FakeOrganizationRepository(IEnumerable<Organization> organizations) : IOrganizationRepository
    {
        private readonly List<Organization> _organizations = organizations.ToList();

        public int GetAllCallCount { get; private set; }
        public int NameExistsCallCount { get; private set; }
        public int CreateWithDtoCallCount { get; private set; }
        public int CreateWithUserCallCount { get; private set; }
        public bool NameExistsResult { get; init; }
        public User? LastCreatedUser { get; private set; }

        public CancellationToken LastCancellationToken { get; private set; }
        public CancellationToken LastNameExistsCancellationToken { get; private set; }
        public CancellationToken LastCreateWithDtoCancellationToken { get; private set; }
        public CancellationToken LastCreateWithUserCancellationToken { get; private set; }

        public Task<List<Organization>> GetAll(CancellationToken cancellationToken = default)
        {
            GetAllCallCount++;
            LastCancellationToken = cancellationToken;
            return Task.FromResult(_organizations);
        }

        public Task<Organization> GetById(Guid id, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<bool> NameExists(string name, CancellationToken cancellationToken = default)
        {
            NameExistsCallCount++;
            LastNameExistsCancellationToken = cancellationToken;
            return Task.FromResult(NameExistsResult);
        }

        public Task<Organization> Create(string name, CancellationToken cancellationToken = default)
        {
            CreateWithDtoCallCount++;
            LastCreateWithDtoCancellationToken = cancellationToken;

            return Task.FromResult(new Organization
            {
                Id = Guid.NewGuid(),
                Name = name,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        public Task<Organization> Create(User user, CancellationToken cancellationToken = default)
        {
            CreateWithUserCallCount++;
            LastCreatedUser = user;
            LastCreateWithUserCancellationToken = cancellationToken;

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

    private sealed class FakeRepositoryManager(IOrganizationRepository organizationRepository) : IRepositoryManager
    {
        public int SaveChangesCallCount { get; private set; }
        public CancellationToken LastSaveChangesCancellationToken { get; private set; }

        public IOrganizationRepository OrganizationRepository { get; } = organizationRepository;
        public IPermissionRepository PermissionRepository => throw new NotSupportedException();
        public IRoleRepository RoleRepository => throw new NotSupportedException();
        public IUserRepository UserRepository => throw new NotSupportedException();
        public IStatusRepository StatusRepository => throw new NotSupportedException();
        
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCallCount++;
            LastSaveChangesCancellationToken = cancellationToken;
            return Task.FromResult(1);
        }
    }
}
