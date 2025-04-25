using AutoTf.Aic.Models;
using AutoTf.Aic.Models.Interfaces;
using AutoTf.CentralBridge.Shared.Models.Enums;
using AutoTf.Logging;

namespace AutoTf.Aic.Timetable;

public class TimetableManager : ITimetableManager
{
    private readonly Logger _logger;

    public TimetableManager(Logger logger)
    {
        _logger = logger;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // TODO: Wait for and hook into timetable stream.
        // TODO: Read location marker (If allowed)
        // TODO: Add method to turn off/on automatic location reading
        
        Task.Run(Initialize, cancellationToken);
        return Task.CompletedTask;
    }

    private async Task Initialize()
    {
        await WaitForTimetable();
    }

    // Usually this should always be done to be on the safe side incase the central bridge goes offline, but if that happens the AIC can just restart and then it should work again.
    private async Task WaitForTimetable()
    {
        string requestPath = $"http://192.168.0.1/display/isDisplayRegistered?type={DisplayType.EbuLa}";
        
        bool isCamAvailable = await HttpHelper.SendGet<bool>(requestPath);
        int retryCount = 0;

        while (!isCamAvailable && retryCount < 10)
        {
            await Task.Delay(1500);
            retryCount++;
            isCamAvailable = await HttpHelper.SendGet<bool>(requestPath);
        }

        // Manual detection via endpoints will be possible, but the Aic won't read the location automatically unless we tell it to manually.
        // TODO: Maybe just tell the AIC "start now"?
        if (retryCount == 10)
        {
            _logger.Log("Failed to wait for EbuLa display after 10 tries.");
        }
        
        // TODO: Start reading? Or only start reading after the central bridge has ensured that the first page is visible?
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // TODO: Hook out of timetable stream.
        return Task.CompletedTask;
    }
}