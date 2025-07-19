import React from 'react';
import { useSudokuStore } from '@/stores/sudokuStore';
import SudokuCell from './SudokuCell';

function SudokuGrid() {
  const { grid, selectedCell, conflicts, selectCell, setCellValue } = useSudokuStore();

  const handleCellClick = (position: { row: number; column: number }) => {
    selectCell(position);
  };

  const handleCellValueChange = (position: { row: number; column: number }, value: number) => {
    setCellValue(position, value);
  };

  const getCellClasses = (rowIndex: number, colIndex: number) => {
    const baseClasses = 'sudoku-cell';
    
    // Add thicker borders for 3x3 box boundaries
    const borderClasses = [];
    if (rowIndex % 3 === 0) borderClasses.push('border-t-2 border-t-slate-600');
    if (rowIndex === 8) borderClasses.push('border-b-2 border-b-slate-600');
    if (colIndex % 3 === 0) borderClasses.push('border-l-2 border-l-slate-600');
    if (colIndex === 8) borderClasses.push('border-r-2 border-r-slate-600');
    
    return `${baseClasses} ${borderClasses.join(' ')}`;
  };

  return (
    <div className="flex justify-center items-center p-4">
      <div className="sudoku-grid">
        {grid.cells.map((row, rowIndex) => 
          row.map((cell, colIndex) => {
            const position = { row: rowIndex, column: colIndex };
            const isSelected = selectedCell?.row === rowIndex && selectedCell?.column === colIndex;
            const hasConflict = conflicts.some(
              conflict => conflict.row === rowIndex && conflict.column === colIndex
            );
            
            return (
              <SudokuCell
                key={`${rowIndex}-${colIndex}`}
                cell={cell}
                isSelected={isSelected}
                hasConflict={hasConflict}
                onClick={() => handleCellClick(position)}
                onValueChange={(value) => handleCellValueChange(position, value)}
                className={getCellClasses(rowIndex, colIndex)}
              />
            );
          })
        )}
      </div>
    </div>
  );
}

export default SudokuGrid; 