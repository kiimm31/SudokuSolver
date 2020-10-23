using SudokuLogic.Constrains.Interface;
using SudokuLogic.Interface;
using System.Collections.Generic;
using System.Linq;

namespace SudokuLogic.Constrains
{
    public class ColumnConstrain : IConstrain
    {
        private readonly IEnumerable<IStrategy> _strategies;

        public ColumnConstrain(IEnumerable<IStrategy> strategies)
        {
            _strategies = strategies;
        }

        public bool Check(Board board)
        {
            IEnumerable<int> possibleValues = new List<int>()
            {
                1,2,3,4,5,6,7,8,9
            };

            for (int col = 1; col <= 9; col++)
            {
                IEnumerable<Cell> colCells = board.Cells.Where(x => x.Column == col);

                IEnumerable<int> myColValues = colCells?.Select(x => x.Value);

                if (new HashSet<int>(myColValues).Equals(new HashSet<int>(possibleValues)))
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
                IEnumerable<Cell> cellsInColumn = board.Cells.Where(x => x.Column == currentCell.Column && x != currentCell);

                if (cellsInColumn.Any() && _strategies.Any())
                {
                    foreach (IStrategy strategy in _strategies)
                    {
                        strategy.DoWork(currentCell, cellsInColumn);
                    }
                }
            }

        }
    }
}
