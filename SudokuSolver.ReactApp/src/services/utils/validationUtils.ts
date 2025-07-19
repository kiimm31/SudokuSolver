import { Cell, CellPosition, SudokuGrid } from '@/types/sudoku';
import { getRow, getColumn, getBox, getBoxCoordinates } from './gridUtils';

// Check if a value is valid in a specific position
export function isValidValue(grid: SudokuGrid, position: CellPosition, value: number): boolean {
  if (value === 0) return true;
  
  const { row, column } = position;
  
  // Check row
  const rowCells = getRow(grid, row);
  for (const cell of rowCells) {
    if (cell.position.row === row && cell.position.column === column) continue;
    if (cell.value === value) return false;
  }
  
  // Check column
  const colCells = getColumn(grid, column);
  for (const cell of colCells) {
    if (cell.position.row === row && cell.position.column === column) continue;
    if (cell.value === value) return false;
  }
  
  // Check box
  const { boxRow, boxCol } = getBoxCoordinates(position);
  const boxCells = getBox(grid, boxRow, boxCol);
  for (const cell of boxCells) {
    if (cell.position.row === row && cell.position.column === column) continue;
    if (cell.value === value) return false;
  }
  
  return true;
}

// Find conflicts in the grid
export function findConflicts(grid: SudokuGrid): CellPosition[] {
  const conflicts: CellPosition[] = [];
  
  // Check rows
  for (let row = 0; row < grid.size; row++) {
    const rowCells = getRow(grid, row);
    const values = rowCells.filter(cell => cell.value !== 0).map(cell => cell.value);
    const duplicates = findDuplicates(values);
    
    for (const value of duplicates) {
      const conflictCells = rowCells.filter(cell => cell.value === value);
      conflicts.push(...conflictCells.map(cell => cell.position));
    }
  }
  
  // Check columns
  for (let col = 0; col < grid.size; col++) {
    const colCells = getColumn(grid, col);
    const values = colCells.filter(cell => cell.value !== 0).map(cell => cell.value);
    const duplicates = findDuplicates(values);
    
    for (const value of duplicates) {
      const conflictCells = colCells.filter(cell => cell.value === value);
      conflicts.push(...conflictCells.map(cell => cell.position));
    }
  }
  
  // Check boxes
  for (let boxRow = 0; boxRow < 3; boxRow++) {
    for (let boxCol = 0; boxCol < 3; boxCol++) {
      const boxCells = getBox(grid, boxRow, boxCol);
      const values = boxCells.filter(cell => cell.value !== 0).map(cell => cell.value);
      const duplicates = findDuplicates(values);
      
      for (const value of duplicates) {
        const conflictCells = boxCells.filter(cell => cell.value === value);
        conflicts.push(...conflictCells.map(cell => cell.position));
      }
    }
  }
  
  return conflicts;
}

// Find duplicate values in an array
function findDuplicates(values: number[]): number[] {
  const seen = new Set<number>();
  const duplicates = new Set<number>();
  
  for (const value of values) {
    if (seen.has(value)) {
      duplicates.add(value);
    } else {
      seen.add(value);
    }
  }
  
  return Array.from(duplicates);
}

// Validate entire grid
export function validateGrid(grid: SudokuGrid): { isValid: boolean; conflicts: CellPosition[] } {
  const conflicts = findConflicts(grid);
  return {
    isValid: conflicts.length === 0,
    conflicts,
  };
}

// Check if a cell has conflicts
export function hasConflict(grid: SudokuGrid, position: CellPosition): boolean {
  const cell = grid.cells[position.row][position.column];
  if (cell.value === 0) return false;
  
  const { row, column } = position;
  
  // Check row
  const rowCells = getRow(grid, row);
  const rowConflicts = rowCells.filter(c => 
    c.value === cell.value && 
    (c.position.row !== row || c.position.column !== column)
  );
  
  // Check column
  const colCells = getColumn(grid, column);
  const colConflicts = colCells.filter(c => 
    c.value === cell.value && 
    (c.position.row !== row || c.position.column !== column)
  );
  
  // Check box
  const { boxRow, boxCol } = getBoxCoordinates(position);
  const boxCells = getBox(grid, boxRow, boxCol);
  const boxConflicts = boxCells.filter(c => 
    c.value === cell.value && 
    (c.position.row !== row || c.position.column !== column)
  );
  
  return rowConflicts.length > 0 || colConflicts.length > 0 || boxConflicts.length > 0;
}

// Get related cells (same row, column, or box)
export function getRelatedCells(grid: SudokuGrid, position: CellPosition): CellPosition[] {
  const { row, column } = position;
  const related: CellPosition[] = [];
  
  // Same row
  for (let col = 0; col < grid.size; col++) {
    if (col !== column) {
      related.push({ row, column: col });
    }
  }
  
  // Same column
  for (let row = 0; row < grid.size; row++) {
    if (row !== row) {
      related.push({ row, column });
    }
  }
  
  // Same box
  const { boxRow, boxCol } = getBoxCoordinates(position);
  const startRow = boxRow * 3;
  const startCol = boxCol * 3;
  
  for (let r = startRow; r < startRow + 3; r++) {
    for (let c = startCol; c < startCol + 3; c++) {
      if (r !== row || c !== column) {
        related.push({ row: r, column: c });
      }
    }
  }
  
  return related;
} 