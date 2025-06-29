using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using NanoSearch.Configuration.Indexing;

namespace NanoSearch.UI.ViewModels;

public class DirectoryFilterOptionsViewModel : INotifyPropertyChanged
{
    private readonly DirectoryFilterOptions _options;
    public ObservableCollection<char> ExcludeBeginningWith => 
        new ObservableCollection<char>(_options.ExcludeBeginningWith);
    
    public ObservableCollection<string> ExcludeDirNames =>
        new ObservableCollection<string>(_options.ExcludedDirectoryNames);
    
    public ObservableCollection<EnumFlagViewModel> DirAttributeFlags { get; }

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

    private string? _newDirName;
    public string? NewDirName
    {
        get => _newDirName;
        set { _newDirName = value; OnPropertyChanged(nameof(NewDirName)); }
    }

    private string? _selectedDirName;
    public string? SelectedDirName
    {
        get => _selectedDirName;
        set { _selectedDirName = value; OnPropertyChanged(nameof(SelectedDirName)); }
    }
    
    public ICommand AddExcludedCharCommand { get; }
    public ICommand RemoveExcludedCharCommand { get; }
    public ICommand AddExcludedDirNameCommand { get; }
    public ICommand RemoveExcludedDirNameCommand { get; }
    public ICommand ResetToDefaultCommand { get; }
    
    public DirectoryFilterOptionsViewModel(DirectoryFilterOptions options)
    {
        _options = options;
        
        DirAttributeFlags = new ObservableCollection<EnumFlagViewModel>(
            Enum.GetValues(typeof(FileAttributes))
                .Cast<FileAttributes>()
                .Select(f => new EnumFlagViewModel(f, () => AttributesToSkip,
                    v => AttributesToSkip = v))
        );
        
        AddExcludedCharCommand = new RelayCommand(AddExcludeChar);
        RemoveExcludedCharCommand = new RelayCommand(RemoveExcludeChar);
        AddExcludedDirNameCommand = new RelayCommand(AddDirName);
        RemoveExcludedDirNameCommand = new RelayCommand(RemoveDirName);
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

    private void AddDirName()
    {
        if (!string.IsNullOrWhiteSpace(NewDirName))
        {
            _options.ExcludedDirectoryNames.Add(NewDirName);
            OnPropertyChanged(nameof(ExcludeDirNames));
            NewDirName = string.Empty;
        }
    }

    private void RemoveDirName()
    {
        if (!string.IsNullOrWhiteSpace(SelectedDirName))
        {
            _options.ExcludedDirectoryNames.Remove(SelectedDirName);
            OnPropertyChanged(nameof(ExcludeDirNames));
        }
    }
    
    private void ResetToDefault()
    {
        var def = new DirectoryFilterOptions();
        _options.CopyFrom(def);
        
        OnPropertyChanged(nameof(ExcludeBeginningWith));
        OnPropertyChanged(nameof(ExcludeDirNames));
        OnPropertyChanged(nameof(RegexNamePattern));
        OnPropertyChanged(nameof(AttributesToSkip));
        
        foreach (var flag in DirAttributeFlags)
            flag.NotifyIsCheckedChanged();
    }

   

    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}