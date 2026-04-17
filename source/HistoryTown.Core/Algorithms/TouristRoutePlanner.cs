using HistoryTown.Core.Collections;
using HistoryTown.Core.Entities;

namespace HistoryTown.Core.Algorithms;

/// <summary>
/// Планировщик туристического маршрута через ключевые объекты (Задача варианта №12)
/// </summary>
public class TouristRoutePlanner(TownGraph graph)
{
    private readonly DijkstraAlgorithm _dijkstra = new(graph);

    /// <summary>
    /// Находит кратчайший маршрут, последовательно посещающий все указанные ключевые объекты
    /// </summary>
    /// <param name="keyObjects">Список ключевых объектов в порядке посещения</param>
    /// <returns>Полный список зданий в маршруте и общая длина</returns>
    public (List<Structure> FullPath, double TotalDistance) PlanRoute(List<Structure> keyObjects)
    {
        if (keyObjects.Count < 2)
            return (keyObjects, 0);

        var fullPath = new List<Structure>();
        double totalDistance = 0;

        for (int i = 0; i < keyObjects.Count - 1; i++)
        {
            var start = keyObjects[i];
            var target = keyObjects[i + 1];

            var (distances, previousNodes) = _dijkstra.FindShortestPaths(start);
            var segment = _dijkstra.ReconstructPath(start, target, previousNodes);

            if (segment.Count == 0)
            {
                // Если какой-то сегмент недостижим, маршрут построить нельзя
                return (new List<Structure>(), double.PositiveInfinity);
            }

            // Добавляем сегмент к полному пути. 
            // Чтобы не дублировать промежуточные точки (конец предыдущего сегмента == начало следующего),
            // пропускаем первый элемент каждого сегмента, кроме самого первого.
            if (i == 0)
            {
                fullPath.AddRange(segment);
            }
            else
            {
                fullPath.AddRange(segment.Skip(1));
            }

            totalDistance += distances[target];
        }

        return (fullPath, totalDistance);
    }
}
