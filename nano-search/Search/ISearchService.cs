namespace NanoSearch.Services;

public interface ISearchService : IFileCountProvider
{
    IEnumerable<AppSearchResult> Search(string query);
}