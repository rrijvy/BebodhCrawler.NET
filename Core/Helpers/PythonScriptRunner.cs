using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace Core.Helpers
{
    public class PythonScriptRunner : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await RunPythonScriptAsync("path_to_your_script.py", "arg1 arg2 arg3", stoppingToken);
        }

        private async Task RunPythonScriptAsync(string scriptPath, string arguments, CancellationToken cancellationToken)
        {
            try
            {
                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = "python"; // Assuming python is in your PATH environment variable
                start.Arguments = string.Format("\"{0}\" {1}", scriptPath, arguments);
                start.UseShellExecute = false;
                start.RedirectStandardOutput = true;
                start.RedirectStandardError = true;
                start.CreateNoWindow = true;

                using (Process process = Process.Start(start))
                {
                    using (System.IO.StreamReader reader = process.StandardOutput)
                    {
                        string result = await reader.ReadToEndAsync();
                        Console.WriteLine("Standard output:");
                        Console.WriteLine(result);
                    }

                    using (System.IO.StreamReader reader = process.StandardError)
                    {
                        string error = await reader.ReadToEndAsync();
                        Console.WriteLine("Standard error:");
                        Console.WriteLine(error);
                    }

                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
