﻿using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using NanoSearch.Configuration;
using NanoSearch.Algorithms.RadixTrie;
using NanoSearch.Enumerators;
using NanoSearch.Services;

namespace NanoSearch.Algorithms.Indexer;

public sealed class FileIndexer : IFileCountProvider
{
    public RadixTree<ImmutableHashSet<string>> RadixTree { get; } = new RadixTree<ImmutableHashSet<string>>();
    private readonly ConcurrentDictionary<string, ImmutableHashSet<string>> _fileDictionary = new();
    private FilterPipeline _filterPipeline = null!;
    public ulong Count => RadixTree.Count;

    public void IndexFileSystem(HashSet<string> rootPath, FilterPipeline filterPipeline)
    {
        if(RadixTree.Count > 0)
        {
            RadixTree.Clear();
            _fileDictionary.Clear();
        }
        _filterPipeline = filterPipeline;
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount,
        };

        List<string> directories = new List<string>(50 * rootPath.Count); // preallocating space for directories depending on the number of root paths
        foreach (var rootP in rootPath)
        {
            using (var directoryEnumerator = new FastDirectoryEnumerator(rootP, _filterPipeline.DirectoryFilter, _filterPipeline.DirectoryAttributesToSkip))
            {
                while (directoryEnumerator.MoveNext())
                {
                    directories.Add(Path.Combine(rootP, directoryEnumerator.Current)); // <rootPath, subDirName>
                                                                                          // ex. <C:\, Program Files>
                }
            }
        }
        
        // collecting files in parallel
        Parallel.ForEach(directories, parallelOptions, TraverseDirectories);
       
        BuildRadixTree();
        Console.WriteLine($"Indexed files - count :{Count}");
    }
    private void BuildRadixTree()
    {
        foreach (var (fileName, paths) in _fileDictionary)
        {
            RadixTree.Insert(fileName, paths, 
                (existingPaths, newPaths) => existingPaths.Union(newPaths));
        }
    }
    private void TraverseDirectories(string rootDirectory)
    {
        var dirStack = new Stack<string>();
        dirStack.Push(rootDirectory);
        
        while (dirStack.Count > 0)
        {
            string currentDir = dirStack.Pop();
            try
            {
                // files
                using (var fileEnumerator = new FastFileEnumerator(currentDir, _filterPipeline.FileFilter, _filterPipeline.FileAttributesToSkip))
                {
                    while (fileEnumerator.MoveNext()) // the filterPipeline.FileFilter is applied here
                    {
                        string fileName = fileEnumerator.Current;
                        string filePath = Path.Combine(currentDir, fileName);
                        
                        // adding to thread-safe dictionary
                        _fileDictionary.AddOrUpdate(
                            fileName,
                            _ => ImmutableHashSet.Create(filePath),
                            (_, existingList) => existingList.Add(filePath)
                        );
                    }
                }
                
                //subdirs
                using (var directoryEnumerator = new FastDirectoryEnumerator(currentDir, _filterPipeline.DirectoryFilter, _filterPipeline.DirectoryAttributesToSkip))
                {
                    while (directoryEnumerator.MoveNext()) // the filterPipeline.DirectoryFilter is applied here
                    {
                        string subDirName = directoryEnumerator.Current;
                        string subDirPath = Path.Combine(currentDir, subDirName);

                        dirStack.Push(subDirPath);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
    }
}