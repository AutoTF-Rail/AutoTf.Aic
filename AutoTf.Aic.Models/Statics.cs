using System.Diagnostics;
using AutoTf.Logging;

namespace AutoTf.Aic.Models;

public class Statics
{
    public static readonly Logger Logger = new Logger(true);

    public static bool? IsCentralBridgeAvailable = null;
	
    public static string GetGitVersion()
    {
        try
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = "describe --tags --always",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using (Process process = new Process { StartInfo = psi })
            {
                process.Start();
                string output = process.StandardOutput.ReadToEnd().Trim();
                string error = process.StandardError.ReadToEnd().Trim();

                process.WaitForExit();

                if (!string.IsNullOrWhiteSpace(error))
                {
                    throw new Exception($"Git Error: {error}");
                }

                return output;
            }
        }
        catch (Exception ex)
        {
            return $"Error retrieving Git version: {ex.Message}";
        }
    }
}