using System.Text;
using System.Windows;
using HistoryTown.Core.Collections;
using HistoryTown.Core.Algorithms;
using HistoryTown.Core.Infrastructure;
using HistoryTown.Core.Entities;

namespace HistoryTown.WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private TownGraph? _graph;
    private Traversal? _traversal;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void BtnLoad_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Файл лежит в корне проекта, при отладке он копируется в bin (если настроено) 
            // или мы можем указать относительный путь.
            _graph = TownLoader.LoadFromFile("city_map.csv");
            _traversal = new Traversal(_graph);

            var structures = _graph.GetAllStructures().OrderBy(s => s.Name).ToList();
            ComboStart.ItemsSource = structures;
            ComboEnd.ItemsSource = structures;

            if (structures.Count > 0)
            {
                ComboStart.SelectedIndex = 0;
                ComboEnd.SelectedIndex = Math.Max(0, structures.Count - 1);
            }

            TxtStatus.Text = $"Загружено зданий: {structures.Count}";
            MainTabs.IsEnabled = true;
            TxtResult.Text = "Карта успешно загружена.\r\n";
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
}