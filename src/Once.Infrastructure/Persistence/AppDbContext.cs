using Once.Domain.Entities;
using Once.Domain.Entities.Common;
using Once.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Once.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext() { }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User>           Users           { get; set; }
    public DbSet<RefreshToken>   RefreshTokens   { get; set; }
    public DbSet<AiConversation> AiConversations { get; set; }
    public DbSet<FraudScenario> FraudScenarios { get; set; }
    public DbSet<FraudAttempt>  FraudAttempts  { get; set; }
    public DbSet<Branch>        Branches       { get; set; }
    public DbSet<Position>      Positions      { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        #region Configurations related to MultiLanguageField

        var mlfs = GetType().GetProperties()
            .Where(x => x.PropertyType.IsGenericType)
            .Where(x => x.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
            .Select(x => x.PropertyType.GetGenericArguments().FirstOrDefault())
            .SelectMany(x => x!.GetProperties())
            .Where(x => x.PropertyType == typeof(MultiLanguageField));

        foreach (var multiLanguageField in mlfs)
            modelBuilder
                .Entity(multiLanguageField.ReflectedType!)
                .Property(multiLanguageField.PropertyType, multiLanguageField.Name)
                .HasColumnType("jsonb");

        #endregion

        #region Reference based models configurations

        var helpers = modelBuilder
            .Model
            .GetEntityTypes()
            .Where(x => x.ClrType.BaseType is not null && x.ClrType.BaseType!.IsGenericType &&
                        x.ClrType.BaseType?.GetGenericTypeDefinition() == typeof(ReferenceModelBase<>));

        foreach (var helperType in helpers)
        {
            modelBuilder
                .Entity(helperType.ClrType)
                .HasIndex(nameof(ReferenceModelBase<long>.Id))
                .IsUnique();

            modelBuilder
                .Entity(helperType.ClrType)
                .HasKey(nameof(ReferenceModelBase<long>.Id));
        }

        #endregion

        #region FraudAttempt configuration

        modelBuilder.Entity<FraudAttempt>()
            .HasOne(a => a.Scenario)
            .WithMany(s => s.Attempts)
            .HasForeignKey(a => a.ScenarioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FraudAttempt>()
            .HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        #endregion

        #region Branch + Position configuration

        modelBuilder.Entity<Branch>()
            .HasIndex(b => b.Code)
            .IsUnique();

        modelBuilder.Entity<Position>()
            .HasIndex(p => p.Code)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasOne(u => u.Branch)
            .WithMany(b => b.Users)
            .HasForeignKey(u => u.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Position)
            .WithMany(p => p.Users)
            .HasForeignKey(u => u.PositionId)
            .OnDelete(DeleteBehavior.Restrict);

        #endregion

        #region User + RefreshToken configuration

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<RefreshToken>()
            .HasIndex(rt => rt.Token)
            .IsUnique();

        modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        #endregion

        #region AiConversation configuration

        modelBuilder.Entity<AiConversation>()
            .HasIndex(c => c.ConversationId)
            .IsUnique();

        modelBuilder.Entity<AiConversation>()
            .HasOne(c => c.Owner)
            .WithMany()
            .HasForeignKey(c => c.OwnerUserId)
            .OnDelete(DeleteBehavior.Cascade);

        #endregion
    }

    private void TrackActionsAt()
    {
        var currentDateTime = DateTime.UtcNow;

        foreach (var entity in ChangeTracker.Entries().Where(x => x.State is EntityState.Added or EntityState.Modified))
        {
            if (entity.State is EntityState.Added)
            {
                entity.Entity.GetType().GetProperty("CreatedAt")?.SetValue(entity.Entity, currentDateTime);
            }

            if (entity.State is EntityState.Modified or EntityState.Added)
            {
                entity.Entity.GetType().GetProperty("UpdatedAt")?.SetValue(entity.Entity, currentDateTime);
            }
        }
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        TrackActionsAt();

        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        TrackActionsAt();

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}
