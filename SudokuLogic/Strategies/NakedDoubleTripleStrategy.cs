using SudokuLogic.Interface;
using System.Collections.Generic;
using System.Linq;

namespace SudokuLogic.Strategies
{
    public class NakedDoubleTripleStrategy : IStrategy
    {
        public void DoWork(Cell currentCell, IEnumerable<Cell> otherCells)
        {
            // is there a pair? if triple, must have 3 to clear all others
            ICollection<Cell> potentialCandiates = new List<Cell>();

            if (otherCells.Any(x => x.PossibleValues == currentCell.PossibleValues))
            {
                foreach (Cell cell in otherCells)
                {
                    if (cell.PossibleValues == currentCell) // another cell with the same possiblity only
                    {
                        potentialCandiates.Add(cell);
                    }
                }
            }

            // are candiates locked pair? all other cells, even if could be value, can no longer be that value if not not enought space for 1-9
            if (IsLockedPair(currentCell, potentialCandiates))
            {
                // remove from others if any
                foreach (Cell nonCandiates in otherCells.Except(potentialCandiates))
                {
                    foreach (int value in currentCell.PossibleValues)
                    {
                        nonCandiates.RemovePossibleValue(value);
                    }
                }
            }
        }

        private bool IsLockedPair(Cell currentCell, ICollection<Cell> potentialCandiates)
        {
            return potentialCandiates.Count() == currentCell.PossibleValues.Count();
        }
    }
}
