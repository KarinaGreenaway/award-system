using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AwardSystemAPI.Domain.Entities;

// create table users
// (
//     "Id"          serial
// primary key,
// "ExternalId"  varchar(255) not null
// unique,
// "WorkEmail"   varchar(255) not null
// unique,
// "Role"        varchar(50)  not null,
// "CreatedAt"   timestamp default now(),
// "UpdatedAt"   timestamp default now(),
// "DisplayName" varchar(255),
// "FirstName"   varchar(100),
// "LastName"    varchar(100)
//     );

[Table("users")]
public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = null!;

    [MaxLength(15)]
    public string? PhoneNumber { get; set; }
}