import React from 'react';
import SudokuGrid from './SudokuGrid';
import NumberPad from './NumberPad';
import Controls from './Controls';

function SudokuGame() {
  return (
    <div className="max-w-7xl mx-auto px-6 py-8">
      <div className="grid grid-cols-1 xl:grid-cols-3 gap-8 items-start">
        {/* Main Game Area */}
        <div className="xl:col-span-2">
          <div className="game-container p-8">
            <h2 className="text-3xl font-bold text-slate-800 dark:text-slate-100 mb-8 text-center">
              Sudoku Puzzle
            </h2>
            <div className="flex justify-center">
              <SudokuGrid />
            </div>
          </div>
        </div>
        
        {/* Controls Sidebar */}
        <div className="space-y-6">
          <div className="control-panel p-6">
            <h3 className="text-xl font-semibold text-slate-800 dark:text-slate-100 mb-6 text-center">
              Number Pad
            </h3>
            <NumberPad />
          </div>
          
          <div className="control-panel p-6">
            <h3 className="text-xl font-semibold text-slate-800 dark:text-slate-100 mb-6 text-center">
              Controls
            </h3>
            <Controls />
          </div>
        </div>
      </div>
    </div>
  );
}

export default SudokuGame; 