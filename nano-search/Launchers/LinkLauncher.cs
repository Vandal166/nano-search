using System.Diagnostics;
using NanoSearch.UI;

namespace NanoSearch.Launchers;

public class LinkLauncher : IAppLauncher
{
    public void Launch(string fullPath)
    {
        if(Uri.TryCreate(fullPath, UriKind.Absolute, out var uri))
        {
            try
            {
                Process.Start(
                    new ProcessStartInfo(uri.AbsoluteUri)
                    {
                        UseShellExecute = true 
                    });

            }
            catch (Exception ex)
            {
                MessageBoxExtensions.Setup(
                    "Error Launching Link",
                    $"An error occurred while trying to open the link: {ex.Message}"
                ).Display();
                
                Console.WriteLine($"{ex.Message}");
            }
            
        }
        else
        {
            MessageBoxExtensions.Setup(
                "Invalid URI",
                $"The provided path '{fullPath}' is not a valid URI."
            ).Display();
            
            Console.WriteLine($"Invalid URI: {fullPath}");
        }
    }
}