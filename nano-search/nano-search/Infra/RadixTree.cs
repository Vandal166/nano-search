using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace NanoSearch.Algorithms.RadixTrie;

public sealed class RadixTree<T> where T : class
{
    internal sealed class RadixNode
    {
        public bool isEndOfWord;
        public T? value;
        public readonly Dictionary<char, (string edgeLabel, RadixNode childNode)> children;

        public RadixNode()
        {
            isEndOfWord = false;
            value = default;
            children = new Dictionary<char, (string, RadixNode)>();
        }
    }

    private readonly RadixNode root;
    private readonly bool _caseInsensitive;
    public ulong Count
    {
        get
        {
            ulong totalCount = 0;
            CollectCount(root, ref totalCount);
            return totalCount;
        }
    }

    public RadixTree(bool caseInsensitive = true)
    {
        _caseInsensitive = caseInsensitive;
        root = new RadixNode();
    }

    private void NormalizeKey(ref string key)
    {
        key = _caseInsensitive ? key.ToLowerInvariant() : key;
    }

    public void Insert(string key, T value, Func<T, T, T> mergeValues)
    {
        NormalizeKey(ref key);
        InsertRecursive(root, key, value, 0, mergeValues);
    }

    private void InsertRecursive(RadixNode node, string key, T value, int depth, Func<T, T, T> mergeValues)
    {
        NormalizeKey(ref key);
        if (depth == key.Length)
        {
            node.isEndOfWord = true;
            node.value = node.value == null 
                ? value 
                : mergeValues(node.value, value);
            return;
        }

        char currentChar = key[depth];
        if (!node.children.TryGetValue(currentChar, out var childEntry))
        {
            // Create new edge for remaining key
            string newEdge = key[depth..];
            node.children[currentChar] = (newEdge, new RadixNode
            {
                isEndOfWord = true,
                value = value
            });
            return;
        }

        string edgeLabel = childEntry.edgeLabel;
        RadixNode childNode = childEntry.childNode;

        int i = 0;
        int minLen = Math.Min(edgeLabel.Length, key.Length - depth);
        while (i < minLen && edgeLabel[i] == key[depth + i])
        {
            i++;
        }

        if (i == edgeLabel.Length)
        {
            // Continue insertion in child node
            InsertRecursive(childNode, key, value, depth + i, mergeValues);
        }
        else
        {
            var splitNode = new RadixNode();
            node.children[currentChar] = (edgeLabel[..i], splitNode);
            
            string remainingEdge = edgeLabel[i..];
            splitNode.children[remainingEdge[0]] = (remainingEdge, childNode);

            if (depth + i < key.Length)
            {
                string newEdge = key.Substring(depth + i);
                splitNode.children[newEdge[0]] = (newEdge, new RadixNode
                {
                    isEndOfWord = true,
                    value = value
                });
            }
            else
            {
                splitNode.isEndOfWord = true;
                splitNode.value = value;
            }
        }
    }

    public T? SearchExact(string key)
    {
        NormalizeKey(ref key);
        
        return SearchRecursive(root, key, 0, exactMatch: true)?.value;
    }
    
    public List<T> SearchRegex(Regex regex)
    {
        var results = new List<T>();
        SearchRegexRecursive(root, "", regex, results);
        return results;
    }

    private void SearchRegexRecursive(
        RadixNode node, string currentPath, Regex regex, 
        List<T> results)
    {
        
        // Check if current path matches regex at any point
        if (regex.IsMatch(currentPath) && node.isEndOfWord && node.value != null)
        {
            results.Add(node.value);
        }

        // Continue traversal even after a match (for partial matches)
        foreach (var (ch, (edge, child)) in node.children)
        {
            string newPath = currentPath + edge;
            SearchRegexRecursive(child, newPath, regex, results);
        }
    }

    
    public ImmutableHashSet<string>? SearchPrefix(string prefix)
    {
        NormalizeKey(ref prefix);
        var nodes = GetAllValuesForPrefix(prefix);
        
        if (nodes == null || nodes.Count == 0)
            return null;

        // Flatten results
        var builder = ImmutableHashSet.CreateBuilder<string>();
        foreach (var nodeValue in nodes)
        {
            if (nodeValue is ImmutableHashSet<string> paths)
            {
                builder.UnionWith(paths);
            }
        }
        return builder.ToImmutable();
    }
    
    private RadixNode? SearchRecursive(RadixNode node, string key, int depth, bool exactMatch)
    {
        if (depth == key.Length)
        {
            return exactMatch && !node.isEndOfWord ? null : node;
        }

        char currentChar = key[depth];
        if (!node.children.TryGetValue(currentChar, out var childEntry))
        {
            return null;
        }

        string edgeLabel = childEntry.edgeLabel;
        RadixNode childNode = childEntry.childNode;

        // Check if remaining key matches edge label
        if (key.Length - depth < edgeLabel.Length)
        {
            return !exactMatch && edgeLabel.StartsWith(key.Substring(depth)) 
                ? childNode 
                : null;
        }

        for (int i = 0; i < edgeLabel.Length; i++)
        {
            if (edgeLabel[i] != key[depth + i])
            {
                return null;
            }
        }

        return SearchRecursive(childNode, key, depth + edgeLabel.Length, exactMatch);
    }
    
    public List<T> GetAllValuesForPrefix(string prefix)
    {
        List<T> results = new();
        NormalizeKey(ref prefix);
        RadixNode? startNode = SearchRecursive(root, prefix, 0, exactMatch: false);
        
        if (startNode != null)
        {
            CollectValues(startNode, results);
        }
        return results;
    }
    
    public List<T> GetAllValues()
    {
        List<T> results = new();
        CollectValues(root, results);
        return results;
    }

    private void CollectValues(RadixNode node, List<T> results)
    {
        
        if (node.isEndOfWord && node.value != null)
        {
            results.Add(node.value);
        }

        foreach (var child in node.children.Values)
        {
            CollectValues(child.childNode, results);
        }
    }
    
    private void CollectCount(RadixNode node, ref ulong count)
    {
        if (node.isEndOfWord && node.value != null)
        {
            count += node.value is IEnumerable<string> enumerable ? (ulong)enumerable.Count() : 1;
        }

        foreach (var child in node.children.Values)
        {
            CollectCount(child.childNode, ref count);
        }
    }
    public List<string> GetAllKeys()
    {
        var keys = new List<string>();
        GetAllKeysRecursive(root, "", keys);
        return keys;
    }

    private void GetAllKeysRecursive(RadixNode node, string currentKey, List<string> keys)
    {
        if (node.isEndOfWord)
        {
            keys.Add(currentKey);
        }

        foreach (var (ch, (edge, child)) in node.children)
        {
            GetAllKeysRecursive(child, currentKey + edge, keys);
        }
    }
    public void Clear()
    {
        root.children.Clear();
        root.isEndOfWord = false;
        root.value = null;
    }
}