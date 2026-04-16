using HistoryTown.Core.Collections;
using HistoryTown.Core.Entities;

namespace HistoryTown.Core.Tests.Collections;

public class TownGraphTests
{
    [Fact]
    public void AddStreet_ShouldAddStructuresAndStreetsCorrectly()
    {
        // Arrange
        var graph = new TownGraph();
        var structureA = new Structure("Дом А");
        var structureB = new Structure("Дом Б");

        // Act
        graph.AddStreet(structureA, structureB);

        // Assert
        Assert.Contains(structureA, graph.GetAllStructures());
        Assert.Contains(structureB, graph.GetAllStructures());
        Assert.Contains(structureB, graph.GetNeighbors(structureA));
        Assert.Contains(structureA, graph.GetNeighbors(structureB));
    }

    [Fact]
    public void GetNeighbors_ShouldReturnEmptyListForNonExistentStructure()
    {
        // Arrange
        var graph = new TownGraph();
        var structureA = new Structure("Дом А");
        var nonExistentStructure = new Structure("Дом В");

        // Act
        graph.AddStreet(structureA, new Structure("Дом Б"));
        var neighbors = graph.GetNeighbors(nonExistentStructure);

        // Assert
        Assert.Empty(neighbors);
    }

    [Fact]
    public void GetAllStructures_ShouldReturnAllUniqueStructures()
    {
        // Arrange
        var graph = new TownGraph();
        var structureA = new Structure("Дом А");
        var structureB = new Structure("Дом Б");
        var structureC = new Structure("Дом В");

        // Act
        graph.AddStreet(structureA, structureB);
        graph.AddStreet(structureB, structureC);

        // Assert
        var allStructures = graph.GetAllStructures().ToList();
        Assert.Contains(structureA, allStructures);
        Assert.Contains(structureB, allStructures);
        Assert.Contains(structureC, allStructures);
        Assert.Equal(3, allStructures.Count);
    }
}
