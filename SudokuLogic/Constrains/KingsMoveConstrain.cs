using SudokuLogic.Constrains.Interface;
using SudokuLogic.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuLogic.Constrains
{
    public class KingsMoveConstrain : IConstrain
    {
        private readonly IEnumerable<IStrategy> _strategies;

        public KingsMoveConstrain(IEnumerable<IStrategy> strategies)
        {
            _strategies = strategies;
        }

        public bool Check(Board board)
        {
            IEnumerable<int> possibleValues = new List<int>()
            {
                1,2,3,4,5,6,7,8,9
            };

            foreach (Cell targetCell in board.Cells)
            {
                IEnumerable<Cell> affectedCells = board.Cells.Where(x => Math.Abs(x.Row - targetCell.Row) == 1 || Math.Abs(x.Column - targetCell.Column) == 1);

                IEnumerable<int> myValues = affectedCells?.Select(x => x.Value);

                if (new HashSet<int>(myValues).Equals(new HashSet<int>(possibleValues)))
                {
                    return false;
                }
            }

            return true;
        }

        public void DoWork(Cell currentCell, Board board)
        {
            // kings move, all one step dignal also
            IEnumerable<Cell> affectedCells = board.Cells.Where(x => Math.Abs(x.Row - currentCell.Row) == 1 || Math.Abs(x.Column - currentCell.Column) == 1);

            if (affectedCells.Any())
            {
                foreach (IStrategy strategy in _strategies)
                {
                    strategy.DoWork(currentCell, affectedCells);
                }
            }
        }
    }
}
