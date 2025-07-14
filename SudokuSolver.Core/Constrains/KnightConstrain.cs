using SudokuSolver.Core.Helpers;
using SudokuSolver.Domain;
using SudokuSolver.Domain.Models;

namespace SudokuSolver.Core.Constrains;

public class KnightConstrain : Constrain
{
    public override string Name => nameof(KnightConstrain);
    
    private static readonly List<(int, int)> KnightMoves = new()
    {
        (2, 1), (2, -1), (-2, 1), (-2, -1),
        (1, 2), (1, -2), (-1, 2), (-1, -2)
    };

    protected override List<Cell> GetInterestedCells(Grid grid, int referenceRow, int referenceColumn)
    {
        var knightCells = new List<Cell>();
        
        foreach (var (knightRow, knightColumn) in KnightMoves)
        {
            var knightCell = grid.GetCell(
                referenceRow + knightRow, 
                referenceColumn + knightColumn
            );
            
            knightCells.Add(knightCell);
        }

        return knightCells;
    }
}