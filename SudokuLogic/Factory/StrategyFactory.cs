using SudokuLogic.Interface;
using SudokuLogic.Strategies;
using System;
using System.Collections.Generic;

namespace SudokuLogic.Factory
{
    public static class StrategyFactory
    {
        public static IStrategy CreateStrategy<T>() where T : IStrategy
        {
            switch (typeof(T))
            {
                case Type type when type == typeof(EliminationStrategy):
                    return new EliminationStrategy();
                case Type type when type == typeof(NakedSingleStrategy):
                    return new NakedSingleStrategy();
                case Type type when type == typeof(NakedDoubleTripleStrategy):
                    return new NakedDoubleTripleStrategy();

                default:
                    return new EliminationStrategy();
            }
        }

        public static IEnumerable<IStrategy> CreateAllStrategies()
        {
            return new List<IStrategy>()
            {
                CreateStrategy<EliminationStrategy>(),
                CreateStrategy<NakedSingleStrategy>(),
                CreateStrategy<NakedDoubleTripleStrategy>()
            };
        }
    }
}
