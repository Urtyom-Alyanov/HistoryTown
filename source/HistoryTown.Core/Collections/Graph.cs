using HistoryTown.Core.Entities;

namespace HistoryTown.Core.Collections;

/// <summary>
/// Граф улиц исторического города
/// </summary>
public class TownGraph
{
    private readonly Dictionary<Structure, List<Structure>> _adjacencyList = new();
    
    /// <summary>
    /// Добавляет новое здание (вершину) в граф города
    /// </summary>
    /// <param name="structure">Здание</param>
    private void AddStructure(Structure structure) => _adjacencyList.TryAdd(structure, new List<Structure>());
    
    /// <summary>
    /// Добавить новую улицу со зданиями
    /// </summary>
    /// <param name="from">От какого здания</param>
    /// <param name="to">До какого здания</param>
    public void AddStreet(Structure from, Structure to)
    {
        AddStructure(from);
        AddStructure(to);
        _adjacencyList[from].Add(to);
        _adjacencyList[to].Add(from);
    }
    
    /// <summary>
    /// Получает соседей (с которыми имеет общие улицы) здания
    /// </summary>
    /// <param name="structure">Здание</param>
    /// <returns>Массив соседних зданий</returns>
    public IEnumerable<Structure> GetNeighbors(Structure structure) => _adjacencyList.GetValueOrDefault(structure, []);
    
    /// <summary>
    /// Получить все здания в городе
    /// </summary>
    /// <returns>Все здания в городе</returns>
    public IEnumerable<Structure> GetAllStructures() => _adjacencyList.Keys;
}