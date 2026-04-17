using HistoryTown.Core.Collections;
using HistoryTown.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace HistoryTown.Core.Algorithms;

public class PrimMST(TownGraph graph)
{
    public List<Street> BuildMST()
    {
        var mstEdges = new List<Street>();
        var visited = new HashSet<Structure>();
        // Приоритетная очередь для хранения ребер, отсортированных по весу
        // Храним (Откуда, Куда, Вес)
        var edgeQueue = new PriorityQueue<Street, double>();

        // Начинаем с первой попавшейся вершины
        var startNode = graph.GetAllStructures().FirstOrDefault();
        if (startNode == null) return mstEdges; // Граф пуст

        visited.Add(startNode);
        AddEdgesToQueue(startNode, visited, edgeQueue);

        while (edgeQueue.Count > 0 && visited.Count < graph.GetAllStructures().Count())
        {
            var (from, to, weight) = edgeQueue.Dequeue();

            // Если вершина 'to' уже посещена, пропускаем это ребро, чтобы не создавать циклы
            if (visited.Contains(to)) continue;

            // Добавляем ребро в MST и отмечаем вершину 'to' как посещенную
            mstEdges.Add(new Street(from, to, weight));
            visited.Add(to);
            AddEdgesToQueue(to, visited, edgeQueue);
        }

        return mstEdges;
    }

    // Вспомогательный метод для добавления ребер из вершины в очередь
    private void AddEdgesToQueue(Structure node, HashSet<Structure> visited, PriorityQueue<Street, double> edgeQueue)
    {
        foreach (var (neighbor, weight) in graph.GetWeightedNeighbors(node))
        {
            // Добавляем ребро, только если сосед еще не посещен (хотя проверка в основном цикле тоже есть)
            // В реальной реализации Prim'а, мы бы проверяли, что сосед не входит в MST, но наша visited покрывает это.
            if (!visited.Contains(neighbor))
            {
                edgeQueue.Enqueue(new Street(node, neighbor, weight), weight);
            }
        }
    }
}
