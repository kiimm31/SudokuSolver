using SudokuLogic.Constraints.Interface;
using SudokuLogic.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuLogic.Constraints
{
    public class BoxConstraint : IConstraint
    {
        private readonly IEnumerable<IStrategy> _strategies;

        public BoxConstraint(IEnumerable<IStrategy> strategies)
        {
            _strategies = strategies;
        }

        public bool Check(Board board)
        {
            IEnumerable<int> possibleValues = new List<int>()
            {
                1,2,3,4,5,6,7,8,9
            };

            for (int box = 1; box <= 9; box++)
            {
                IEnumerable<Cell> boxCells = board.Cells.Where(x => GetCurrentBoxNumber(x) == box);

                IEnumerable<int> myBoxValues = boxCells?.Select(x => x.Value);

                if (new HashSet<int>(myBoxValues).Equals(new HashSet<int>(possibleValues)))
                {
                    return false;
                }
            }

            return true;
        }

        public void DoWork(Cell currentCell, Board board)
        {
            if (currentCell.Value == 0)
            {
                IEnumerable<Cell> cellsInBox = board.Cells.Where(x => GetCurrentBoxNumber(currentCell) == GetCurrentBoxNumber(x) && x != currentCell);

                if (cellsInBox.Any())
                {
                    foreach (IStrategy strategy in _strategies)
                    {
                        strategy.DoWork(currentCell, cellsInBox);
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
