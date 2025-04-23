using System.Timers;
using AutoTf.Aic.Models;
using AutoTf.Logging;
using Microsoft.Extensions.Hosting;
using Timer = System.Timers.Timer;

namespace AutoTf.Aic.Services;

public class NetworkService : IHostedService
{
    private readonly Logger _logger;
    private readonly Timer _availabilityTimer = new Timer(15000);

    public NetworkService(Logger logger)
    {
        _logger = logger;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Configure();
        StartCentralBridgeTimer();
        
        return Task.CompletedTask;
    }

    private void StartCentralBridgeTimer()
    {
        // Call it once on startup.
        CentralBridgeTimerElapsed(null, null!);
        _logger.Log($"Verbose: Central bridge online state has changed to: Online: {Statics.IsCentralBridgeAvailable}.");
        
        _availabilityTimer.Elapsed += CentralBridgeTimerElapsed;
        _availabilityTimer.Start();
    }

    private async void CentralBridgeTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        bool newFoundState = await HttpHelper.SendGetString("http://192.168.0.1/information/trainId", false) != "";
        
        if (Statics.IsCentralBridgeAvailable != newFoundState)
        {
            _logger.Log($"Verbose: Central bridge online state has changed to: Online: {newFoundState}.");
        }

        Statics.IsCentralBridgeAvailable = newFoundState;
    }

    private void Configure()
    {
        try
        {
            _logger.Log("Configuring network");
        
            NetworkConfigurator.SetStaticIpAddress("192.168.0.3", "24");
        
            _logger.Log("Set local IP to 192.168.0.3/24.");
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