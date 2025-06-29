using Application = System.Windows.Application;

namespace NanoSearch.UI.Services;

public class DialogService : IDialogService
{
    private readonly IServiceProvider _provider;
    public DialogService(IServiceProvider provider)
    {
        _provider = provider;
    }

    public bool ShowDialog(object viewModel)
    {
        // Convention: for a FooViewModel, there is a FooWindow in XAML
        var vmType     = viewModel.GetType();
        var viewType   = Type.GetType(vmType.FullName!.Replace("ViewModel", "Window"));
        if (viewType == null)
            throw new InvalidOperationException($"No view found for {vmType.Name}");

        // Use DI to instantiate the Window if needed
        var window = (System.Windows.Window)(_provider.GetService(viewType) 
                                             ?? Activator.CreateInstance(viewType)!);
        window.DataContext = viewModel;
        window.Owner       = Application.Current.MainWindow;
        return window.ShowDialog() == true;
    }
}