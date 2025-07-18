using SudokuSolver.Core.Interfaces;
using SudokuSolver.Domain.Models;

namespace SudokuSolver.Core.Services;

/// <summary>
/// Classic Sudoku solver implementation using constraint propagation and solving strategies
/// </summary>
public class ClassicSudokuSolver : ISudokuSolver
{
    private Grid _grid;
    private readonly List<IConstraint> _constraints;
    private readonly List<ISolvingStrategy> _strategies;
    private int _unchangedIterations;
    private const int MaxUnchangedIterations = 5;
    
    public Grid Grid => _grid;
    public IReadOnlyList<IConstraint> Constraints => _constraints.AsReadOnly();
    public IReadOnlyList<ISolvingStrategy> Strategies => _strategies.AsReadOnly();
    
    public ClassicSudokuSolver(Grid grid)
    {
        _grid = grid ?? throw new ArgumentNullException(nameof(grid));
        _constraints = new List<IConstraint>();
        _strategies = new List<ISolvingStrategy>();
        _unchangedIterations = 0;
    }
    
    public ClassicSudokuSolver(Grid grid, IEnumerable<IConstraint> constraints, IEnumerable<ISolvingStrategy> strategies) 
        : this(grid)
    {
        _constraints.AddRange(constraints ?? Enumerable.Empty<IConstraint>());
        _strategies.AddRange(strategies ?? Enumerable.Empty<ISolvingStrategy>());
    }
    
    public void SetGrid(Grid grid)
    {
        _grid = grid ?? throw new ArgumentNullException(nameof(grid));
        _unchangedIterations = 0;
    }
    
    public Grid Solve()
    {
        ValidateInput();
        
        try
        {
            while (!_grid.IsSolved())
            {
                var previousUnsolvedCount = _grid.GetAllUnsolvedCells().Count();
                
                ApplyConstraints();
                
                if (HasConstraintViolation())
                {
                    break;
                }
                
                ApplyStrategies();
                
                _unchangedIterations = UpdateProgressTracking(previousUnsolvedCount, _unchangedIterations);
                
                if (ShouldStopSolving(_unchangedIterations))
                {
                    break;
                }
            }
        }
        catch (Exception e)
        {
            HandleSolvingError(e);
        }
        
        return _grid;
    }
    
    private void ValidateInput()
    {
        if (_grid == null)
        {
            throw new ArgumentNullException(nameof(_grid));
        }
        
        if (!_grid.GetAllUnsolvedCells().Any())
        {
            throw new InvalidOperationException("The grid is already solved.");
        }
        
        if (_grid.GetAllUnsolvedCells().Count() > 81)
        {
            throw new InvalidOperationException("The grid has more than 81 unsolved cells, which is invalid.");
        }
        
        _unchangedIterations = 0;
    }
    
    private void ApplyConstraints()
    {
        foreach (var constraint in _constraints)
        {
            foreach (var cell in _grid.GetAllUnsolvedCells())
            {
                _grid = constraint.Apply(_grid, cell.Row, cell.Column);
            }
        }
    }
    
    private bool HasConstraintViolation()
    {
        // Only validate constraints if the grid is completely solved
        if (!_grid.IsSolved())
            return false;
            
        try
        {
            ValidateAllConstraints();
            return false;
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Constraint violation detected: {ex.Message}");
            return true;
        }
    }
    
    private void ValidateAllConstraints()
    {
        foreach (var constraint in _constraints)
        {
            foreach (var cell in _grid.GetSolvedCells())
            {
                if (!constraint.IsSatisfied(_grid, cell.Row, cell.Column))
                {
                    throw new InvalidOperationException($"Constraint {constraint.Name} violated at position ({cell.Row}, {cell.Column})");
                }
            }
        }
    }
    
    private void ApplyStrategies()
    {
        var sortedStrategies = _strategies.OrderBy(s => s.Priority).ToList();
        
        foreach (var strategy in sortedStrategies)
        {
            if (!strategy.CanApply(_grid))
                continue;
                
            var strategyPreviousGrid = _grid.Clone();
            _grid = strategy.Apply(_grid);
            
            // Only check for constraint violations if the strategy made changes
            if (_grid.GetAllUnsolvedCells().Count() < strategyPreviousGrid.GetAllUnsolvedCells().Count())
            {
                if (HasConstraintViolation())
                {
                    // Revert to previous state and skip this strategy
                    _grid = strategyPreviousGrid;
                }
            }
        }
    }
    
    private int UpdateProgressTracking(int previousUnsolvedCount, int unchangedIterations)
    {
        var currentUnsolvedCount = _grid.GetAllUnsolvedCells().Count();
        
        if (currentUnsolvedCount == previousUnsolvedCount)
        {
            return unchangedIterations + 1;
        }
        
        return 0;
    }
    
    private bool ShouldStopSolving(int unchangedIterations)
    {
        return unchangedIterations >= MaxUnchangedIterations;
    }
    
    private void HandleSolvingError(Exception e)
    {
        Console.WriteLine($"Error during solving: {e.Message}");
        Console.WriteLine($"Grid state:\n{_grid}");
        throw new InvalidOperationException($"Error during solving: {e.Message}", e);
    }
} 