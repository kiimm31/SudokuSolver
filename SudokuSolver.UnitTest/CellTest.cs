using FluentAssertions;
using SudokuSolver.Domain.Models;
using SudokuSolver.UnitTest.Base;

namespace SudokuSolver.UnitTest;

public class CellTest : BaseTest
{
    [TestCase(1, 1, ExpectedResult = 1)]
    [TestCase(1, 2, ExpectedResult = 1)]
    [TestCase(1, 3, ExpectedResult = 1)]
    [TestCase(2, 3, ExpectedResult = 1)]
    [TestCase(3, 3, ExpectedResult = 1)]
    [TestCase(1, 4, ExpectedResult = 2)]
    [TestCase(4, 1, ExpectedResult = 4)]
    [TestCase(4, 2, ExpectedResult = 4)]
    [TestCase(4, 3, ExpectedResult = 4)]
    [TestCase(4, 4, ExpectedResult = 5)]
    [TestCase(9, 9, ExpectedResult = 9)]
    [TestCase(9, 5, ExpectedResult = 8)]
    public int BoxIndex(int row, int column)
    {
        var cell = GenerateCellWithPossibleValues(row, column);

        return cell.GetBoxIndex();
    }

    [Test]
    public void GridIsEqual()
    {
        var cell1 = GenerateCellWithPossibleValues(1, 1, [1, 2, 3]);
        var cell2 = GenerateCellWithPossibleValues(1, 2, [1, 2, 3]);
        var cell3 = GenerateCellWithPossibleValues(1, 3, [1, 2, 3]);
        
        var grid1 = new Grid([cell1, cell2, cell3]);
        var grid2 = new Grid([cell1, cell2, cell3]);

        grid2.Equals(grid1).Should().BeTrue();
    }
    
    [Test]
    public void GridIsNotEqual()
    {
        var cell1 = GenerateCellWithPossibleValues(1, 1, [1, 2, 3]);
        var cell2 = GenerateCellWithPossibleValues(1, 2, [1, 2, 3]);
        var cell3 = GenerateCellWithPossibleValues(1, 3, [1, 2, 3]);
        var cell4 = GenerateCellWithPossibleValues(1, 3, [1, 2]);
        
        var grid1 = new Grid([cell1, cell2, cell3]);
        var grid2 = new Grid([cell1, cell2, cell4]);

        grid2.Equals(grid1).Should().BeFalse();
    }
}