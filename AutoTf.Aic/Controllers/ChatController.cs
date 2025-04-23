using System.ComponentModel.DataAnnotations;
using AutoTf.Aic.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoTf.Aic.Controllers;

[ApiController]
[Route("/chat")]
public class ChatController : ControllerBase
{
    private readonly OllamaService _ollama;

    public ChatController(OllamaService ollama)
    {
        _ollama = ollama;
    }

    [HttpGet("response")]
    public async Task<ActionResult<string>> GetResponse([FromQuery, Required] string message, [FromQuery, Required] bool keep)
    {
        try
        {
            return await _ollama.GetResponse(message, keep);
        }
        catch (Exception e)
        {
            Console.WriteLine("An error occured while trying to get a response:");
            Console.WriteLine(e.ToString());
            
            return BadRequest("An error occured while trying to get a response.");
        }
    }
}