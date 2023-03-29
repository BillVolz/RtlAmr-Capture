using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RtlAmrCapture.Config;

namespace RtlAmrCapture.Services
{
    public class RunAndCaptureStdout
    {
        private readonly IOptions<ServiceConfiguration> _options;

        public RunAndCaptureStdout(IOptions<ServiceConfiguration> options)
        {
            _options = options;
        }

        public async Task<int> CaptureApp(Action<string, CancellationToken> lineCapture, CancellationToken cancellationToken)
        {
            var process = new Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.Arguments = _options.Value.RtlAmrArguments;
            process.StartInfo.FileName = _options.Value.FullPathToRtlAmr;
            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(_options.Value.FullPathToRtlAmr);
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.OutputDataReceived += (sender, args) =>
            {
                if (args.Data != null) lineCapture(args.Data, cancellationToken);
            };
            process.Start();
            process.BeginOutputReadLine();
            await process.WaitForExitAsync(cancellationToken);
            if (process.ExitCode != 0)
                throw (new Exception($"Processed closed Code: {process.ExitCode} {await process.StandardError.ReadToEndAsync()}"));
            return process.ExitCode;
        }
    }
}
