namespace WebApi.Configurations;

internal static class ConfigJwtExt
{
    extension (IConfiguration configuration)
    {
        public string ISSUER    => configuration["Jwt:ISSUER"]    ?? throw new NullReferenceException("ISSUER not found");
        public string AUDIENCE  => configuration["Jwt:AUDIENCE"]  ?? throw new NullReferenceException("AUDIENCE not found");
        public string SECRETKEY => configuration["Jwt:SECRETKEY"] ?? throw new NullReferenceException("SECRETKEY not found");
    }
}
