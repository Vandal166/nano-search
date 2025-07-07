using System.IO;
using NanoSearch.UI;

namespace NanoSearch.Launchers;

public class AppLauncher : IAppLauncher
{
    public void Launch(string fullPath)
    {
        if (string.IsNullOrWhiteSpace(fullPath) || !File.Exists(fullPath))
        {
            MessageBoxExtensions.Setup(
                "File Not Found",
                $"The file '{fullPath}' does not exist or the path is invalid."
            ).Display();
            return;
        }

        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = fullPath,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBoxExtensions.Setup(
                "Error Launching Application",
                $"An error occurred while trying to open the file: {ex.Message}"
            ).Display();
            
            Console.WriteLine(ex.Message);        
        }
    }
}