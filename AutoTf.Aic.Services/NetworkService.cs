using AutoTf.Aic.Models;
using AutoTf.Logging;
using Microsoft.Extensions.Hosting;

namespace AutoTf.Aic.Services;

public class NetworkService : IHostedService
{
    private readonly Logger _logger;

    public NetworkService(Logger logger)
    {
        _logger = logger;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Configure();
    }

    private async Task Configure()
    {
        try
        {
            _logger.Log("Configuring network");
        
            NetworkConfigurator.SetStaticIpAddress("192.168.0.3", "24");
        
            _logger.Log("Set local IP to 192.168.0.1/24.");

            if (await HttpHelper.SendGetString("192.168.0.1/information/trainId", false) == "")
                _logger.Log("Verbose: Central bridge was not detected at 192.168.0.1");
        }
        catch (Exception e)
        {
            _logger.Log("An error occured while configuring the network service:");
            _logger.Log(e.ToString());
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}