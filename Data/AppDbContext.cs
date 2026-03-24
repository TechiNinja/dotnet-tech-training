using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data.Entities;
using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SportsManagementApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Sport> Sports { get; set; }
        public DbSet<EventRequest> EventRequests { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventCategory> EventCategories { get; set; }
        public DbSet<ParticipantRegistration> ParticipantRegistrations { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<MatchSet> MatchSets { get; set; }
        public DbSet<Result> Results { get; set; }

        public DbSet<Notification> Notifications {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Event>()
    .Property(e => e.EndDate)
    .HasConversion(
        v => v.ToDateTime(TimeOnly.MinValue),   // DateOnly → DateTime
        v => DateOnly.FromDateTime(v)           // DateTime → DateOnly
    );

    modelBuilder.Entity<Event>()
    .Property(e => e.StartDate)
    .HasConversion(
        v => v.ToDateTime(TimeOnly.MinValue),
        v => DateOnly.FromDateTime(v)
    );

    modelBuilder.Entity<Event>()
    .Property(e => e.RegistrationDeadline)
    .HasConversion(
        v => v.ToDateTime(TimeOnly.MinValue),
        v => DateOnly.FromDateTime(v)
    );

    modelBuilder.Entity<EventRequest>()
    .Property(e => e.EndDate)
    .HasConversion(
        v => v.ToDateTime(TimeOnly.MinValue),
        v => DateOnly.FromDateTime(v)
    );

    modelBuilder.Entity<EventRequest>()
    .Property(e => e.StartDate)
    .HasConversion(
        v => v.ToDateTime(TimeOnly.MinValue),
        v => DateOnly.FromDateTime(v)
    );

            modelBuilder.Entity<TeamMember>()
                .HasOne(member => member.User)
                .WithMany(user => user.TeamMembers)
                .HasForeignKey(member => member.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TeamMember>()
                .HasOne(member => member.Team)
                .WithMany(team => team.Members)
                .HasForeignKey(member => member.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Match>()
                .HasOne(match => match.Result)
                .WithOne(result => result.Match)
                .HasForeignKey<Result>(result => result.MatchId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ParticipantRegistration>()
                .HasIndex(result => new { result.UserId, result.EventCategoryId })
                .IsUnique();

            modelBuilder.Entity<ParticipantRegistration>()
                .HasOne(registration => registration.User)
                .WithMany(user => user.Registrations)
                .HasForeignKey(registration => registration.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ParticipantRegistration>()
                .HasOne(registration => registration.EventCategory)
                .WithMany(category => category.EventRegistrations)
                .HasForeignKey(registration => registration.EventCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventCategory>()
                .HasOne(category => category.Event)
                .WithMany(events => events.Categories)
                .HasForeignKey(category => category.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventRequest>()
                .HasOne(request => request.OperationsReviewer)
                .WithMany()
                .HasForeignKey(request => request.OperationsReviewerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
                .HasMany(match => match.MatchSets)
                .WithOne(set => set.Match)
                .HasForeignKey(set => set.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

                modelBuilder.Entity<Event>()
    .HasOne(e => e.Sport)
    .WithMany()
    .HasForeignKey(e => e.SportId)
    .OnDelete(DeleteBehavior.Restrict);

modelBuilder.Entity<Event>()
    .HasOne(e => e.Organizer)
    .WithMany()
    .HasForeignKey(e => e.OrganizerId)
    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Event>()
                .HasIndex(e => e.EventRequestId)
                .IsUnique();
            modelBuilder.Entity<Sport>()
    .Property(s => s.AllowedFormats)
    .HasConversion(
        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
    );    
        }
    }
}