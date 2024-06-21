// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Minio;
using SqlSugar.DistributedSystem.Snowflake;

namespace Admin.NET.Core;

/// <summary>
/// 清理日志作业任务
/// </summary>
[JobDetail("job_syncdata", Description = "同步数据测试", GroupName = "default", Concurrent = false)]
[Daily(TriggerId = "trigger_syncdata", Description = "同步数据测试", MaxNumberOfRuns = 1, RunOnStart = true)]
public class SyncJob : IJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger _logger;

    public SyncJob(IServiceScopeFactory scopeFactory, ILoggerFactory loggerFactory)
    {
        _scopeFactory = scopeFactory;
        _logger = loggerFactory.CreateLogger("System.Logging.LoggingMonitor");
    }

    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        var systemOS = ComputerUtil.GetOSInfo();
        var cmd = context.JobDetail.GetProperty<string>("cmd");
        var args = context.JobDetail.GetProperty<string>("args");
        var isWindows = systemOS.ToLower().Contains("windows");
        cmd = string.IsNullOrEmpty(cmd) ? (isWindows?"cmd" :"sh"): cmd;
        args = string.IsNullOrEmpty(args) ? "dir" : args;
        args = isWindows ? $"/c {args}" : args;
        // 创建一个新的ProcessStartInfo对象
        ProcessStartInfo startInfo = new ProcessStartInfo(cmd, args); // /c 是执行完命令后关闭CMD窗口
        startInfo.UseShellExecute = false; // 不使用系统外壳程序启动
        startInfo.RedirectStandardOutput = true; // 重定向标准输出
        startInfo.CreateNoWindow = true; // 不创建新窗口

        // 启动进程
        using (Process process = Process.Start(startInfo))
        {
            // 获取CMD的输出信息
            using (StreamReader reader = process.StandardOutput)
            {
                string result = reader.ReadToEnd(); // 读取CMD的输出
                Console.WriteLine(result);
            }
        }
    }
}