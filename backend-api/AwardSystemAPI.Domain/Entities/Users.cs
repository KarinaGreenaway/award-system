using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AwardSystemAPI.Domain.Entities;

[Table("users")]
public class Users
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string ExternalId { get; set; } = string.Empty;
    
    [Required]
    public string WorkEmail { get; set; } = string.Empty;
    
    [Required]
    public string Role { get; set; } = string.Empty;
    
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public string? DisplayName { get; set; } = string.Empty;
        
    public string? FirstName { get; set; } = string.Empty;
    
    public string? LastName { get; set; } = string.Empty;
    
    // Navigation
    public ICollection<NomineeSummary> NomineeSummaries { get; set; } = new List<NomineeSummary>();
    
}