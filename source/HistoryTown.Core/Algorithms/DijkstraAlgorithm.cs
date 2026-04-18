using HistoryTown.Core.Collections;
using HistoryTown.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace HistoryTown.Core.Algorithms;

public class DijkstraAlgorithm(TownGraph graph)
{
    // Метод для поиска кратчайших путей от начальной вершины
    public (Dictionary<Structure, double> distances, Dictionary<Structure, Structure?> previousNodes)
        FindShortestPaths(Structure start)
    {
        // Инициализация расстояний: бесконечность для всех, кроме стартовой (0)
        var distances = graph.GetAllStructures().ToDictionary(s => s, s => double.PositiveInfinity);
        // Словарь для отслеживания предыдущих вершин на кратчайшем пути
        var previousNodes = graph.GetAllStructures().ToDictionary<Structure, Structure, Structure?>(s => s, s => null);
        // Приоритетная очередь для хранения вершин, которые нужно посетить, отсортированных по расстоянию
        var priorityQueue = new PriorityQueue<Structure, double>();

        distances[start] = 0;
        priorityQueue.Enqueue(start, 0);

        while (priorityQueue.Count > 0)
        {
            var current = priorityQueue.Dequeue(); // Получаем вершину с наименьшим расстоянием

            // Если текущее расстояние уже бесконечность, значит, мы эту вершину обошли с более коротким путем ранее, или она недостижима.
            if (double.IsPositiveInfinity(distances[current])) continue;

            // Проходим по всем соседям текущей вершины
            foreach (var (neighbor, weight) in graph.GetWeightedNeighbors(current))
            {
                var newDist = distances[current] + weight; // Вычисляем новое расстояние до соседа
                
                // Если новое расстояние короче найденного ранее
                if (newDist < distances[neighbor])
                {
                    distances[neighbor] = newDist;
                    previousNodes[neighbor] = current;
                    priorityQueue.Enqueue(neighbor, newDist);
                }
            }
        }

        return (distances, previousNodes);
    }

    // Метод для восстановления кратчайшего пути между двумя вершинами
    public List<Structure> ReconstructPath(Structure start, Structure target, Dictionary<Structure, Structure?> previousNodes)
    {
        var path = new List<Structure>();
        var current = target;

        try
        {
            while (current != null && !current.Equals(start))
            {
                path.Add(current);
                current = previousNodes[current];
            }
        }
        catch (KeyNotFoundException e)
        {
            return new List<Structure>();
        }

        // Если дошли до начальной вершины (current == start)
        if (current != null && current.Equals(start))
        {
            path.Add(start);
            path.Reverse(); // Разворачиваем путь, чтобы он был от начала к концу
            return path;
        }

        return new List<Structure>(); // Возвращаем пустой список, если путь не найден
    }
}
