using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using NanoSearch.Configuration.Indexing;

namespace NanoSearch.Configuration;

public class JsonConfigService : IConfigService
{
    public IndexingOptions IndexingOptions { get; }
    /*C:\Users\username\AppData\Roaming\NanoSearch\indexing_options.json*/
    private readonly string FILE_PATH = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "NanoSearch",
        "indexing_options.json"
    );  
    
    public JsonConfigService(IndexingOptions indexingOptions)
    {
        IndexingOptions = indexingOptions;
    }   
    
    public void Load()
    {
        if (!File.Exists(FILE_PATH))
        {
            Save();
            return;
        }

        try
        {
            Console.WriteLine($"Reading from full path: {Path.GetFullPath(FILE_PATH)}");
            var json = File.ReadAllText(FILE_PATH);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy        = JsonNamingPolicy.SnakeCaseUpper,
                Converters                  = { new JsonStringEnumConverter() }
            };
            var loadedOptions = JsonSerializer.Deserialize<IndexingOptions>(json, options);
            
            if (loadedOptions != null)
            {
                IndexingOptions.CopyFrom(loadedOptions);
            }
        }
        catch (Exception ex){/*noop*/}
    }

    public void Save()
    {
        try
        {
            var cfg = JsonSerializer.Serialize(IndexingOptions, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseUpper,
                Converters = { new JsonStringEnumConverter() } // For enum serialization
            });

            Directory.CreateDirectory(Path.GetDirectoryName(FILE_PATH)!);
            File.WriteAllText(FILE_PATH, cfg);
        }
        catch (Exception ex){/*noop*/}
    }
}