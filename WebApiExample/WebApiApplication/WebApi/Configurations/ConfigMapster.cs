using Mapster;
using WebApiApplication.Models;

namespace WebApi.Configurations;

internal class ConfigMapster : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<WebApiDatabase.Models.User, User>().TwoWays();
        config.NewConfig<WebApiDatabase.Models.User, UserWithPassword>().TwoWays();
    }
}
