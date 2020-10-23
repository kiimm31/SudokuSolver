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

        public int Value => _value;

        private int _value { get; set; } = 0;
        public IReadOnlyCollection<int> PossibleValues
        {
            get
            {
                if (_value > 0 && _possibleValues.Count() == 1)
                {
                    _possibleValues.RemoveAll(x => x != _value);
                }
                return _possibleValues;
            }
        }
        private List<int> _possibleValues { get; set; } = new List<int>()
                                                          {
                                                              1,2,3,4,5,6,7,8,9
                                                          };

        public Cell(int row, int column)
        {
            _row = row;
            _column = column;
        }

        public void SetValue(int value)
        {
            _value = value;
        }

        public void RemovePossibleValue(int value)
        {
            _possibleValues.Remove(value);

            if (_possibleValues.Count() == 1)
            {
                SetValue(_possibleValues.FirstOrDefault());
            }
        }
    }
}
