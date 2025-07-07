using MessageBox = Wpf.Ui.Controls.MessageBox;

namespace NanoSearch.UI;

public static class MessageBoxExtensions
{
    public static MessageBox Setup(string title, string content)
    {
        return new MessageBox
        {
            Title = title,
            Content = content,
            CloseButtonText = "OK",
            ShowTitle = true
        };
    }

    public static void Display(this MessageBox messageBox)
    {
        messageBox.ShowDialogAsync();
    }
}