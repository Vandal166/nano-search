using NanoSearch.Configuration.Indexing;

namespace NanoSearch.Configuration;

public interface IConfigService
{
    IndexingOptions IndexingOptions { get; }
    void Load();
    void Save();
}