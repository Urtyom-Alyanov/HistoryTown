using HistoryTown.Core.Collections;
using HistoryTown.Core.Entities;

namespace HistoryTown.Core.Infrastructure;

public static class TownLoader
{
    public static TownGraph LoadFromFile(string path)
    {
        var graph = new TownGraph();
        if (!File.Exists(path)) return graph;

        foreach (var line in File.ReadLines(path))
        {
            var parts = line.Split(';');
            if (parts.Length >= 2)
            {
                graph.AddStreet(new Structure(parts[0]), new Structure(parts[1]));
            }
        }
        return graph;
    }
}