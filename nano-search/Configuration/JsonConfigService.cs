using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using NanoSearch.Configuration.Indexing;

namespace NanoSearch.Configuration;

public class JsonConfigService<T> : IConfigService<T> where T : new()
{
    public T Options { get; }
    /*C:\Users\username\AppData\Roaming\NanoSearch\cfg.json*/
    private readonly string _filePath; /*= Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "NanoSearch",
        "indexing_options.json"
    );  */
    private readonly JsonSerializerOptions _serializerOptions;

    /*public JsonConfigService(IndexingOptions indexingOptions)
    {
        IndexingOptions = indexingOptions;
    }   */
    public JsonConfigService(string fileName)
    {
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
        Load();
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
            
            if (loadedOptions != null)
            {
                Copy(loadedOptions, Options);
                /*IndexingOptions.CopyFrom(loadedOptions);*/
            }
        }
        catch (Exception ex){/*noop*/}
    }

    public void Save()
    {
        try
        {
            var cfg = JsonSerializer.Serialize(Options, _serializerOptions);

            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
            File.WriteAllText(_filePath, cfg);
        }
        catch (Exception ex){/*noop*/}
    }
    
    private static void Copy(T from, T to)
    {
        foreach (var prop in typeof(T).GetProperties().Where(p => p.CanRead && p.CanWrite))
        {
            prop.SetValue(to, prop.GetValue(from));
        }
    }
}