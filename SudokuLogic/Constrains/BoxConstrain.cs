using SudokuLogic.Constrains.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuLogic.Constrains
{
    public class BoxConstrain : IConstrain
    {
        public void DoWork(Cell currentCell, Board board)
        {
            if (currentCell.Value == 0)
            {
                IEnumerable<Cell> cellsInBox = board.Cells.Where(x => GetCurrentBoxNumber(currentCell) == GetCurrentBoxNumber(x));

                if (cellsInBox.Any())
                {
                    // remove all confirmed possibles
                    foreach (Cell cell in cellsInBox.Where(x => x.Value > 0))
                    {
                        currentCell.RemovePossibleValue(cell.Value);
                    }

                    //check any distinct possible values

                    List<int> otherCellsPossibles = new List<int>();
                    cellsInBox.Where(x => x != currentCell).Select(x => x.PossibleValues).ToList().ForEach(z => otherCellsPossibles.AddRange(z));

                    IEnumerable<int> myCellPossible = currentCell.PossibleValues.Except(otherCellsPossibles.Distinct());

                    if (myCellPossible.Count() == 1)
                    {
                        currentCell.SetValue(myCellPossible.FirstOrDefault());
                    }
                }
            }
        }

        private int GetCurrentBoxNumber(Cell targetCell)
        {
            double col = Math.Ceiling((double)targetCell.Column / 3);

            double row = Math.Ceiling((double)targetCell.Row / 3);

            return Convert.ToInt32(((row - 1) * 3) + col);
        }
    }
}
