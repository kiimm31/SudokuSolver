import { create } from 'zustand';
import { UIState, Theme } from '@/types/ui';

interface UIStore extends UIState {
  // Actions
  toggleTheme: () => void;
  setTheme: (theme: Theme) => void;
  setModalState: (modal: string, show: boolean) => void;
  setLoading: (loading: boolean) => void;
}

const createInitialState = (): UIState => ({
  theme: 'light',
  showSettings: false,
  showHelp: false,
  showStatistics: false,
  isLoading: false,
});

export const useUIStore = create<UIStore>()((set) => ({
  ...createInitialState(),

  toggleTheme: () => {
    set((state) => ({
      theme: state.theme === 'light' ? 'dark' : 'light',
    }));
  },

  setTheme: (theme: Theme) => {
    set({ theme });
  },

  setModalState: (modal: string, show: boolean) => {
    set((state) => ({
      ...state,
      [modal]: show,
    }));
  },

  setLoading: (loading: boolean) => {
    set({ isLoading: loading });
  },
})); 