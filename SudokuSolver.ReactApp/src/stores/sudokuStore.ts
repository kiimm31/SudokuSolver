import { create } from 'zustand';
import { subscribeWithSelector } from 'zustand/middleware';
import { GameState, CellPosition, SudokuGrid, Difficulty, SolvingStep } from '@/types/sudoku';
import { createEmptyGrid, arrayToGrid, gridToArray, cloneGrid, setCellValue } from '@/services/utils/gridUtils';
import { validateGrid, findConflicts } from '@/services/utils/validationUtils';
import { sudokuAPI } from '@/services/api/sudokuAPI';

interface SudokuStore extends GameState {
  // Actions
  setCellValue: (position: CellPosition, value: number) => void;
  selectCell: (position: CellPosition | null) => void;
  clearGrid: () => void;
  solvePuzzle: () => Promise<void>;
  validatePuzzle: () => Promise<void>;
  newGame: (difficulty: Difficulty) => void;
  undo: () => void;
  redo: () => void;
  highlightRelatedCells: (position: CellPosition) => void;
  clearHighlights: () => void;
  setSolvingSteps: (steps: SolvingStep[]) => void;
  resetGame: () => void;
}

// History for undo/redo
interface HistoryEntry {
  grid: SudokuGrid;
  timestamp: number;
}

const createInitialState = (): GameState => ({
  grid: createEmptyGrid(),
  originalGrid: createEmptyGrid(),
  selectedCell: null,
  conflicts: [],
  solvingSteps: [],
  isSolving: false,
  isSolved: false,
  difficulty: 'medium',
  startTime: null,
  endTime: null,
});

export const useSudokuStore = create<SudokuStore>()(
  subscribeWithSelector((set, get) => ({
    ...createInitialState(),

    setCellValue: (position: CellPosition, value: number) => {
      set((state) => {
        const newGrid = cloneGrid(state.grid);
        setCellValue(newGrid, position, value);
        
        // Update conflicts
        const conflicts = findConflicts(newGrid);
        
        // Check if solved
        const isSolved = newGrid.cells.every(row => 
          row.every(cell => cell.value !== 0)
        );
        
        return {
          grid: newGrid,
          conflicts,
          isSolved,
          endTime: isSolved ? Date.now() : state.endTime,
        };
      });
    },

    selectCell: (position: CellPosition | null) => {
      set({ selectedCell: position });
    },

    clearGrid: () => {
      set((state) => ({
        grid: createEmptyGrid(),
        originalGrid: createEmptyGrid(),
        conflicts: [],
        solvingSteps: [],
        isSolving: false,
        isSolved: false,
        startTime: null,
        endTime: null,
      }));
    },

    solvePuzzle: async () => {
      set({ isSolving: true });
      
      try {
        const { grid } = get();
        const gridArray = gridToArray(grid);
        const response = await sudokuAPI.solvePuzzle(gridArray);
        
        if (response.isSolved) {
          const solvedGrid = arrayToGrid(response.grid);
          set({
            grid: solvedGrid,
            isSolved: true,
            endTime: Date.now(),
            solvingSteps: [], // TODO: Parse solving steps from response
          });
        } else {
          throw new Error(response.message || 'Failed to solve puzzle');
        }
      } catch (error) {
        console.error('Error solving puzzle:', error);
        // TODO: Handle error state
      } finally {
        set({ isSolving: false });
      }
    },

    validatePuzzle: async () => {
      try {
        const { grid } = get();
        const gridArray = gridToArray(grid);
        const response = await sudokuAPI.validatePuzzle(gridArray);
        
        if (!response.isValid) {
          // Update conflicts based on validation result
          // TODO: Parse conflicts from response
          const conflicts = findConflicts(grid);
          set({ conflicts });
        } else {
          set({ conflicts: [] });
        }
      } catch (error) {
        console.error('Error validating puzzle:', error);
        // TODO: Handle error state
      }
    },

    newGame: (difficulty: Difficulty) => {
      set((state) => ({
        ...createInitialState(),
        difficulty,
        startTime: Date.now(),
      }));
      
      // TODO: Generate new puzzle based on difficulty
      // For now, create an empty grid
    },

    undo: () => {
      // TODO: Implement undo functionality
      console.log('Undo not implemented yet');
    },

    redo: () => {
      // TODO: Implement redo functionality
      console.log('Redo not implemented yet');
    },

    highlightRelatedCells: (position: CellPosition) => {
      set((state) => {
        const newGrid = cloneGrid(state.grid);
        
        // Clear previous highlights
        newGrid.cells.forEach(row => 
          row.forEach(cell => cell.isHighlighted = false)
        );
        
        // Highlight related cells (same row, column, box)
        const { row, column } = position;
        
        // Same row
        for (let col = 0; col < newGrid.size; col++) {
          newGrid.cells[row][col].isHighlighted = true;
        }
        
        // Same column
        for (let r = 0; r < newGrid.size; r++) {
          newGrid.cells[r][column].isHighlighted = true;
        }
        
        // Same box
        const boxRow = Math.floor(row / 3);
        const boxCol = Math.floor(column / 3);
        const startRow = boxRow * 3;
        const startCol = boxCol * 3;
        
        for (let r = startRow; r < startRow + 3; r++) {
          for (let c = startCol; c < startCol + 3; c++) {
            newGrid.cells[r][c].isHighlighted = true;
          }
        }
        
        return { grid: newGrid };
      });
    },

    clearHighlights: () => {
      set((state) => {
        const newGrid = cloneGrid(state.grid);
        newGrid.cells.forEach(row => 
          row.forEach(cell => cell.isHighlighted = false)
        );
        return { grid: newGrid };
      });
    },

    setSolvingSteps: (steps: SolvingStep[]) => {
      set({ solvingSteps: steps });
    },

    resetGame: () => {
      set((state) => ({
        ...state,
        grid: cloneGrid(state.originalGrid),
        conflicts: [],
        solvingSteps: [],
        isSolving: false,
        isSolved: false,
        startTime: Date.now(),
        endTime: null,
      }));
    },
  }))
); 