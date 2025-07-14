using SudokuSolver.Domain.Models;

namespace SudokuSolver.Domain;

public abstract class Constrain
{
    public abstract string Name { get; }
    
    protected static List<int> PossibleValues = [1, 2, 3, 4, 5, 6, 7, 8, 9];

    public abstract Grid TrySolve(Grid grid, int referenceRow, int referenceColumn);
}