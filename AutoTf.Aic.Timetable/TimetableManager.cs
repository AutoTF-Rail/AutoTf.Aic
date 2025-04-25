using AutoTf.Aic.Models;
using AutoTf.Aic.Models.Interfaces;

namespace AutoTf.Aic.Timetable;

public class TimetableManager : ITimetableManager
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // TODO: Wait for and hook into timetable stream.
        // TODO: Read location marker (If allowed)
        // TODO: Add method to turn off/on automatic location reading
        return Task.CompletedTask;
    }

    private async Task WaitForTimetable()
    {
        HttpHelper.SendGet<bool>("http://192.168.0.1/display/isDisplayRegistered")
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // TODO: Hook out of timetable stream.
        return Task.CompletedTask;
    }
}