using HistoryTown.Core.Collections;
using HistoryTown.Core.Algorithms;
using HistoryTown.Core.Entities;
using Xunit;

namespace HistoryTown.Core.Tests.Algorithms;

public class TouristRoutePlannerTests
{
    private TownGraph SetupGraph()
    {
        var graph = new TownGraph();
        var a = new Structure("A");
        var b = new Structure("B");
        var c = new Structure("C");
        var d = new Structure("D");

        graph.AddStreet(a, b, 10);
        graph.AddStreet(b, c, 10);
        graph.AddStreet(c, d, 10);
        graph.AddStreet(a, d, 50);

        return graph;
    }

    [Fact]
    public void PlanRoute_SequentialPath_ShouldReturnCombinedPath()
    {
        // Arrange
        var graph = SetupGraph();
        var planner = new TouristRoutePlanner(graph);
        var keyObjects = new List<Structure> { new("A"), new("C"), new("D") };

        // Act
        var (fullPath, totalDistance) = planner.PlanRoute(keyObjects);

        // Assert
        Assert.Equal(30, totalDistance); // A->B->C (20) + C->D (10)
        Assert.Equal(4, fullPath.Count); // A, B, C, D
        Assert.Equal("A", fullPath[0].Name);
        Assert.Equal("B", fullPath[1].Name);
        Assert.Equal("C", fullPath[2].Name);
        Assert.Equal("D", fullPath[3].Name);
    }

    [Fact]
    public void PlanRoute_UnreachableNode_ShouldReturnInfinity()
    {
        // Arrange
        var graph = SetupGraph();
        var planner = new TouristRoutePlanner(graph);
        var keyObjects = new List<Structure> { new("A"), new("Z") };

        // Act
        var (fullPath, totalDistance) = planner.PlanRoute(keyObjects);

        // Assert
        Assert.Equal(double.PositiveInfinity, totalDistance);
        Assert.Empty(fullPath);
    }

    [Fact]
    public void PlanRoute_SingleNode_ShouldReturnThatNode()
    {
        // Arrange
        var graph = SetupGraph();
        var planner = new TouristRoutePlanner(graph);
        var keyObjects = new List<Structure> { new("A") };

        // Act
        var (fullPath, totalDistance) = planner.PlanRoute(keyObjects);

        // Assert
        Assert.Equal(0, totalDistance);
        Assert.Single(fullPath);
        Assert.Equal("A", fullPath[0].Name);
    }
}
