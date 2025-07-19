import { Cell, CellPosition, SudokuGrid } from '@/types/sudoku';

// Create an empty Sudoku grid
export function createEmptyGrid(size: number = 9): SudokuGrid {
  const cells: Cell[][] = [];
  
  for (let row = 0; row < size; row++) {
    cells[row] = [];
    for (let col = 0; col < size; col++) {
      cells[row][col] = {
        position: { row, column: col },
        value: 0,
        isLocked: false,
        possibleValues: [1, 2, 3, 4, 5, 6, 7, 8, 9],
        isHighlighted: false,
      };
    }
  }
  
  return { cells, size };
}

// Convert flat array to 2D grid
export function arrayToGrid(array: number[]): SudokuGrid {
  const size = Math.sqrt(array.length);
  const grid = createEmptyGrid(size);
  
  for (let i = 0; i < array.length; i++) {
    const row = Math.floor(i / size);
    const col = i % size;
    const value = array[i];
    
    grid.cells[row][col].value = value;
    grid.cells[row][col].isLocked = value !== 0;
    if (value !== 0) {
      grid.cells[row][col].possibleValues = [value];
    }
  }
  
  return grid;
}

// Convert 2D grid to flat array
export function gridToArray(grid: SudokuGrid): number[] {
  const array: number[] = [];
  
  for (let row = 0; row < grid.size; row++) {
    for (let col = 0; col < grid.size; col++) {
      array.push(grid.cells[row][col].value);
    }
  }
  
  return array;
}

// Get cell at position
export function getCell(grid: SudokuGrid, position: CellPosition): Cell | null {
  const { row, column } = position;
  if (row < 0 || row >= grid.size || column < 0 || column >= grid.size) {
    return null;
  }
  return grid.cells[row][column];
}

// Set cell value
export function setCellValue(grid: SudokuGrid, position: CellPosition, value: number): SudokuGrid {
  const cell = getCell(grid, position);
  if (cell && !cell.isLocked) {
    cell.value = value;
    cell.possibleValues = value === 0 ? [1, 2, 3, 4, 5, 6, 7, 8, 9] : [value];
  }
  return grid;
}

// Get row cells
export function getRow(grid: SudokuGrid, row: number): Cell[] {
  return grid.cells[row] || [];
}

// Get column cells
export function getColumn(grid: SudokuGrid, column: number): Cell[] {
  const cells: Cell[] = [];
  for (let row = 0; row < grid.size; row++) {
    cells.push(grid.cells[row][column]);
  }
  return cells;
}

// Get box cells (3x3 subgrid)
export function getBox(grid: SudokuGrid, boxRow: number, boxCol: number): Cell[] {
  const cells: Cell[] = [];
  const startRow = boxRow * 3;
  const startCol = boxCol * 3;
  
  for (let row = startRow; row < startRow + 3; row++) {
    for (let col = startCol; col < startCol + 3; col++) {
      cells.push(grid.cells[row][col]);
    }
  }
  
  return cells;
}

// Get box coordinates for a cell
export function getBoxCoordinates(position: CellPosition): { boxRow: number; boxCol: number } {
  return {
    boxRow: Math.floor(position.row / 3),
    boxCol: Math.floor(position.column / 3),
  };
}

// Check if grid is solved
export function isGridSolved(grid: SudokuGrid): boolean {
  for (let row = 0; row < grid.size; row++) {
    for (let col = 0; col < grid.size; col++) {
      if (grid.cells[row][col].value === 0) {
        return false;
      }
    }
  }
  return true;
}

// Clone grid
export function cloneGrid(grid: SudokuGrid): SudokuGrid {
  const clonedCells: Cell[][] = [];
  
  for (let row = 0; row < grid.size; row++) {
    clonedCells[row] = [];
    for (let col = 0; col < grid.size; col++) {
      const originalCell = grid.cells[row][col];
      clonedCells[row][col] = {
        position: { ...originalCell.position },
        value: originalCell.value,
        isLocked: originalCell.isLocked,
        possibleValues: [...originalCell.possibleValues],
        isHighlighted: originalCell.isHighlighted,
      };
    }
  }
  
  return { cells: clonedCells, size: grid.size };
} 