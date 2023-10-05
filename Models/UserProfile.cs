using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace HouseRules.Models;

public class UserProfile
{
    public int Id { get; set; }
    // [Required]
    public string FirstName { get; set; }
    // [Required]
    public string LastName { get; set; }
    // [Required]
    public string Address { get; set; }

    // [Required]
    [NotMapped] // not mapped means that EF Core won't create column for this property in the db
    public string UserName { get; set; }

    public IdentityUser IdentityUser { get; set; }
    // [Required]
    public string IdentityUserId { get; set; }
    // [Required]
    [NotMapped] // not mapped means that EF Core won't create column for this property in the db
    public string Email { get; set; }
    public List<ChoreAssignment> ChoreAssignments { get; set; }
    public List<ChoreCompletion> ChoreCompletions { get; set; }

    [NotMapped]
    public List<string> Roles { get; set; }

}