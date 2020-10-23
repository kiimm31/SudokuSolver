using System.Collections.Generic;

namespace SudokuLogic.Interface
{
    public interface IStrategy
    {
        void DoWork(Cell currentCell, IEnumerable<Cell> otherCells);
    }
}
