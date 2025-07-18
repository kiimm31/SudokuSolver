using FluentAssertions;
using SudokuSolver.Core.Strategies;
using SudokuSolver.Domain.Models;
using SudokuSolver.UnitTest.Base;

namespace SudokuSolver.UnitTest.Strategies;

public class PointingStrategyTest : BaseTest
{
    private PointingStrategy _pointingStrategy;

    [SetUp]
    public void Setup()
    {
        // Initialize any necessary components or state before each test
        // This could include setting up a grid, loading strategies, etc.
        _pointingStrategy = new PointingStrategy();
    }

    [Test]
    public void PointingStrategyApplicationTest()
    {
        var partialGrid = GeneratePartialGrid();
        var request = partialGrid.GetAllUnsolvedCells()
            .GroupBy(x => x.GetBoxIndex())
            .Single(h => h.Key == 9);
        
        _pointingStrategy.ProcessBoxCandidates(partialGrid, request);

        partialGrid.GetCell(7, 4)!.GetPossibleValues().Should().NotContain(1);
        partialGrid.GetCell(7, 6)!.GetPossibleValues().Should().NotContain(1);
        
        partialGrid.GetCell(7, 7)!.GetPossibleValues().Should().Contain(1);
        partialGrid.GetCell(7, 8)!.GetPossibleValues().Should().Contain(1);
        partialGrid.GetCell(7, 9)!.GetPossibleValues().Should().Contain(1);
    }

    private Grid GeneratePartialGrid()
    {
        var cellList = new List<Cell>
        {
           GenerateCellWithValue(7, 1, 6),
           GenerateCellWithPossibleValues(7,2, [5,7,9]),
           GenerateCellWithPossibleValues(7,3, [2,4,5,7,8]),
           GenerateCellWithPossibleValues(7,4, [1,2,5,7,8,9]),
           GenerateCellWithValue(7,5, 3),
           GenerateCellWithPossibleValues(7,6, [1,2,4,5,8,9]),
           GenerateCellWithPossibleValues(7,7, [1,5,7,9]),
           GenerateCellWithPossibleValues(7,8, [1,5,9]),
           GenerateCellWithPossibleValues(7,9, [1,5,7,9]),
           
           GenerateCellWithPossibleValues(8,7,[3,5,7,9]),
           GenerateCellWithPossibleValues(8,8,[3,5,6,9]),
           GenerateCellWithPossibleValues(8,9,[3,5,6,7,9]),
           
           GenerateCellWithValue(9,7, 2),
           GenerateCellWithValue(9,8, 8),
           GenerateCellWithValue(9,9, 4),
        };
        
        return new(cellList);
    }
}