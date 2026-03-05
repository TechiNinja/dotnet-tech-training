using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Entities;

namespace SportsManagementApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User>                    Users                    { get; set; }
        public DbSet<Role>                    Roles                    { get; set; }
        public DbSet<Sport>                   Sports                   { get; set; }
        public DbSet<EventRequest>            EventRequests            { get; set; }
        public DbSet<Event>                   Events                   { get; set; }
        public DbSet<EventCategory>           EventCategories          { get; set; }
        public DbSet<ParticipantRegistration> ParticipantRegistrations { get; set; }
        public DbSet<Team>                    Teams                    { get; set; }
        public DbSet<TeamMember>              TeamMembers              { get; set; }
        public DbSet<Match>                   Matches                  { get; set; }
        public DbSet<MatchSet>                MatchSets                { get; set; }
        public DbSet<Result>                  Results                  { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TeamMember>()
                .HasOne(m => m.User)
                .WithMany(u => u.TeamMembers)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TeamMember>()
                .HasOne(m => m.Team)
                .WithMany(t => t.Members)
                .HasForeignKey(m => m.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.Result)
                .WithOne(r => r.Match)
                .HasForeignKey<Result>(r => r.MatchId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ParticipantRegistration>()
                .HasIndex(r => new { r.UserId, r.EventCategoryId })
                .IsUnique();

            modelBuilder.Entity<ParticipantRegistration>()
                .HasOne(r => r.User)
                .WithMany(u => u.Registrations)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ParticipantRegistration>()
                .HasOne(r => r.EventCategory)
                .WithMany(c => c.EventRegistrations)
                .HasForeignKey(r => r.EventCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventCategory>()
                .HasOne(c => c.Event)
                .WithMany(e => e.Categories)
                .HasForeignKey(c => c.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventRequest>()
                .HasOne(r => r.OperationsReviewer)
                .WithMany()
                .HasForeignKey(r => r.OperationsReviewerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
                .HasMany(m => m.MatchSets)
                .WithOne(s => s.Match)
                .HasForeignKey(s => s.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.EventRequest)
                .WithOne(r => r.Event)
                .HasForeignKey<Event>(e => e.EventRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Event>()
                .HasIndex(e => e.EventRequestId)
                .IsUnique();
        }
    }
}
