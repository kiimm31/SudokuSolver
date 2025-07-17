using SudokuSolver.Domain;
using SudokuSolver.Domain.Models;

namespace SudokuSolver.Core.Interface;

public abstract class AbstractSudokuSolverService
{
    public abstract Grid MyGrid { get; set; }
    public abstract List<Constraint> Constrains { get; init; }
    public abstract Grid Solve();
    public abstract List<Strategy> Strategies { get; init; }
    
}