using FluentAssertions;
using SudokuSolver.Core.Constrains;
using SudokuSolver.Domain;
using SudokuSolver.Domain.Models;
using SudokuSolver.UnitTest.Base;

namespace SudokuSolver.UnitTest.Constrains;

public class BoxConstraintTest : BaseTest
{
    private Constraint _boxConstraint;

    [SetUp]
    public void Setup()
    {
        // Initialize any required objects or state before each test
        _boxConstraint = new BoxConstraint();
    }

    [Test]
    public void GivenBox1HasAllButOneValue()
    {
        var grid = new Grid(new List<Cell>
        {
            new Cell { Row = 1, Column = 1, Value = 1 },
            new Cell { Row = 1, Column = 2, Value = 2 },
            new Cell { Row = 1, Column = 3, Value = 3 },
            new Cell { Row = 2, Column = 1, Value = 4 },
            new Cell { Row = 2, Column = 2, Value = 5 },
            new Cell { Row = 2, Column = 3, Value = 6 },
            new Cell { Row = 3, Column = 1, Value = 7 },
            new Cell { Row = 3, Column = 2, Value = 8 },
            new Cell { Row = 3, Column = 3 } // This cell is empty
        });
        
        var result = _boxConstraint.TrySolve(grid, 3, 3);
        result.GetCell(3, 3)!.Value.Should().Be(9);
        result.GetCell(3, 3)!.IsSolved.Should().BeTrue();
        result.GetCell(3, 3)!.IsConfirmed.Should().BeTrue();
    }

    [TestCase(1,1)] // Top-left box
    [TestCase(1,2)]
    [TestCase(1,3)]
    [TestCase(2,1)]
    [TestCase(2,2)]
    [TestCase(2,3)]
    [TestCase(3,1)]
    [TestCase(3,2)]
    [TestCase(3,3)]
    public void GetBox_ShouldReturnTopLeftBox_ForCellsInBox1(int row, int column)
    {
        // Arrange
        var grid = CreateFullGrid();


        // Act
        var boxCells = grid.GetBox(row, column);

        // Assert
        boxCells.Should().HaveCount(9);
        boxCells.Should().OnlyContain(c => c.Row >= 1 && c.Row <= 3 && c.Column >= 1 && c.Column <= 3);
    }

    [TestCase(1,4)] // Top-center box
    [TestCase(1,5)]
    [TestCase(1,6)]
    [TestCase(2,4)]
    [TestCase(2,5)]
    [TestCase(2,6)]
    [TestCase(3,4)]
    [TestCase(3,5)]
    [TestCase(3,6)]
    public void GetBox_ShouldReturnTopCenterBox_ForCellsInBox2(int row, int column)
    {
        // Arrange
        var grid = CreateFullGrid();

        // Act
        var boxCells = grid.GetBox(row, column);

        // Assert
        boxCells.Should().HaveCount(9);
        boxCells.Should().OnlyContain(c => c.Row >= 1 && c.Row <= 3 && c.Column >= 4 && c.Column <= 6);
    }

    [TestCase(1,7)] // Top-right box
    [TestCase(1,8)]
    [TestCase(1,9)]
    [TestCase(2,7)]
    [TestCase(2,8)]
    [TestCase(2,9)]
    [TestCase(3,7)]
    [TestCase(3,8)]
    [TestCase(3,9)]
    public void GetBox_ShouldReturnTopRightBox_ForCellsInBox3(int row, int column)
    {
        // Arrange
        var grid = CreateFullGrid();


        // Act
        var boxCells = grid.GetBox(row, column);

        // Assert
        boxCells.Should().HaveCount(9);
        boxCells.Should().OnlyContain(c => c.Row >= 1 && c.Row <= 3 && c.Column >= 7 && c.Column <= 9);
    }

    [TestCase(4,1)] // Middle-left box
    [TestCase(4,2)]
    [TestCase(4,3)]
    [TestCase(5,1)]
    [TestCase(5,2)]
    [TestCase(5,3)]
    [TestCase(6,1)]
    [TestCase(6,2)]
    [TestCase(6,3)]
    public void GetBox_ShouldReturnMiddleLeftBox_ForCellsInBox4(int row, int column)
    {
        // Arrange
        var grid = CreateFullGrid();


        // Act
        var boxCells = grid.GetBox(row, column);

        // Assert
        boxCells.Should().HaveCount(9);
        boxCells.Should().OnlyContain(c => c.Row >= 4 && c.Row <= 6 && c.Column >= 1 && c.Column <= 3);
    }

