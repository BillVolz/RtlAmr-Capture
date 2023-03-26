using System.Text.Json;
using RtlAmrCapture.Data;
using RtlAmrCapture.Services;

namespace RtlAmrCapture
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly CaptureService _captureService;
        private readonly RunAndCaptureStdout _runAndCaptureStdout;
        public Worker(ILogger<Worker> logger, CaptureService captureService, RunAndCaptureStdout runAndCaptureStdout)
        {
            _logger = logger;
            _captureService = captureService;
            _runAndCaptureStdout = runAndCaptureStdout;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await _captureService.Initialize(stoppingToken);

                await _runAndCaptureStdout.CaptureApp(async line=>
                {
                    if (string.IsNullOrEmpty(line))
                        return;
                    try
                    {
                        var obj = JsonSerializer.Deserialize<RtlAmrData>(line);
                        if (obj == null)
                        {
                            _logger.LogError("Unable to deserialize line {line}", line);
                            return;
                        }

                        await _captureService.CapturePacket(obj, stoppingToken);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e,"Main loop error processing packet.");
                        throw;
                    }
                    
                    return;
                },stoppingToken);

            }
            catch (TaskCanceledException)
            {
                // When the stopping token is canceled, for example, a call made from services.msc,
                // we shouldn't exit with a non-zero exit code. In other words, this is expected...
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message}", ex.Message);

                // Terminates this process and returns an exit code to the operating system.
                // This is required to avoid the 'BackgroundServiceExceptionBehavior', which
                // performs one of two scenarios:
                // 1. When set to "Ignore": will do nothing at all, errors cause zombie services.
                // 2. When set to "StopHost": will cleanly stop the host, and log errors.
                //
                // In order for the Windows Service Management system to leverage configured
                // recovery options, we need to terminate the process with a non-zero exit code.
                Environment.Exit(1);
            }
        }
    }
}