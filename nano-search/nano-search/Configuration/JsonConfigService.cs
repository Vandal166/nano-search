using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using NanoSearch.Configuration.Indexing;

namespace NanoSearch.Configuration;

public class JsonConfigService : IConfigService
{
    public IndexingOptions IndexingOptions { get; }
    private readonly string _filePath;
     
    
    public JsonConfigService(string filePath, IndexingOptions indexingOptions)
    {
        _filePath = filePath;
        IndexingOptions = indexingOptions;
    }
    
    public void Load()
    {
        if (!File.Exists(_filePath))
        {
            //_notificationService.Post("[yellow]Configuration file not found. Using default settings.[/]");
            Save();
            return;
        }

        try
        {
            var json = File.ReadAllText(_filePath);
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
                //_notificationService.Post("[green]Configuration loaded successfully.[/]");
            }
            else
            {
                //_notificationService.Post("[red]Failed to load configuration. Using default settings.[/]");
            }
        }
        catch (Exception ex)
        {
            //_notificationService.Post($"[red]Error loading configuration: {Markup.Escape(ex.Message)}[/]");
        }
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
            
            File.WriteAllText(_filePath, cfg);
        }
        catch (Exception ex)
        {
            //_notificationService.Post($"[red]Error saving configuration: {Markup.Escape(ex.Message)}[/]");
            return;
        }
        //_notificationService.Post("[green]Saved to configuration file.[/]");
    }
}