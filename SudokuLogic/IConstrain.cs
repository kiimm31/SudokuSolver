using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SudokuLogic.Constrains.Interface
{
    public interface IConstrain
    {
        void DoWork(Cell currentCell, Board board);
    }
}
