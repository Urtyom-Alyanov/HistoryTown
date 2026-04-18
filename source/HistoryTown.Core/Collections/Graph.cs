using HistoryTown.Core.Entities;

namespace HistoryTown.Core.Collections;

/// <summary>
/// Граф улиц исторического города
/// </summary>
public class TownGraph
{
    private readonly Dictionary<Structure, List<(Structure Neighbor, double Weight)>> _adjacencyList = new();
    
    /// <summary>
    /// Добавляет новое здание (вершину) в граф города
    /// </summary>
    /// <param name="structure">Здание</param>
    private void AddStructure(Structure structure) => _adjacencyList.TryAdd(structure, new List<(Structure Neighbor, double Weight)>());
    
    /// <summary>
    /// Добавить новую улицу со зданиями и весом
    /// </summary>
    /// <param name="from">От какого здания</param>
    /// <param name="to">До какого здания</param>
    /// <param name="weight">Вес улицы (расстояние)</param>
    public void AddStreet(Structure from, Structure to, double weight)
    {
        AddStructure(from);
        AddStructure(to);
        _adjacencyList[from].Add((to, weight));
        _adjacencyList[to].Add((from, weight)); // Граф неориентированный
    }
    
    /// <summary>
    /// Получает соседей (с которыми имеет общие улицы) здания с весами
    /// </summary>
    /// <param name="structure">Здание</param>
    /// <returns>Массив соседних зданий с весами</returns>
    public IEnumerable<(Structure Neighbor, double Weight)> GetWeightedNeighbors(Structure structure) => _adjacencyList.GetValueOrDefault(structure, []);

    /// <summary>
    /// Получает соседей (с которыми имеет общие улицы) здания без весов (для BFS/DFS)
    /// </summary>
    /// <param name="structure">Здание</param>
    /// <returns>Массив соседних зданий</returns>
    public IEnumerable<Structure> GetNeighbors(Structure structure) => GetWeightedNeighbors(structure).Select(n => n.Neighbor);
    
    /// <summary>
    /// Получить все здания в городе
    /// </summary>
    /// <returns>Все здания в городе</returns>
    public IEnumerable<Structure> GetAllStructures() => _adjacencyList.Keys;
}