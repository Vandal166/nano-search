using System.IO;

namespace NanoSearch;

public class ExplorerLauncher : IAppLauncher
{
    public void Launch(string fullPath)
    {
        if (!File.Exists(fullPath))
            return;

        try
        {
            System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{fullPath}\"");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}