using SudokuLogic.Constrains;
using SudokuLogic.Constrains.Interface;
using SudokuLogic.Interface;
using System;
using System.Collections.Generic;

namespace SudokuLogic.Factory
{
    public static class ConstrainFactory
    {
        public static IConstrain CreateContrain<T>(IEnumerable<IStrategy> strategies) where T : IConstrain
        {
            switch (typeof(T))
            {
                case Type type when type == typeof(BoxConstrain):
                    return new BoxConstrain(strategies);
                case Type type when type == typeof(RowConstrain):
                    return new RowConstrain(strategies);
                case Type type when type == typeof(ColumnConstrain):
                    return new ColumnConstrain(strategies);
                case Type type when type == typeof(KingsMoveConstrain):
                    return new KingsMoveConstrain(strategies);
                case Type type when type == typeof(KnightsMoveConstrain):
                    return new KnightsMoveConstrain(strategies);
                default:
                    return new BoxConstrain(strategies);
            }
        }

        public static IEnumerable<IConstrain> CreateNormalSudokuContrains(IEnumerable<IStrategy> strategies)
        {
            return new List<IConstrain>()
            {
                new BoxConstrain(strategies),
                new RowConstrain(strategies),
                new ColumnConstrain(strategies)
            };
        }
    }
}
