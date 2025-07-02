using System.Text.RegularExpressions;

namespace NanoSearch.Configuration.Indexing;

public static class StringExtensions
{
    public static bool IsValidRegex(this string pattern, out string? errorMessage)
    {
        try
        {
            _ = new Regex(pattern);
            errorMessage = null;
            return true;
        }
        catch (ArgumentException ex)
        {
            errorMessage = ex.Message;
            return false;
        }
    }
    
    public static bool HasFileExtension(this string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return false;

        int dotIndex = fileName.LastIndexOf('.');
        // Valid if dot is not the last character
        return dotIndex != -1 && dotIndex < fileName.Length - 1;
    }
}