// Core Sudoku types
export interface CellPosition {
  row: number;
  column: number;
}

export interface Cell {
  position: CellPosition;
  value: number;
  isLocked: boolean;
  possibleValues: number[];
  isHighlighted: boolean;
}

export interface SudokuGrid {
  cells: Cell[][];
  size: number;
}

export interface SolvingStep {
  id: string;
  strategy: string;
  description: string;
  cells: CellPosition[];
  values: number[];
  timestamp: number;
}

export type Difficulty = 'easy' | 'medium' | 'hard' | 'expert';

export interface GameState {
  grid: SudokuGrid;
  originalGrid: SudokuGrid;
  selectedCell: CellPosition | null;
  conflicts: CellPosition[];
  solvingSteps: SolvingStep[];
  isSolving: boolean;
  isSolved: boolean;
  difficulty: Difficulty;
  startTime: number | null;
  endTime: number | null;
}

export interface ValidationResult {
  isValid: boolean;
  conflicts: CellPosition[];
  message: string;
}

export interface SolveResult {
  isSolved: boolean;
  grid: number[][];
  steps: SolvingStep[];
  message: string;
} 