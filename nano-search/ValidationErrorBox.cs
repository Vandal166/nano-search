using NanoSearch.Configuration;
using MessageBox = Wpf.Ui.Controls.MessageBox;

namespace NanoSearch.UI;

public class ValidationErrorBox<TOptions> where TOptions : class, new()
{
    public ValidationErrorBox(IConfigService<TOptions> configService)
    {
        configService.ConfigurationLoadFailed += (sender, args) =>
        {
            var messageBox = new MessageBox
            {
                Title = args.Title,
                Content = args.Message,
                CloseButtonText = "OK", // Set the close button text
                ShowTitle = true
            };
            messageBox.ShowDialogAsync();
        };
    }
}