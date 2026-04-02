using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TaskForge.Domain.Entities;
using TaskEntity = TaskForge.Domain.Entities.Task;
using TaskStatusEntity = TaskForge.Domain.Entities.TaskStatus;

namespace TaskForge.Tests.Persistence;

internal sealed class TaskForgeDbContext(DbContextOptions<TaskForgeDbContext> options) : DbContext(options)
{
    public DbSet<Project> Projects => Set<Project>();

    public DbSet<TaskStatusEntity> TaskStatuses => Set<TaskStatusEntity>();

    public DbSet<TaskPriority> TaskPriorities => Set<TaskPriority>();

    public DbSet<TaskEntity> Tasks => Set<TaskEntity>();

    public DbSet<Epic> Epics => Set<Epic>();

    public DbSet<Sprint> Sprints => Set<Sprint>();

    public DbSet<TaskLabel> TaskLabels => Set<TaskLabel>();

    public DbSet<TaskUser> TaskUsers => Set<TaskUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var persistenceAssembly = Assembly.Load("TaskForge.Persistence");
        modelBuilder.ApplyConfigurationsFromAssembly(persistenceAssembly);
    }
}
