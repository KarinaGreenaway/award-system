using System.ComponentModel.DataAnnotations;

namespace AwardSystemAPI.Application.DTOs;

public class NomineeSummaryCreateDto
{
    public int? NomineeId { get; set; }
    
    
    public int? TeamNominationId { get; set; }
    
    [Required]
    public int CategoryId { get; set; }
    
    [Required]
    public string Location { get; set; } = string.Empty;
    
    [Required]
    public int? TotalNominations { get; set; }
    
    [Required]
    public bool IsPinned { get; set; }
    
    [Required]
    public bool IsShortlisted { get; set; }
    
    [Required]
    public bool IsWinner { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class NomineeSummaryUpdateDto
{
    public int? NomineeId { get; set; }
    
    public int? TeamNominationId { get; set; }
    
    [Required]
    public int CategoryId { get; set; }
    
    [Required]
    public string Location { get; set; } = string.Empty;
    
    [Required]
    public int? TotalNominations { get; set; }
    
    [Required]
    public bool IsPinned { get; set; }
    
    [Required]
    public bool IsShortlisted { get; set; }
    
    [Required]
    public bool IsWinner { get; set; }
}

public class NomineeSummaryResponseDto
{
    public int Id { get; set; }
    
    public int? NomineeId { get; set; }
    
    public int? TeamNominationId { get; set; }
    
    public int CategoryId { get; set; }
    
    public string Location { get; set; } = string.Empty;
    
    public int? TotalNominations { get; set; }
    
    public bool IsPinned { get; set; }
    
    public bool IsShortlisted { get; set; }
    
    public bool IsWinner { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
}

public class NomineeSummaryWithDetailedDto
{
    public int Id { get; set; }
    public int? NomineeId { get; set; }
    public string? NomineeName { get; set; }
    
    public int? TeamNominationId { get; set; }
    public string? TeamName { get; set; }
    public string Location { get; set; } = string.Empty;
    
    public int CategoryId { get; set; }
    public int? TotalNominations { get; set; }
    public bool IsPinned { get; set; }
    public bool IsShortlisted { get; set; }
    public bool IsWinner { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
