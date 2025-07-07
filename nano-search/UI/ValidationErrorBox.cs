using NanoSearch.Configuration;
using MessageBox = Wpf.Ui.Controls.MessageBox;

namespace NanoSearch.UI;

public class ValidationErrorBox<TOptions> where TOptions : class, new()
{
    public ValidationErrorBox(IConfigService<TOptions> configService)
    {
        configService.ConfigurationLoadFailed += (sender, args) =>
        {
            MessageBoxExtensions.Setup(
                args.Title,
                args.Message
            ).Display();
        };
    }
}