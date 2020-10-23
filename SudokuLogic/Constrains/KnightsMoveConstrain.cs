using SudokuLogic.Constrains.Interface;
using SudokuLogic.Interface;
using System;
using System.Collections.Generic;

namespace SudokuLogic.Constrains
{
    public class KnightsMoveConstrain : IConstrain
    {
        public KnightsMoveConstrain(IEnumerable<IStrategy> strategies)
        {
        }

        public void DoWork(Cell currentCell, Board board)
        {
            throw new NotImplementedException();
        }
    }
}
