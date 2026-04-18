using HistoryTown.Core.Algorithms;
using HistoryTown.Core.Collections;
using HistoryTown.Core.Entities;

namespace HistoryTown.Core.Tests.Algorithms;

public class DijkstraAlgorithmTests {
  private TownGraph SetupWeightedGraph() {
    var graph = new TownGraph();
    var a = new Structure("A");
    var b = new Structure("B");
    var c = new Structure("C");
    var d = new Structure("D");
    var e = new Structure("E");

    graph.AddStreet(a, b, 10);
    graph.AddStreet(a, c, 3);
    graph.AddStreet(b, c, 1);
    graph.AddStreet(b, d, 2);
    graph.AddStreet(c, d, 8);
    graph.AddStreet(c, e, 2);
    graph.AddStreet(d, e, 7);

    // a -10- b
    // |    /|
    // 3  1  2
    // |/    |
    // c -8- d
    // |    /
    // 2  7
    // |/
    // c

    return graph;
  }

  [Fact]
  public void FindShortestPaths_ShouldReturnCorrectDistancesFromStart() {
    // Arrange
    var graph = SetupWeightedGraph();
    var dijkstra = new DijkstraAlgorithm(graph);
    var start = new Structure("A");

    // Act
    var (distances, previousNodes) = dijkstra.FindShortestPaths(start);

    // Assert
    // Expected distances from A:
    // A: 0
    // C: 3 (A->C)
    // E: 5 (A->C->E)
    // B: 4 (A->C->B)
    // D: 6 (A->C->B->D)

    Assert.Equal(0, distances[new Structure("A")]);
    Assert.Equal(4, distances[new Structure("B")]);
    Assert.Equal(3, distances[new Structure("C")]);
    Assert.Equal(6, distances[new Structure("D")]);
    Assert.Equal(5, distances[new Structure("E")]);
  }

  [Fact]
  public void ReconstructPath_ShouldReturnCorrectPath() {
    // Arrange
    var graph = SetupWeightedGraph();
    var dijkstra = new DijkstraAlgorithm(graph);
    var start = new Structure("A");
    var target = new Structure("D");

    var (_, previousNodes) = dijkstra.FindShortestPaths(start);

    // Act
    var path = dijkstra.ReconstructPath(start, target, previousNodes);

    // Assert
    Assert.Collection(path,
        s => Assert.Equal("A", s.Name),
        s => Assert.Equal("C", s.Name),
        s => Assert.Equal("B", s.Name),
        s => Assert.Equal("D", s.Name));
  }

  [Fact]
  public void ReconstructPath_ShouldReturnEmptyPathIfNoPathExists() {
    // Arrange
    var graph = SetupWeightedGraph();
    var dijkstra = new DijkstraAlgorithm(graph);
    var start = new Structure("A");
    var disconnected = new Structure("Z");

    var (distances, previousNodes) = dijkstra.FindShortestPaths(start);

    // Act
    var path = dijkstra.ReconstructPath(start, disconnected, previousNodes);

    // Assert
    Assert.Empty(path);
  }
}
