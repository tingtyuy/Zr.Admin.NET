using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ZR.ServiceCore.Services
{
    /// <summary>
    /// AI任务超时检测后台服务
    /// </summary>
    public class AiTaskTimeoutService : BackgroundService
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IServiceProvider _serviceProvider;

        public AiTaskTimeoutService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.Info("AI任务超时检测服务启动");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // 每30秒检测一次
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

                    using var scope = _serviceProvider.CreateScope();
                    var aiTaskService = scope.ServiceProvider.GetRequiredService<IAiTaskService>();

                    var timeoutStr = AppSettings.GetConfig("AiTask:TimeoutMinutes");
                    int timeoutMinutes = string.IsNullOrEmpty(timeoutStr) ? 5 : int.Parse(timeoutStr);

                    aiTaskService.CheckTimeout(timeoutMinutes);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "AI任务超时检测异常");
                }
            }

            logger.Info("AI任务超时检测服务停止");
        }
    }
}
