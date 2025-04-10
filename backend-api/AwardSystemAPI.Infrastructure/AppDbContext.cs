using AwardSystemAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AwardSystemAPI.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<MobileUserSettings> MobileUserSettings { get; set; }
    public DbSet<AwardCategory> AwardCategories { get; set; }
    public DbSet<NomineeSummary> NomineeSummaries { get; set; }
    public DbSet<Nomination> Nominations { get; set; }
    public DbSet<TeamMember> TeamMembers { get; set; }
    public DbSet<NominationQuestion> NominationQuestions { get; set; }
    public DbSet<NominationAnswer> NominationAnswers { get; set; }
    public DbSet<AwardEvent> AwardEvents { get; set; }
    public DbSet<Rsvp> Rsvps { get; set; }
    public DbSet<RsvpFormQuestion> RsvpFormQuestions { get; set; }
    public DbSet<RsvpResponse> RsvpResponses { get; set; }
    public DbSet<Feedback> Feedback { get; set; }
    public DbSet<FeedbackFormQuestion> FeedbackFormQuestions { get; set; }
    public DbSet<FeedbackResponse> FeedbackResponses { get; set; }
    public DbSet<Announcement> Announcements { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<AwardProcess> AwardProcesses { get; set; }
    public DbSet<JudgingRound> JudgingRounds { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AwardProcess>()
            .Property(a => a.StartDate)
            .HasConversion(
                v => v, 
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
            );
    
        modelBuilder.Entity<AwardProcess>()
            .Property(a => a.EndDate)
            .HasConversion(
                v => v, 
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : (DateTime?)null
            );
    
        modelBuilder.Entity<AwardProcess>()
            .Property(a => a.CreatedAt)
            .HasConversion(
                v => v, 
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
            );
    
        modelBuilder.Entity<AwardProcess>()
            .Property(a => a.UpdatedAt)
            .HasConversion(
                v => v, 
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
            );
        
        modelBuilder.Entity<AwardCategory>()
            .Property(a => a.CreatedAt)
            .HasConversion(
                v => v, 
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
            );
        
        modelBuilder.Entity<AwardCategory>()
            .Property(a => a.UpdatedAt)
            .HasConversion(
                v => v, 
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
            );
        
        modelBuilder.Entity<Notification>()
            .Property(a => a.CreatedAt)
            .HasConversion(
                v => v, 
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
            );
    
        base.OnModelCreating(modelBuilder);
    }

}
