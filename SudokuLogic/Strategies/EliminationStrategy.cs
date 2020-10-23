using SudokuLogic.Interface;
using System.Collections.Generic;

namespace SudokuLogic.Strategies
{
    public class EliminationStrategy : IStrategy
    {
        public void DoWork(Cell currentCell, IEnumerable<Cell> otherCells)
        {
            foreach (Cell cell in otherCells)
            {
                currentCell.RemovePossibleValue(cell.Value);
            }
        }
    }
}
