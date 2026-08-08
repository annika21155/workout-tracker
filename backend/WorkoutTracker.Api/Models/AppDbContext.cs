using Microsoft.EntityFrameworkCore;

namespace WorkoutTracker.Api.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Challenge> Challenges => Set<Challenge>();
    public DbSet<ChallengeDay> ChallengeDays => Set<ChallengeDay>();
    public DbSet<ChallengeParticipant> ChallengeParticipants => Set<ChallengeParticipant>();
    public DbSet<DayCompletion> DayCompletions => Set<DayCompletion>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<Friendship> Friendships => Set<Friendship>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Challenge -> ChallengeDay (cascade)
        modelBuilder.Entity<ChallengeDay>()
            .HasOne(cd => cd.Challenge)
            .WithMany(c => c.Days)
            .HasForeignKey(cd => cd.ChallengeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Challenge -> ChallengeParticipant (cascade)
        modelBuilder.Entity<ChallengeParticipant>()
            .HasOne(cp => cp.Challenge)
            .WithMany(c => c.Participants)
            .HasForeignKey(cp => cp.ChallengeId)
            .OnDelete(DeleteBehavior.Cascade);

        // User -> ChallengeParticipant (cascade)
        modelBuilder.Entity<ChallengeParticipant>()
            .HasOne(cp => cp.User)
            .WithMany(u => u.ChallengeParticipations)
            .HasForeignKey(cp => cp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // ChallengeParticipant -> DayCompletion (cascade)
        modelBuilder.Entity<DayCompletion>()
            .HasOne(dc => dc.ChallengeParticipant)
            .WithMany(cp => cp.Completions)
            .HasForeignKey(dc => dc.ChallengeParticipantId)
            .OnDelete(DeleteBehavior.Cascade);

        // ChallengeDay -> DayCompletion (cascade)
        modelBuilder.Entity<DayCompletion>()
            .HasOne(dc => dc.ChallengeDay)
            .WithMany(cd => cd.Completions)
            .HasForeignKey(dc => dc.ChallengeDayId)
            .OnDelete(DeleteBehavior.Cascade);

        // User -> ActivityLog (cascade)
        modelBuilder.Entity<ActivityLog>()
            .HasOne(a => a.User)
            .WithMany(u => u.ActivityLogs)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // User -> Challenge (created by) — restrict, don't cascade-delete challenges if creator deleted
        modelBuilder.Entity<Challenge>()
            .HasOne(c => c.CreatedByUser)
            .WithMany(u => u.CreatedChallenges)
            .HasForeignKey(c => c.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Friendship: two FKs to User — both must be Restrict to avoid
        // multiple cascade paths (SQL Server/Postgres will reject cyclic cascades here)
        modelBuilder.Entity<Friendship>()
            .HasOne(f => f.User)
            .WithMany(u => u.FriendshipsInitiated)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Friendship>()
            .HasOne(f => f.FriendUser)
            .WithMany(u => u.FriendshipsReceived)
            .HasForeignKey(f => f.FriendUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}