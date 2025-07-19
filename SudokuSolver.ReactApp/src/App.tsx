import React from 'react';
import { useUIStore } from '@/stores/uiStore';
import Layout from '@/components/layout/Layout';
import SudokuGame from '@/components/sudoku/SudokuGame';

function App() {
  const { theme } = useUIStore();

  React.useEffect(() => {
    // Apply theme to document
    document.documentElement.classList.toggle('dark', theme === 'dark');
  }, [theme]);

  return (
    <Layout>
      <SudokuGame />
    </Layout>
  );
}

export default App; 