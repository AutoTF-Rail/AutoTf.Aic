using Microsoft.AspNetCore.Mvc;

namespace AutoTf.Aic.Controllers;

/// <summary>
/// The location marker on the EbuLa is always automatically scanned. However this is automatically being turned off when the endpoint to scan the current page is being run.
/// </summary>
[ApiController]
[Route("/timetable")]
public class TimetableController : ControllerBase
{
    
}