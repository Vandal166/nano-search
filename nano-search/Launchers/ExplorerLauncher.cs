using System.IO;
using NanoSearch.UI;

namespace NanoSearch.Launchers;

public class ExplorerLauncher : IAppLauncher
{
    public void Launch(string fullPath)
    {
        if (!File.Exists(fullPath))
        {
            MessageBoxExtensions.Setup(
                "File Not Found",
                $"The file '{fullPath}' does not exist."
            ).Display();
            return;
        }

        try
        {
            System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{fullPath}\"");
        }
        catch (Exception ex)
        {
            MessageBoxExtensions.Setup(
                "Error Launching Explorer",
                $"An error occurred while trying to open the file in Explorer: {ex.Message}"
            ).Display();
            
            Console.WriteLine(ex.Message);
        }
    }
}