using SudokuLogic.Constraints.Interface;
using SudokuLogic.Interface;
using System;
using System.Collections.Generic;

namespace SudokuLogic.Constraints
{
    public class KnightsMoveConstraint : IConstraint
    {
        public KnightsMoveConstraint(IEnumerable<IStrategy> strategies)
        {
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
