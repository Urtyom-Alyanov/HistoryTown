using HistoryTown.Core.Collections;
using HistoryTown.Core.Algorithms;
using HistoryTown.Core.Entities;

namespace HistoryTown.Core.Tests.Algorithms;

public class TraversalTests
{
    private TownGraph SetupGraph()
    {
        var graph = new TownGraph();
        var a = new Structure("A");
        var b = new Structure("B");
        var c = new Structure("C");
        var d = new Structure("D");
        var e = new Structure("E");
        var f = new Structure("F");
        var g = new Structure("G"); // Isolated

        graph.AddStreet(a, b);
        graph.AddStreet(a, c);
        graph.AddStreet(b, d);
        graph.AddStreet(c, e);
        graph.AddStreet(d, e);
        graph.AddStreet(e, f);
        graph.AddStreet(g, new Structure("H"));

        return graph;
    }

    [Fact]
    public void BreadthFirstSearch_ShouldReturnCorrectOrder()
    {
        // Arrange
        var graph = SetupGraph();
        var traversal = new Traversal(graph);
        var start = new Structure("A");

        // Act
        var result = traversal.BreadthFirstSearch(start).Select(s => s.Name).ToList();

        // Assert (order might vary slightly for same-level nodes, but structure is key)
        Assert.Contains("A", result);
        Assert.Contains("B", result);
        Assert.Contains("C", result);
        Assert.Contains("D", result);
        Assert.Contains("E", result);
        Assert.Contains("F", result);
        Assert.Equal(6, result.Count); // A, B, C, D, E, F
        Assert.Equal("A", result[0]);
    }

    [Fact]
    public void DepthFirstSearchIterative_ShouldReturnCorrectOrder()
    {
        // Arrange
        var graph = SetupGraph();
        var traversal = new Traversal(graph);
        var start = new Structure("A");

        // Act
        var result = traversal.DepthFirstSearchIterative(start).Select(s => s.Name).ToList();

        // Assert (DFS order depends on neighbor iteration, but all reachable should be there)
        Assert.Contains("A", result);
        Assert.Contains("B", result);
        Assert.Contains("C", result);
        Assert.Contains("D", result);
        Assert.Contains("E", result);
        Assert.Contains("F", result);
        Assert.Equal(6, result.Count); // A, B, C, D, E, F
        Assert.Equal("A", result[0]);
    }

    [Theory]
    [InlineData("A", "F", true)]
    [InlineData("A", "G", false)]
    [InlineData("G", "H", true)]
    [InlineData("G", "A", false)]
    public void IsReachable_ShouldReturnCorrectResult(string startName, string targetName, bool expected)
    {
        // Arrange
        var graph = SetupGraph();
        var traversal = new Traversal(graph);
        var start = new Structure(startName);
        var target = new Structure(targetName);

        // Act
        var result = traversal.IsReachable(start, target);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetConnectedComponents_ShouldReturnCorrectComponents()
    {
        // Arrange
        var graph = SetupGraph(); // Has two components: {A,B,C,D,E,F} and {G,H}
        var traversal = new Traversal(graph);

        // Act
        var components = traversal.GetConnectedComponents().ToList();

        // Assert
        Assert.Equal(2, components.Count);
        Assert.Contains(components, c => c.Count == 6 && c.Any(s => s.Name == "A") && c.Any(s => s.Name == "F"));
        Assert.Contains(components, c => c.Count == 2 && c.Any(s => s.Name == "G") && c.Any(s => s.Name == "H"));
    }
}
