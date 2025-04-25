using System.ComponentModel.DataAnnotations;
using AutoTf.Logging;
using Microsoft.AspNetCore.Mvc;

namespace AutoTf.Aic.Controllers;

[ApiController]
[Route("/information")]
public class InformationController : ControllerBase
{
    private readonly Logger _logger;
    private readonly string _logDir = "/var/log/AutoTF/AutoTf.Aic/";

    public InformationController(Logger logger)
    {
        _logger = logger;
    }

    [HttpGet("logDates")]
    public ActionResult<List<string?>> LogDates()
    {
        try
        {
            string[] files = Directory.GetFiles(_logDir).Order().ToArray();
            return files.Select(Path.GetFileNameWithoutExtension).ToList();
        }
        catch (Exception e)
        {
            _logger.Log("Could not get log dates:");
            _logger.Log(e.ToString());
            return BadRequest("Could not get log dates.");
        }
    }

    [HttpGet("logs")]
    public ActionResult<List<string>> Logs([FromQuery, Required] string date)
    {
        try
        {
            // TODO: We could also just return an empty array here if the file doesn't exist?
            return System.IO.File.ReadAllLines(_logDir + date + ".txt").ToList();
        }
        catch (Exception e)
        {
            _logger.Log($"Could not get logs for date {date}:");
            _logger.Log(e.ToString());
            return BadRequest($"Could not get logs for date {date}j.");
        }
    }
}