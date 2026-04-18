using HistoryTown.Core.Algorithms;
using HistoryTown.Core.Collections;
using HistoryTown.Core.Entities;
using Xunit;

namespace HistoryTown.Core.Tests.Algorithms;

public class PrimMSTTests {
  [Fact]
  public void BuildMST_SimpleGraph_ShouldReturnMinimumSpanningTree() {
    // Arrange
    var graph = new TownGraph();
    var a = new Structure("A");
    var b = new Structure("B");
    var c = new Structure("C");
    graph.AddStreet(a, b, 1);
    graph.AddStreet(b, c, 2);
    graph.AddStreet(a, c, 3);
    var prim = new PrimMST(graph);

    // Act
    var mst = prim.BuildMST();

    // Assert
    Assert.Equal(2, mst.Count);
    Assert.Equal(3, mst.Sum(e => e.Weight));
    Assert.Contains(mst, e => (e.From == a && e.To == b) || (e.From == b && e.To == a));
    Assert.Contains(mst, e => (e.From == b && e.To == c) || (e.From == c && e.To == b));
  }

  [Fact]
  public void BuildMST_DisconnectedGraph_ShouldReturnMSTForOneComponent() {
    // Arrange
    var graph = new TownGraph();
    var a = new Structure("A");
    var b = new Structure("B");
    var c = new Structure("C");
    var d = new Structure("D");
    graph.AddStreet(a, b, 1);
    graph.AddStreet(c, d, 2);
    var prim = new PrimMST(graph);

    // Act
    var mst = prim.BuildMST();

    // Assert
    // Prim's algorithm as implemented starts from one node and finds the MST of its connected component.
    Assert.Single(mst);
    Assert.Equal(1, mst.Sum(e => e.Weight));
  }

  [Fact]
  public void BuildMST_EmptyGraph_ShouldReturnEmptyList() {
    // Arrange
    var graph = new TownGraph();
    var prim = new PrimMST(graph);

    // Act
    var mst = prim.BuildMST();

    // Assert
    Assert.Empty(mst);
  }
}
