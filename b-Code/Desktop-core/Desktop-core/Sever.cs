using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Runtime.InteropServices;

namespace WallpaperService
{
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;

        static void Main(string[] args)
        {
          
            IHost host = Host.CreateDefaultBuilder(args)
                .UseWindowsService(options =>
                {
                    options.ServiceName = "WallpaperChangerService";
                })
                .ConfigureServices(services =>
                {
                    services.AddHostedService<WallpaperWorker>();
                })
                .Build();

            host.Run();
        }
    }
}