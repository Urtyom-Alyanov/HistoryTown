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
        var order = _traversal.BreadthFirstSearch(start);
        TxtResult.Text = $"BFS (от {start.Name}):\r\n" + string.Join(" -> ", order.Select(s => s.Name));
    }

    private void BtnDfs_Click(object sender, RoutedEventArgs e)
    {
        if (ComboStart.SelectedItem is not Structure start || _traversal == null) return;
        var order = _traversal.DepthFirstSearchIterative(start);
        TxtResult.Text = $"DFS (от {start.Name}):\r\n" + string.Join(" -> ", order.Select(s => s.Name));
    }

    private void BtnCheckReach_Click(object sender, RoutedEventArgs e)
    {
        if (ComboStart.SelectedItem is not Structure start || 
            ComboEnd.SelectedItem is not Structure target || 
            _traversal == null) return;

        var reachable = _traversal.IsReachable(start, target);
        TxtResult.Text = $"Достижимость {start.Name} -> {target.Name}: {(reachable ? "ДОСТИЖИМО" : "НЕТ СВЯЗИ")}";
    }

    private void BtnComponents_Click(object sender, RoutedEventArgs e)
    {
        if (_traversal == null) return;
        var components = _traversal.GetConnectedComponents().ToList();
        var sb = new StringBuilder();
        sb.AppendLine($"Найдено компонент связности: {components.Count}");
        for (int i = 0; i < components.Count; i++)
        {
            sb.AppendLine($"Компонента {i + 1}: {string.Join(", ", components[i].Select(s => s.Name))}");
        }
        TxtResult.Text = sb.ToString();
    }

    private void BtnDijkstra_Click(object sender, RoutedEventArgs e)
    {
        if (ComboDijkstraStart.SelectedItem is not Structure start || _dijkstraAlgorithm == null) return;

        (_currentDijkstraDistances, _currentDijkstraPreviousNodes) = _dijkstraAlgorithm.FindShortestPaths(start);

        var sb = new StringBuilder();
        sb.AppendLine($"Кратчайшие расстояния от {start.Name}:\r\n");

        foreach (var entry in _currentDijkstraDistances.OrderBy(kvp => kvp.Key.Name))
        {
            sb.AppendLine($"  К {entry.Key.Name}: {(entry.Value == double.PositiveInfinity ? "Недостижимо" : entry.Value.ToString("F2"))}");
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

        var path = _dijkstraAlgorithm.ReconstructPath(start, target, _currentDijkstraPreviousNodes);

        if (path.Any())
        {
            TxtDijkstraResult.Text += $"\r\n\r\nМаршрут от {start.Name} до {target.Name}:\r\n" + string.Join(" -> ", path.Select(s => s.Name));
        }
        else
        {
            TxtDijkstraResult.Text += $"\r\n\r\nМаршрут от {start.Name} до {target.Name}: Не найден.";
        }
    }
}