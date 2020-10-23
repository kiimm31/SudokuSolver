using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuLogic.Interface
{
    public interface IStrategy
    {
        void DoWork(Cell currentCell, IEnumerable<Cell> otherCells);
    }
}
