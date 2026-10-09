using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Services.Errors;

[Route("ErrorHandling")]
[ApiController]
[AllowAnonymous]
[ApiExplorerSettings(IgnoreApi = true)]
public sealed class ErrorHandlingController(ILogger<ErrorHandlingController>? logger) : ControllerBase
{
    [Route(nameof(ProcessError))]
    public IActionResult ProcessError(IHostEnvironment hostEnvironment)
    {
        var features = HttpContext.Features.Get<IExceptionHandlerFeature>();
        if (features == null) return Problem("Unknow exception");

        logger?.LogError("{source} {error}", features.Endpoint?.DisplayName,features.Error.StackTrace ?? features.Error.Message);
        
        return hostEnvironment.IsDevelopment()?
               Problem(detail: features.Error.StackTrace, title: features.Error.Message, instance: hostEnvironment.EnvironmentName):
               Problem();
    }
}
