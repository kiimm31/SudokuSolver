using SudokuLogic.Constraints.Interface;
using SudokuLogic.Interface;
using System.Collections.Generic;
using System.Linq;

namespace SudokuLogic.Constraints
{
    public class RowConstraint : IConstraint
    {
        private readonly IEnumerable<IStrategy> _strategies;

        public RowConstraint(IEnumerable<IStrategy> strategies)
        {
            _strategies = strategies;
        }

        public bool Check(Board board)
        {
            IEnumerable<int> possibleValues = new List<int>()
            {
                1,2,3,4,5,6,7,8,9
            };

            for (int row = 1; row <= 9; row++)
            {
                IEnumerable<Cell> rowCells = board.Cells.Where(x => x.Row == row);

                IEnumerable<int> myRowValues = rowCells?.Select(x => x.Value);

                if (new HashSet<int>(myRowValues).Equals(new HashSet<int>(possibleValues)))
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
                IEnumerable<Cell> cellsInRow = board.Cells.Where(x => x.Row == currentCell.Row && x != currentCell);

                if (cellsInRow.Any() && _strategies.Any())
                {
                    foreach (IStrategy strategy in _strategies)
                    {
                        strategy.DoWork(currentCell, cellsInRow);
                    }
                }
            }
        }
    }
}
