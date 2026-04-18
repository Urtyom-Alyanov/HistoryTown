using HistoryTown.Core.Collections;
using HistoryTown.Core.Entities;

namespace HistoryTown.Core.Algorithms;

/// <summary>
/// Алгоритмы обхода для поиска и построения туристического маршрута через ключевые объекты
/// </summary>
/// <param name="graph">Граф города</param>
public class Traversal(TownGraph graph)
{
    /// <summary>
    /// Алгоритм (генератор) поиска по графу по ширине
    /// </summary>
    /// <param name="start">От какого здания начать</param>
    /// <returns>Порядок посещения вершин</returns>
    public IEnumerable<Structure> BreadthFirstSearch(Structure start)
    {
        var visited = new HashSet<Structure>();
        var queue = new Queue<Structure>();

        visited.Add(start);
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            yield return current;

            foreach (var neighbor in graph.GetNeighbors(current))
            {
                if (visited.Add(neighbor)) 
                    queue.Enqueue(neighbor);
            }
        }
    }
    
    /// <summary>
    /// Алгоритм (генератор) поиска по графу по глубине
    /// </summary>
    /// <param name="start">От какого здания начать</param>
    /// <returns>Порядок посещения вершин</returns>
    public IEnumerable<Structure> DepthFirstSearchIterative(Structure start)
    {
        var visited = new HashSet<Structure>();
        var stack = new Stack<Structure>();

        stack.Push(start);

        while (stack.Count > 0)
        {
            var current = stack.Pop();

            if (visited.Add(current))
            {
                yield return current;
            
                foreach (var neighbor in graph.GetNeighbors(current).Reverse())
                {
                    if (!visited.Contains(neighbor))
                        stack.Push(neighbor);
                }
            }
        }
    }

    /// <summary>
    /// Проверяет, достижима ли вершина B из вершины A
    /// </summary>
    public bool IsReachable(Structure start, Structure target)
    {
        return BreadthFirstSearch(start).Any(s => s == target);
    }

    /// <summary>
    /// Возвращает все компоненты связности графа
    /// </summary>
    public IEnumerable<List<Structure>> GetConnectedComponents()
    {
        var visited = new HashSet<Structure>();
        var components = new List<List<Structure>>();

        foreach (var structure in graph.GetAllStructures())
        {
            if (!visited.Contains(structure))
            {
                var component = BreadthFirstSearch(structure).ToList();
                foreach (var s in component) visited.Add(s);
                components.Add(component);
            }
        }

        return components;
    }
}