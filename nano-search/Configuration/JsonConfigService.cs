using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using NanoSearch.Configuration.Validators;

namespace NanoSearch.Configuration;

public class JsonConfigService<T> : IConfigService<T> where T : new()
{
    public T Options { get; }
    public event EventHandler? OptionsChanged;
    public event EventHandler<ConfigurationValidationFailEventArgs>? ConfigurationLoadFailed;
    private readonly IValidator<T> _validator;
    
    /*C:\Users\username\AppData\Roaming\NanoSearch\cfg.json*/
    private readonly string _filePath;
    private readonly JsonSerializerOptions _serializerOptions;

    public JsonConfigService(string fileName, IValidator<T> validator)
    {
        _validator = validator;
        Options = new T();
        _filePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "NanoSearch", fileName);
        _serializerOptions = new JsonSerializerOptions
        {
            WriteIndented            = true,
            PropertyNamingPolicy     = JsonNamingPolicy.SnakeCaseUpper,
            Converters               = { new JsonStringEnumConverter() },
            PropertyNameCaseInsensitive = true
        };
    }
    public void Load()
    {
        if (!File.Exists(_filePath))
        {
            Save();
            return;
        }

        try
        {
            Console.WriteLine($"Reading from full path: {Path.GetFullPath(_filePath)}");
            var json = File.ReadAllText(_filePath);

            var loadedOptions = JsonSerializer.Deserialize<T>(json, _serializerOptions);

            if (loadedOptions == null)
            {
                ConfigurationLoadFailed?.Invoke
                (
                    this,
                    new ConfigurationValidationFailEventArgs
                    (
                        "Loading error",
                        $"Error loading configuration from '{_filePath}'."
                    )
                );
                return;
            }
            
            if (HasErrors(loadedOptions, "Loading error", "Unable to load configuration due to validation errors"))
                return;
            
            Copy(loadedOptions, Options);
            OptionsChanged?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception)
        {
            ConfigurationLoadFailed?.Invoke
            (
                this,
                new ConfigurationValidationFailEventArgs
                (
                    "Configuration error",$"Error loading configuration from '{_filePath}' - resetting to default."
                )
            );
            Save();
        }
    }
    
    public void Save()
    {
        try
        {
            if (HasErrors(Options, "Error while saving","Unable to save configuration due to validation errors"))
                return;
            
            var cfg = JsonSerializer.Serialize(Options, _serializerOptions);

            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
            File.WriteAllText(_filePath, cfg);
            OptionsChanged?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception){/*noop*/}
    }
    
    private static void Copy(T from, T to)
    {
        foreach (var prop in typeof(T).GetProperties().Where(p => p.CanRead && p.CanWrite))
        {
            prop.SetValue(to, prop.GetValue(from));
        }
    }
    
    private bool HasErrors(T loadedOptions, string title, string message)
    {
        var errors = _validator.Validate(loadedOptions).ToList();
        if (errors.Count != 0)
        {
            ConfigurationLoadFailed?.Invoke
            (
                this,
                new ConfigurationValidationFailEventArgs
                (
                    title,
                    $"{message}:\n{string.Join(Environment.NewLine, errors)}"
                )
            );
            
            return true;
        }

        return false;
    }
}