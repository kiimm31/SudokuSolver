using SudokuLogic.Constraints.Interface;
using System.Collections.Generic;
using System.Linq;

namespace SudokuLogic
{
    public class Board
    {
        private readonly IEnumerable<IConstraint> _constrains;

        public IList<Cell> Cells { get; set; }

        public Board(IEnumerable<IConstraint> constrains)
        {
            //initialise cells

            Cells = new List<Cell>();

            for (int row = 1; row <= 9; row++)
            {
                for (int col = 1; col <= 9; col++)
                {
                    Cells.Add(new Cell(row, col));
                }
            }
            _constrains = constrains;
        }

        public void SetFixValue(int row, int col, int value)
        {
            if (Cells?.Any(x => x.Row == row && x.Column == col) ?? false)
            {
                Cell myCell = Cells.FirstOrDefault(x => x.Column == col && x.Row == row);

                myCell.SetValue(value);
            }
        }

        public void AttemptSolve()
        {
            while (Cells.Any(x => x.Value == 0)) // there are still unsolved Cells
            {
                StepIteration();
            }
        }

        public void StepIteration()
        {
            foreach (Cell targetCell in Cells)
            {
                if (targetCell.Value == 0) // this cell is unsolved
                {
                    foreach (IConstraint constrain in _constrains)
                    {
                        constrain.DoWork(targetCell, this);
                    }
                }
            }
        }

        public bool Check()
        {
            foreach (IConstraint constrain in _constrains)
            {
                if (!constrain.Check(this))
                {
                    return false;// something wrong
                }
            }
            return true;
        }
    }
}
