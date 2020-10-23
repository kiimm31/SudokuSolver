using SudokuLogic.Interface;
using System;
using System.Collections.Generic;
using System.Text;

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
