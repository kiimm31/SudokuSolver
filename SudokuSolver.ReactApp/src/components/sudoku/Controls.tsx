import React from 'react';
import { useSudokuStore } from '@/stores/sudokuStore';
import { Difficulty } from '@/types/sudoku';

function Controls() {
  const { 
    isSolving, 
    isSolved, 
    solvePuzzle, 
    validatePuzzle, 
    clearGrid, 
    newGame,
    undo,
    redo 
  } = useSudokuStore();

  const handleNewGame = (difficulty: Difficulty) => {
    newGame(difficulty);
  };

  return (
    <div className="space-y-4">
      <div className="grid grid-cols-2 gap-2">
        <button
          onClick={() => handleNewGame('easy')}
          className="control-button"
        >
          Easy
        </button>
        <button
          onClick={() => handleNewGame('medium')}
          className="control-button"
        >
          Medium
        </button>
        <button
          onClick={() => handleNewGame('hard')}
          className="control-button"
        >
          Hard
        </button>
        <button
          onClick={() => handleNewGame('expert')}
          className="control-button"
        >
          Expert
        </button>
      </div>
      
      <div className="space-y-2">
        <button
          onClick={solvePuzzle}
          disabled={isSolving}
          className="control-button w-full"
        >
          {isSolving ? 'Solving...' : 'Solve'}
        </button>
        
        <button
          onClick={validatePuzzle}
          className="control-button secondary w-full"
        >
          Validate
        </button>
        
        <button
          onClick={clearGrid}
          className="control-button danger w-full"
        >
          Clear
        </button>
      </div>
      
      <div className="grid grid-cols-2 gap-2">
        <button
          onClick={undo}
          className="control-button secondary"
        >
          Undo
        </button>
        <button
          onClick={redo}
          className="control-button secondary"
        >
          Redo
        </button>
      </div>
      
      {isSolved && (
        <div className="p-4 bg-green-100 dark:bg-green-900 rounded-lg">
          <p className="text-green-800 dark:text-green-200 text-center font-semibold">
            🎉 Puzzle Solved!
          </p>
        </div>
      )}
    </div>
  );
}

export default Controls; 