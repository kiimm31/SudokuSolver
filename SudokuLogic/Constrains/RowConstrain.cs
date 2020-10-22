using SudokuLogic.Constrains.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuLogic.Constrains
{
    public class RowConstrain : IConstrain
    {
        public void DoWork(Cell currentCell, Board board)
        {
            if (currentCell.Value == 0)
            {
                IEnumerable<Cell> cellsInRow = board.Cells.Where(x => x.Row == currentCell.Row);

                if (cellsInRow.Any())
                {
                    foreach (Cell cell in cellsInRow)
                    {
                        currentCell.RemovePossibleValue(cell.Value);
                    }
                    // check distinct => only this box can be certain value

                    List<int> otherCellsPossibles = new List<int>();
                    cellsInRow.Where(x => x != currentCell).Select(x => x.PossibleValues).ToList().ForEach(z => otherCellsPossibles.AddRange(z));

                    IEnumerable<int> myCellPossible = currentCell.PossibleValues.Except(otherCellsPossibles.Distinct());

                    if (myCellPossible.Count() == 1)
                    {
                        currentCell.SetValue(myCellPossible.FirstOrDefault());
                    }
                }



            }
        }
    }
}
