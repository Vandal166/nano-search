namespace NanoSearch.Configuration;

public class ConfigurationValidationFailEventArgs : EventArgs
{
    public string Title { get; }
    public string Message { get; }
    public ConfigurationValidationFailEventArgs(string messageBoxTitle, string message)
    {
        Title = messageBoxTitle;
        Message = message;
    }
}