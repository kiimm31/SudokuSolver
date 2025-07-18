namespace SudokuSolver.Domain.Models;

public class Cell
{
    public int Row { get; init; }
    public int Column { get; init; }
    public int Value { get; set; }

    private List<int> PossibleValues { get; set; } = [1, 2, 3, 4, 5, 6, 7, 8, 9];

    public void SetValue(int value)
    {
        if (value <= 0 || value > 9)
            throw new ArgumentOutOfRangeException(nameof(value), "Value must be between 1 and 9.");
        Value = value;
        PossibleValues.Clear();
        PossibleValues.Add(value);
    }

    public void EliminatePossibleValue(int value)
    {
        if (IsSolved)
            return;
            
        PossibleValues.Remove(value);

        if (IsConfirmed && !IsSolved)
        {
            SetValue(PossibleValues[0]);
        }
    }

    public List<int> GetPossibleValues()
    {
        return PossibleValues;
    }

    public bool IsSolved => Value != 0;

    public bool IsConfirmed => PossibleValues.Count == 1;

    public int GetBoxIndex()
    {
        // Determine which box the cell belongs to (convert to 0-based for calculation)
        var boxRow = (Row - 1) / 3;
        var boxColumn = (Column - 1) / 3;

        // Calculate the starting row and column for the box (convert back to 1-based)
        var startRow = boxRow * 3;
        var startColumn = boxColumn + 1;

        return startRow + startColumn;
    }
    
    /// <summary>
    /// Creates a deep copy of this cell
    /// </summary>
    /// <returns>A new cell with the same properties</returns>
    public Cell Clone()
    {
        var clonedCell = new Cell
        {
            Row = this.Row,
            Column = this.Column,
            Value = this.Value
        };
        
        // Copy possible values
        clonedCell.PossibleValues.Clear();
        clonedCell.PossibleValues.AddRange(this.PossibleValues);
        
        return clonedCell;
    }
}