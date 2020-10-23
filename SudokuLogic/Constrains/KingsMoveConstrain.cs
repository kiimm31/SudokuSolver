using SudokuLogic.Constrains.Interface;
using SudokuLogic.Interface;
using System;
using System.Collections.Generic;

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
            throw new NotImplementedException();
        }

        public void DoWork(Cell currentCell, Board board)
        {
            throw new NotImplementedException();
        }
    }
}
