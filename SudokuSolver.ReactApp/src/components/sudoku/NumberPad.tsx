import React from 'react';
import { useSudokuStore } from '@/stores/sudokuStore';

function NumberPad() {
  const { selectedCell, setCellValue } = useSudokuStore();

  const handleNumberClick = (number: number) => {
    if (selectedCell) {
      setCellValue(selectedCell, number);
    }
  };

  const handleClear = () => {
    if (selectedCell) {
      setCellValue(selectedCell, 0);
    }
  };

  return (
    <div className="space-y-4">
      <div className="number-pad">
        {[1, 2, 3, 4, 5, 6, 7, 8, 9].map((number) => (
          <button
            key={number}
            onClick={() => handleNumberClick(number)}
            disabled={!selectedCell}
            className="number-button disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {number}
          </button>
        ))}
      </div>
      
      <div className="flex justify-center">
        <button
          onClick={handleClear}
          disabled={!selectedCell}
          className="control-button secondary disabled:opacity-50 disabled:cursor-not-allowed"
        >
          Clear
        </button>
      </div>
    </div>
  );
}

export default NumberPad; 