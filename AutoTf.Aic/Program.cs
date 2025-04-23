using AutoTf.Aic.Extensions;
using AutoTf.Aic.Models;
using AutoTf.Aic.Services;
using AutoTf.Logging;

namespace AutoTf.Aic;

public static class Program
{
    private static readonly Logger Logger = Statics.Logger;
    
    public static void Main(string[] args)
    {
        try
        {
            Logger.Log($"Assembling at {DateTime.Now:hh:mm:ss}.");
            
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            ConfigureServices(builder);

            WebApplication app = builder.Build();
			
            app.MapControllers();

            app.UseWebSockets();

            app.Run("http://0.0.0.0:80");
        }
        catch (Exception e)
        {
            Statics.Logger.Log("Root Error:");
            Statics.Logger.Log(e.ToString());
        }
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        
        builder.Services.AddSingleton(Logger);
        
        builder.Services.AddHostedService<NetworkService>();
        
        builder.Services.AddHostedSingleton<OllamaService>();
        
        builder.Services.Configure<HostOptions>(x => x.ShutdownTimeout = TimeSpan.FromSeconds(20));
    }
}