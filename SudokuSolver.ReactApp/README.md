# Sudoku Solver - React TypeScript

A modern, interactive Sudoku solver built with React, TypeScript, and Tailwind CSS. This application provides a beautiful, responsive interface for solving Sudoku puzzles with real-time validation and solving capabilities.

## Features

- 🎯 **Interactive Sudoku Grid**: Click to select cells and input numbers
- 🔍 **Real-time Validation**: Instant feedback on conflicts and errors
- 🧠 **Smart Solving**: Integration with advanced solving algorithms
- 📱 **Responsive Design**: Works perfectly on desktop, tablet, and mobile
- 🌙 **Dark Mode**: Toggle between light and dark themes
- ⌨️ **Keyboard Support**: Use number keys and arrow keys for navigation
- 🎨 **Modern UI**: Clean, accessible interface with smooth animations

## Tech Stack

- **Frontend**: React 18 + TypeScript
- **Build Tool**: Vite
- **State Management**: Zustand
- **Styling**: Tailwind CSS + Headless UI
- **HTTP Client**: Axios
- **Testing**: Vitest + React Testing Library
- **Linting**: ESLint + Prettier

## Getting Started

### Prerequisites

- Node.js 18+ and npm
- The .NET backend running on `http://localhost:5000`

### Installation

1. **Install dependencies**:
   ```bash
   npm install
   ```

2. **Start the development server**:
   ```bash
   npm run dev
   ```

3. **Open your browser** and navigate to `http://localhost:3000`

### Available Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run preview` - Preview production build
- `npm run lint` - Run ESLint
- `npm run lint:fix` - Fix ESLint errors
- `npm run test` - Run tests
- `npm run test:ui` - Run tests with UI
- `npm run test:coverage` - Run tests with coverage

## Project Structure

```
src/
├── components/
│   ├── sudoku/          # Sudoku-specific components
│   ├── ui/              # Reusable UI components
│   └── layout/          # Layout components
├── hooks/               # Custom React hooks
├── stores/              # Zustand state stores
├── services/
│   ├── api/             # API service layer
│   └── utils/           # Utility functions
├── types/               # TypeScript type definitions
└── styles/              # CSS and styling
```

## API Integration

The application integrates with the .NET backend API:

- `POST /api/sudoku/solve` - Solve a Sudoku puzzle
- `POST /api/sudoku/validate` - Validate a Sudoku puzzle
- `POST /api/sudoku/generate` - Generate a new puzzle

## Development

### Adding New Features

1. **Components**: Add new components in the appropriate directory under `src/components/`
2. **State Management**: Extend the Zustand stores in `src/stores/`
3. **API Integration**: Add new API methods in `src/services/api/`
4. **Types**: Define new TypeScript interfaces in `src/types/`

### Testing

- Write unit tests for components in `__tests__` directories
- Use React Testing Library for component testing
- Test state management with Zustand store tests

### Styling

- Use Tailwind CSS classes for styling
- Custom styles can be added in `src/styles/`
- Follow the mobile-first responsive design approach

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Run the linter and tests
6. Submit a pull request

## License

This project is licensed under the MIT License.

## Acknowledgments

- Built with modern React patterns and best practices
- Inspired by classic Sudoku solving techniques
- Designed for accessibility and user experience 