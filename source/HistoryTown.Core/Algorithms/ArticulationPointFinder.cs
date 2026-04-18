using HistoryTown.Core.Collections;
using HistoryTown.Core.Entities;
using System.Collections;

namespace HistoryTown.Core.Algorithms;

public class ArticulationPointFinder(TownGraph graph)
{
    private int _time;
    private Dictionary<Structure, int> _discoveryTime = new();
    private Dictionary<Structure, int> _lowLinkValue = new();
    private Dictionary<Structure, Structure?> _parent = new();
    private HashSet<Structure> _articulationPoints = new();
    private HashSet<Structure> _visited = new();

    public HashSet<Structure> FindArticulationPoints()
    {
        _time = 0;
        _discoveryTime = new Dictionary<Structure, int>();
        _lowLinkValue = new Dictionary<Structure, int>();
        _parent = new Dictionary<Structure, Structure?>();
        _articulationPoints = new HashSet<Structure>();
        _visited = new HashSet<Structure>();

        // Инициализация словарей для всех вершин
        foreach (var structure in graph.GetAllStructures())
        {
            _discoveryTime[structure] = -1;
            _lowLinkValue[structure] = -1;
            _parent[structure] = null;
        }

        // Проходим по всем вершинам, чтобы обработать несвязные графы
        foreach (var structure in graph.GetAllStructures())
        {
            if (!_visited.Contains(structure))
            {
                DFS(structure);
            }
        }

        return _articulationPoints;
    }

    private void DFS(Structure u)
    {
        _visited.Add(u);
        _discoveryTime[u] = _lowLinkValue[u] = _time++;
        int children = 0;

        // Получаем соседей с весами, но для поиска точек сочленения вес нам не нужен
        foreach (var (v, weight) in graph.GetWeightedNeighbors(u))
        {
            if (!_visited.Contains(v))
            {
                children++;
                _parent[v] = u;
                DFS(v);

                // Обновляем low-link значение для u
                _lowLinkValue[u] = Math.Min(_lowLinkValue[u], _lowLinkValue[v]);

                // Условие для точки сочленения:
                // 1. u - корень DFS дерева и имеет более одного ребенка.
                if (_parent[u] == null && children > 1)
                    _articulationPoints.Add(u);

                // 2. u не корень, и low-link значение потомка v больше или равно времени открытия u.
                // Это значит, что нет обратного ребра из поддерева v к предкам u.
                if (_parent[u] != null && _lowLinkValue[v] >= _discoveryTime[u])
                    _articulationPoints.Add(u);
            }
            else if (v != _parent[u]) // Если v уже посещена и это не родитель u
            {
                // Обновляем low-link значение u, если v - предок (через обратное ребро)
                _lowLinkValue[u] = Math.Min(_lowLinkValue[u], _discoveryTime[v]);
            }
        }
    }
}
