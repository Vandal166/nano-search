using NanoSearch.Configuration.Indexing;

namespace NanoSearch.Configuration;

public interface IConfigService<T>
{
    T Options { get; }
    void Load();
    void Save();
}
