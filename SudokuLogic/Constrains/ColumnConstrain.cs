using SudokuLogic.Constrains.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuLogic.Constrains
{
    public class ColumnConstrain : IConstrain
    {
        public void DoWork(Cell currentCell, Board board)
        {
            if (currentCell.Value == 0)
            {
                IEnumerable<Cell> cellsInColumn = board.Cells.Where(x => x.Column == currentCell.Column);

                if (cellsInColumn.Any())
                {
                    foreach (Cell cell in cellsInColumn)
                    {
                        currentCell.RemovePossibleValue(cell.Value);
                    }

                    List<int> otherCellsPossibles = new List<int>();
                    cellsInColumn.Where(x => x != currentCell).Select(x => x.PossibleValues).ToList().ForEach(z => otherCellsPossibles.AddRange(z));

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
