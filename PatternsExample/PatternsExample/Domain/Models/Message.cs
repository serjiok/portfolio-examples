namespace PatternsExample.Domain.Models;

internal struct Message<T> where T: notnull
{
    public required T Value { get; set; }
}
