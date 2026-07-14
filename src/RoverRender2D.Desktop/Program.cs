using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RoverRender2D.Application;
using RoverRender2D.Application.Abstractions;
using RoverRender2D.Infrastructure.Time;

namespace RoverRender2D.Desktop;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();

        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
        builder.Services.Configure<RoverRenderOptions>(options =>
        {
            options.ApplicationName = "RoverRender2D";
            options.WorkspacePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "RoverRender2D");
        });
        builder.Services.AddSingleton<IClock, SystemClock>();
        builder.Services.AddSingleton<MainForm>();

        using IHost host = builder.Build();
        host.StartAsync().GetAwaiter().GetResult();

        try
        {
            System.Windows.Forms.Application.Run(host.Services.GetRequiredService<MainForm>());
        }
        finally
        {
            host.StopAsync(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
        }
    }
}
