using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using NanoSearch.Configuration.Indexing;

namespace NanoSearch.UI.ViewModels;

public class FileFilterOptionsViewModel : INotifyPropertyChanged
{
    private readonly FileFilterOptions _options;
    public ObservableCollection<char> ExcludeBeginningWith => 
        new ObservableCollection<char>(_options.ExcludeBeginningWith);

    public ObservableCollection<string> IncludedFileExtensions =>
        new ObservableCollection<string>(_options.IncludedFileExtensions);

    public ObservableCollection<EnumFlagViewModel> FileAttributeFlags { get; }

    public string RegexNamePattern
    {
        get => _options.RegexNamePattern;
        set { _options.RegexNamePattern = value; OnPropertyChanged(nameof(RegexNamePattern)); }
    }
    public FileAttributes AttributesToSkip
    {
        get => _options.AttributesToSkip;
        set { _options.AttributesToSkip = value; OnPropertyChanged(nameof(AttributesToSkip)); }
    }
    private string? _newChar;
    public string? NewChar
    {
        get => _newChar;
        set { _newChar = value; OnPropertyChanged(nameof(NewChar)); }
    }

    private char? _selectedExcludedChar;
    public char? SelectedExcludedChar
    {
        get => _selectedExcludedChar;
        set { _selectedExcludedChar = value; OnPropertyChanged(nameof(SelectedExcludedChar)); }
    }

    private string? _newExtension;
    public string? NewExtension
    {
        get => _newExtension;
        set { _newExtension = value; OnPropertyChanged(nameof(NewExtension)); }
    }

    private string? _selectedExtension;
    public string? SelectedExtension
    {
        get => _selectedExtension;
        set { _selectedExtension = value; OnPropertyChanged(nameof(SelectedExtension)); }
    }
    
    public ICommand AddExcludedCharCommand { get; }
    public ICommand RemoveExcludedCharCommand { get; }
    public ICommand AddExtensionCommand { get; }
    public ICommand RemoveExtensionCommand { get; }
    public ICommand ResetToDefaultCommand { get; }
    
    public FileFilterOptionsViewModel(FileFilterOptions options)
    {
        _options = options;
        
        FileAttributeFlags = new ObservableCollection<EnumFlagViewModel>(
            Enum.GetValues(typeof(FileAttributes))
                .Cast<FileAttributes>()
                .Select(f => new EnumFlagViewModel(f,
                    () => AttributesToSkip,
                    v => AttributesToSkip = v))
        );
        
        AddExcludedCharCommand = new RelayCommand(AddExcludeChar);
        RemoveExcludedCharCommand = new RelayCommand(RemoveExcludeChar);
        AddExtensionCommand = new RelayCommand(AddExtension);
        RemoveExtensionCommand = new RelayCommand(RemoveExtension);
        ResetToDefaultCommand = new RelayCommand(ResetToDefault);
        
    }
    
    private void AddExcludeChar()
    {
        if (!string.IsNullOrEmpty(NewChar))
        {
            var c = NewChar[0];
            _options.ExcludeBeginningWith.Add(c);
            OnPropertyChanged(nameof(ExcludeBeginningWith));
            NewChar = string.Empty;
        }
    }

    private void RemoveExcludeChar()
    {
        if (SelectedExcludedChar.HasValue)
        {
            _options.ExcludeBeginningWith.Remove(SelectedExcludedChar.Value);
            OnPropertyChanged(nameof(ExcludeBeginningWith));
        }
    }

    private void AddExtension()
    {
        if (!string.IsNullOrWhiteSpace(NewExtension))
        {
            var ext = NewExtension.Trim();
            if (!ext.StartsWith('.'))
                ext = "." + ext;

            _options.IncludedFileExtensions.Add(ext);
            OnPropertyChanged(nameof(IncludedFileExtensions));
            NewExtension = string.Empty;
        }
    }

    private void RemoveExtension()
    {
        if (!string.IsNullOrWhiteSpace(SelectedExtension))
        {
            _options.IncludedFileExtensions.Remove(SelectedExtension);
            OnPropertyChanged(nameof(IncludedFileExtensions));
        }
    }
    
    private void ResetToDefault()
    {
        var def = new FileFilterOptions();
        _options.CopyFrom(def);
        
        OnPropertyChanged(nameof(ExcludeBeginningWith));
        OnPropertyChanged(nameof(IncludedFileExtensions));
        OnPropertyChanged(nameof(RegexNamePattern));
        OnPropertyChanged(nameof(AttributesToSkip));
        
        foreach (var flag in FileAttributeFlags)
            flag.NotifyIsCheckedChanged();
    }

   

    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}