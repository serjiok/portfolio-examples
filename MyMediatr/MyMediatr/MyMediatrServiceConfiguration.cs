using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MyMediatr;

public sealed class MyMediatrServiceConfiguration
{
    private List<Assembly> _assemblies = [];

    public IEnumerable<Assembly> Assemblies => _assemblies;

    public ServiceLifetime LifeTime { get; set; } = ServiceLifetime.Transient;

    public void RegisterServicesFromAssembly(Assembly assembly) => _assemblies.Add(assembly);

    public void RegisterServicesFromAssembly(Assembly[] assemblies) => _assemblies.AddRange(assemblies);
}
