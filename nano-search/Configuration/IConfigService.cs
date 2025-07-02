namespace NanoSearch.Configuration;

public interface IConfigService<T>
{
    T Options { get; }
    void Load();
    void Save();

    event EventHandler OptionsChanged;
    event EventHandler<ConfigurationValidationFailEventArgs> ConfigurationLoadFailed;
}
