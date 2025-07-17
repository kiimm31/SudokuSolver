using FluentAssertions;
using SudokuSolver.Core.Constrains;
using SudokuSolver.Domain;
using SudokuSolver.Domain.Models;
using SudokuSolver.UnitTest.Base;

namespace SudokuSolver.UnitTest.Constrains;

public class RowConstraintTest : BaseTest
{
    private Constraint _rowConstraint;

    [SetUp]
    public void Setup()
    {
        _rowConstraint = new RowConstraint();
        
    }

    [Test]
    public void GivenRowHasAllButOneValue_WhenTrySolve_ThenReferenceCellShouldSetToLeftOverValue()
    {
        var grid = new Grid(new List<Cell>
        {
            new Cell { Row = 1, Column = 1, Value = 1 },
            new Cell { Row = 1, Column = 2, Value = 2 },
            new Cell { Row = 1, Column = 3, Value = 3 },
            new Cell { Row = 1, Column = 4, Value = 4 },
            new Cell { Row = 1, Column = 5, Value = 5 },
            new Cell { Row = 1, Column = 6, Value = 6 },
            new Cell { Row = 1, Column = 7, Value = 7 },
            new Cell { Row = 1, Column = 8, Value = 8 },
            new Cell { Row = 1, Column = 9 } // This cell is empty
        });
        
        var result = _rowConstraint.TrySolve(grid, 1,9);
        
        result.GetCell(1, 9)!.Value.Should().Be(9);
        result.GetCell(1, 9)!.IsSolved.Should().BeTrue();
        result.GetCell(1, 9)!.IsConfirmed.Should().BeTrue();
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

        var cells = grid.GetRow(row);
        cells.Should().NotBeNull();
        cells.Should().HaveCount(9);
        cells.Should().OnlyContain(c => c.Row == row);
    }
}