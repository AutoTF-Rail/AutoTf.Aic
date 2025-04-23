using AutoTf.Aic.Models;
using Microsoft.AspNetCore.Mvc;

namespace AutoTf.Aic.Controllers;

[ApiController]
[Route("/system")]
public class SystemController : ControllerBase
{
    [HttpGet("version")]
    public ActionResult<string> Version()
    {
        return Statics.GetGitVersion();
    }
}