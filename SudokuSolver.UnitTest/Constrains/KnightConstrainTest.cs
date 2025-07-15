using FluentAssertions;
using SudokuSolver.Core.Constrains;
using SudokuSolver.Domain;
using SudokuSolver.UnitTest.Base;

namespace SudokuSolver.UnitTest.Constrains;

public class KnightConstrainTest : BaseTest
{
    private Constrain _knightConstrain;

    [SetUp]
    public void Setup()
    {
        // Initialize any required resources or state before each test
        _knightConstrain = new KnightConstrain();
    }
    
  [Test]
    public void GivenKnightMovesFromCell_ShouldReturnCorrectCells()
    {
        var gridRaw = new int[9, 9]
        {
            {0,0,0,0,0,0,0,0,0},
            {0,0,0,8,0,1,0,0,0},
            {0,0,7,0,0,0,2,0,0},
            {0,0,0,0,0,0,0,0,0},
            {0,0,6,0,0,0,3,0,0},
            {0,0,0,5,0,4,0,0,0},
            {0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0}
        };

        var grid = GenerateGrid(gridRaw);

        var result = _knightConstrain.TrySolve(grid, 4,5);

        result.GetCell(4, 5).IsConfirmed.Should().BeTrue();
        result.GetCell(4,5 ).Value.Should().Be(9);
    }
}