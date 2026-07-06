using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace WallpaperService
{
    public class WallpaperWorker : BackgroundService
    {
        private readonly ILogger<WallpaperWorker> _logger;
        private readonly string imageFolder;
        private int lastImageIndex = 0;

        private static readonly List<WallpaperSchedule> Schedule = new List<WallpaperSchedule>
        {
            new WallpaperSchedule(new TimeSpan(8, 0, 0),  1),
            new WallpaperSchedule(new TimeSpan(12, 0, 0), 2),
            new WallpaperSchedule(new TimeSpan(13, 30, 0),1),
            new WallpaperSchedule(new TimeSpan(16, 0, 0), 3),
            new WallpaperSchedule(new TimeSpan(18, 0, 0), 2),
            new WallpaperSchedule(new TimeSpan(20, 0, 0), 4),
            new WallpaperSchedule(new TimeSpan(23, 59, 59),5)
        };

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        private const int SPI_SETDESKWALLPAPER = 0x0014;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDWININICHANGE = 0x02;

        public WallpaperWorker(ILogger<WallpaperWorker> logger)
        {
            _logger = logger;
            imageFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Wallpapers");

            if (!Directory.Exists(imageFolder))
                Directory.CreateDirectory(imageFolder);

            _logger.LogInformation($"监控文件夹: {imageFolder}");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("壁纸服务已启动");

            // ================== 启动时立即更换一次 ==================
            _logger.LogInformation("=== 启动时立即尝试更换壁纸 ===");
            await ForceChangeWallpaperAtStartup();
            _logger.LogInformation("=== 启动更换执行完成 ===");
            // ======================================================

            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckAndChangeWallpaper();
                await Task.Delay(20000, stoppingToken);
            }
        }

        // 启动时强制更换（使用当前时间匹配的图片）
        private async Task ForceChangeWallpaperAtStartup()
        {
            try
            {
                DateTime now = DateTime.Now;
                var schedule = Schedule
                    .Where(s => s.Time <= now.TimeOfDay)
                    .OrderByDescending(s => s.Time)
                    .FirstOrDefault();

                int imageNumber = schedule?.ImageNumber ?? 1;   // 默认使用1号

                _logger.LogInformation($"启动时当前时间 {now:HH:mm:ss}，计划使用 {imageNumber} 号图片");

                await ChangeWallpaper(imageNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "启动时强制更换壁纸失败");
            }
        }

        private async Task CheckAndChangeWallpaper()
        {
            try
            {
                DateTime now = DateTime.Now;

                var todaySchedule = Schedule
                    .Where(s => s.Time <= now.TimeOfDay)
                    .OrderByDescending(s => s.Time)
                    .FirstOrDefault();

                if (todaySchedule != null && todaySchedule.ImageNumber != lastImageIndex)
                {
                    await ChangeWallpaper(todaySchedule.ImageNumber);
                    lastImageIndex = todaySchedule.ImageNumber;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "定时检查壁纸出错");
            }
        }

        private async Task ChangeWallpaper(int imageNumber)
        {
            try
            {
                string jpgPath = Path.Combine(imageFolder, $"{imageNumber}.jpg");
                string pngPath = Path.Combine(imageFolder, $"{imageNumber}.png");
                string imagePath = File.Exists(jpgPath) ? jpgPath : pngPath;

                if (!File.Exists(imagePath))
                {
                    _logger.LogWarning($"未找到 {imageNumber}.jpg 或 {imageNumber}.png");
                    return;
                }

                // 执行壁纸更换
                bool success = SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, imagePath,
                                                   SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE) != 0;

                if (success)
                    _logger.LogInformation($"✅ 壁纸更换成功 → {imageNumber}");
                else
                    _logger.LogWarning($"⚠️ SystemParametersInfo 调用失败 → {imageNumber}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"更换 {imageNumber} 号壁纸时发生异常");
            }

            // 解决 async 方法没有 await 的警告
            await Task.CompletedTask;
        }
    }

    public class WallpaperSchedule
    {
        public TimeSpan Time { get; }
        public int ImageNumber { get; }

        public WallpaperSchedule(TimeSpan time, int imageNumber)
        {
            Time = time;
            ImageNumber = imageNumber;
        }
    }
}