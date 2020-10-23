using SudokuLogic.Constraints;
using SudokuLogic.Constraints.Interface;
using SudokuLogic.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuLogic.Factory
{
    public static class ConstraintFactory
    {
        public static IConstraint CreateContraint<T>(IEnumerable<IStrategy> strategies) where T : IConstraint
        {
            switch (typeof(T))
            {
                case Type type when type == typeof(BoxConstraint):
                    return new BoxConstraint(strategies);
                case Type type when type == typeof(RowConstraint):
                    return new RowConstraint(strategies);
                case Type type when type == typeof(ColumnConstraint):
                    return new ColumnConstraint(strategies);
                case Type type when type == typeof(KingsMoveConstraint):
                    return new KingsMoveConstraint(strategies);
                case Type type when type == typeof(KnightsMoveConstraint):
                    return new KnightsMoveConstraint(strategies);
                default:
                    return new BoxConstraint(strategies);
            }
        }

        public static IEnumerable<IConstraint> CreateNormalSudokuContraints(IEnumerable<IStrategy> strategies)
        {
            return new List<IConstraint>()
            {
                new BoxConstraint(strategies),
                new RowConstraint(strategies),
                new ColumnConstraint(strategies)
            };
        }
    }
}
