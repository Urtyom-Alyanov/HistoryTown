using System.Diagnostics;
using System.Text;
using System.Windows;
using HistoryTown.Core.Collections;
using HistoryTown.Core.Algorithms;
using HistoryTown.Core.Infrastructure;
using HistoryTown.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace HistoryTown.WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private TownGraph? _graph;
    private Traversal? _traversal;
    private DijkstraAlgorithm? _dijkstraAlgorithm;
    private Dictionary<Structure, double>? _currentDijkstraDistances;
    private Dictionary<Structure, Structure?>? _currentDijkstraPreviousNodes;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void BtnLoad_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _graph = TownLoader.LoadFromFile("city_map.csv");
            _traversal = new Traversal(_graph);
            _dijkstraAlgorithm = new DijkstraAlgorithm(_graph);

            var structures = _graph.GetAllStructures().OrderBy(s => s.Name).ToList();
            ComboStart.ItemsSource = structures;
            ComboEnd.ItemsSource = structures;
            ComboDijkstraStart.ItemsSource = structures;
            ComboDijkstraEnd.ItemsSource = structures;
            ListKeyObjects.ItemsSource = structures;

            if (structures.Count > 0)
            {
                ComboStart.SelectedIndex = 0;
                ComboEnd.SelectedIndex = Math.Max(0, structures.Count - 1);
                ComboDijkstraStart.SelectedIndex = 0;
                ComboDijkstraEnd.SelectedIndex = Math.Max(0, structures.Count - 1);
            }

            TxtStatus.Text = $"Загружено зданий: {structures.Count}";
            MainTabs.IsEnabled = true;
            TxtResult.Text = "Карта успешно загружена.\r\n";
            TxtDijkstraResult.Text = "";
            BtnDijkstraPath.IsEnabled = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при загрузке: {ex.Message}");
        }
    }

    private void BtnBfs_Click(object sender, RoutedEventArgs e)
    {
        if (ComboStart.SelectedItem is not Structure start || _traversal == null) return;
        
        var sw = Stopwatch.StartNew();
        var order = _traversal.BreadthFirstSearch(start).ToList();
        sw.Stop();

        TxtResult.Text = $"BFS (от {start.Name}) [Время: {sw.Elapsed.TotalMilliseconds:F4} мс]:\r\n" + 
                         string.Join(" -> ", order.Select(s => s.Name));
    }

    private void BtnDfs_Click(object sender, RoutedEventArgs e)
    {
        if (ComboStart.SelectedItem is not Structure start || _traversal == null) return;
        
        var sw = Stopwatch.StartNew();
        var order = _traversal.DepthFirstSearchIterative(start).ToList();
        sw.Stop();

        TxtResult.Text = $"DFS (от {start.Name}) [Время: {sw.Elapsed.TotalMilliseconds:F4} мс]:\r\n" + 
                         string.Join(" -> ", order.Select(s => s.Name));
    }

    private void BtnCheckReach_Click(object sender, RoutedEventArgs e)
    {
        if (ComboStart.SelectedItem is not Structure start || 
            ComboEnd.SelectedItem is not Structure target || 
            _traversal == null) return;

        var sw = Stopwatch.StartNew();
        var reachable = _traversal.IsReachable(start, target);
        sw.Stop();

        TxtResult.Text = $"Достижимость {start.Name} -> {target.Name} [Время: {sw.Elapsed.TotalMilliseconds:F4} мс]:\r\n" + 
                         $"{(reachable ? "ДОСТИЖИМО" : "НЕТ СВЯЗИ")}";
    }

    private void BtnComponents_Click(object sender, RoutedEventArgs e)
    {
        if (_traversal == null) return;
        
        var sw = Stopwatch.StartNew();
        var components = _traversal.GetConnectedComponents().ToList();
        sw.Stop();

        var sb = new StringBuilder();
        sb.AppendLine($"Поиск компонент связности [Время: {sw.Elapsed.TotalMilliseconds:F4} мс]");
        sb.AppendLine($"Найдено компонент: {components.Count}");
        for (int i = 0; i < components.Count; i++)
        {
            sb.AppendLine($"Компонента {i + 1}: {string.Join(", ", components[i].Select(s => s.Name))}");
        }
        TxtResult.Text = sb.ToString();
    }

    private void BtnDijkstra_Click(object sender, RoutedEventArgs e)
    {
        if (ComboDijkstraStart.SelectedItem is not Structure start || _dijkstraAlgorithm == null) return;

        var sw = Stopwatch.StartNew();
        (_currentDijkstraDistances, _currentDijkstraPreviousNodes) = _dijkstraAlgorithm.FindShortestPaths(start);
        sw.Stop();

        var sb = new StringBuilder();
        sb.AppendLine($"Алгоритм Дейкстры от {start.Name} [Время: {sw.Elapsed.TotalMilliseconds:F4} мс]:\r\n");

        foreach (var entry in _currentDijkstraDistances.OrderBy(kvp => kvp.Key.Name))
        {
            sb.AppendLine($"  К {entry.Key.Name}: {(double.IsPositiveInfinity(entry.Value) ? "Недостижимо" : entry.Value.ToString("F2"))}");
        }
        TxtDijkstraResult.Text = sb.ToString();
        BtnDijkstraPath.IsEnabled = true;
    }

    private void BtnDijkstraPath_Click(object sender, RoutedEventArgs e)
    {
        if (ComboDijkstraStart.SelectedItem is not Structure start || 
            ComboDijkstraEnd.SelectedItem is not Structure target || 
            _dijkstraAlgorithm == null || 
            _currentDijkstraPreviousNodes == null) return;

        var sw = Stopwatch.StartNew();
        var path = _dijkstraAlgorithm.ReconstructPath(start, target, _currentDijkstraPreviousNodes);
        sw.Stop();

        if (path.Any())
        {
            TxtDijkstraResult.Text += $"\r\n\r\nМаршрут от {start.Name} до {target.Name} [Время: {sw.Elapsed.TotalMilliseconds:F4} мс]:\r\n" + 
                                      string.Join(" -> ", path.Select(s => s.Name));
        }
        else
        {
            TxtDijkstraResult.Text += $"\r\n\r\nМаршрут от {start.Name} до {target.Name}: Не найден.";
        }
    }

    private void BtnTouristRoute_Click(object sender, RoutedEventArgs e)
    {
        if (_graph == null || ListKeyObjects.SelectedItems.Count < 2)
        {
            MessageBox.Show("Выберите минимум 2 объекта для маршрута.");
            return;
        }

        var keyObjects = ListKeyObjects.SelectedItems.Cast<Structure>().ToList();
        var planner = new TouristRoutePlanner(_graph);

        var sw = Stopwatch.StartNew();
        var (fullPath, totalDistance) = planner.PlanRoute(keyObjects);
        sw.Stop();

        var sb = new StringBuilder();
        sb.AppendLine($"Туристический маршрут (задача варианта) [Время: {sw.Elapsed.TotalMilliseconds:F4} мс]:");
        
        if (double.IsPositiveInfinity(totalDistance))
        {
            sb.AppendLine("Маршрут невозможен (объекты в разных компонентах связности).");
        }
        else
        {
            sb.AppendLine($"Общая длина: {totalDistance:F2}");
            sb.AppendLine("Путь: " + string.Join(" -> ", fullPath.Select(p => p.Name)));
        }
        
        TxtAnalysisResult.Text = sb.ToString();
    }

    private void BtnArticulationPoints_Click(object sender, RoutedEventArgs e)
    {
        if (_graph == null) return;
        var finder = new ArticulationPointFinder(_graph);
        
        var sw = Stopwatch.StartNew();
        var points = finder.FindArticulationPoints();
        sw.Stop();

        var sb = new StringBuilder();
        sb.AppendLine($"Точки сочленения [Время: {sw.Elapsed.TotalMilliseconds:F4} мс]:");
        sb.AppendLine(points.Any() ? string.Join(", ", points.Select(p => p.Name)) : "Нет точек сочленения.");
        sb.AppendLine("\nЗначение для города: это критические узлы, при перекрытии которых город распадется на части.");
        
        TxtAnalysisResult.Text = sb.ToString();
    }

    private void BtnMST_Click(object sender, RoutedEventArgs e)
    {
        if (_graph == null) return;
        var prim = new PrimMST(_graph);
        
        var sw = Stopwatch.StartNew();
        var mstEdges = prim.BuildMST();
        sw.Stop();

        var sb = new StringBuilder();
        sb.AppendLine($"Минимальное остовное дерево (Prim) [Время: {sw.Elapsed.TotalMilliseconds:F4} мс]:");
        sb.AppendLine($"Суммарный вес: {mstEdges.Sum(e => e.Weight):F2}");
        sb.AppendLine("Ребра МОД:");
        foreach (var edge in mstEdges)
        {
            sb.AppendLine($"  {edge.From.Name} --({edge.Weight})-- {edge.To.Name}");
        }
        
        TxtAnalysisResult.Text = sb.ToString();
    }

    private void BtnCompareAlgorithms_Click(object sender, RoutedEventArgs e)
    {
        if (_graph == null || _traversal == null || _dijkstraAlgorithm == null) return;

        var start = _graph.GetAllStructures().First();
        var target = _graph.GetAllStructures().Last();

        var sb = new StringBuilder();
        sb.AppendLine($"Сравнение алгоритмов поиска пути: {start.Name} -> {target.Name}\r\n");

        // BFS для поиска кратчайшего пути по количеству ребер
        var swBfs = Stopwatch.StartNew();
        // В Traversal нет прямого метода PathByBFS, но BFS возвращает порядок посещения. 
        // Для сравнения сделаем BFS именно как поиск пути (минимальное число ребер).
        var bfsPath = FindPathByBfs(start, target);
        swBfs.Stop();
        
        sb.AppendLine($"1. BFS (по количеству ребер):");
        sb.AppendLine($"   Время: {swBfs.Elapsed.TotalMilliseconds:F4} мс");
        sb.AppendLine($"   Путь: {(bfsPath.Any() ? string.Join(" -> ", bfsPath.Select(p => p.Name)) : "Не найден")}");
        sb.AppendLine($"   Ребер: {Math.Max(0, bfsPath.Count - 1)}");

        // Дейкстра
        var swDijkstra = Stopwatch.StartNew();
        var (dists, prevs) = _dijkstraAlgorithm.FindShortestPaths(start);
        var dijkstraPath = _dijkstraAlgorithm.ReconstructPath(start, target, prevs);
        swDijkstra.Stop();

        sb.AppendLine($"\r\n2. Дейкстра (с учетом весов):");
        sb.AppendLine($"   Время: {swDijkstra.Elapsed.TotalMilliseconds:F4} мс");
        sb.AppendLine($"   Путь: {(dijkstraPath.Any() ? string.Join(" -> ", dijkstraPath.Select(p => p.Name)) : "Не найден")}");
        sb.AppendLine($"   Общий вес: {(dijkstraPath.Any() ? dists[target] : 0):F2}");

        sb.AppendLine("\r\nВывод:");
        if (bfsPath.SequenceEqual(dijkstraPath))
        {
            sb.AppendLine("Пути совпали. Это происходит, когда путь с минимальным числом ребер также является самым дешевым.");
        }
        else
        {
            sb.AppendLine("Пути РАЗЛИЧАЮТСЯ. BFS нашел путь с меньшим числом остановок, но Дейкстра нашел путь с меньшим суммарным расстоянием.");
        }

        TxtComparisonResult.Text = sb.ToString();
    }

    private List<Structure> FindPathByBfs(Structure start, Structure target)
    {
        if (start == target) return new List<Structure> { start };
        var queue = new Queue<List<Structure>>();
        queue.Enqueue(new List<Structure> { start });
        var visited = new HashSet<Structure> { start };

        while (queue.Count > 0)
        {
            var path = queue.Dequeue();
            var last = path.Last();

            foreach (var neighbor in _graph!.GetNeighbors(last))
            {
                if (visited.Contains(neighbor)) continue;
                
                var newPath = new List<Structure>(path) { neighbor };
                if (neighbor == target) return newPath;
                
                visited.Add(neighbor);
                queue.Enqueue(newPath);
            }
        }
        return new List<Structure>();
    }
}
