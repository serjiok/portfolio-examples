namespace WebApiApplication.Models;

public class Filter
{
    public string? Property { get; init; }
    public bool? Asc { get; init; }
    public int? Skip { get; init; }
    public int? Take { get; init; }
    public string? Search { get; init; }
}
