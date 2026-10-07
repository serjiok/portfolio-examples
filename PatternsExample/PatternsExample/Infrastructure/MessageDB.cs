using System.ComponentModel.DataAnnotations;

namespace PatternsExample.Infrastructure;

internal sealed class MessageDB
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Text { get; set; } = string.Empty;
}
