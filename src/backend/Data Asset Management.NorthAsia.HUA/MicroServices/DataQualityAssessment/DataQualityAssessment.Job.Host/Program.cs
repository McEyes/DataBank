using System.Diagnostics;
using DataQualityAssessment.Application.QualityRating.Services;
using Furion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

Serve.RunNative(RunOptions.Default.Silence(false).ConfigureBuilder(builder =>
{
    builder.UseSerilogDefault();
}));

ILogger<Program> logger = App.GetRequiredService<ILogger<Program>>();
IServiceScopeFactory scopeFactory = App.GetRequiredService<IServiceScopeFactory>();
bool keepRunning = true;
var cts = new CancellationTokenSource();
CancellationToken token = cts.Token;

var ctsTimeOut = new CancellationTokenSource(1000 * 60 * 20);//20 min
CancellationToken tokenTimeOut = ctsTimeOut.Token;

Console.CancelKeyPress += (sender, e) =>
{
    logger.LogInformation($"\nCtrl+C detected! Exiting...");
    e.Cancel = true; // Prevent immediate termination
    keepRunning = false;
    cts.Cancel();
    ctsTimeOut.Cancel();
};
logger.LogInformation($"Press Ctrl+C to stop the loop.");

while (keepRunning)
{
    await Task.Delay(1000); // 每 1 秒执行一次

    ctsTimeOut = new CancellationTokenSource(1000 * 60 * 20);
    tokenTimeOut = ctsTimeOut.Token;
    logger.LogInformation($">>Start evaluation");
    var stopwatch = Stopwatch.StartNew();
    using (var scope = scopeFactory.CreateScope())
    {
        try
        {
            var serviceProvider = scope.ServiceProvider;
            var qualityRatingService = serviceProvider.GetRequiredService<IQualityRatingService>();
            await qualityRatingService.EvaluateAsync(tokenTimeOut);
            qualityRatingService.CloseAllConnections();
            stopwatch.Stop();

            if (cts.IsCancellationRequested)
            {
                ctsTimeOut.Cancel();
                logger.LogInformation($">>IsCancellationRequested");
                break;
            }
        }
        catch (Exception ex)
        {
            logger.LogError($"{ex.Message}");
        }
    }

    logger.LogInformation($">>End of evaluation, took {stopwatch.ElapsedMilliseconds} milliseconds");
}


AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
{
    Console.WriteLine("Unhandled exception: " + e.ExceptionObject.ToString());
    // 记录日志或执行其他操作
};
