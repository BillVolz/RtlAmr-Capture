using NLog.Extensions.Logging;
using RtlAmrCapture;
using RtlAmrCapture.Config;
using RtlAmrCapture.Services;
using RtlAmrCapture.Sql;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((builderContext, config) =>
    {
        config.AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true);
        config.AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true);
    }).ConfigureLogging((hostContext, logging) =>
    {
        logging.AddNLog();
    })
    .ConfigureServices((context,services) =>
    {
        services.AddWindowsService(options =>
        {
            options.ServiceName = "Rtl Amr Capture";
        });

        services.AddHostedService<Worker>();
        services.AddSingleton<CaptureService>();
        services.AddSingleton<RunAndCaptureStdout>();
        services.Configure<ServiceConfiguration>(context.Configuration.GetSection("ServiceConfiguration"));
        services.AddSingleton<MsSqlDataRepo>();
    })
    .Build();

await host.RunAsync();
