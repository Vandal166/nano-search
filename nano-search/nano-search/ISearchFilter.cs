using System.Collections.Immutable;

namespace NanoSearch;

public interface ISearchFilter
{
    /// <summary>
    /// Takes the raw results (or null) and returns a new (or same) set.
    /// </summary>
    ImmutableHashSet<string>? Apply(ImmutableHashSet<string>? rawResults, string query);
}