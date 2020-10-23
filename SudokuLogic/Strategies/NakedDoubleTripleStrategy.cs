using SudokuLogic.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SudokuLogic.Strategies
{
    public class NakedDoubleTripleStrategy : IStrategy
    {
        public void DoWork(Cell currentCell, IEnumerable<Cell> otherCells)
        {
            if (currentCell.Row == 7 && currentCell.Column == 3)
            {

            }

            // is there a pair? if triple, must have 3 to clear all others
            ICollection<Cell> potentialCandiates = new List<Cell>();

            foreach (Cell cell in otherCells)
            {
                if (cell.PossibleValues.All(x => currentCell.PossibleValues.Contains(x))) 
                {
                    // current cell should have the largest possible values
                    // any other cell should be subsets.
                    potentialCandiates.Add(cell);
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
            return potentialCandiates.Count() + 1 == currentCell.PossibleValues.Count();
        }
    }
}
