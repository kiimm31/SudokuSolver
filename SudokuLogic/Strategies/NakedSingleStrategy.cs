using SudokuLogic.Interface;
using System.Collections.Generic;
using System.Linq;

namespace SudokuLogic.Strategies
{
    public class NakedSingleStrategy : IStrategy
    {
        public void DoWork(Cell currentCell, IEnumerable<Cell> otherCells)
        {
            List<int> otherCellsPossibles = new List<int>();
            otherCells.Where(x => x != currentCell).Select(x => x.PossibleValues).ToList().ForEach(z => otherCellsPossibles.AddRange(z));

            IEnumerable<int> myCellPossible = currentCell.PossibleValues.Except(otherCellsPossibles.Distinct());

            if (myCellPossible.Count() == 1)
            {
                currentCell.SetValue(myCellPossible.FirstOrDefault());
            }
        }
    }
}
