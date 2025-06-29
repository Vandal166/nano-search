namespace NanoSearch.UI.Services;

public interface IDialogService
{
    /// <summary>
    /// Show the dialog for the given view‑model. 
    /// Return true if the user clicked OK, false otherwise.
    /// </summary>
    bool ShowDialog(object viewModel);
}