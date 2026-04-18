using HistoryTown.Core.Algorithms;
using HistoryTown.Core.Collections;
using HistoryTown.Core.Entities;
using Xunit;

namespace HistoryTown.Core.Tests.Algorithms;

public class ArticulationPointFinderTests {
  [Fact]
  public void FindArticulationPoints_LinearGraph_ShouldReturnInteriorPoints() {
    // Arrange
    var graph = new TownGraph();
    var a = new Structure("A");
    var b = new Structure("B");
    var c = new Structure("C");
    graph.AddStreet(a, b, 1.0);
    graph.AddStreet(b, c, 1.0);
    var finder = new ArticulationPointFinder(graph);

    // Act
    var points = finder.FindArticulationPoints();

    // Assert
    Assert.Single(points);
    Assert.Contains(b, points);
  }

  [Fact]
  public void FindArticulationPoints_StarGraph_ShouldReturnCenterPoint() {
    // Arrange
    var graph = new TownGraph();
    var center = new Structure("Center");
    var a = new Structure("A");
    var b = new Structure("B");
    var c = new Structure("C");
    graph.AddStreet(center, a, 1.0);
    graph.AddStreet(center, b, 1.0);
    graph.AddStreet(center, c, 1.0);
    var finder = new ArticulationPointFinder(graph);

    // Act
    var points = finder.FindArticulationPoints();

    // Assert
    Assert.Single(points);
    Assert.Contains(center, points);
  }

  [Fact]
  public void FindArticulationPoints_CycleGraph_ShouldReturnNoPoints() {
    // Arrange
    var graph = new TownGraph();
    var a = new Structure("A");
    var b = new Structure("B");
    var c = new Structure("C");
    graph.AddStreet(a, b, 1.0);
    graph.AddStreet(b, c, 1.0);
    graph.AddStreet(c, a, 1.0);
    var finder = new ArticulationPointFinder(graph);

    // Act
    var points = finder.FindArticulationPoints();

    // Assert
    Assert.Empty(points);
  }

  [Fact]
  public void FindArticulationPoints_TwoCyclesConnectedByOneEdge_ShouldReturnEndPointsOfThatEdge() {
    // Arrange
    var graph = new TownGraph();
    // Cycle 1
    var a = new Structure("A");
    var b = new Structure("B");
    var c = new Structure("C");
    graph.AddStreet(a, b, 1.0);
    graph.AddStreet(b, c, 1.0);
    graph.AddStreet(c, a, 1.0);

    // Cycle 2
    var d = new Structure("D");
    var e = new Structure("E");
    var f = new Structure("F");
    graph.AddStreet(d, e, 1.0);
    graph.AddStreet(e, f, 1.0);
    graph.AddStreet(f, d, 1.0);

    // Connector
    graph.AddStreet(c, d, 1.0);

    var finder = new ArticulationPointFinder(graph);

    // Act
    var points = finder.FindArticulationPoints();

    // Assert
    Assert.Equal(2, points.Count);
    Assert.Contains(c, points);
    Assert.Contains(d, points);
  }
}
