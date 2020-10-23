using System.Collections.Generic;
using System.Linq;

namespace SudokuLogic
{
    public class Cell
    {
        public int Row => _row;
        public int Column => _column;

        private readonly int _row;
        private readonly int _column;

        public int Value { get; set; }
        public ICollection<int> PossibleValues { get; set; }

        public Cell(int row, int column)
        {
            PossibleValues = new List<int>()
            {
                1,2,3,4,5,6,7,8,9
            };
            Value = 0;
            _row = row;
            _column = column;
        }

        public Cell(int fixedValue)
        {
            Value = fixedValue;
        }

        public void SetValue(int value)
        {
            this.Value = value;
            PossibleValues = new List<int>() { value };
        }

        public void RemovePossibleValue(int value)
        {
            PossibleValues.Remove(value);

            if (PossibleValues.Count() == 1)
            {
                SetValue(PossibleValues.FirstOrDefault());
            }
        }
    }
}
