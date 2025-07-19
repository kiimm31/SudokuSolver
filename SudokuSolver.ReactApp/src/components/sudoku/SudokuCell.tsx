import React from 'react';
import { Cell } from '@/types/sudoku';

interface SudokuCellProps {
  cell: Cell;
  isSelected: boolean;
  hasConflict: boolean;
  onClick: () => void;
  onValueChange: (value: number) => void;
  className?: string;
}

function SudokuCell({ cell, isSelected, hasConflict, onClick, onValueChange, className = '' }: SudokuCellProps) {
  const getCellClasses = () => {
    const baseClasses = 'sudoku-cell';
    const stateClasses = [];
    
    if (isSelected) stateClasses.push('selected');
    if (hasConflict) stateClasses.push('conflict');
    if (cell.isLocked) stateClasses.push('locked');
    if (cell.isHighlighted) stateClasses.push('highlighted');
    
    return `${baseClasses} ${stateClasses.join(' ')} ${className}`;
  };

  const handleKeyDown = (event: React.KeyboardEvent) => {
    if (cell.isLocked) return;
    
    const key = event.key;
    if (key >= '1' && key <= '9') {
      onValueChange(parseInt(key));
    } else if (key === 'Backspace' || key === 'Delete' || key === '0') {
      onValueChange(0);
    }
  };

  return (
    <div
      className={getCellClasses()}
      onClick={onClick}
      onKeyDown={handleKeyDown}
      tabIndex={0}
      role="button"
      aria-label={`Cell ${cell.position.row + 1}, ${cell.position.column + 1}, value: ${cell.value || 'empty'}`}
    >
      {cell.value !== 0 ? cell.value : ''}
    </div>
  );
}

export default SudokuCell; 