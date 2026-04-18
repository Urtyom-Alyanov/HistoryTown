using HistoryTown.Core.Collections;
using HistoryTown.Core.Entities;

namespace HistoryTown.Core.Infrastructure;

public static class TownLoader {
  public static TownGraph LoadFromFile(string path) {
    var graph = new TownGraph();
    if (!File.Exists(path)) return graph;

    foreach (var line in File.ReadLines(path)) {
      var parts = line.Split(';');
      if (parts.Length >= 3 && double.TryParse(parts[2], out var weight)) {
        graph.AddStreet(new Structure(parts[0]), new Structure(parts[1]), weight);
      }
      else if (parts.Length >= 2) // Для обратной совместимости с ЛР4, если вес отсутствует
      {
        graph.AddStreet(new Structure(parts[0]), new Structure(parts[1]), 1.0); // Вес по умолчанию
      }
    }
    return graph;
  }
}