    [TestCase(4,4)] // Middle-center box
    [TestCase(4,5)]
    [TestCase(4,6)]
    [TestCase(5,4)]
    [TestCase(5,5)]
    [TestCase(5,6)]
    [TestCase(6,4)]
    [TestCase(6,5)]
    [TestCase(6,6)]
    public void GetBox_ShouldReturnMiddleCenterBox_ForCellsInBox5(int row, int column)
    {
        // Arrange
        var grid = CreateFullGrid();


        // Act
        var boxCells = grid.GetBox(row, column);

        // Assert
        boxCells.Should().HaveCount(9);
        boxCells.Should().OnlyContain(c => c.Row >= 4 && c.Row <= 6 && c.Column >= 4 && c.Column <= 6);
    }

    [TestCase(4,7)] // Middle-right box
    [TestCase(4,8)]
    [TestCase(4,9)]
    [TestCase(5,7)]
    [TestCase(5,8)]
    [TestCase(5,9)]
    [TestCase(6,7)]
    [TestCase(6,8)]
    [TestCase(6,9)]
    public void GetBox_ShouldReturnMiddleRightBox_ForCellsInBox6(int row, int column)
    {
        // Arrange
        var grid = CreateFullGrid();


        // Act
        var boxCells = grid.GetBox(row, column);

        // Assert
        boxCells.Should().HaveCount(9);
        boxCells.Should().OnlyContain(c => c.Row >= 4 && c.Row <= 6 && c.Column >= 7 && c.Column <= 9);
    }

    [TestCase(7,1)] // Bottom-left box
    [TestCase(7,2)]
    [TestCase(7,3)]
    [TestCase(8,1)]
    [TestCase(8,2)]
    [TestCase(8,3)]
    [TestCase(9,1)]
    [TestCase(9,2)]
    [TestCase(9,3)]
    public void GetBox_ShouldReturnBottomLeftBox_ForCellsInBox7(int row, int column)
    {
        // Arrange
        var grid = CreateFullGrid();


        // Act
        var boxCells = grid.GetBox(row, column);

        // Assert
        boxCells.Should().HaveCount(9);
        boxCells.Should().OnlyContain(c => c.Row >= 7 && c.Row <= 9 && c.Column >= 1 && c.Column <= 3);
    }

    [TestCase(7,4)] // Bottom-center box
    [TestCase(7,5)]
    [TestCase(7,6)]
    [TestCase(8,4)]
    [TestCase(8,5)]
    [TestCase(8,6)]
    [TestCase(9,4)]
    [TestCase(9,5)]
    [TestCase(9,6)]
    public void GetBox_ShouldReturnBottomCenterBox_ForCellsInBox8(int row, int column)
    {
        // Arrange
        var grid = CreateFullGrid();


        // Act
        var boxCells = grid.GetBox(row, column);

        // Assert
        boxCells.Should().HaveCount(9);
        boxCells.Should().OnlyContain(c => c.Row >= 7 && c.Row <= 9 && c.Column >= 4 && c.Column <= 6);
    }

    [TestCase(7,7)] // Bottom-right box
    [TestCase(7,8)]
    [TestCase(7,9)]
    [TestCase(8,7)]
    [TestCase(8,8)]
    [TestCase(8,9)]
    [TestCase(9,7)]
    [TestCase(9,8)]
    [TestCase(9,9)]
    public void GetBox_ShouldReturnBottomRightBox_ForCellsInBox9(int row, int column)
    {
        // Arrange
        var grid = CreateFullGrid();


        // Act
        var boxCells = grid.GetBox(row, column);

        // Assert
        boxCells.Should().HaveCount(9);
        boxCells.Should().OnlyContain(c => c.Row >= 7 && c.Row <= 9 && c.Column >= 7 && c.Column <= 9);
    }

    [Test]
    public void GetBox_ShouldReturnSameBoxForAllCellsInBox()
    {
        // Arrange
        var grid = CreateFullGrid();

        // Act
        var box1 = grid.GetBox(1, 1);
        var box2 = grid.GetBox(2, 2);
        var box3 = grid.GetBox(3, 3);

        // Assert
        box1.Should().BeEquivalentTo(box2);
        box2.Should().BeEquivalentTo(box3);
    }

    [Test]
    public void GetBox_ShouldReturnDifferentBoxesForDifferentBoxes()
    {
        // Arrange
        var grid = CreateFullGrid();


        // Act
        var topLeftBox = grid.GetBox(1, 1);
        var topCenterBox = grid.GetBox(1, 4);
        var centerBox = grid.GetBox(5, 5);

        // Assert
        topLeftBox.Should().NotBeEquivalentTo(topCenterBox);
        topLeftBox.Should().NotBeEquivalentTo(centerBox);
        topCenterBox.Should().NotBeEquivalentTo(centerBox);
    }

    
}