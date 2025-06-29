using System.Diagnostics;

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
                Console.WriteLine($"{ex.Message}");
            }
            
        }
        else
        {
            Console.WriteLine($"Invalid URI: {fullPath}");
        }
    }
}