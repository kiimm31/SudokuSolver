namespace SudokuSolver.Domain.Models;

public class Cell
{
    public int Row { get; init; }
    public int Column { get; init; }
    public int Value { get; set; }

    private List<int> PossibleValues { get; set; } = [1,2,3,4,5,6,7,8,9];
    
    public void EliminatePossibleValue(int value)
    {
        PossibleValues.Remove(value);
        
        if (IsConfirmed)
        {
            Value = PossibleValues[0];
        }
    }
    
    public List<int> GetPossibleValues()
    {
        return PossibleValues;
    }
    
    public bool IsSolved => Value != 0;
    
    public bool IsConfirmed => PossibleValues.Count == 1;
}