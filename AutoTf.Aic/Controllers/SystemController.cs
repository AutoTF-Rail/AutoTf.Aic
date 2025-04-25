using AutoTf.Aic.Extensions;
using AutoTf.Aic.Models;
using AutoTf.Aic.Models.Static;
using AutoTf.Logging;
using Microsoft.AspNetCore.Mvc;

namespace AutoTf.Aic.Controllers;

[ApiController]
[Route("/system")]
public class SystemController : ControllerBase
{
    private readonly Logger _logger;
    private readonly IHostApplicationLifetime _lifetime;

    public SystemController(Logger logger, IHostApplicationLifetime lifetime)
    {
        _logger = logger;
        _lifetime = lifetime;
    }

    /// <summary>
    /// Endpoint to check if the AIC is online
    /// </summary>
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(true);
    }
    
    [HttpGet("available")]
    public ActionResult<bool?> Available()
    {
        return Statics.IsCentralBridgeAvailable;
    }
    
    [HttpGet("version")]
    public ActionResult<string> Version()
    {
        return Statics.GetGitVersion();
    }

    [LocalAuth]
    [HttpPost("shutdown")]
    public IActionResult Shutdown()
    {
        _logger.Log("Shutdown was requested.");
		
        _lifetime.StopApplication();
        CommandExecuter.ExecuteSilent("bash -c \"sleep 30; shutdown -h now\"&", true);
        return Ok();
    }

    [LocalAuth]
    [HttpPost("restart")]
    public IActionResult Restart()
    {
        _logger.Log("Restart was requested.");
        
        _lifetime.StopApplication();
        CommandExecuter.ExecuteSilent("bash -c \"sleep 30; reboot now\"&", true);
        return Ok();
    }
}