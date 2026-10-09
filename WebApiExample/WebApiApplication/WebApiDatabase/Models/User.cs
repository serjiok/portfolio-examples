using System.ComponentModel.DataAnnotations;

namespace WebApiDatabase.Models;

public sealed class User
{
    [Key]
    public int Id { get; set; }
    [Required]
    public required string Login { get; set; }
    [Required]
    public required string Password { get; set; }
}
