using FluentAssertions;
using SudokuSolver.Core.Constrains;
using SudokuSolver.Domain;
using SudokuSolver.Domain.Models;
using SudokuSolver.UnitTest.Base;

namespace SudokuSolver.UnitTest.Constrains;

public class ColumnConstraintTest : BaseTest
{
    private Constraint _columnConstraint;

    [SetUp]
    public void Setup()
    {
        // Initialize any required objects or state before each test
        _columnConstraint = new ColumnConstraint();
    }

    [Test]
    public void GivenCellsInColumnHasAllButOneValue()
    {
        var grid = new Grid(new List<Cell>
        {
            new Cell { Row = 1, Column = 1, Value = 1 },
            new Cell { Row = 2, Column = 1, Value = 2 },
            new Cell { Row = 3, Column = 1, Value = 3 },
            new Cell { Row = 4, Column = 1, Value = 4 },
            new Cell { Row = 5, Column = 1, Value = 5 },
            new Cell { Row = 6, Column = 1, Value = 6 },
            new Cell { Row = 7, Column = 1, Value = 7 },
            new Cell { Row = 8, Column = 1, Value = 8 },
            new Cell { Row = 9, Column = 1 } // This cell is empty
        });
        
        var result = _columnConstraint.TrySolve(grid, 9, 1);
        result.GetCell(9, 1)!.Value.Should().Be(9);
        result.GetCell(9, 1)!.IsSolved.Should().BeTrue();
        result.GetCell(9, 1)!.IsConfirmed.Should().BeTrue();
    }
    
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(4)]
    [TestCase(5)]
    [TestCase(6)]
    [TestCase(7)]
    [TestCase(8)]
    [TestCase(9)]
    public void GridGetRow(int row)
    {
        var grid = CreateFullGrid();

        var cells = grid.GetColumn(row);
        cells.Should().NotBeNull();
        cells.Should().HaveCount(9);
        cells.Should().OnlyContain(c => c.Column == row);
    }
}